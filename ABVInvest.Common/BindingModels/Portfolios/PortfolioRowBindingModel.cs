﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ABVInvest.Common.BindingModels.Portfolios
{

    [XmlType("New", IncludeInSchema = true)]
    public class PortfolioRowBindingModel
    {
        [XmlElement("Client")]
        [Required]
        public Client Client { get; set; }

        [XmlElement("Instrument")]
        [Required]
        public Instrument Instrument { get; set; }

        [XmlElement("AccountData")]
        [Required]
        public AccountData AccountData { get; set; }

        [XmlElement("Other")]
        [Required]
        public Other Other { get; set; }
    }
}