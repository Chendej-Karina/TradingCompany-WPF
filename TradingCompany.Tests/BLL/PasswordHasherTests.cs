using TradingCompany.BLL.Security;

namespace TradingCompany.Tests.BLL;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.Hash("Secret123!");
        Assert.True(_hasher.Verify("Secret123!", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = _hasher.Hash("Secret123!");
        Assert.False(_hasher.Verify("secret123!", hash));
    }

    [Fact]
    public void Hash_SamePasswordTwice_GivesDifferentHashes()
    {
        Assert.NotEqual(_hasher.Hash("abc"), _hasher.Hash("abc"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("100000.@@@.@@@")]
    [InlineData("abc.def.ghi")]
    public void Verify_MalformedHash_ReturnsFalse(string hash)
    {
        Assert.False(_hasher.Verify("abc", hash));
    }

    [Fact]
    public void Verify_HashFromDatabaseScript_Works()
    {
        
        const string hash = "100000.BC3a9LiyrBEP0pBFdDvHFg==.dU8xFVDJpDPTWx4s9xikAj9wA2IhxpR9/QOO0ToBIis=";
        Assert.True(_hasher.Verify("Manager123!", hash));
        Assert.False(_hasher.Verify("Manager123", hash));
    }
}
