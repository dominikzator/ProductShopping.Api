namespace ProductShopping.UI.Selection;

/// <summary>
/// Adresy obu frontendów ProductShopping (sekcja "Frontends" w appsettings.json).
/// </summary>
public sealed class FrontendLinksOptions
{
    public const string SectionName = "Frontends";

    public string RazorPagesUrl { get; set; } = string.Empty;
    public string BlazorUrl { get; set; } = string.Empty;
    public string SourceCodeUrl { get; set; } = string.Empty;
}
