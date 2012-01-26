/*
Purpose: definitinon of the XmlPreprocessor.Exceptions class
Author: Jeremy Lew
Created: 2008.03.24
*/
using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// 	
    /// </summary>
    public sealed class UndefinedSymbolException : EvaluationException
    {
        internal UndefinedSymbolException(string message) : base(message)
        {
        }
        new internal static Exception CreateException(string message, params object[] args)
        {
            return new UndefinedSymbolException(String.Format(message, args));
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public sealed class CyclicalEvaluationException : EvaluationException
    {
        internal CyclicalEvaluationException(string message)
            : base(message)
        {
        }
        new internal static Exception CreateException(string message, params object[] args)
        {
            return new CyclicalEvaluationException(String.Format(CultureInfo.CurrentCulture, message, args));
        }
    }
    /// <summary>
    /// Exception for preprocessor constant evaluation problems
    /// </summary>
    public class EvaluationException : PreprocessorException
    {
        internal EvaluationException(string message) : base( message )
        {
        }

        internal static Exception CreateException(string message, params object[] args)
        {
            return new EvaluationException( String.Format( CultureInfo.CurrentCulture, message, args ) );
        }

        internal static Exception CreateException(string expr, Exception cause)
        {
            var target_ex = cause as TargetInvocationException;
            string cause_msg = target_ex == null ? cause.Message : cause.InnerException.Message;
            return CreateException( "Could not evaluate expression '{0}'\nReason: {1}", expr,
                                    cause_msg );
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public sealed class ExplicitDefinitionRequiredException : EvaluationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplicitDefinitionRequiredException" /> class.	
        /// </summary>
        /// <param name="message">The MSG.</param>
        /// <remarks></remarks>
        public ExplicitDefinitionRequiredException(string message)
            : base( message )
        {
        }

        internal new static Exception CreateException(string message, params object[] args)
        {
            return new ExplicitDefinitionRequiredException( String.Format( CultureInfo.CurrentCulture, message, args ) );
        }
    }


    /// <summary>
    /// 	
    /// </summary>
    public sealed class ImportException : PreprocessorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportException" /> class.	
        /// </summary>
        /// <param name="message">The MSG.</param>
        /// <remarks></remarks>
        public ImportException(string message)
            : base( message )
        {
        }

        internal static Exception CreateException(string message, params object[] args)
        {
            return new ImportException( String.Format( CultureInfo.CurrentCulture, message, args ) );
        }
    }

    /// <summary>
    /// Exception for preprocessor constant definition problems
    /// </summary>
    public sealed class DefinitionException : PreprocessorException
    {
        internal DefinitionException(string message) : base( message )
        {
        }

        internal static Exception CreateException(string message, params object[] args)
        {
            return new EvaluationException( String.Format( CultureInfo.CurrentCulture, message, args ) );
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public sealed class InvalidMarkupException : PreprocessorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMarkupException" /> class.	
        /// </summary>
        /// <param name="message">The MSG.</param>
        /// <remarks></remarks>
        public InvalidMarkupException(string message) : base( message )
        {
        }

        internal static Exception CreateException(string message, params object[] args)
        {
            return new InvalidMarkupException( String.Format( CultureInfo.CurrentCulture, message, args ) );
        }
    }

    /// <summary>
    /// 	
    /// </summary>
    public sealed class MissingIncludeException : PreprocessorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingIncludeException" /> class.	
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <remarks></remarks>
        public MissingIncludeException(string msg)
            : base(msg)
        {
        }

        internal static Exception CreateException(string message, params object[] args)
        {
            return new MissingIncludeException(String.Format(CultureInfo.CurrentCulture, message, args));
        }
    }

    /// <summary>
    /// Base preprocessor exception class
    /// </summary>
    public abstract class PreprocessorException : ApplicationException
    {
        internal PreprocessorException(Exception innerEX) : this( null, innerEX, null )
        {
        }

        internal PreprocessorException(string message) : this( message, null, null )
        {
        }

        internal PreprocessorException(string message, Exception innerEX, XmlContext ctx)
            : base( message, innerEX )
        {
            Context = ctx;
        }

        /// <summary>
        /// Gets or sets the context.	
        /// </summary>
        /// <value>The context.</value>
        /// <remarks></remarks>
        public XmlContext Context { get; internal set; }

        #region Nested type: XmlContext

        /// <summary>
        /// 	
        /// </summary>
        public class XmlContext
        {
            internal XmlContext()
            {
            }

            /// <summary>
            /// Gets or sets the line info.	
            /// </summary>
            /// <value>The line info.</value>
            /// <remarks></remarks>
            public IXmlLineInfo LineInfo { get; internal set; }
            /// <summary>
            /// Gets or sets the path.	
            /// </summary>
            /// <value>The path.</value>
            /// <remarks></remarks>
            public string Path { get; internal set; }

            /// <summary>
            /// Toes the string.	
            /// </summary>
            /// <returns></returns>
            /// <remarks></remarks>
            public override string ToString()
            {
                string path = Path ?? "Unknown";
                string line = "Unknown", pos = "Unknown";
                if ( LineInfo != null && LineInfo.HasLineInfo() )
                {
                    line = LineInfo.LineNumber.ToString(CultureInfo.CurrentCulture);
                    pos = LineInfo.LinePosition.ToString(CultureInfo.CurrentCulture);
                }
                return String.Format( CultureInfo.CurrentCulture, "File: {0} (line {1}, pos{2})", path, line, pos );
            }

            internal static XmlContext CreateFrom(IXmlLineInfo lineInfo)
            {
                return new XmlContext {LineInfo = lineInfo};
            }
        }

        #endregion
    }

    internal sealed class UnexpectedPreprocessorException : PreprocessorException
    {
        private UnexpectedPreprocessorException(Exception innerEX, string message, XmlContext context)
            : base( message, innerEX, context )
        {
            Context = context;
        }

        internal static Exception CreateException(Exception innerEX, XmlContext context)
        {
            return
                new UnexpectedPreprocessorException( innerEX,
                                                     String.Format(
                                                         CultureInfo.CurrentCulture, "Unexpected exception in {0}\n{1}",
                                                         context, innerEX.Message ), context );
        }
    }

    /// <summary>
    /// Exception factory delegate
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    internal delegate Exception ExceptionFactory(string message, params object[] args);
}
