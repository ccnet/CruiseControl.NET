using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	[Serializable]
	public class IntegrationResult
	{
		private string _projectName;
		private IntegrationStatus _status = IntegrationStatus.Unknown;
		private string _label;
		private DateTime _startTime;
		private DateTime _endTime;
		private string _output;
		private Modification[] _modifications;
		private IntegrationStatus _lastIntegrationStatus = IntegrationStatus.Unknown;
		private Exception _exception;

		// Default constructor required for serialization
		public IntegrationResult() { }

		public IntegrationResult(string projectName)
		{
			_projectName = projectName;
		}

		public string ProjectName
		{
			get { return _projectName; }
			set { _projectName = value; }
		}

		public IntegrationStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		public string Label
		{
			get { return _label; }
			set { _label = value; }
		}

		public DateTime StartTime
		{
			get { return _startTime; }
			set { _startTime = value; }
		}

		public DateTime EndTime
		{
			get { return _endTime; }
			set { _endTime = value; }
		}

		public string Output
		{
			get { return _output; }
			set { _output = value; }
		}

		public Modification[] Modifications
		{
			get
			{
				if (_modifications == null)
				{
					_modifications = new Modification[0];
				}
				return _modifications;
			}
			set { _modifications = value; }
		}

		public IntegrationStatus LastIntegrationStatus
		{
			get { return _lastIntegrationStatus; }
			set { _lastIntegrationStatus = value; }
		}

		[XmlIgnore] // Exceptions cannot be serialised because of permission attributes
		public Exception ExceptionResult
		{
			get { return _exception; }
			set 
			{ 
				_exception = value; 
				if (_exception != null)
				{
					Status = IntegrationStatus.Exception;
				}
			}
		}

		public void Start()
		{
			_startTime = DateTime.Now;
		}

		public void End()
		{
			_endTime = DateTime.Now;
		}

		public bool ShouldRunIntegration()
		{
			return Modifications.Length > 0;
		}

		public DateTime LastModificationDate
		{
			get
			{
				DateTime latestDate = DateTime.MinValue;
				if (Modifications.Length == 0)
				{
					latestDate = DateTime.Now;
					latestDate = latestDate.AddDays(-1.0);
				}

				foreach (Modification modification in Modifications)
				{
					latestDate = DateUtil.MaxDate(modification.ModifiedTime, latestDate);
				}
				return latestDate;
			}
		}

		public bool Succeeded
		{
			get { return Status == IntegrationStatus.Success; }
		}

		public bool Failed
		{
			get { return Status == IntegrationStatus.Failure; }
		}

		public bool Fixed
		{
			get { return Succeeded && LastIntegrationStatus == IntegrationStatus.Failure; }
		}

		public TimeSpan TotalIntegrationTime
		{
			get { return EndTime - StartTime; }
		}

		public override bool Equals(object obj)
		{
			IntegrationResult other = obj as IntegrationResult;
			if (other == null)
			{
				return false;
			}
			return this.ProjectName == other.ProjectName
				&& this.Status == other.Status
				&& this.Label == other.Label
				&& this.StartTime == other.StartTime
				&& this.EndTime == other.EndTime
				&& this.Output == other.Output;
				// && this.ExceptionResult == other.ExceptionResult;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return ReflectionUtil.ReflectionToString(this);
		}
	}
}
