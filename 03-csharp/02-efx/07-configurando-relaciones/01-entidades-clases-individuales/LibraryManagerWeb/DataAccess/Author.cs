using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagerWeb.DataAccess
{
	[Table("Authors")]
	[Comment("Tabla para almacenar los autores que tienen libros en la biblioteca")]
	[Index(nameof(Name), nameof(LastName))]
    public class Author
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int AuthorId { get; set; }

		[Required]
		[MinLength(3), MaxLength(50)]
		public required string Name { get; set; }

		[Required]
		[MinLength(3), MaxLength(100)]
		public required string LastName { get; set; }

		public List<Book> Books { get; set; } = new();
		[NotMapped]
		public DateTime LoadedDate { get; set; }

		public required string AuthorUrl { get; set; }

		public string? DisplayName { get; set; }

	}
}
