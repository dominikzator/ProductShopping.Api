using AutoMapper;
using Moq;
using ProductShopping.Api.Contracts;
using ProductShopping.Application.Contracts.Persistence;
using ProductShopping.Application.DTOs;
using ProductShopping.Application.Features.Product.Queries.GetProductDetails;
using ProductShopping.Application.Models.Filtering;
using ProductShopping.Application.Models.Paging;
using ProductShopping.Domain.Models;

namespace ProductShopping.Application.UnitTests.Mocks;

public class MockProductsRepository
{
    public static (Mock<IProductsRepository>, Mock<IMapper>) GetProductsRepository_CreateProductSetup()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Test Product",
                CategoryId = 1,
                Price = 2
            }
        };

        var productsDtos = new List<ProductDto>
        {
            new ProductDto
            {
                Id = 1,
                Name = "Test Product",
                CategoryName = "Food",
                Price = 2
            }
        };

        var categoriesDtos = new List<ProductCategoryDto>
        {
            new ProductCategoryDto
            {
                Id=1,
                Name = "Food"
            }
        };

        var mockRepo = new Mock<IProductsRepository>();
        var mapper = new Mock<IMapper>();

        mockRepo.Setup(r => r.GetAsync()).ReturnsAsync(products);

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Returns((Product product) =>
            {
                products.Add(product);

                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.GetCategoryFromName(It.IsAny<string>())).Returns((string categoryName) =>
        {
            return categoriesDtos.FirstOrDefault(c => c.Name == categoryName)!;
        });

        mockRepo.Setup(r => r.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((string productName) =>
        {
            return productsDtos.FirstOrDefault(c => c.Name == productName)!;
        });

        return (mockRepo, mapper);
    }

    public static (Mock<IProductsRepository>, Mock<IMapper>) GetProductsRepository_UpdateProductSetup()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Test Product",
                CategoryId = 1,
                Price = 2
            }
        };

        var productsDtos = new List<ProductDto>
        {
            new ProductDto
            {
                Id = 1,
                Name = "Test Product",
                CategoryName = "Food",
                Price = 2
            }
        };

        var categoriesDtos = new List<ProductCategoryDto>
        {
            new ProductCategoryDto
            {
                Id=1,
                Name = "Food"
            }
        };

        var mockRepo = new Mock<IProductsRepository>();
        var mapper = new Mock<IMapper>();

        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns((Product product) =>
            {
                return Task.CompletedTask;
            });

        mockRepo.Setup(r => r.GetCategoryFromName(It.IsAny<string>())).Returns((string categoryName) =>
        {
            return categoriesDtos.FirstOrDefault(c => c.Name == categoryName)!;
        });

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            return products.FirstOrDefault(q => q.Id == id);
        });

        return (mockRepo, mapper);
    }

    public static (Mock<IProductsRepository>, Mock<IMapper>) GetProductsRepository_DeleteProduct()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Test Product",
                CategoryId = 1,
                Price = 2
            }
        };

        var productsDtos = new List<ProductDto>
        {
            new ProductDto
            {
                Id = 1,
                Name = "Test Product",
                CategoryName = "Food",
                Price = 2
            }
        };

        var categoriesDtos = new List<ProductCategoryDto>
        {
            new ProductCategoryDto
            {
                Id=1,
                Name = "Food"
            }
        };

        var mockRepo = new Mock<IProductsRepository>();
        var mapper = new Mock<IMapper>();

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            return products.FirstOrDefault(q => q.Id == id);
        });

        return (mockRepo, mapper);
    }

    public static (Mock<IProductsRepository>, Mock<IMapper>) GetProductsRepository_GetProductsSetup()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Little Canoli",
                CategoryId = 1,
                Price = (decimal)2.49
            },
            new Product
            {
                Id = 2,
                Name = "Car Cleaner",
                CategoryId = 3,
                Price = (decimal)49.99
            },
            new Product
            {
                Id = 3,
                Name = "Car Wheel",
                CategoryId = 3,
                Price = (decimal)39.99
            },
            new Product
            {
                Id = 4,
                Name = "Bananas",
                CategoryId = 1,
                Price = (decimal)2.99
            },
            new Product
            {
                Id = 5,
                Name = "Milk",
                CategoryId = 1,
                Price = (decimal)1.99
            },
            new Product
            {
                Id = 6,
                Name = "Vacuum Cleaner",
                CategoryId = 3,
                Price = (decimal)69.99
            },
            new Product
            {
                Id = 7,
                Name = "Birthday Cake",
                CategoryId = 1,
                Price = (decimal)29.99
            },
        };

        var productsDtos = new List<ProductDto>
        {
            new ProductDto
            {
                Id = 1,
                Name = "Little Canoli",
                CategoryName = "Food",
                Price = (decimal)2.49
            },
            new ProductDto
            {
                Id = 2,
                Name = "Car Cleaner",
                CategoryName = "Automotive",
                Price = (decimal)49.99
            },
            new ProductDto
            {
                Id = 3,
                Name = "Car Wheel",
                CategoryName = "Automotive",
                Price = (decimal)39.99
            },
            new ProductDto
            {
                Id = 4,
                Name = "Bananas",
                CategoryName = "Food",
                Price = (decimal)2.99
            },
            new ProductDto
            {
                Id = 5,
                Name = "Milk",
                CategoryName = "Food",
                Price = (decimal)1.99
            },
            new ProductDto
            {
                Id = 6,
                Name = "Vacuum Cleaner",
                CategoryName = "Automotive",
                Price = (decimal)69.99
            },
            new ProductDto
            {
                Id = 7,
                Name = "Birthday Cake",
                CategoryName = "Food",
                Price = (decimal)29.99
            },
        };

        var categoriesDtos = new List<ProductCategoryDto>
        {
            new ProductCategoryDto
            {
                Id=1,
                Name = "Food"
            },
            new ProductCategoryDto
            {
                Id=3,
                Name = "Automotive"
            }
        };

        var mockRepo = new Mock<IProductsRepository>();
        var mapper = new Mock<IMapper>();

        mockRepo.Setup(r => r.GetAsync()).ReturnsAsync(products);

        mockRepo.Setup(r => r.GetFilteredRawPagedAsync(It.IsAny<ProductFilterParameters>(), It.IsAny<PaginationParameters>()))
        .ReturnsAsync((ProductFilterParameters productFilterParameters, PaginationParameters paginationParameters) => {
            return (productsDtos, 7, 1);
        });

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            return products.FirstOrDefault(q => q.Id == id);
        });

        mockRepo.Setup(r => r.GetCategoryFromName(It.IsAny<string>())).Returns((string categoryName) =>
        {
            return categoriesDtos.FirstOrDefault(c => c.Name == categoryName)!;
        });

        mockRepo.Setup(r => r.GetProductByNameAsync(It.IsAny<string>())).ReturnsAsync((string productName) =>
        {
            return productsDtos.FirstOrDefault(c => c.Name == productName)!;
        });

        return (mockRepo, mapper);
    }
}
