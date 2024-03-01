using Merrsoft.MerrMail.Domain.Types;

namespace Merrsoft.MerrMail.Domain.UnitTests.TypesTests;

public class DataStorageTypeTests
{
    [Fact]
    public void DataStorageType_ShouldHaveExpectedValues()
    {
        DataStorageType.Sqlite.ToString().Should().Be("Sqlite");
        DataStorageType.Csv.ToString().Should().Be("Csv");
    }
}