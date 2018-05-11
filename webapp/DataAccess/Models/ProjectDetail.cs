using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.ProjectStats, PluralName = Globalisation.Strings.Names.ProjectStats, Name = Globalisation.Strings.Names.ProjectStats)]
    public class ProjectDetail : ObjectBase
	{
	    
        [Required]
		[Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.IbogasPlantedLabel)]
		public int NumberOfIbogasPlantedToDate { get; set; }
        
        [Required]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.IbogasToBePlantedLabel)]
	    public int NumberOfIbogasProjectedToBePlantedPerYear { get; set; }

    }
}
