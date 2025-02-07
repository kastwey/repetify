using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagerWeb.DataAccess
{
	[Comment("Tabla para almacenar los libros existentes en esta biblioteca")]
	public class Book
	{

		public int BookId { get; set; }

		public required string AuthorUrl { get; set; }

		public required Author Author { get; set; }

		[MaxLength(200)]
		public required string Title { get; set; }

		public string? Sinopsis { get; set; }

		public int PublisherId { get; set; }

		public required Publisher Publisher { get; set; }

		public DateTime LoadedDate { get; set; }

		public List<BookRating> Ratings { get; set; } = [];

		public List<BookFile> BookFiles { get; set; } = [];

	}
}
