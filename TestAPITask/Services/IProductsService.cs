using TestAPITask.Dtos;

namespace TestAPITask.Services
{
    public interface IProductsService
    {
        Task<ProductsDto> GetProductsAsync(ProductsFilterDto filter, CancellationToken ct);
    }
}
