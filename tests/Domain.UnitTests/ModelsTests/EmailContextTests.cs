using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Domain.UnitTests.ModelsTests;

public class EmailContextTests
{
    [Fact]
    public void EmailContext_ShouldBeEqual_WhenValuesAreEqual()
    {
        var context1 = new EmailContext("Concern", "Response");
        var context2 = new EmailContext("Concern", "Response");

        context1.Should().Be(context2);
        context1.GetHashCode().Should().Be(context2.GetHashCode());
    }

    [Fact]
    public void EmailContext_ShouldNotBeEqual_WhenValuesAreDifferent()
    {
        var context1 = new EmailContext("Concern", "Response");
        var context2 = new EmailContext("DifferentConcern", "DifferentResponse");

        context1.Should().NotBe(context2);
        context1.GetHashCode().Should().NotBe(context2.GetHashCode());
    }
}