using TradingCompany.DAL.DTO;

namespace TradingCompany.BLL.Validation;

public record ValidationError(string Property, string Message);

public interface IAuctionValidator
{
    IReadOnlyList<ValidationError> Validate(AuctionDto auction);
}


public class AuctionValidator : IAuctionValidator
{
    public IReadOnlyList<ValidationError> Validate(AuctionDto auction)
    {
        var errors = new List<ValidationError>();

        if (auction.ItemId <= 0)
            errors.Add(new(nameof(AuctionDto.ItemId), "Не вибрано товар."));

        if (auction.CurrencyId <= 0)
            errors.Add(new(nameof(AuctionDto.CurrencyId), "Виберіть валюту."));

        if (auction.StartDate == default)
            errors.Add(new(nameof(AuctionDto.StartDate), "Вкажіть дату початку."));

        if (auction.EndDate == default)
            errors.Add(new(nameof(AuctionDto.EndDate), "Вкажіть дату завершення."));
        else if (auction.EndDate <= DateTime.Now)
            errors.Add(new(nameof(AuctionDto.EndDate), "Дата завершення має бути в майбутньому."));

        if (auction.StartDate != default && auction.EndDate != default && auction.EndDate <= auction.StartDate)
            errors.Add(new(nameof(AuctionDto.EndDate), "Дата завершення має бути пізніше за дату початку."));

        if (auction.StartPrice <= 0)
            errors.Add(new(nameof(AuctionDto.StartPrice), "Стартова ціна має бути більшою за 0."));

        if (auction.BuyoutPrice.HasValue && auction.BuyoutPrice.Value <= auction.StartPrice)
            errors.Add(new(nameof(AuctionDto.BuyoutPrice), "Ціна викупу має бути більшою за стартову."));

        return errors;
    }
}
