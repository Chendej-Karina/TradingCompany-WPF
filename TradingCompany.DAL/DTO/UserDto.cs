namespace TradingCompany.DAL.DTO;

public class UserDto : BaseDto
{
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int? RoleId { get; set; }

    public string? RoleName { get; set; }
}
