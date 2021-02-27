using System;
using System.Collections.Generic;
using System.Linq;

namespace Spine.Signals {
	public class EventHub {
		private readonly ChannelMap channelMap = new ChannelMap();

		public void On<T>(Action<T> handler, bool once) {
			AddReceiver( handler, once );
		}

		public void On<T>(Action<T> handler) {
			AddReceiver( handler, once: false );
		}

		public void Once<T>(Action<T> handler) {
			AddReceiver( handler, once: true );
		}

		public void Off<T>(Action<T> handler, bool once) {
			RemoveReceiver( handler, once );
		}

		public void OffAll() {
			channelMap.Clear();
		}

		public void Emit<T>(T signal) {
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
		private readonly Dictionary<Type, Channel> channelMap = new Dictionary<Type, Channel>();

		internal void Clear() {
			channelMap.Clear();
		}

		internal Channel<TSignal> GetChannel<TSignal>() {
			var type = typeof(TSignal);

			if (channelMap.ContainsKey( type )) {
				return (Channel<TSignal>) channelMap[type];
			}

			return null;
		}

		internal Channel<TSignal> GetOrCreateChannel<TSignal>() {
			Channel<TSignal> result;

			var type = typeof(TSignal);

			if (channelMap.ContainsKey( type )) {
				result = (Channel<TSignal>) channelMap[type];
			}
			else {
				channelMap[type] = result = new Channel<TSignal>();
			}

			return result;
		}
	}

	/***
	 *
	 */
	internal class Channel {
	}

	/***
	 *
	 */
	internal class Channel<TSignal> : Channel {
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
