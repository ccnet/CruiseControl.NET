using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessArgumentBuilder
	{
		private StringBuilder builder = new StringBuilder();

		public void AppendArgument(string format, string value)
		{
			if (StringUtil.IsBlank(value)) return;

			AppendSpaceIfNotEmpty();
			builder.AppendFormat(format, value);
		}

		public void AppendArgument(string value)
		{
			if (StringUtil.IsBlank(value)) return;

			AppendSpaceIfNotEmpty();
			builder.Append(value);
		}

		private void AppendSpaceIfNotEmpty()
		{
			if (builder.Length > 0) builder.Append(" ");
		}

		public void AppendIf(bool condition, string value)
		{
			if (condition) AppendArgument(value);
		}

		public void AppendIf(bool condition, string format, string argument)
		{
			if (condition) AppendArgument(format, argument);
		}

		public void Append(string text)
		{
			builder.Append(text);
		}

//		public string Separator = ":";

		public void AddArgument(string arg, string value)
		{
			AddArgument(arg, " ", value);
		}

		public void AddArgument(string arg, string separator, string value)
		{
			if (StringUtil.IsBlank(value)) return;
			AppendSpaceIfNotEmpty();

			builder.Append(string.Format("{0}{1}{2}", arg, separator, SurroundInQuotesIfContainsSpace(value)));
		}

		public void AddArgument(string value)
		{
			if (StringUtil.IsBlank(value)) return;
			AppendSpaceIfNotEmpty();

			builder.Append(SurroundInQuotesIfContainsSpace(value));			
		}
		
		private string SurroundInQuotesIfContainsSpace(string value)
		{
			if (! StringUtil.IsBlank(value) && value.IndexOf(' ') >= 0)
				return string.Format(@"""{0}""", value);
			return value;
		}

		public override string ToString()
		{
			return builder.ToString();
		}
	}
}