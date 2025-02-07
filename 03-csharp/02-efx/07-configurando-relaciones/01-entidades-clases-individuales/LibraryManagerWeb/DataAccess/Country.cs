using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagerWeb.DataAccess
{
	[Comment("Tabla para guardar los paises")]
	public class Country
	{
		[Comment("Clave primaria de la tabla paises")]
		public int CountryId { get; set; }

		public required string NativeName { get; set; }

		public required string EnglishName { get; set; }
	}
}
