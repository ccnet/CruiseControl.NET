using System.Windows.Forms;
namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public abstract class ErrorLevel
	{
		public static readonly ErrorLevel Info = new InfoErrorLevel();

		private class InfoErrorLevel : ErrorLevel
		{
			public override ToolTipIcon NotifyInfo
			{
				get { return ToolTipIcon.Info; }
			}
		}

		public static readonly ErrorLevel Warning = new WarningErrorLevel();

		private class WarningErrorLevel : ErrorLevel
		{
			public override ToolTipIcon NotifyInfo
			{
				get { return ToolTipIcon.Warning; }
			}
		}

		public static readonly ErrorLevel Error = new ErrorErrorLevel();

		private class ErrorErrorLevel : ErrorLevel
		{
			public override ToolTipIcon NotifyInfo
			{
				get { return ToolTipIcon.Error; }
			}
		}

		protected ErrorLevel()
		{
		}

        public abstract ToolTipIcon NotifyInfo { get; }
	}
}