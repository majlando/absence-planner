using Business;

namespace UnitTests;

public class PasswordsTests
{
    [Fact]
    public void Hash_is_not_the_password()
    {
        Assert.NotEqual("secret123", Passwords.Hash("secret123"));
    }

    [Fact]
    public void Correct_password_works()
    {
        var hash = Passwords.Hash("secret123");

        Assert.True(Passwords.Verify(hash, "secret123"));
    }

    [Fact]
    public void Wrong_password_does_not_work()
    {
        var hash = Passwords.Hash("secret123");

        Assert.False(Passwords.Verify(hash, "wrong"));
    }
}
