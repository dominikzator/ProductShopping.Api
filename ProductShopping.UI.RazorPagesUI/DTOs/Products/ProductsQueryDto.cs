namespace ProductShopping.UI.RazorPagesUI.DTOs.Products;

public class ProductsQueryDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? CategoryName { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}