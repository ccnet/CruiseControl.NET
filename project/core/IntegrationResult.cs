using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Contains all the results of a project's integration.
	/// </summary>
	[Serializable]
	public class IntegrationResult
	{
		IntegrationStatus _status = IntegrationStatus.Unknown;
		Modification[] _modifications;
		IntegrationStatus _lastIntegrationStatus = IntegrationStatus.Unknown;
		string _projectName;
		string _label;
		string _output;
		DateTime _startTime;
		DateTime _endTime;
		Exception _exception;

		#region Constructors

		// Default constructor required for serialization
		public IntegrationResult() { }

		public IntegrationResult(string projectName)
		{
			_projectName = projectName;
		}

		#endregion

		#region Read/Write Properties

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

		/// <summary>
		/// The date and time at which the integration commenced.
		/// </summary>
		public DateTime StartTime
		{
			get { return _startTime; }
			set { _startTime = value; }
		}

		/// <summary>
		/// The date and time at which the integration was completed.
		/// </summary>
		public DateTime EndTime
		{
			get { return _endTime; }
			set { _endTime = value; }
		}

		public virtual Modification[] Modifications
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

		#endregion

		#region Readonly properties

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

		public bool Working
		{
			get { return Status == IntegrationStatus.Unknown || Succeeded; }
		}

		/// <summary>
		/// Gets the time taken to perform the project's integration.
		/// </summary>
		public TimeSpan TotalIntegrationTime
		{
			get { return EndTime - StartTime; }
		}

		#endregion

		#region Properties not serialised into Xml

		[XmlIgnore]
		public virtual string Output
		{
			get { return _output; }
			set { _output = value; }
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

		#endregion

		public void MarkStartTime()
		{
			_startTime = DateTime.Now;
		}

		public void MarkEndTime()
		{
			_endTime = DateTime.Now;
		}

		public bool HasModifications()
		{
			return Modifications.Length > 0;
		}

		#region Overridden methods

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
				&& this.EndTime == other.EndTime;
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

		#endregion
	}
}
