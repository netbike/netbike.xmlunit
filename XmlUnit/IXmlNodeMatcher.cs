namespace NetBike.XmlUnit
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public interface IXmlNodeMatcher
    {
        IEnumerable<XmlNodeMatch> MatchNodes(IEnumerable<XNode> expectedNodes, IEnumerable<XNode> actualNodes);
    }
}