namespace NetBike.XmlUnit
{
    using System.Xml.Schema;

    public class XmlValidationError
    {
        private readonly string message;
        private readonly XmlSeverityType severityType;
        private readonly int lineNumber;
        private readonly int linePosition;

        public XmlValidationError(string message, XmlSeverityType severityType, int lineNumber, int linePosition)
        {
            this.message = message;
            this.severityType = severityType;
            this.lineNumber = lineNumber;
            this.linePosition = linePosition;
        }

        public string Message
        {
            get { return this.message; }
        }

        public XmlSeverityType SeverityType
        {
            get { return this.severityType; }
        }

        public int LineNumber
        {
            get { return this.lineNumber; }
        }

        public int LinePosition
        {
            get { return this.linePosition; }
        }
    }
}