using MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Domain.UnitTests.ModelsTests;

public class EmailContextTests
{
    [Fact]
    public void EmailContext_ShouldBeEqual_WhenValuesAreEqual()
    {
        // Act
        var context1 = new EmailContext("Concern", "Response");
        var context2 = new EmailContext("Concern", "Response");

        // Assert
        context1.Should().Be(context2);
        context1.GetHashCode().Should().Be(context2.GetHashCode());
    }

    [Fact]
    public void EmailContext_ShouldNotBeEqual_WhenValuesAreDifferent()
    {
        // Act
        var context1 = new EmailContext("Concern", "Response");
        var context2 = new EmailContext("DifferentConcern", "DifferentResponse");

        // Assert
        context1.Should().NotBe(context2);
        context1.GetHashCode().Should().NotBe(context2.GetHashCode());
    }
}