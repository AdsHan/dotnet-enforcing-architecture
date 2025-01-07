using EnforcingArchitecture.API.Domain.Entities;

namespace EnforcingArchitecture.API.Domain.Interfaces;

public interface IProductRepository : IDisposable
{
    Task<IEnumerable<ProductModel>> GetAllAsync();
    Task SaveAsync();
    void Add(ProductModel obj);
}