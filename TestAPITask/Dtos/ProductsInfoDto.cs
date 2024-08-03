namespace TestAPITask.Dtos
{
    public class ProductsInfoDto
    {
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string[] Sizes { get; set; }
        public string[] CommonWords { get; set; }

    }
}
