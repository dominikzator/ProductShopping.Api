using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductShopping.Api.Constants;

namespace ProductShopping.Api.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "20c6b8ed-2a3f-4d2d-8f34-022c6a6c2fb7",
                Name = RoleNames.Administrator,
                NormalizedName = "ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "79cde6d7-7f52-4d4e-95fc-9084a64970f1",
                Name = RoleNames.User,
                NormalizedName = "USER"
            });
    }
}
