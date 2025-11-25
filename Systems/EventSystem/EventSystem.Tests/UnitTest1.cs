using NomadCore.Abstractions.Services;
using NomadCore.Systems.EventSystem.Services;
using NomadCore.Systems.ConsoleSystem.Services;
using NomadCore.Interfaces;
using NomadCore.Infrastructure;
using NomadCore.Systems.ConsoleSystem.Infrastructure;
using NomadCore.Systems.ConsoleSystem.Infrastructure.Sinks;
using NUnit.Framework;

namespace NomadCore.Systems.EventSystem.Tests {
	[TestFixture]
	public class Tests {
		private readonly struct EventArgs( int test1, bool conditional ) : IEventArgs {
			public readonly int Test1 = test1;
			public readonly bool TestConditional = conditional;
		};

		private IGameEventBusService EventBus;

		private IGameEvent<EventArgs> TestArgs;

		[SetUp]
		public void Setup() {
			EventBus = ServiceRegistry.Register<IGameEventBusService>( new GameEventBus() );

			TestArgs = EventBus.CreateEvent<EventArgs>( nameof( TestArgs ) );
			TestArgs.Subscribe( this, OnEventTriggered );
		}

		[TearDown]
		public void TearDown() {
			TestArgs.Dispose();
		}

		[Test]
		public void Test_EventArgsCorrect() {
			TestArgs.Publish( new EventArgs( 21, true ) );
		}

		private void OnEventTriggered( in EventArgs args ) {
			Assert.That( args.Test1 == 21 );
			Assert.That( args.TestConditional == true );
		}
	}
};