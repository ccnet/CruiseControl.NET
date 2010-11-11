/*
Purpose: definitinon of the XmlPreprocessor.Exceptions class
Author: Jeremy Lew
Created: 2008.03.24
*/
using System;
using System.Reflection;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    public class UndefinedSymbolException : EvaluationException
    {
        internal UndefinedSymbolException(string msg) : base(msg)
        {
        }
        internal static Exception CreateException(string msg, params object[] args)
        {
            return new UndefinedSymbolException(String.Format(msg, args));
        }
    }

    public class CyclicalEvaluationException : EvaluationException
    {
        internal CyclicalEvaluationException(string msg)
            : base(msg)
        {
        }
        internal static Exception CreateException(string msg, params object[] args)
        {
            return new CyclicalEvaluationException(String.Format(msg, args));
        }
    }
    /// <summary>
    /// Exception for preprocessor constant evaluation problems
    /// </summary>
    public class EvaluationException : PreprocessorException
    {
        internal EvaluationException(string msg) : base( msg )
        {
        }

        internal static Exception CreateException(string msg, params object[] args)
        {
            return new EvaluationException( String.Format( msg, args ) );
        }

        internal static Exception CreateException(string expr, Exception cause)
        {
            var target_ex = cause as TargetInvocationException;
            string cause_msg = target_ex == null ? cause.Message : cause.InnerException.Message;
            return CreateException( "Could not evaluate expression '{0}'\nReason: {1}", expr,
                                    cause_msg );
        }
    }

    public class ExplicitDefinitionRequiredException : EvaluationException
    {
        public ExplicitDefinitionRequiredException(string msg)
            : base( msg )
        {
        }

        internal new static Exception CreateException(string msg, params object[] args)
        {
            return new ExplicitDefinitionRequiredException( String.Format( msg, args ) );
        }
    }


    public class ImportException : PreprocessorException
    {
        public ImportException(string msg)
            : base( msg )
        {
        }

        internal static Exception CreateException(string msg, params object[] args)
        {
            return new ImportException( String.Format( msg, args ) );
        }
    }

    /// <summary>
    /// Exception for preprocessor constant definition problems
    /// </summary>
    public class DefinitionException : PreprocessorException
    {
        internal DefinitionException(string msg) : base( msg )
        {
        }

        internal static Exception CreateException(string msg, params object[] args)
        {
            return new EvaluationException( String.Format( msg, args ) );
        }
    }

    public class InvalidMarkupException : PreprocessorException
    {
        public InvalidMarkupException(string msg) : base( msg )
        {
        }

        internal static Exception CreateException(string msg, params object[] args)
        {
            return new InvalidMarkupException( String.Format( msg, args ) );
        }
    }

    /// <summary>
    /// Base preprocessor exception class
    /// </summary>
    public abstract class PreprocessorException : ApplicationException
    {
        internal PreprocessorException(Exception inner_ex) : this( null, inner_ex, null )
        {
        }

        internal PreprocessorException(string msg) : this( msg, null, null )
        {
        }

        internal PreprocessorException(string msg, Exception inner_ex, XmlContext ctx)
            : base( msg, inner_ex )
        {
            Context = ctx;
        }

        public XmlContext Context { get; internal set; }

        #region Nested type: XmlContext

        public class XmlContext
        {
            internal XmlContext()
            {
            }

            public IXmlLineInfo LineInfo { get; internal set; }
            public string Path { get; internal set; }

            public override string ToString()
            {
                string path = Path ?? "Unknown";
                string line = "Unknown", pos = "Unknown";
                if ( LineInfo != null && LineInfo.HasLineInfo() )
                {
                    line = LineInfo.LineNumber.ToString();
                    pos = LineInfo.LinePosition.ToString();
                }
                return String.Format( "File: {0} (line {1}, pos{2})", path, line, pos );
            }

            internal static XmlContext CreateFrom(IXmlLineInfo line_info)
            {
                return new XmlContext {LineInfo = line_info};
            }
        }

        #endregion
    }

    internal class UnexpectedPreprocessorException : PreprocessorException
    {
        private UnexpectedPreprocessorException(Exception inner_ex, string msg, XmlContext context)
            : base( msg, inner_ex, context )
        {
            Context = context;
        }

        internal static Exception CreateException(Exception inner_ex, XmlContext context)
        {
            return
                new UnexpectedPreprocessorException( inner_ex,
                                                     String.Format(
                                                         "Unexpected exception in {0}\n{1}",
                                                         context, inner_ex.Message ), context );
        }
    }

    /// <summary>
    /// Exception factory delegate
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    internal delegate Exception ExceptionFactory(string msg, params object[] args);
}
