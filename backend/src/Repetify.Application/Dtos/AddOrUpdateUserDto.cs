using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repetify.Application.Dtos;

/// <summary>  
/// Data Transfer Object for adding or editing a user.  
/// </summary>  
public class AddOrUpdateUserDto
{
	/// <summary>  
	/// Gets or sets the username of the user.  
	/// </summary>  
	/// <remarks>  
	/// This field is required and cannot exceed 100 characters.  
	/// </remarks>  
	[Required(ErrorMessage = "The Username is required.")]
	[MaxLength(100, ErrorMessage = "The username cannot exceed 100 characters.")]
	public string? Username { get; set; }

	/// <summary>  
	/// Gets or sets the email of the user.  
	/// </summary>  
	/// <remarks>  
	/// This field is required and cannot exceed 100 characters.  
	/// </remarks>  
	[Required(ErrorMessage = "The Emailis required.")]
	[MaxLength(100, ErrorMessage = "The Email cannot exceed 100 characters.")]
	public string? Email { get; set; }
}
