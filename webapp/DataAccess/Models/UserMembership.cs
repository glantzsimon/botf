using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
    [Name(ResourceType = typeof(Dictionary), Name = Globalisation.Strings.Names.MembershipOption, PluralName = Globalisation.Strings.Names.MembershipOptions)]
    [DefaultPermissions(Role = RoleNames.DefaultUsers)]
    public class UserMembership : ObjectBase, IUserData
    {
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("MembershipOption")]
        public int MembershipOptionId { get; set; }
            
        public virtual User User { get; set; }
        public virtual MembershipOption MembershipOption { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [LinkedColumn(LinkedTableName = "MembershipOption", LinkedColumnName = "Description")]
        public string MembershipOptionName { get; set; }

    }
}
