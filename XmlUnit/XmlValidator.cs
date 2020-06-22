namespace NetBike.XmlUnit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;

    public class XmlValidator : IXmlValidator
    {
        private const XmlSchemaValidationFlags DocumentSchemasFlags = XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation;

        public XmlValidator()
        {
            this.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
        }

        public XmlSchemaValidationFlags ValidationFlags { get; set; }

        public XmlValidationResult Validate(XDocument document, XmlSchemaSet schemaSet)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (schemaSet == null)
            {
                throw new ArgumentNullException(nameof(schemaSet));
            }

            var errors = new List<XmlValidationError>();
            var settings = this.GetXmlReaderSettings(schemaSet);
            settings.ValidationEventHandler += (o, e) => this.ValidationEventHandler(errors, o, e);

            using (var nodeReader = document.CreateReader())
            using (var xmlReader = XmlReader.Create(nodeReader, settings))
            {
                return this.Validate(xmlReader, schemaSet, errors);
            }
        }

        public XmlValidationResult Validate(TextReader textReader, XmlSchemaSet schemaSet)
        {
            if (textReader == null)
            {
                throw new ArgumentNullException(nameof(textReader));
            }

            if (schemaSet == null)
            {
                throw new ArgumentNullException(nameof(schemaSet));
            }

            var errors = new List<XmlValidationError>();
            var settings = this.GetXmlReaderSettings(schemaSet);
            settings.ValidationEventHandler += (o, e) => this.ValidationEventHandler(errors, o, e);

            using (var xmlReader = XmlReader.Create(textReader, settings))
            {
                return this.Validate(xmlReader, schemaSet, errors);
            }
        }

        private XmlValidationResult Validate(XmlReader xmlReader, XmlSchemaSet schemaSet, List<XmlValidationError> errors)
        {
            try
            {
                while (xmlReader.Read())
                {
                }
            }
            catch (XmlException exception)
            {
                errors.Add(
                    new XmlValidationError(
                        exception.Message,
                        XmlSeverityType.Error,
                        exception.LineNumber,
                        exception.LinePosition));
            }

            return new XmlValidationResult(errors.Count == 0, errors);
        }

        private XmlReaderSettings GetXmlReaderSettings(XmlSchemaSet schemaSet)
        {
            var settings = new XmlReaderSettings();

            if (schemaSet.Count > 0 || (this.ValidationFlags & DocumentSchemasFlags) > 0)
            {
                settings.ValidationType = ValidationType.Schema;
            }

            settings.ValidationFlags = this.ValidationFlags;
            settings.Schemas.Add(schemaSet);
            return settings;
        }

        private void ValidationEventHandler(List<XmlValidationError> errors, object sender, ValidationEventArgs ea)
        {
            var ex = ea.Exception;
            var error = new XmlValidationError(
                ea.Message,
                ea.Severity,
                ex != null ? ex.LineNumber : -1,
                ex != null ? ex.LinePosition : -1);

            errors.Add(error);
        }
    }
}