using System.ComponentModel.DataAnnotations;
namespace Lab2.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tên")]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
