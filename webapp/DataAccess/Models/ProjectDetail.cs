using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = "ProjectDetails", PluralName = Globalisation.Strings.Names.ProjectStats, Name = Globalisation.Strings.Names.ProjectStats)]
    public class ProjectDetail : ObjectBase
	{

	    [Required]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.IbogasSponsoredSoFarLabel)]
	    public int NumberOfIbogasSponsoredToDate { get; set; }

        [Required]
		[Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.IbogasPlantedSoFarLabel)]
		public int NumberOfIbogasPlantedToDate { get; set; }
        
        [Required]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.IbogasToBePlantedThisYearLabel)]
	    public int NumberOfIbogasProjectedToBePlantedThisYear { get; set; }

	    [Required]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.IbogasToBePlantedTwoYearsLabel)]
	    public int NumberOfIbogasProjectedToBePlantedTwoYears { get; set; }

    }
}
