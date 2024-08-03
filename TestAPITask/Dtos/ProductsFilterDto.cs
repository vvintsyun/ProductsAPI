using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TestAPITask.Dtos
{
    public class ProductsFilterDto
    {
        [Range(1, int.MaxValue)]
        public int? MinPrice { get; set; }
        [Range(1, int.MaxValue)]
        public int? MaxPrice { get; set; }
        public string? Size { get; set; }
        public string? Highlight { get; set; }
    }
}
