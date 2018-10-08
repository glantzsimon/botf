using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Contacts, PluralName = Globalisation.Strings.Names.Contacts, Name = Globalisation.Strings.Names.Donation)]
    public class Contact : ObjectBase
	{
	    
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
        [StringLength(128)]
        public string FullName { get; set; }
        
	    [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
	    [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
	    [EmailAddress(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
	    [StringLength(255)]
	    public string EmailAddress { get; set; }
        
	    [DataType(DataType.PhoneNumber)]
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PhoneNumberLabel)]
	    [StringLength(255)]
	    public string PhoneNumber { get; set; }

	    [Display(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.CompanyLabel)]
	    [StringLength(255)]
	    public string CompanyName { get; set; }
        
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IsUnsubscribedLabel)]
        public bool IsUnsubscribed { get; set; }

	}
}
