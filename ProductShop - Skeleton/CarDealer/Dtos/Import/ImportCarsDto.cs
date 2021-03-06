﻿using System.Xml.Serialization;
using CarDealer.Models;

namespace CarDealer.Dtos.Import
{
    [XmlType("Car")]
    public class ImportCarsDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public int TraveledDistance { get; set; }

        [XmlElement("parts")]
        public PartsDto[] Parts { get; set; }

        /* <Car>
           <make>Opel</make>
           <model>Astra</model>
           <TraveledDistance>516628215</TraveledDistance>
           <parts>
           <partId id="39"/>
           <partId id="62"/>
           <partId id="72"/>
           </parts>
           </Car>*/
    }

    public class PartsDto
    {

        [XmlAttribute("partId id")]
        public int PartId { get; set; }
    }
}