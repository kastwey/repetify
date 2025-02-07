using System.ComponentModel.DataAnnotations;

namespace LibraryManagerWeb.DataAccess
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        [MinLength(10), MaxLength(50)]
        public required string Name { get; set; }
        public ICollection<Magazine>? Magazines { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category? Parent { get; set; }
    }
}
