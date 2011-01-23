namespace CruiseControl.Core.Tests
{
    using System.Xml;
    using Moq;

    public static class XmlWriterHelper
    {
        public static void MockWriteElementString(this Mock<XmlWriter> writerMock, string name, string value)
        {
            writerMock.Setup(w => w.WriteStartElement(null, name, null)).Verifiable();
            writerMock.Setup(w => w.WriteString(value)).Verifiable();
            writerMock.Setup(w => w.WriteEndElement()).Verifiable();
        }

        public static void MockWriteAttributeString(this Mock<XmlWriter> writerMock, string name, string value)
        {
            writerMock.Setup(w => w.WriteStartAttribute(null, name, null)).Verifiable();
            writerMock.Setup(w => w.WriteString(value)).Verifiable();
            writerMock.Setup(w => w.WriteEndAttribute()).Verifiable();
        }
    }
}
