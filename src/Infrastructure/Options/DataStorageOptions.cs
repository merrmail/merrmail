using Merrsoft.MerrMail.Domain.Types;

namespace Merrsoft.MerrMail.Infrastructure.Options;

public class DataStorageOptions
{
    public required DataStorageType DataStorageType { get; init; }
    public required string DataStorageAccess { get; init; }
}