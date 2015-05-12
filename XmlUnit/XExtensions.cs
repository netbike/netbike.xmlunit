namespace NetBike.XmlUnit
{
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    internal static class XExtensions
    {
        public static XName GetName(this XObject item)
        {
            XName name = null;

            if (item.NodeType == XmlNodeType.Element)
            {
                name = ((XElement)item).Name;
            }
            else if (item.NodeType == XmlNodeType.Attribute)
            {
                name = ((XAttribute)item).Name;
            }

            return name;
        }

        public static string GetXPath(this XNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                return GetElementXPath((XElement)node);
            }

            var path = node.Parent != null ? GetXPath(node.Parent) : string.Empty;

            switch (node.NodeType)
            {
                case XmlNodeType.Comment:
                    path += "/comment()";
                    break;

                case XmlNodeType.Text:
                    path += "/text()";
                    break;

                case XmlNodeType.ProcessingInstruction:
                    path += "/processing-instruction()";
                    break;

                default:
                    path += "/";
                    break;
            }

            return path;
        }

        public static int GetElementIndex(this XElement element)
        {
            var index = 0;
            var parent = element.Parent;

            if (parent != null)
            {
                foreach (var item in element.Parent.Elements())
                {
                    if (item == element)
                    {
                        break;
                    }

                    index++;
                }
            }

            return index;
        }

        public static string GetElementXPath(this XElement element)
        {
            var pathStack = new Stack<string>();
            var pathBuilder = new StringBuilder();

            while (element != null)
            {
                var ns = element.Name.Namespace;

                pathBuilder.Append('/');

                if (ns != null && !string.IsNullOrEmpty(ns.NamespaceName))
                {
                    var prefix = element.GetPrefixOfNamespace(ns);
                    pathBuilder.Append(prefix).Append(':');
                }

                pathBuilder.Append(element.Name.LocalName);

                if (element.Parent != null)
                {
                    var index = 0;

                    foreach (var sibling in element.Parent.Elements(element.Name))
                    {
                        if (sibling == element)
                        {
                            break;
                        }

                        index++;
                    }

                    if (index != 0)
                    {
                        index += 1;
                        pathBuilder.Append('[').Append(index).Append(']');
                    }
                }

                pathStack.Push(pathBuilder.ToString());
                pathBuilder.Length = 0;

                element = element.Parent;
            }

            return string.Concat(pathStack);
        }
    }
}