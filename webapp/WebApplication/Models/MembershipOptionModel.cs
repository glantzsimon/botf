﻿using System;
using K9.Globalisation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;

namespace K9.WebApplication.Models
{
    public class StripeModel
    {
        public string PublishableKey { get; set; }
        private const string AutoLocale = "auto";
        private const int AmountPerTree = 10;

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DonationAmountLabel)]
        [DataType(DataType.Currency)]
        public double DonationAmount { get; set; }
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TreeDonationAmountLabel)]
        [DataType(DataType.Currency)]
        public double TreeDonationAmount => DonationAmount;

        private int _numberOfTrees;
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NumberOfTreesLabel)]
        public int NumberOfTrees
        {
            get { return _numberOfTrees; }
            set
            {
                _numberOfTrees = value;
                DonationAmount = NumberOfTrees * AmountPerTree;
            }
        }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.AmountToDonateLabel)]
        [DataType(DataType.Currency)]
        public double AmountToDonate => DonationAmount;

        public double DonationAmountInCents => DonationAmount * 100;
        
        public string Locale => GetLocale();

        public string LocalisedCurrencyThreeLetters { get; set; }

        public string Description { get; set; }

        public string StripeToken { get; set; }

        public string StripeEmail { get; set; }

        public string StripeBillingName { get; set; }
        
        public static string GetLocalisedCurrency()
        {
            try
            {
                return new RegionInfo(Thread.CurrentThread.CurrentUICulture.LCID).ISOCurrencySymbol;
            }
            catch (Exception e)
            {
                return "USD";
            }
        }

        private static string GetLocale()
        {
            try
            {
                var locale = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
                return string.IsNullOrEmpty(locale) ? AutoLocale : locale;
            }
            catch (Exception e)
            {
                return AutoLocale;
            }
        }
    }
}