using System;
using System.Globalization;
using System.IO;
using NMock;
using NUnit.Framework;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class VssApplyLabelTest : CustomAssertion
	{
		public const string VSS_XML =
			@"<sourceControl type=""vss"">
		<executable>..\tools\vss\ss.exe</executable>
    <ssdir>..\tools\vss</ssdir>
    <project>$/fooProject</project>
    <username>Admin</username>
    <password>admin</password>
	</sourceControl>";	
		
		private IMock _executor;
		private IMock _historyParser;
		
		private Vss _vss;
		
		[SetUp]
		protected void SetUp()
		{
			_vss = CreateVss();
		}
		

		[Test]
		public void ApplyLabelIsDisabledByDefault()
		{
			AssertFalse(_vss.ApplyLabel);    
		}

		[Test]
		public void GetModificationsWhenApplyLabelIsDisabledDoesNotCreateLabels()
		{
			ProcessResult result=new ProcessResult("", "", 0, false);
			Modification[] dummyArray=new Modification[1];
			dummyArray[0]=new Modification();
			_historyParser.SetupResult("Parse", dummyArray, typeof(TextReader), typeof(DateTime), typeof(DateTime));
			_executor.ExpectAndReturn("Execute", result, new NMock.Constraints.IsTypeOf(typeof(ProcessInfo)));
			_executor.ExpectNoCall("Execute", typeof(ProcessInfo));
		    _vss.GetModifications(DateTime.Now, DateTime.Now);
			_executor.Verify();
		}
		[Test]
		public void GetModificationsDoesNotCreateLabelWhenThereAreNoModifications()
		{
			ProcessResult result=new ProcessResult("", "", 0, false);
			Modification[] emptyArray=new Modification[0];
			_historyParser.SetupResult("Parse",emptyArray, typeof(TextReader),typeof(DateTime), typeof(DateTime));
			_executor.ExpectAndReturn("Execute", result, new NMock.Constraints.IsTypeOf(typeof(ProcessInfo)));
			_executor.ExpectNoCall("Execute", typeof(ProcessInfo));
			_vss.GetModifications(DateTime.Now, DateTime.Now);
			_executor.Verify();
		}

		private Vss CreateVss()
		{
			_executor=new DynamicMock(typeof(ProcessExecutor));
			_historyParser=new DynamicMock(typeof(IHistoryParser));
			Vss vss = new Vss((IHistoryParser)_historyParser.MockInstance, (ProcessExecutor)_executor.MockInstance);

			NetReflector.Read(VSS_XML, vss);
			vss.CultureInfo = CultureInfo.InvariantCulture;
			return vss;
		}
	}
}
