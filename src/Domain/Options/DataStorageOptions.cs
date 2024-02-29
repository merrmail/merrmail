using Merrsoft.MerrMail.Domain.Types;

namespace Merrsoft.MerrMail.Domain.Options;

public class DataStorageOptions
{
    public required DataStorageType DataStorageType { get; set; }
    public required string DataStorageAccess { get; set; }
}