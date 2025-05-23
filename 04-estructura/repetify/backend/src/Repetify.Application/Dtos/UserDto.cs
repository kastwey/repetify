namespace Repetify.Application.Dtos;

public class UserDto
{
	public Guid Id { get; set; }
	public string Username { get; set; }
	public string Email { get; set; }

	public UserDto(Guid id, string username, string email)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(username);
		ArgumentException.ThrowIfNullOrWhiteSpace(email);
		Id = id;
		Username = username;
		Email = email;
	}
}