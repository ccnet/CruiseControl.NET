namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    using System;
    using System.Collections;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// <para>
    /// The Multiple Trigger is used to support the execution of multiple nested triggers. Each trigger will be executed sequentially in the order specified
    /// in the configuration file. By default, if any of the triggers specify that a build should occur then a build will be triggered. The build condition
    /// will be ForceBuild if any trigger returns a ForceBuild condition. Otherwise, the build condition will be IfModificationsExist if any trigger returns
    /// that condition. Multiple Triggers can contain nested multiple triggers.
    /// </para>
    /// <para>
    /// It is possible to change the logical operator applied to assessing the build conditions. If the Multiple Trigger's operator property is set to "And"
    /// then if any trigger says that a build should not happen, then the build will not happen. This is particularly useful when using multiple <link>Filter
    /// Trigger</link>s.
    /// </para>
    /// <para type="info">
    /// Like all triggers, the multiTrigger must be enclosed within a triggers element in the appropriate <link>Project Configuration Block</link>.
    /// </para>
    /// </summary>
    /// <title>Multiple Trigger</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;multiTrigger /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;multiTrigger operator="And"&gt;
    /// &lt;triggers&gt;
    /// &lt;urlTrigger url="http://server/page.html" seconds="30" buildCondition="ForceBuild"/&gt;
    /// &lt;filterTrigger startTime="23:30" endTime="23:45"&gt;
    /// &lt;trigger type="intervalTrigger" seconds="60" /&gt;
    /// &lt;weekDays&gt;
    /// &lt;weekDay&gt;Sunday&lt;/weekDay&gt;
    /// &lt;/weekDays&gt;
    /// &lt;/filterTrigger&gt;
    /// &lt;/triggers&gt;
    /// &lt;/multiTrigger&gt;    
    /// </code>
    /// </example>
    [ReflectorType("multiTrigger")]
	public class MultipleTrigger : IList, ITrigger
	{
		private ITrigger[] triggers = new ITrigger[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleTrigger"/> class.
        /// </summary>
        /// <param name="triggers">The triggers.</param>
		public MultipleTrigger(ITrigger[] triggers)
		{
			this.triggers = triggers;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleTrigger"/> class.
        /// </summary>
		public MultipleTrigger() : this(new ITrigger[0])
		{}

        /// <summary>
        /// The logical operator to apply to the results of the nested triggers (And or Or).
        /// </summary>
        /// <default>Or</default>
        /// <version>1.1</version>
		[ReflectorProperty("operator", Required=false)]
		public Operator Operator = Operator.Or;

        /// <summary>
        /// The nested triggers.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a/</default>
        [ReflectorProperty("triggers", Required = false)]
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