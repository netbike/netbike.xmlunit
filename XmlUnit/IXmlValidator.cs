namespace NetBike.XmlUnit
{
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Schema;

    public interface IXmlValidator
    {
        XmlValidationResult Validate(TextReader textReader, XmlSchemaSet schemaSet);
    }
}