using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{

	public abstract class ErrorLevel
	{

		public static readonly ErrorLevel Info = new InfoErrorLevel();
		private class InfoErrorLevel : ErrorLevel
		{
			public override NotifyInfoFlags NotifyInfo
			{
				get { return NotifyInfoFlags.Info; }
			}
		}

		
		public static readonly ErrorLevel Warning = new WarningErrorLevel();
		private class WarningErrorLevel : ErrorLevel
		{
			public override NotifyInfoFlags NotifyInfo
			{
				get { return NotifyInfoFlags.Warning; }
			}
		}

		
		public static readonly ErrorLevel Error = new ErrorErrorLevel();
		private class ErrorErrorLevel : ErrorLevel
		{
			public override NotifyInfoFlags NotifyInfo
			{
				get
				{
					return NotifyInfoFlags.Error;			
				}
			}
		}


		
		protected ErrorLevel()
		{
		}
		public abstract NotifyInfoFlags NotifyInfo
		{
			get;
		}
	}

}
