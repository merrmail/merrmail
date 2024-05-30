using FluentAssertions;
using MerrMail.Infrastructure.Types;

namespace Infrastructure.IntegrationTests.TypesTests;

public class DataStorageTypeTests
{
    [Fact]
    public void DataStorageType_ShouldHaveExpectedValues()
    {
        DataStorageType.Sqlite.ToString().Should().Be("Sqlite");
        DataStorageType.Csv.ToString().Should().Be("Csv");
    }
}