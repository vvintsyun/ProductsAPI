using System.Text;
using TestAPITask.Models;

namespace TestAPITask.Dtos
{
    public class ProductDto
    {
        public ProductDto(Product product, string[] highligts)
        {
            Sizes = product.Sizes;
            Title = product.Title;
            Price = product.Price;

            var sb = new StringBuilder(product.Description);
            foreach (var item in highligts)
            {
                sb.Replace(item, $"<em>{item}</em>");
                var capitalizedItem = char.ToUpper(item[0]) + item.Substring(1);
                sb.Replace(capitalizedItem, $"<em>{capitalizedItem}</em>");
            }
            Description = sb.ToString();
        }

        public string Title { get; set; }
        public int Price { get; set; }
        public ICollection<string> Sizes { get; set; }
        public string Description { get; set; }
    }
}
