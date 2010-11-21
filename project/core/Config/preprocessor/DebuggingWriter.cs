using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    /// <summary>
    /// 	
    /// </summary>
    public class DebuggingWriter : XmlWriter
    {
        private readonly XmlWriter _real_writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggingWriter" /> class.	
        /// </summary>
        /// <param name="real_writer">The real_writer.</param>
        /// <remarks></remarks>
        public DebuggingWriter(XmlWriter real_writer)
        {
            _real_writer = real_writer;
        }

        /// <summary>
        /// Gets the state of the write.	
        /// </summary>
        /// <value>The state of the write.</value>
        /// <remarks></remarks>
        public override WriteState WriteState
        {
            get { return _real_writer.WriteState; }
        }

        /// <summary>
        /// Writes the start document.	
        /// </summary>
        /// <remarks></remarks>
        public override void WriteStartDocument()
        {
            _real_writer.WriteStartDocument();
        }

        /// <summary>
        /// Writes the start document.	
        /// </summary>
        /// <param name="standalone">The standalone.</param>
        /// <remarks></remarks>
        public override void WriteStartDocument(bool standalone)
        {
            _real_writer.WriteStartDocument( standalone );
        }

        /// <summary>
        /// Writes the end document.	
        /// </summary>
        /// <remarks></remarks>
        public override void WriteEndDocument()
        {
            _real_writer.WriteEndDocument();
        }

        /// <summary>
        /// Writes the type of the doc.	
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pubid">The pubid.</param>
        /// <param name="sysid">The sysid.</param>
        /// <param name="subset">The subset.</param>
        /// <remarks></remarks>
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            _real_writer.WriteDocType( name, pubid, sysid, subset );
        }

        /// <summary>
        /// Writes the start element.	
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="ns">The ns.</param>
        /// <remarks></remarks>
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            _real_writer.WriteStartElement( prefix, localName, ns );
        }

        /// <summary>
        /// Writes the end element.	
        /// </summary>
        /// <remarks></remarks>
        public override void WriteEndElement()
        {
            _real_writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the full end element.	
        /// </summary>
        /// <remarks></remarks>
        public override void WriteFullEndElement()
        {
            _real_writer.WriteFullEndElement();
        }

        /// <summary>
        /// Writes the start attribute.	
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="ns">The ns.</param>
        /// <remarks></remarks>
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            _real_writer.WriteStartAttribute( prefix, localName, ns );
        }

        /// <summary>
        /// Writes the end attribute.	
        /// </summary>
        /// <remarks></remarks>
        public override void WriteEndAttribute()
        {
            _real_writer.WriteEndAttribute();
        }

        /// <summary>
        /// Writes the C data.	
        /// </summary>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
        public override void WriteCData(string text)
        {
            _real_writer.WriteCData( text );
        }

        /// <summary>
        /// Writes the comment.	
        /// </summary>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
        public override void WriteComment(string text)
        {
            _real_writer.WriteComment( text );
        }

        /// <summary>
        /// Writes the processing instruction.	
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
        public override void WriteProcessingInstruction(string name, string text)
        {
            _real_writer.WriteProcessingInstruction( name, text );
        }

        /// <summary>
        /// Writes the entity ref.	
        /// </summary>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
        public override void WriteEntityRef(string name)
        {
            _real_writer.WriteEntityRef( name );
        }

        /// <summary>
        /// Writes the char entity.	
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <remarks></remarks>
        public override void WriteCharEntity(char ch)
        {
            _real_writer.WriteCharEntity( ch );
        }

        /// <summary>
        /// Writes the whitespace.	
        /// </summary>
        /// <param name="ws">The ws.</param>
        /// <remarks></remarks>
        public override void WriteWhitespace(string ws)
        {
            _real_writer.WriteWhitespace( ws );
        }

        /// <summary>
        /// Writes the string.	
        /// </summary>
        /// <param name="text">The text.</param>
        /// <remarks></remarks>
        public override void WriteString(string text)
        {
            _real_writer.WriteString( text );
        }

        /// <summary>
        /// Writes the surrogate char entity.	
        /// </summary>
        /// <param name="lowChar">The low char.</param>
        /// <param name="highChar">The high char.</param>
        /// <remarks></remarks>
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            _real_writer.WriteSurrogateCharEntity( lowChar, highChar );
        }

        /// <summary>
        /// Writes the chars.	
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <remarks></remarks>
        public override void WriteChars(char[] buffer, int index, int count)
        {
            _real_writer.WriteChars( buffer, index, count );
        }

        /// <summary>
        /// Writes the raw.	
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <remarks></remarks>
        public override void WriteRaw(char[] buffer, int index, int count)
        {
            _real_writer.WriteRaw( buffer, index, count );
        }

        /// <summary>
        /// Writes the raw.	
        /// </summary>
        /// <param name="data">The data.</param>
        /// <remarks></remarks>
        public override void WriteRaw(string data)
        {
            _real_writer.WriteRaw( data );
        }

        /// <summary>
        /// Writes the base64.	
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <remarks></remarks>
        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            _real_writer.WriteBase64( buffer, index, count );
        }

        /// <summary>
        /// Closes this instance.	
        /// </summary>
        /// <remarks></remarks>
        public override void Close()
        {
            _real_writer.Close();
        }

        /// <summary>
        /// Flushes this instance.	
        /// </summary>
        /// <remarks></remarks>
        public override void Flush()
        {
            _real_writer.Flush();
        }

        /// <summary>
        /// Lookups the prefix.	
        /// </summary>
        /// <param name="ns">The ns.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string LookupPrefix(string ns)
        {
            return _real_writer.LookupPrefix( ns );
        }
    }
}
