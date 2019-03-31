using System.Xml;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class ExportCustomUserProductDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public ExportUserAndProductDto[] ExportUserAndProductDto { get; set; }
    }
}