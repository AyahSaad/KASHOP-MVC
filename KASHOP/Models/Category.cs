using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace KASHOP.Models
{
    public class Category
    {
        public int Id { get; set; }

        [MinLength(3)]
        [Required]
        [MaxLength(10)]
        public string Name { get; set; }

        [ValidateNever]
        public List<Product> products { get; set; } = new List<Product>();
    }
}
