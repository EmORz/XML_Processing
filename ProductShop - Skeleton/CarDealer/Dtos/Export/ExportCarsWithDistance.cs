using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("Car")]
    public class ExportCarsWithDistance
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public decimal Travelled { get; set; }

    //    <car>
    //<make>BMW</make>
    //<model>1M Coupe</model>
    //<travelled-distance>39826890</travelled-distance>
    //</car>

    }
}