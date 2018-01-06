namespace NetBike.XmlUnit.NUnitAdapter
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using global::NUnit.Framework.Constraints;

    public class XmlValidateConstraint : Constraint
    {
        private readonly XmlValidator schemaValidator;
        private readonly XmlSchemaSet schemaSet;
        private XmlValidationResult validationResult;

        public XmlValidateConstraint()
        {
            this.schemaValidator = new XmlValidator();
            this.schemaSet = new XmlSchemaSet();
            this.UseResolver(XmlGlobalsSettings.XmlResolver);
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
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
                    reader = actual as TextReader;
                }
                else if (actual is string)
                {
                    reader = new StringReader(actual as string);
                }
                else if (actual is Stream)
                {
                    reader = new StreamReader(actual as Stream);
                }
                else
                {
                    throw new ArgumentException("Actual must be string, TextReader, Stream or XDocument", "actual");
                }

                this.validationResult = this.schemaValidator.Validate(reader, this.schemaSet);
            }

            return new ConstraintResult(this, actual, ConstraintStatus.Success);
        }

        public XmlValidateConstraint WithSchema(XmlSchema schema)
        {
            this.schemaSet.Add(schema);
            return this;
        }

        public XmlValidateConstraint WithSchema(XmlSchemaSet schemas)
        {
            this.schemaSet.Add(schemas);
            return this;
        }

        public XmlValidateConstraint WithSchema(XmlReader schemaReader)
        {
            return this.WithSchema(string.Empty, schemaReader);
        }

        public XmlValidateConstraint WithSchema(TextReader schemaReader)
        {
            return this.WithSchema(string.Empty, schemaReader);
        }

        public XmlValidateConstraint WithSchema(string schemaPath)
        {
            return this.WithSchema(string.Empty, schemaPath);
        }

        public XmlValidateConstraint WithSchema(Stream stream)
        {
            return this.WithSchema(string.Empty, stream);
        }

        public XmlValidateConstraint WithSchema(string ns, XmlReader schemaReader)
        {
            using (schemaReader)
            {
                return this.AddSchema(ns, schemaReader);
            }
        }

        public XmlValidateConstraint WithSchema(string ns, TextReader schemaReader)
        {
            using (schemaReader)
            using (var reader = XmlReader.Create(schemaReader))
            {
                return this.AddSchema(ns, reader);
            }
        }

        public XmlValidateConstraint WithSchema(string ns, Stream stream)
        {
            using (stream)
            using (var reader = XmlReader.Create(stream))
            {
                return this.AddSchema(ns, reader);
            }
        }

        public XmlValidateConstraint WithSchema(string ns, string schemaPath)
        {
            this.schemaSet.Add(ns, schemaPath);
            return this;
        }

        public XmlValidateConstraint UseResolver(XmlResolver resolver)
        {
            this.schemaSet.XmlResolver = resolver;
            return this;
        }

        public XmlValidateConstraint WithDocumentSchemas(bool validate = true)
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

        public XmlValidateConstraint IgnoreWarnings(bool value)
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

        private XmlValidateConstraint AddSchema(string ns, XmlReader reader)
        {
            this.schemaSet.Add(ns, reader);
            return this;
        }
    }
}