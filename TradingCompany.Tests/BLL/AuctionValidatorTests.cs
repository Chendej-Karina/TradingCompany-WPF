using TradingCompany.BLL.Validation;
using TradingCompany.DAL.DTO;

namespace TradingCompany.Tests.BLL;

public class AuctionValidatorTests
{
    private readonly AuctionValidator _validator = new();

    private static AuctionDto Valid() => new()
    {
        ItemId = 1,
        CurrencyId = 1,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(3),
        StartPrice = 100,
        BuyoutPrice = 500
    };

    [Fact]
    public void ValidAuction_HasNoErrors() => Assert.Empty(_validator.Validate(Valid()));

    [Fact]
    public void BuyoutPriceIsOptional()
    {
        var a = Valid();
        a.BuyoutPrice = null;
        Assert.Empty(_validator.Validate(a));
    }

    [Fact]
    public void MissingItem_IsError()
    {
        var a = Valid();
        a.ItemId = 0;
        Assert.Contains(_validator.Validate(a), e => e.Property == nameof(AuctionDto.ItemId));
    }

    [Fact]
    public void MissingCurrency_IsError()
    {
        var a = Valid();
        a.CurrencyId = 0;
        Assert.Contains(_validator.Validate(a), e => e.Property == nameof(AuctionDto.CurrencyId));
    }

    [Fact]
    public void EndBeforeStart_IsError()
    {
        var a = Valid();
        a.StartDate = DateTime.Now.AddDays(5);
        a.EndDate = DateTime.Now.AddDays(2);
        Assert.Contains(_validator.Validate(a), e => e.Property == nameof(AuctionDto.EndDate));
    }

    [Fact]
    public void EndInThePast_IsError()
    {
        var a = Valid();
        a.StartDate = DateTime.Now.AddDays(-5);
        a.EndDate = DateTime.Now.AddDays(-1);
        Assert.Contains(_validator.Validate(a), e => e.Property == nameof(AuctionDto.EndDate));
    }

    [Fact]
    public void MissingDates_AreErrors()
    {
        var a = Valid();
        a.StartDate = default;
        a.EndDate = default;
        var errors = _validator.Validate(a);
        Assert.Contains(errors, e => e.Property == nameof(AuctionDto.StartDate));
        Assert.Contains(errors, e => e.Property == nameof(AuctionDto.EndDate));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void NonPositiveStartPrice_IsError(double price)
    {
        var a = Valid();
        a.StartPrice = (decimal)price;
        Assert.Contains(_validator.Validate(a), e => e.Property == nameof(AuctionDto.StartPrice));
    }

    [Theory]
    [InlineData(100)]
    [InlineData(50)]
    public void BuyoutNotGreaterThanStart_IsError(double buyout)
    {
        var a = Valid();
        a.StartPrice = 100;
        a.BuyoutPrice = (decimal)buyout;
        Assert.Contains(_validator.Validate(a), e => e.Property == nameof(AuctionDto.BuyoutPrice));
    }
}
