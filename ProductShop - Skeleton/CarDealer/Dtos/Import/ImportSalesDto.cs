﻿using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Sale")]
    public class ImportSalesDto
    {
        [XmlElement("carId")]
        public int CarId { get; set; }

        [XmlElement("customerId")]
        public int CustomerId { get; set; }

        [XmlElement("discount")]
        public int Discount { get; set; }


        //    <Sale>
        //<carId>105</carId>
        //<customerId>30</customerId>
        //<discount>30</discount>
        //</Sale>
    }
}