using System.IO;
using System.Reflection;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Logging;

namespace ThoughtWorks.CruiseControl.UnitTests.Logging
{
	[TestFixture()]
	public class CircularArrayTest
	{
		private static readonly string TestResourceName = @"ThoughtWorks.CruiseControl.UnitTests.Core.Log.TestLog.txt";
		private CircularArray arrayStringBuffer;

		[SetUp]
		public void SetUp()
		{
			arrayStringBuffer = new CircularArray(20);
		}

		[Test]
		public void ReadZeroLines()
		{
			Assert.AreEqual(string.Empty, arrayStringBuffer.ToString(EnumeratorDirection.Backward), "Should be blank when no lines added");
			Assert.AreEqual(string.Empty, arrayStringBuffer.ToString(EnumeratorDirection.Forward), "Should be blank when no lines added");
		}

		[Test]
		public void ReadOneLine()
		{
			AddLines(1);

			string newest = arrayStringBuffer.ToString(EnumeratorDirection.Backward);
			Assert.AreEqual(CircularArrayTestResources.SingleLine, newest, "Wrong line read");
			
			string oldest = arrayStringBuffer.ToString(EnumeratorDirection.Forward);
			Assert.AreEqual(CircularArrayTestResources.SingleLine, oldest, "Wrong line read");

			Assert.AreEqual(newest, oldest, "NewestFirst and OldestFirst should be identical for single line");
		}

		[Test]
		public void ReadFiveLines()
		{
			arrayStringBuffer = new CircularArray(5);
			AddLines(5);
			Assert.AreEqual(CircularArrayTestResources.ForwardFive, arrayStringBuffer.ToString(EnumeratorDirection.Forward), "Wrong lines returned when filling array");
			Assert.AreEqual(CircularArrayTestResources.BackwardFive, arrayStringBuffer.ToString(EnumeratorDirection.Backward), "Wrong lines returned");
		}

		[Test]
		public void ReadFewerLinesThanCapacity()
		{
			arrayStringBuffer = new CircularArray(30);
			AddLines(20);
			Assert.AreEqual(CircularArrayTestResources.BackwardThirty, arrayStringBuffer.ToString(EnumeratorDirection.Backward), "Wrong lines returned when underfilling array");
			Assert.AreEqual(CircularArrayTestResources.ForwardThirty, arrayStringBuffer.ToString(EnumeratorDirection.Forward), "Wrong lines returned when underfilling array");
		}
		
		[Test]
		public void ReadMoreLinesThanCapacity()
		{
			arrayStringBuffer = new CircularArray(5);
			AddLines(20);
			Assert.AreEqual(CircularArrayTestResources.ForwardLastFive, arrayStringBuffer.ToString(EnumeratorDirection.Forward), "Wrong lines returned when overfilling array");
			Assert.AreEqual(CircularArrayTestResources.BackwardLastFive, arrayStringBuffer.ToString(EnumeratorDirection.Backward), "Wrong lines returned when overfilling array");
		}

		private void AddLines(int numberOfLines)
		{
			using (StreamReader Reader = new StreamReader(GetTestStream()))
			{
				for(int i = 0; i < numberOfLines; i++)
				{
					arrayStringBuffer.Add(Reader.ReadLine());
				}
			}
		}

		private Stream GetTestStream()
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(TestResourceName);
		}
	}
}
