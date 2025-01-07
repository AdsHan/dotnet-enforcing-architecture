using EnforcingArchitecture.API.Application.DTO;
using MediatR;

namespace EnforcingArchitecture.API.Application.Messages.Queries;

public record FindAllProductsQuery : IRequest<List<ProductDTO>>;