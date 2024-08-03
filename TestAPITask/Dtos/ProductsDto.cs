namespace TestAPITask.Dtos
{
    public class ProductsDto
    {
        public IList<ProductDto> Products { get; set; }
        public ProductsInfoDto Info { get; set; }
    }
}
