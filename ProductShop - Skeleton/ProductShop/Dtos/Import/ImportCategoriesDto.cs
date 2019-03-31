using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("Category")]
    public class ImportCategoriesDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        //    <Category>x
        //<name>Drugs</name>
        //</Category>
    }
}