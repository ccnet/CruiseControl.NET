using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class IntegrationRequestQueueTest
	{
		private IntegrationRequestQueue queue;

		[SetUp]
		public void CreateIntegrationRequestQueue()
		{
			queue = new IntegrationRequestQueue();
		}

		[Test]
		public void SingleForceBuildRequestShouldBeRetrievableFromQueue()
		{
			queue.RequestBuild(BuildCondition.ForceBuild);
			Assert.IsTrue(queue.HasPendingRequests());
			Assert.AreEqual(BuildCondition.ForceBuild, queue.WaitForRequest());
		}

		[Test]
		public void SingleModificationBuildRequestShouldBeRetrievableFromQueue()
		{
			queue.RequestBuild(BuildCondition.IfModificationExists);
			Assert.IsTrue(queue.HasPendingRequests());
			Assert.AreEqual(BuildCondition.IfModificationExists, queue.WaitForRequest());
		}

		[Test]
		public void QueueShouldInitialBeEmpty()
		{
			Assert.IsFalse(queue.HasPendingRequests());
		}

		[Test]
		public void QueueShouldBeEmptyAfterRequestIsRetrieved()
		{
			queue.RequestBuild(BuildCondition.IfModificationExists);
			queue.WaitForRequest();
			Assert.IsFalse(queue.HasPendingRequests());
		}

		[Test]
		public void QueueShouldOnlyContainASingleRequest()
		{
			queue.RequestBuild(BuildCondition.IfModificationExists);
			queue.RequestBuild(BuildCondition.IfModificationExists);
			queue.WaitForRequest();
			Assert.IsFalse(queue.HasPendingRequests());
		}

		[Test]
		public void IfForceBuildIsRequestedAfterModificationBuildThenForceBuildShouldBeRetrieved()
		{
			queue.RequestBuild(BuildCondition.IfModificationExists);
			queue.RequestBuild(BuildCondition.ForceBuild);
			Assert.AreEqual(BuildCondition.ForceBuild, queue.WaitForRequest());
			Assert.IsFalse(queue.HasPendingRequests());
		}

		[Test]
		public void IfModificationBuildIsRequestedAfterForceBuildThenForceBuildShouldBeRetrieved()
		{
			queue.RequestBuild(BuildCondition.ForceBuild);
			queue.RequestBuild(BuildCondition.IfModificationExists);
			Assert.AreEqual(BuildCondition.ForceBuild, queue.WaitForRequest());
			Assert.IsFalse(queue.HasPendingRequests());
		}

		[Test]
		public void WaitForRequestShouldBlockUntilNewBuildIsRequested()
		{
			int processedRequests = 0;
			int processedForcedBuildRequests = 0;
			int processedModExistsRequests = 0;

			new Thread(new ThreadStart(SpawnNewThreads)).Start();

			completedThreads = 0;
			while (completedThreads < totalThreads)
			{
				processedRequests++;
				BuildCondition condition = queue.WaitForRequest();
				if (condition == BuildCondition.ForceBuild) processedForcedBuildRequests++;
				else if (condition == BuildCondition.IfModificationExists) processedModExistsRequests++;
				else Assert.Fail("Unexpected build request");
			}
//			Console.Out.WriteLine("processedRequests = {0}", processedRequests);
//			Console.Out.WriteLine("force build requests = {0}", processedForcedBuildRequests);
//			Console.Out.WriteLine("mod build requests = {0}", processedModExistsRequests);
		}

		private int completedThreads = 0;
		private int totalThreads = 400;

		private void SpawnNewThreads()
		{
			for (int i = 0; i < totalThreads; i++)
			{
				Thread thread = new Thread(new ThreadStart(RequestNewBuild));
				thread.Priority = ThreadPriority.BelowNormal;
				thread.Start();				
			}			
		}

		private void RequestNewBuild()
		{
			Thread.Sleep(new Random().Next(4));
			queue.RequestBuild(BuildCondition.ForceBuild);
			Thread.Sleep(new Random().Next(4));
			queue.RequestBuild(BuildCondition.IfModificationExists);
			Interlocked.Increment(ref completedThreads);
		}

		public class IntegrationRequestQueue
		{
			private static readonly BuildRequest NoBuildRequested = new BuildRequest(BuildCondition.NoBuild);
			private BuildRequest request = NoBuildRequested;
			private ManualResetEvent latch = new ManualResetEvent(false);

			public void RequestBuild(BuildCondition condition)
			{
				lock (this)
				{
					if (request.IsHigherPriority(condition))
					{
						request = new BuildRequest(condition);
						latch.Set();
					}
				}
			}

			public bool HasPendingRequests()
			{
				lock(this)
				{
					return request != NoBuildRequested;
				} 
			}

			public BuildCondition WaitForRequest()
			{
				if (! HasPendingRequests())
				{
					latch.WaitOne();
				}

				lock (this)
				{
					BuildCondition result = request.Condition;
					request = NoBuildRequested;
					latch.Reset();

					Debug.Assert(result != BuildCondition.NoBuild);
					return result;					
				}
			}
		}

		public class BuildRequest
		{
			private readonly BuildCondition condition;

			public BuildRequest(BuildCondition condition)
			{
				this.condition = condition;
			}

			public bool IsHigherPriority(BuildCondition condition)
			{
				return this.condition < condition;
			}

			public BuildCondition Condition
			{
				get { return condition; }
			}
		}
	}
}