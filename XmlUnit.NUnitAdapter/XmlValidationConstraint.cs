namespace NetBike.XmlUnit.NUnitAdapter
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using NUnit.Framework.Constraints;

    public class XmlValidationConstraint : Constraint
    {
        private readonly XmlValidator schemaValidator;
        private readonly XmlSchemaSet schemaSet;
        private XmlValidationResult validationResult;

        public XmlValidationConstraint()
        {
            this.schemaValidator = new XmlValidator();
            this.schemaSet = new XmlSchemaSet();
            this.UseResolver(XmlGlobalsSettings.XmlResolver);
        }

        public override bool Matches(object actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException("actual");
            }

            var document = actual as XDocument;

            if (document != null)
            {
                this.validationResult = this.schemaValidator.Validate(document, this.schemaSet);
            }
            else
            {
                TextReader reader;

                if (actual is TextReader)
                {
                    reader = (TextReader)actual;
                }
                else if (actual is string)
                {
                    reader = new StringReader((string)actual);
                }
                else if (actual is Stream)
                {
                    reader = new StreamReader((Stream)actual);
                }
                else
                {
                    throw new ArgumentException("Actual must be string, TextReader, Stream or XDocument", "actual");
                }

                this.validationResult = this.schemaValidator.Validate(reader, this.schemaSet);
            }

            return this.validationResult.IsSuccess;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write(this.validationResult.ToString());
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            this.WriteDescriptionTo(writer);
        }

        public XmlValidationConstraint WithSchema(XmlSchema schema)
        {
            this.schemaSet.Add(schema);
            return this;
        }

        public XmlValidationConstraint WithSchema(XmlSchemaSet schemas)
        {
            this.schemaSet.Add(schemas);
            return this;
        }

        public XmlValidationConstraint WithSchema(XmlReader schemaReader)
        {
            return this.WithSchema(string.Empty, schemaReader);
        }

        public XmlValidationConstraint WithSchema(TextReader schemaReader)
        {
            return this.WithSchema(string.Empty, schemaReader);
        }

        public XmlValidationConstraint WithSchema(string schemaPath)
        {
            return this.WithSchema(string.Empty, schemaPath);
        }

        public XmlValidationConstraint WithSchema(Stream stream)
        {
            return this.WithSchema(string.Empty, stream);
        }

        public XmlValidationConstraint WithSchema(string ns, XmlReader schemaReader)
        {
            using (schemaReader)
            {
                return this.AddSchema(ns, schemaReader);
            }
        }

        public XmlValidationConstraint WithSchema(string ns, TextReader schemaReader)
        {
            using (schemaReader)
            using (var reader = XmlReader.Create(schemaReader))
            {
                return this.AddSchema(ns, reader);
            }
        }

        public XmlValidationConstraint WithSchema(string ns, Stream stream)
        {
            using (stream)
            using (var reader = XmlReader.Create(stream))
            {
                return this.AddSchema(ns, reader);
            }
        }

        public XmlValidationConstraint WithSchema(string ns, string schemaPath)
        {
            this.schemaSet.Add(ns, schemaPath);
            return this;
        }

        public XmlValidationConstraint UseResolver(XmlResolver resolver)
        {
            this.schemaSet.XmlResolver = resolver;
            return this;
        }

        public XmlValidationConstraint WithDocumentSchemas(bool validate = true)
        {
            var schemasFlags = XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation;

            if (validate)
            {
                this.schemaValidator.ValidationFlags |= schemasFlags;
            }
            else
            {
                this.schemaValidator.ValidationFlags &= ~schemasFlags;
            }

            return this;
        }

        public XmlValidationConstraint IgnoreWarnings(bool value)
        {
            if (value)
            {
                this.schemaValidator.ValidationFlags &= ~XmlSchemaValidationFlags.ReportValidationWarnings;
            }
            else
            {
                this.schemaValidator.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            }

            return this;
        }

        private XmlValidationConstraint AddSchema(string ns, XmlReader reader)
        {
            this.schemaSet.Add(ns, reader);
            return this;
        }
    }
}