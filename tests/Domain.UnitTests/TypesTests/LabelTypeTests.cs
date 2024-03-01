using Merrsoft.MerrMail.Domain.Types;

namespace Merrsoft.MerrMail.Domain.UnitTests.TypesTests;

public class LabelTypeTests
{
    [Fact]
    public void LabelType_ShouldHaveExpectedValues()
    {
        LabelType.High.ToString().Should().Be("High");
        LabelType.Low.ToString().Should().Be("Low");
        LabelType.Other.ToString().Should().Be("Other");
        LabelType.None.ToString().Should().Be("None");
    }
}