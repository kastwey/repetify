using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagerWeb.DataAccess
{
	public class Publisher
	{
		[Key]
		public int PublisherId { get; set; }

		public required string Name { get; set; }
	}
}
