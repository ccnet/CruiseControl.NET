using System;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Implemented by a source control that needs notifications of when to create and delete
	/// a temporary label.
	/// </summary>
	public interface ITemporaryLabeller : ISourceControl
	{
		void CreateTemporaryLabel();

		void DeleteTemporaryLabel();
	}
}
