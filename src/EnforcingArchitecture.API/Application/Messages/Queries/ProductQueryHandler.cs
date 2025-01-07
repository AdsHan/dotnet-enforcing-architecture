using EnforcingArchitecture.API.Application.DTO;
using EnforcingArchitecture.API.Domain.Interfaces;
using MediatR;

namespace EnforcingArchitecture.API.Application.Messages.Queries;

public class ProductQueryHandler : IRequestHandler<FindAllProductsQuery, List<ProductDTO>>
{
    private readonly IProductRepository _productRepository;

    public ProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDTO>> Handle(FindAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(x => ProductDTO.FromProduct(x)).ToList();
    }
}
