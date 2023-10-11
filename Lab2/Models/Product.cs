using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models
{
    public class Product
    {
        [Key]
        public int Id { set; get; }
        [Required(ErrorMessage ="Vui lòng nhập tên")]
        public string Name { set; get; }
        public string Description { set; get; }
        [Required(ErrorMessage = "Vui lòng nhập giá")]
        public double Price { set; get; }
        public int CategoryId { set; get; }

        [ForeignKey("CategoryId")]
        public Category Category { set; get; }
        public string ImageUrl { set; get; }
    }
}
