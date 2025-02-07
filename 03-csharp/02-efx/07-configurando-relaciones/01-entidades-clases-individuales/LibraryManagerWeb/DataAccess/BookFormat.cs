using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagerWeb.DataAccess
{
	[Table("BookFormats")]
	public class BookFormat
	{

		public int BookformatId { get; set; }

		public required string Name { get; set; }
	}
}
