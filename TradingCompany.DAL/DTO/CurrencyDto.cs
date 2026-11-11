namespace TradingCompany.DAL.DTO;

public class CurrencyDto : BaseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public override string ToString() => $"{Code} - {Name}";
}
