using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using TestAPITask.Dtos;
using TestAPITask.Models;

namespace TestAPITask.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductsService> _logger;

        public ProductsService(IHttpClientFactory httpClientFactory, ILogger<ProductsService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ProductsDto> GetProductsAsync(ProductsFilterDto filter, CancellationToken ct)
        {
            var highligts = filter.Highlight?.Split(',')
                .Distinct()
                .ToArray() ?? new string[] { };

            using (var httpClient = _httpClientFactory.CreateClient("Products"))
            {
                var response = await httpClient.GetAsync("https://poqtest001.blob.core.windows.net/backendtest/mock-product-data.json", ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"HttpClient request failed with message: {response.ReasonPhrase}");
                    throw new Exception("Cannot receive data from external source");
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var products = JsonSerializer.Deserialize<ICollection<Product>>(json, options);

                if (products is null)
                {
                    throw new Exception("No data exists");
                }

                var min = products.Min(x => x.Price);
                var max = products.Max(x => x.Price);
                var sizes = products.SelectMany(x => x.Sizes).Distinct().ToArray();
                var commonWords = FindDescriptionMostCommonWords(products);

                var filteredProducts = products
                    .Where(x => !filter.MinPrice.HasValue || x.Price >= filter.MinPrice.Value)
                    .Where(x => !filter.MaxPrice.HasValue || x.Price <= filter.MaxPrice.Value)
                    .Where(x => filter.Size is null || x.Sizes.Contains(filter.Size))
                    .Select(x => new ProductDto(x, highligts))
                    .ToArray();
                return new ProductsDto
                {
                    Products = filteredProducts,
                    Info = new ProductsInfoDto
                    {
                        CommonWords = commonWords,
                        MaxPrice = max,
                        MinPrice = min,
                        Sizes = sizes
                    }
                };
            }            
        }

        private string[] FindDescriptionMostCommonWords(ICollection<Product> products)
        {
            var dict = new Dictionary<string, int>();
            
            foreach(var product in products)
            {
                var words = Regex.Matches(product.Description.ToLowerInvariant(), "[a-zA-Z]+").Select(match => match.Value);

                foreach (var word in words)
                {
                    if (dict.ContainsKey(word))
                        dict[word]++;

                    else dict.Add(word, 1);
                }
            }

            return dict.OrderByDescending(x => x.Value)
                    .Skip(5)
                    .Take(10)
                    .Select(x => x.Key)
                    .ToArray();
        }
    }
}
