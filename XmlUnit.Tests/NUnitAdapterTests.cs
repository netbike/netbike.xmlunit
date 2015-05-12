namespace NetBike.XmlUnit.Tests
{
    using System.Xml.Linq;
    using NetBike.XmlUnit;
    using NetBike.XmlUnit.NUnitAdapter;
    using NUnit.Framework;

    [TestFixture]
    public class NUnitAdapterCompareTests
    {
        [Test]
        public void XmlCompareConstraintSimpleEqualsTest()
        {
            var expected = XmlSamples.GetContent("attr");
            var actual = XmlSamples.GetContent("attr-with-another-order");
            Assert.That(expected, IsXml.Equals(actual));
        }

        [Test]
        public void XmlCompareConstrainsSimpleEquals()
        {
            var expected = XmlSamples.GetContent("elements");
            var actual = XmlSamples.GetContent("elements-with-comment");
            Assert.That(expected, IsXml.Equals(actual).WithIgnoreComment());
        }

        [Test]
        public void XmlCompareConstrainsXDocumentEqual()
        {
            var expected = XDocument.Parse(XmlSamples.GetContent("elements"));
            var actual = XDocument.Parse(XmlSamples.GetContent("elements-with-comment"));
            Assert.That(expected, IsXml.Equals(actual).WithIgnoreComment());
        }
        
        [Test]
        public void XmlCompareConstrainCustomAnalyzerIgnoreNodeMissing()
        {
            var expected = XmlSamples.GetContent("elements");
            var actual = XmlSamples.GetContent("elements-with-comment");
            Assert.That(
                expected,
                IsXml.Equals(actual)
                    .WithIgnoreComment(false)
                    .UseAnalizer(XmlAnalyzer.Custom()
                        .SetEqual(XmlComparisonType.NodeList)
                        .SetEqual(XmlComparisonType.NodeListLookup)));
        }

        [Test]
        public void XmlCompareConstrainsEqualsWithDifferentOrder()
        {
            var expected = XmlSamples.GetContent("elements");
            var actual = XmlSamples.GetContent("elements-with-different-order");
            Assert.That(
                expected,
                IsXml.Equals(actual).UseAnalizer(XmlAnalyzer.Custom().SetEqual(XmlComparisonType.NodeListSequence)));
        }

        [Test]
        public void XmlCompareConstrainsSimpleSimilarTest()
        {
            var expected = XmlSamples.GetContent("elements");
            var actual = XmlSamples.GetContent("elements-with-different-order");
            Assert.That(expected, IsXml.Similar(actual));
        }

        [Test]
        public void XmlCompareConstrainSimpleDifferentTest()
        {
            var actual = XmlSamples.GetContent("elements");
            var expected = XmlSamples.GetContent("elements-with-comment");
            Assert.That(expected, IsXml.Different(actual).WithIgnoreComment(false));
        }

        [Test]
        public void XmlCompareConstrainWithXsdReference()
        {
            var actual = XmlSamples.GetContent("with-xsd-reference");
            var expected = XmlSamples.GetContent("with-xsd-another-reference");
            Assert.That(
                expected,
                IsXml.Equals(actual)
                    .UseAnalizer(XmlAnalyzer.Custom().SetEqual(XmlComparisonType.SchemaLocation)));
        }
        
        [Test]
        public void XmlCompareConstrainUseNUnitFormatter()
        {
            var actual = XmlSamples.GetContent("elements");
            var expected = XmlSamples.GetContent("elements-with-comment");
            Assert.That(expected, IsXml.Different(actual).WithIgnoreComment(false));
        }

        [Test]
        public void CompareNamespacesWithDifferentPrefixesIsSimilarTest()
        {
            var actual = XmlSamples.GetContent("ns");
            var expected = XmlSamples.GetContent("ns-with-another-prefix");
            Assert.That(expected, IsXml.Similar(actual));
        }
    }
}
