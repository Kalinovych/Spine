using System;
using System.Collections.Generic;
using System.Linq;

namespace Spine.Signals {
	public class EventHub {
		private readonly ChannelMap channelMap = new ChannelMap();

		public void On<T>(Action<T> handler) {
			AddReceiver( handler, once: false );
		}

		public void On<T>(Type eventType, Action<T> handler) {
			if (typeof(T) != eventType) {
				throw new ArgumentException($"Generic type {{typeof(T)}} should match event type {eventType}");
			}

			channelMap.GetOrCreateChannel<T>().Add( CreateReceiver( handler, false ) );
		}

		public void Once<T>(Action<T> handler) {
			AddReceiver( handler, once: true );
		}

		public void Off<T>(Action<T> handler) {
			RemoveReceiver( handler, false );
		}

		public void OffOnce<T>(Action<T> handler) {
			RemoveReceiver( handler, true );
		}

		public void OffAll() {
			channelMap.Clear();
		}

		public void Send<T>(T signal) {
			channelMap.GetChannel<T>()?.Emit( signal );
		}

		void RemoveReceiver<T>(Action<T> handler, bool once) {
			channelMap.GetChannel<T>()?.Remove( CreateReceiver( handler, once ) );
		}

		void AddReceiver<T>(Action<T> handler, bool once) {
			channelMap.GetOrCreateChannel<T>().Add( CreateReceiver( handler, once ) );
		}

		Receiver<T> CreateReceiver<T>(Action<T> handler, bool once) {
			return new Receiver<T>( handler, once );
		}
	}

	// Internal kitchen //

	/***
	 *
	 */
	internal class ChannelMap {
		private readonly Dictionary<Type, IChannel> channelMap = new Dictionary<Type, IChannel>();
		private readonly object lockObject = new object();

		internal void Clear() {
			lock (lockObject) {
				channelMap.Clear();
			}
		}

		internal Channel<TSignal> GetChannel<TSignal>() {
            lock (lockObject) {
                if (channelMap.TryGetValue(typeof(TSignal), out object channel)) {
                    return (Channel<TSignal>)channel;
                }

                return null;
            }
        }

		internal Channel<TSignal> GetOrCreateChannel<TSignal>() {
			lock (lockObject)
            {
                if (channelMap.TryGetValue(typeof(TSignal), out object channel))
                {
                    return (Channel<TSignal>)channel;
                }

                var newChannel = new Channel<TSignal>();
                channelMap.Add(typeof(TSignal), newChannel);
                return newChannel;
            }
		}
	}

	/***
	 *
	 */
	internal interface IChannel {
	}

	/***
	 *
	 */
	internal class Channel<TSignal> : IChannel {
		private readonly HashSet<Receiver<TSignal>> _receivers = new HashSet<Receiver<TSignal>>();

		public void Add(Receiver<TSignal> receiver) {
			_receivers.Add( receiver );
		}

		public void Remove(Receiver<TSignal> receiver) {
			_receivers.Remove( receiver );
		}

		public void Emit(TSignal signal) {
			// take a snapshot of the current receivers
			var receivers = _receivers.ToArray();
			foreach (var receiver in receivers) {
				// no second chance to get here
				if (receiver.once) {
					// remove receiver from the original list
					_receivers.Remove( receiver );
				}

				// handle it, baby
				receiver.Handle( signal );
			}
		}
	}

	/**
	 *
	 */
	internal readonly struct Receiver<TSignal> : IEquatable<Receiver<TSignal>> {
		public readonly Action<TSignal> callback;
		public readonly bool once;

		public Receiver(Action<TSignal> callback) : this() {
			this.callback = callback;
		}

		public Receiver(Action<TSignal> callback, bool once) : this() {
			this.callback = callback;
			this.once = once;
		}

		public void Handle(TSignal signal) {
			callback( signal );
		}

		public bool Equals(Receiver<TSignal> other) {
			return Equals( callback, other.callback ) && once == other.once;
		}

		public override bool Equals(object obj) {
			if (obj is null) return false;
			return (obj is Receiver<TSignal> other) && Equals( other );
		}

		public override int GetHashCode() {
			unchecked {
				return ((callback != null ? callback.GetHashCode() : 0) * 397) ^ once.GetHashCode();
			}
		}
	}
}
