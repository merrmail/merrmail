using Merrsoft.MerrMail.Infrastructure.Types;

namespace MerrMail.Infrastructure.Options;

public class DataStorageOptions
{
    public required DataStorageType DataStorageType { get; init; }
    public required string DataStorageAccess { get; init; }
}