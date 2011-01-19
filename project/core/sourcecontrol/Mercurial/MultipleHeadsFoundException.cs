namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
	/// <summary>
	/// Exception that indicates that a <see cref="Mercurial"/> repository contains multiple heads.
	/// </summary>
	public class MultipleHeadsFoundException : CruiseControlException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultipleHeadsFoundException" /> class.
		/// </summary>
		/// <remarks></remarks>
		public MultipleHeadsFoundException() : base("Multiple Heads found in repository, couldn't choose one to update to.") { }
	}
}
