namespace FindJobAPI.DTOs;

public class LoginDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResultDto
{
    public string Token { get; set; } = "";
    public string Username { get; set; } = "";
}
