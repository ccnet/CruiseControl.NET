using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("urlTrigger")]
	public class UrlTrigger : IntervalTrigger
	{
		private HttpWrapper httpRequest;
		private DateTime lastModified;
		private Uri uri;

		public UrlTrigger() : this(new DateTimeProvider(), new HttpWrapper())
		{}

		public UrlTrigger(DateTimeProvider dtProvider, HttpWrapper httpWrapper) : base(dtProvider)
		{
			this.httpRequest = httpWrapper;
		}

		[ReflectorProperty("url", Required=true)]
		public virtual string Url
		{
			get { return uri.ToString(); }
			set { uri = new Uri(value); }
		}

		public override BuildCondition ShouldRunIntegration()
		{
			BuildCondition condition = base.ShouldRunIntegration();
			if (condition == BuildCondition.NoBuild) return condition;

			Log.Debug(string.Format("More than {0} seconds since last integration, checking url.", IntervalSeconds));
			if (HasUrlChanged())
			{
				return BuildCondition;
			}
			
			IncrementNextBuildTime();
			return BuildCondition.NoBuild;
		}

		private bool HasUrlChanged()
		{
			try
			{
				DateTime newModifiedTime = httpRequest.GetLastModifiedTimeFor(uri, lastModified);
				if (newModifiedTime > lastModified)
				{
					lastModified = newModifiedTime;
					return true;
				}
			}
			catch (Exception e)
			{
				Log.Error("Error accessing url: " + uri);
				Log.Error(e);
			}
			return false;
		}
	}
}