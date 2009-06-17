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

        [Test, ExpectedException("System.Exception", "Failing GetModifications")]
        public void ShouldFailGetModsWhenFailModsIsTrue()
        {
            sourceControl.FailGetModifications = true;
            sourceControl.GetModifications(null,null);
        }

        [Test, ExpectedException("System.Exception", "Failing getting the source")]
        public void ShouldFailGetSourceWhenFailGetSourceIsTrue()
        {
            sourceControl.FailGetSource = true;
            sourceControl.GetSource(null);
        }

        [Test, ExpectedException("System.Exception", "Failing label source control")]
        public void ShouldFailLabelSourceWhenFailLabelSourceIsTrue()
        {
            sourceControl.FailLabelSourceControl = true;
            sourceControl.LabelSourceControl(null);
        }
  
    }

}
