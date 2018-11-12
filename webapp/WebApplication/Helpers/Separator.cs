using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
    public static partial class HtmlHelpers
	{

		public static MvcHtmlString Separator(this HtmlHelper html, bool hideLastOfType = false)
		{
			return html.Partial("_Separator", new SeparatorOptions
			{
                IsHideLastOfType = hideLastOfType
			});
		}

	}
}