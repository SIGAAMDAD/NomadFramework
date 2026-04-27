using System;
using NUnit.Framework;
using Nomad.Core.Input;
using Nomad.Input.Private.Services;

namespace Nomad.Input.Tests {
	[TestFixture]
	[Category("Nomad.Input")]
	[Category("State")]
	[Category("Unit")]
	public class MouseStateServiceTests {
		[Test]
		public void Constructor_WhenSnapshotServiceIsNull_Throws() {
			Assert.Throws<ArgumentNullException>( () => new MouseStateService( null ) );
		}

		[Test]
		public void IsPressed_CurrentlyThrowsNotImplemented() {
			using var snapshot = new InputStateService();
			var service = new MouseStateService( snapshot );

			Assert.Throws<NotImplementedException>( () => service.IsPressed( MouseButton.Left ) );
		}
	}
}
