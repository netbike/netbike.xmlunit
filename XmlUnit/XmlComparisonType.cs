namespace NetBike.XmlUnit
{
    /// <summary>
    /// Kind of comparison.
    /// <remarks>Inspired by http://xmlunit.org.</remarks>
    /// </summary>
    public enum XmlComparisonType
    {
        /// <summary>
        /// Do both documents have a declaration (or neither of each)?
        /// </summary>
        Declaration,

        /// <summary>
        /// Do both documents specify the same version in their XML declaration?
        /// </summary>
        Version,

        /// <summary>
        /// Do both documents specify the same standalone declaration in their XML declaration?
        /// </summary>
        Standalone,

        /// <summary>
        /// Do both documents specify the same encoding in their XMLdeclaration?
        /// </summary>
        Encoding,

        /// <summary>
        /// Do both documents have a DOCTYPE (or neither of each)?
        /// </summary>
        Doctype,

        /// <summary>
        /// If the documents both have DOCTYPEs, compare the names.
        /// </summary>
        DoctypeName,

        /// <summary>
        /// If the documents both have DOCTYPEs, compare the PUBLIC identifiers.
        /// </summary>
        DoctypePublicId,

        /// <summary>
        /// If the documents both have DOCTYPEs, compare the SYSTEM identifiers.
        /// </summary>
        DoctypeSystemId,

        /// <summary>
        /// Check whether both documents provide the same values for xsi:schemaLocation or xsi:noNamespaceSchemaLocation (may even be null).
        /// </summary>
        SchemaLocation,

        /// <summary>
        /// Compare the node types.
        /// </summary>
        NodeType,

        /// <summary>
        /// Compare the node's namespace prefixes.
        /// </summary>
        NamespacePrefix,

        /// <summary>
        /// Compare the node's namespace URIs.
        /// </summary>
        NamespaceUri,

        /// <summary>
        /// Compare targets of processing instructions.
        /// </summary>
        ProcessingInstructionTarget,

        /// <summary>
        /// Compare data of processing instructions.
        /// </summary>
        ProcessingInstructionData,

        /// <summary>
        /// Compare content of text nodes and CDATA sections.
        /// </summary>
        TextValue,

        /// <summary>
        /// Compare content of comments.
        /// </summary>
        Comment,

        /// <summary>
        /// Compare number of attributes.
        /// </summary>
        AttributeList,

        /// <summary>
        /// Compare attribute's value.
        /// </summary>
        AttributeValue,

        /// <summary>
        /// Search for an atribute with a name matching a specific
        /// attribute of the other node.
        /// </summary>
        AttributeLookup,

        /// <summary>
        /// Compare element names.
        /// </summary>
        ElementName,

        /// <summary>
        /// Compare number of child nodes.
        /// </summary>
        NodeList,

        /// <summary>
        /// Compare order of child nodes.
        /// </summary>
        NodeListSequence,

        /// <summary>
        /// Search for a child node matching a specific child node of the other node.
        /// </summary>
        NodeListLookup
    }
}