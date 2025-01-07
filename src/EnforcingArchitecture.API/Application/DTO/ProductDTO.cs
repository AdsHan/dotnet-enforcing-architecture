using EnforcingArchitecture.API.Domain.Entities;
using EnforcingArchitecture.API.Domain.Enums;

namespace EnforcingArchitecture.API.Application.DTO;

public record ProductDTO(string Description, EntityStatusEnum Status)
{
    public static ProductDTO FromProduct(ProductModel product)
    {
        return new ProductDTO(
            product.Description,
            product.Status
        );
    }
}
