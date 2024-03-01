using Merrsoft.MerrMail.Domain.Models;

namespace Merrsoft.MerrMail.Domain.UnitTests.ModelsTests;

public class EmailThreadTests
{
    [Fact]
    public void EmailThread_ShouldBeEqual_WhenValuesAreEqual()
    {
        var guid = Guid.NewGuid().ToString();
        var thread1 = new EmailThread(guid, "Subject", "Body", "Sender");
        var thread2 = new EmailThread(guid, "Subject", "Body", "Sender");

        thread1.Should().Be(thread2);
        thread1.GetHashCode().Should().Be(thread2.GetHashCode());
    }

    [Fact]
    public void EmailThread_ShouldNotBeEqual_WhenValuesAreDifferent()
    {
        var thread1 = new EmailThread(Guid.NewGuid().ToString(), "Subject", "Body", "Sender");
        var thread2 = new EmailThread(Guid.NewGuid().ToString(), "DifferentSubject", "DifferentBody",
            "DifferentSender");

        thread1.Should().NotBe(thread2);
        thread1.GetHashCode().Should().NotBe(thread2.GetHashCode());
    }
}