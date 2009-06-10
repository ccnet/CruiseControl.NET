using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    /// <summary>
    /// The Func delegate from .NET 3.5.
    /// </summary>
    /// <remarks>This can be removed when CC.NET migrates to .NET 3.5.</remarks>
    public delegate TResult Func<T, TResult>(T value);

    /// <summary>
    /// The Func delegate from .NET 3.5.
    /// </summary>
    /// <remarks>This can be removed when CC.NET migrates to .NET 3.5.</remarks>
    public delegate TResult Func<TResult>();
}
