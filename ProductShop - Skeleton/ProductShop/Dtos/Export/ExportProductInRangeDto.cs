using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Product")]
    public class ExportProductInRangeDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("buyer")]
        public string FullName { get; set; }



        /*  <Product>
           <name>TRAMADOL HYDROCHLORIDE</name>
           <price>516.48</price>
           </Product>
           */
    }
}