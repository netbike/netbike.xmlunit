namespace NetBike.XmlUnit
{
    using System.Collections.Generic;
    using System.Text;

    public sealed class XmlValidationResult
    {
        private readonly List<XmlValidationError> errors;

        private readonly bool isSuccess;

        public XmlValidationResult(bool isSuccess, IEnumerable<XmlValidationError> errors)
        {
            this.errors = new List<XmlValidationError>();

            if (errors != null)
            {
                this.errors.AddRange(errors);
            }

            this.isSuccess = isSuccess;
        }

        public bool IsSuccess
        {
            get { return this.isSuccess; }
        }

        public IEnumerable<XmlValidationError> Errors
        {
            get { return this.errors; }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (this.isSuccess)
            {
                builder.AppendLine("Schema validation was successful.");
            }
            else
            {
                builder.AppendLine("Xml document does not match schema.");
            }

            foreach (var error in this.errors)
            {
                builder.AppendLine(
                    string.Format(
                        "XML {0}: {1} ({2}:{3})",
                        error.SeverityType,
                        error.Message,
                        error.LineNumber,
                        error.LinePosition));
            }

            return builder.ToString();
        }
    }
}