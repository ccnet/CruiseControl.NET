using System;

namespace ThoughtWorks.CruiseControl.ControlPanel.Test
{
	public class MockItem : ConfigurationItem
	{
		private string _value;
		private bool _canHaveChildren;

		public MockItem(string name, string value) : this(name, value, false) {}

		public MockItem(string name, string value, bool canHaveChildren) : base(name, null, null) 
		{
			this._value = value;
			this._canHaveChildren = canHaveChildren;
		}

		public override string ValueAsString
		{
			get { return _value; }
			set { _value = value; }
		}

		public override bool CanHaveChildren
		{
			get { return _canHaveChildren; }
		}
	}
}
