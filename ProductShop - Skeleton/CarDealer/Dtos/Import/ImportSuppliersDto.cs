using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Supplier")]
    public class ImportSuppliersDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }


        /*    <Supplier>
           <name>Caterpillar Inc.</name>
           <isImporter>false</isImporter>
           </Supplier>*/
    }
}