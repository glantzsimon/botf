namespace K9.WebApplication.Options
{
    public class SeparatorOptions
    {
        public bool IsHideLastOfType { get; set; }

        public string CssClass => !IsHideLastOfType ? "show-last" : string.Empty;
    }
}