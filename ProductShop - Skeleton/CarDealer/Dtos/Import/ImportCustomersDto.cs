using System;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Customer")]
    public class ImportCustomersDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; } 


        /*    <Customer>
           <name>Teddy Hobby</name>
           <birthDate>1975-10-01T00:00:00</birthDate>
           <isYoungDriver>true</isYoungDriver>
           </Customer>*/

    }
}