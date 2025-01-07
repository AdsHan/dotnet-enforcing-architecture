using EnforcingArchitecture.API.Common;
using EnforcingArchitecture.API.Domain.Entities;
using EnforcingArchitecture.API.Domain.Interfaces;
using MediatR;

namespace EnforcingArchitecture.API.Application.Messages.Commands;

public class ProductCommandHandler : CommandHandler, IRequestHandler<CreateProductCommand, BaseResult>
{

    private readonly IProductRepository _productRepository;

    public ProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<BaseResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new ProductModel()
        {
            Title = command.Title,
            Description = command.Description,
            Price = command.Price,
            Quantity = command.Quantity
        };

        _productRepository.Add(product);

        await _productRepository.SaveAsync();

        BaseResult.Response = product.Id;

        return BaseResult;
    }
}
