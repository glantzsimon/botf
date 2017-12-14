﻿using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Donations, PluralName = Globalisation.Strings.Names.Donations)]
    public class Donation : ObjectBase
	{
        [Required]
		[StringLength(128)]
		[Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CustomerLabel)]
		public string Customer { get; set; }

	    [Required]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AmountDonatedLabel)]
	    [DataType(DataType.Currency)]
	    public double DonationAmount { get; set; }
        
        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DonatedOnLabel)]
        public DateTime DonatedOn { get; set; }

	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CurrencyLabel)]
        public string Currency { get; set; }

	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DescriptionLabel)]
        public string DonationDescription { get; set; }

	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CustomerLabel)]
        public string CustomerEmail { get; set; }

    }
}
