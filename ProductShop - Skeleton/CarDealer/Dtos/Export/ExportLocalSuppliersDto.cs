using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("supplier")]
    public class ExportLocalSuppliersDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public int Parts_count { get; set; }

    }
}

//<? xml version="1.0" encoding="utf-8"?>
//<suppliers>
//<suplier id = "2" name="VF Corporation" parts-count="3" />
//<suplier id = "5" name="Saks Inc" parts-count="2" />
//...
//</suppliers>
