using System;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class ProcessSourceControlTest : CustomAssertion
	{
		[Test]
		public void SettingNullTimeoutSetsItToDefault()
		{
			StubSourceControl control = new StubSourceControl(null, null);
			control.Timeout = null;
			Assert.AreEqual(Timeout.DefaultTimeout, control.Timeout);
		}

		[Test]
		public void DeserializesTimeoutAsElement()
		{
			string xml =
			@"<sourceControl type=""vss"" >
				<timeout>1000</timeout>
			</sourceControl>";

			assertTimeoutIs(new Timeout(1000), xml); 
		}

		[Test]
		public void DeserializesTimeoutAsElementWithUnits()
		{
			string xml =
			@"<sourceControl type=""vss"" >
				<timeout units=""minutes"">2</timeout>
			</sourceControl>";
			assertTimeoutIs(new Timeout(2, TimeUnits.MINUTES), xml); 
		}

		[Test]
		public void DeserializesTimeoutAsAttribute()
		{
			string xml =
			@"<sourceControl type=""vss"" timeout=""666"">
			</sourceControl>";
			assertTimeoutIs(new Timeout(666, TimeUnits.MILLIS), xml); 
		}

		[Test]
		public void DeserializesMissingTimeoutAsDefault()
		{
			string xml =
				@"<sourceControl type=""vss"" >
			</sourceControl>";
			assertTimeoutIs(Timeout.DefaultTimeout, xml);
		}

		[Test]
		public void DeserializesEmptyAttributeAsDefault()
		{
			string xml =
				@"<sourceControl type=""vss"" timeout="""">
			</sourceControl>";
			assertTimeoutIs(Timeout.DefaultTimeout, xml);
		}
		
		[Test]
		public void DeserializesEmptyElementAsDefault()
		{
			string xml =
				@"<sourceControl type=""vss"">
				<timeout></timeout>
			</sourceControl>";
			assertTimeoutIs(Timeout.DefaultTimeout, xml);
		}

		[Test]
		public void DeserializesInvalidUnitsAsDefault()
		{
			string xml =
				@"<sourceControl type=""vss"">
				<timeout units=""foot pounds per furlong"">100</timeout>
			</sourceControl>";
			assertTimeoutIs(Timeout.DefaultTimeout, xml);
		}

		private static void assertTimeoutIs(Timeout expected, string xml)
		{
			StubSourceControl sc = new StubSourceControl(null, null);
			NetReflector.Read(xml, sc);
			Assert.AreEqual(expected, sc.Timeout);
		}
	}
	
	class StubSourceControl : ProcessSourceControl
	{
		public StubSourceControl(IHistoryParser historyParser, ProcessExecutor executor) : base(historyParser, executor)
		{
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			throw new NotImplementedException();
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			throw new NotImplementedException();
		}
	}
}
