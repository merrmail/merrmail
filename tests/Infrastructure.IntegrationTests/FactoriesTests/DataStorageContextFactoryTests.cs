using FluentAssertions;
using Merrsoft.MerrMail.Infrastructure.External;
using Merrsoft.MerrMail.Infrastructure.Factories;
using Merrsoft.MerrMail.Infrastructure.Options;
using Merrsoft.MerrMail.Infrastructure.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.IntegrationTests.FactoriesTests;

public class DataStorageContextFactoryTests
{
    [Theory]
    [InlineData(DataStorageType.Sqlite, typeof(SqliteDataStorageContext))]
    [InlineData(DataStorageType.Csv, typeof(CsvDataStorageContext))]
    public void CreateDataStorageContext_WithDifferentTypes_ShouldReturnCorrectType(DataStorageType storageType, Type expectedType)
    {
        // Arrange
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        
        loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(() => new Mock<ILogger>().Object);

        var optionsMock = new Mock<IOptions<DataStorageOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new DataStorageOptions { DataStorageType = storageType, DataStorageAccess = string.Empty});

        var factory = new DataStorageContextFactory(loggerFactoryMock.Object, optionsMock.Object);

        // Act
        var result = factory.CreateDataStorageContext();

        // Assert
        result.Should().BeOfType(expectedType);
    }
}