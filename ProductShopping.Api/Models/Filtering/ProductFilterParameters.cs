namespace ProductShopping.Api.Models.Filtering;

public class ProductFilterParameters : BaseFilterParameters
{
    public double? MinRating { get; set; }
    public double? MaxRating { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? CategoryName { get; set; }
}
