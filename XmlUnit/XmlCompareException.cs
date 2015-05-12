namespace NetBike.XmlUnit
{
    using System;

    [Serializable]
    public class XmlCompareException : Exception
    {
        public XmlCompareException(string message)
            : base(message)
        {
        }

        public XmlCompareException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}