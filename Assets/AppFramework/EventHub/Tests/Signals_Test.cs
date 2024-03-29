﻿using System.Diagnostics;
using NUnit.Framework;
using Spine.Signals;
using Debug = UnityEngine.Debug;

public class Signals_Test
{
	public struct TestSignal
	{
		public string test;
	}

	public struct TestSignal2
	{
		public string test;
	}

	private EventHub _eventHub;

	[SetUp]
	public void BeforeEachTest()
	{
		_eventHub = new EventHub();
	}

	[Test]
	public void SignalHubInstantiated()
	{
		Assert.NotNull( _eventHub );
	}

	[Test]
	public void BroadcastDataStructPasses()
	{
		_eventHub.Send( new TestSignal() );
		_eventHub.Send( new TestSignal2() );
	}

	[Test]
	public void SubscriberReceivesData()
	{
		_eventHub.On<TestSignal>( signal =>
		{
			Assert.Pass();
		} );

		_eventHub.Send( new TestSignal() );

		Assert.Fail();
	}

	[Test]
	public void AllSubscribersReceiveData()
	{
		var receivedCount = 0;
		_eventHub.On<TestSignal>( signal =>
		{
			receivedCount++;
		} );
		_eventHub.On<TestSignal>( signal =>
		{
			receivedCount++;
		} );
		_eventHub.On<TestSignal>( signal =>
		{
			receivedCount++;
			Assert.AreEqual( 3, receivedCount );
			Assert.Pass();
		} );

		_eventHub.Send( new TestSignal() );

		Assert.Fail();
	}

	[Test]
	public void SubscriberReceivesCorrectTypeData()
	{
		// other events shouldn't pass
		_eventHub.On<TestSignal2>( signal =>
		{
			Assert.Fail();
		} );

		_eventHub.On<TestSignal>( signal =>
		{
			Assert.Pass();
		} );

		_eventHub.Send( new TestSignal() );

		Assert.Fail();
	}

	[Test]
	public void OncePasses()
	{
		var receivedCount = 0;
		_eventHub.Once<TestSignal>( signal =>
		{
			receivedCount++;
			if (receivedCount > 1)
				Assert.Fail( "Signal received more than once" );
		} );

		_eventHub.Send( new TestSignal() );
		_eventHub.Send( new TestSignal() );
		_eventHub.Send( new TestSignal() );
	}

	void TestSignal_FailHandler(TestSignal signal)
	{
		Assert.Fail();
	}

	void TestSignal2_FailHandler(TestSignal2 signal)
	{
		Assert.Fail();
	}

	[Test]
	public void RemovePasses()
	{
		_eventHub.On<TestSignal>( TestSignal_FailHandler );
		_eventHub.Off<TestSignal>( TestSignal_FailHandler );

		_eventHub.Send( new TestSignal() );
		_eventHub.Send( new TestSignal() );
	}

	[Test]
	public void RemoveAllPasses()
	{
		_eventHub.On<TestSignal>( TestSignal_FailHandler );
		_eventHub.On<TestSignal2>( TestSignal2_FailHandler );

		_eventHub.OffAll();

		_eventHub.Send( new TestSignal() );
		_eventHub.Send( new TestSignal2() );
	}

	[Test]
	public void RemoveFromHandlerPasses()
	{
		_eventHub.On<TestSignal>( RemoveSelfHandler );
		_eventHub.Send( new TestSignal() );
		_eventHub.Send( new TestSignal() );
	}

	private void RemoveSelfHandler(TestSignal signal)
	{
		_eventHub.Off<TestSignal>( RemoveSelfHandler );
	}

	[Test]
	public void Performance()
	{
		var receiveCount = 0;
		_eventHub.On<TestSignal>( signal =>
		{
			receiveCount++;
		} );
		_eventHub.On<TestSignal>( signal =>
		{
			receiveCount++;
		} );
		_eventHub.On<TestSignal2>( signal =>
		{
			receiveCount++;
		} );

		var stopwatch = new Stopwatch();
		stopwatch.Start();

		const int runCount = 1000 * 1000;
		for (var i = 0; i < runCount; i++)
		{
			_eventHub.Send( new TestSignal() );
			//signalHub.Send(new TestSignal2());
		}

		stopwatch.Stop();

		Debug.Log( $"Time elapsed to send {runCount} singals: {stopwatch.ElapsedMilliseconds} ms" );

		Assert.AreEqual( runCount * 2, receiveCount );
	}
}
