using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class NullSourceControlTest
	{
		private NullSourceControl sourceControl;

		[SetUp]
		public void Setup()
		{
			sourceControl = new NullSourceControl();
		}

		[Test]
		public void ShouldReturnEmptyListOfModifications()
		{
			Assert.AreEqual(0, sourceControl.GetModifications(IntegrationResultMother.CreateSuccessful(DateTime.MinValue), IntegrationResultMother.CreateSuccessful(DateTime.MaxValue)).Length);
		}

		[Test]
		public void ShouldReturnSilentlyForOtherOperations()
		{
			sourceControl.GetSource(null);
			sourceControl.Initialize(null);
			sourceControl.Purge(null);
			sourceControl.LabelSourceControl(null);
		}
	}
}
