using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("multiTrigger")]
	public class MultipleTrigger : IList, ITrigger
	{
		private ITrigger[] triggers = new ITrigger[0];

		public MultipleTrigger(ITrigger[] triggers)
		{
			this.triggers = triggers;
		}

		public MultipleTrigger() : this(new ITrigger[0])
		{}


		[ReflectorProperty("operator", Required=false)]
		public Operator Operator = Operator.Or;

		[ReflectorArray("triggers", Required=false)]
		public ITrigger[] Triggers
		{
			get { return triggers; }
			set { triggers = value; }
		}

		public void IntegrationCompleted()
		{
			foreach (ITrigger trigger in triggers)
			{
				trigger.IntegrationCompleted();
			}
		}

		public DateTime NextBuild
		{
			get
			{
				DateTime earliestDate = DateTime.MaxValue;
				foreach (ITrigger trigger in triggers)
				{
					if (trigger.NextBuild <= earliestDate)
						earliestDate = trigger.NextBuild;
				}
				return earliestDate;
			}
		}

		public IntegrationRequest Fire()
		{
			IntegrationRequest request = null;
			foreach (ITrigger trigger in triggers)
			{
				IntegrationRequest triggerRequest = trigger.Fire();

				if (Operator == Operator.And && triggerRequest == null) return null;

				if (triggerRequest != null)
				{
					if (request == null || triggerRequest.BuildCondition == BuildCondition.ForceBuild)
						request = triggerRequest;
				}
			}
			return request;
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public int Add(object value)
		{
			ArrayList t = new ArrayList(triggers);
			t.Add(value);
			triggers = (ITrigger[]) t.ToArray(typeof(ITrigger));
			return triggers.Length;
		}

		public bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public int IndexOf(object value)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		public object this[int index]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerator GetEnumerator()
		{
			return triggers.GetEnumerator();
		}
	}

	public enum Operator
	{
		Or,
		And
	}
}