namespace NetBike.XmlUnit.Tests
{
    using NetBike.XmlUnit.NUnitAdapter;
    using NUnit.Framework;

    [TestFixture]
    public class NUnitAdapterValidationTest: TestBase
    {
        [Test]
        public void XmlValidationConstraintSchemaWithNamespaceTest()
        {
            var document = XmlSamples.GetContent("ns");
            var emptyNamespaceSchemaPath = XmlSamples.GetFullPathByName(@"Xsd/ns.xsd");
            var namespaceSchemaPath = XmlSamples.GetFullPathByName(@"Xsd/ns_app1.xsd");
            string nameSpace = @"http://example.org";

            Assert.That(document, IsXml.Valid().WithSchema(emptyNamespaceSchemaPath).WithSchema(nameSpace, namespaceSchemaPath));
        }

        [Test]
        public void XmlValidationConstraintWithDocumentsSchema()
        {
            var document = XmlSamples.GetContent("with-xsd-reference");
            Assert.That(document, IsXml.Valid().WithDocumentSchemas());
        }

        [Test]
        public void XmlValidationConstraintTextReaderSchema()
        {
            var document = XmlSamples.GetContent("ns-without-namespace");
            var namespaceSchemaPath = XmlSamples.GetFullPathByName(@"Xsd/ns-without-namespace.xsd");

            Assert.That(
                document,
                IsXml.Valid().WithSchema(namespaceSchemaPath));
        }

        [Test]
        public void XmlValidationSimpleTest()
        {
            var document = XmlSamples.GetContent("ns");
            Assert.That(document, IsXml.Valid());
        }
    }
}