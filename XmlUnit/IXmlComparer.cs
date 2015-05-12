namespace NetBike.XmlUnit
{
    using System.IO;

    public interface IXmlComparer
    {
        XmlComparisonResult Compare(TextReader expectedReader, TextReader actualReader);
    }
}