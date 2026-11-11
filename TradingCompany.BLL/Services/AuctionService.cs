using TradingCompany.BLL.Common;
using TradingCompany.BLL.Security;
using TradingCompany.BLL.Validation;
using TradingCompany.DAL.DTO;
using TradingCompany.DAL.Interfaces;

namespace TradingCompany.BLL.Services;

public interface IAuctionService
{
    OperationResult<IReadOnlyList<AuctionDto>> GetActive();
    OperationResult<IReadOnlyList<AuctionDto>> GetFinished();
    OperationResult<IReadOnlyList<CurrencyDto>> GetCurrencies();
    OperationResult<AuctionDto> Create(AuctionDto auction);
    OperationResult Deactivate(int auctionId);
}

public class AuctionService : IAuctionService
{
    private readonly IAuctionDal _auctions;
    private readonly ICurrencyDal _currencies;
    private readonly IItemDal _items;
    private readonly IAuctionValidator _validator;
    private readonly IUserSession _session;
    private readonly IPermissionService _permissions;

    public AuctionService(IAuctionDal auctions, ICurrencyDal currencies, IItemDal items,
        IAuctionValidator validator, IUserSession session, IPermissionService permissions)
    {
        _auctions = auctions;
        _currencies = currencies;
        _items = items;
        _validator = validator;
        _session = session;
        _permissions = permissions;
    }

    private bool Can(Permission permission) => _permissions.HasPermission(_session.Role, permission);

    public OperationResult<IReadOnlyList<AuctionDto>> GetActive() =>
        Can(Permission.ViewAuctions)
            ? OperationResult<IReadOnlyList<AuctionDto>>.Ok(_auctions.GetActive())
            : OperationResult<IReadOnlyList<AuctionDto>>.Fail("Немає доступу до перегляду аукціонів.");

    public OperationResult<IReadOnlyList<AuctionDto>> GetFinished() =>
        Can(Permission.ViewAuctions)
            ? OperationResult<IReadOnlyList<AuctionDto>>.Ok(_auctions.GetFinished())
            : OperationResult<IReadOnlyList<AuctionDto>>.Fail("Немає доступу до перегляду аукціонів.");

    public OperationResult<IReadOnlyList<CurrencyDto>> GetCurrencies() =>
        Can(Permission.ViewAuctions)
            ? OperationResult<IReadOnlyList<CurrencyDto>>.Ok(_currencies.GetAll())
            : OperationResult<IReadOnlyList<CurrencyDto>>.Fail("Немає доступу.");

    public OperationResult<AuctionDto> Create(AuctionDto auction)
    {
        if (!Can(Permission.ManageAuctions))
            return OperationResult<AuctionDto>.Fail("Немає прав на створення аукціонів.");

        var errors = _validator.Validate(auction);
        if (errors.Count > 0)
            return OperationResult<AuctionDto>.Fail(string.Join(Environment.NewLine, errors.Select(e => e.Message)));

        var item = _items.GetById(auction.ItemId);
        if (item == null)
            return OperationResult<AuctionDto>.Fail("Товар не знайдено.");
        if (!item.IsActive)
            return OperationResult<AuctionDto>.Fail("Не можна створити аукціон для неактивного товару.");

        auction.IsActive = true;
        _auctions.Add(auction);
        return OperationResult<AuctionDto>.Ok(auction);
    }

    public OperationResult Deactivate(int auctionId)
    {
        if (!Can(Permission.ManageAuctions))
            return OperationResult.Fail("Немає прав на деактивацію аукціонів.");

        var auction = _auctions.GetById(auctionId);
        if (auction == null)
            return OperationResult.Fail("Аукціон не знайдено.");
        if (!auction.IsActive)
            return OperationResult.Fail("Аукціон уже деактивовано.");

        _auctions.Deactivate(auctionId);
        return OperationResult.Ok();
    }
}
