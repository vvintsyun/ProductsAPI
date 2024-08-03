using Microsoft.AspNetCore.Mvc;
using TestAPITask.Dtos;
using TestAPITask.Services;

namespace TestAPITask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        public async Task<ActionResult<ProductsDto>> Get([FromQuery] ProductsFilterDto filter, CancellationToken ct)
        {
            return Ok(await _productsService.GetProductsAsync(filter, ct));
        }
    }
}
