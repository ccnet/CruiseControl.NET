using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Config.Preprocessor
{
    public class DebuggingWriter : XmlWriter
    {
        private readonly XmlWriter _real_writer;

        public DebuggingWriter(XmlWriter real_writer)
        {
            _real_writer = real_writer;
        }

        public override WriteState WriteState
        {
            get { return _real_writer.WriteState; }
        }

        public override void WriteStartDocument()
        {
            _real_writer.WriteStartDocument();
        }

        public override void WriteStartDocument(bool standalone)
        {
            _real_writer.WriteStartDocument( standalone );
        }

        public override void WriteEndDocument()
        {
            _real_writer.WriteEndDocument();
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            _real_writer.WriteDocType( name, pubid, sysid, subset );
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            _real_writer.WriteStartElement( prefix, localName, ns );
        }

        public override void WriteEndElement()
        {
            _real_writer.WriteEndElement();
        }

        public override void WriteFullEndElement()
        {
            _real_writer.WriteFullEndElement();
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            _real_writer.WriteStartAttribute( prefix, localName, ns );
        }

        public override void WriteEndAttribute()
        {
            _real_writer.WriteEndAttribute();
        }

        public override void WriteCData(string text)
        {
            _real_writer.WriteCData( text );
        }

        public override void WriteComment(string text)
        {
            _real_writer.WriteComment( text );
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            _real_writer.WriteProcessingInstruction( name, text );
        }

        public override void WriteEntityRef(string name)
        {
            _real_writer.WriteEntityRef( name );
        }

        public override void WriteCharEntity(char ch)
        {
            _real_writer.WriteCharEntity( ch );
        }

        public override void WriteWhitespace(string ws)
        {
            _real_writer.WriteWhitespace( ws );
        }

        public override void WriteString(string text)
        {
            _real_writer.WriteString( text );
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            _real_writer.WriteSurrogateCharEntity( lowChar, highChar );
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            _real_writer.WriteChars( buffer, index, count );
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            _real_writer.WriteRaw( buffer, index, count );
        }

        public override void WriteRaw(string data)
        {
            _real_writer.WriteRaw( data );
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            _real_writer.WriteBase64( buffer, index, count );
        }

        public override void Close()
        {
            _real_writer.Close();
        }

        public override void Flush()
        {
            _real_writer.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return _real_writer.LookupPrefix( ns );
        }
    }
}
