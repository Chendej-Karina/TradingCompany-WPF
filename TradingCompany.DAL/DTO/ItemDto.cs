namespace TradingCompany.DAL.DTO;

public class ItemDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int ConditionId { get; set; }
    public bool IsActive { get; set; } = true;

    public string CategoryName { get; set; } = string.Empty;
    public string ConditionName { get; set; } = string.Empty;
}
