using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class ExportSoldProductsDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("soldProducts")]
        public ProductDto[] ProductDto { get; set; }
        /*<User>
           <firstName>Almire</firstName>
           <lastName>Ainslee</lastName>
           <soldProducts>
           <Product>
           <name>olio activ mouthwash</name>
           <price>206.06</price>
           </Product>
           <Product>
           <name>Acnezzol Base</name>
           <price>710.6</price>
           </Product>
           <Product>
           <name>ENALAPRIL MALEATE</name>
           <price>210.42</price>
           </Product>
           </soldProducts>
           </User>...
           */
    }

    public class ProductDto 
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}