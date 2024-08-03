namespace TestAPITask.Models
{
    public class Product
    {
        public string Title { get; set; }
        public int Price { get; set; }
        public ICollection<string> Sizes { get; set; }
        public string Description { get; set; }
    }
}
