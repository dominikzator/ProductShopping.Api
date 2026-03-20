using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductShopping.Domain.Models;

public class Address
{
    [Required, MaxLength(100)]
    public string Street { get; set; } = "Milkyway street";

    [MaxLength(4)]
    public string? BuildingNumber { get; set; } = "19";

    [MaxLength(3)]
    public string? ApartmentNumber { get; set; } = "3";

    [Required, MaxLength(50)]
    public string City { get; set; } = "Glasgow";

    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = "12-345";

    [Required, MaxLength(50)]
    public string Country { get; set; } = "Scotland";

    [MaxLength(20)]
    public string? PhoneNumber { get; set; } = "123-456-789";
}