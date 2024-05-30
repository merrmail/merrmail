using MerrMail.Infrastructure.Types;

namespace MerrMail.Infrastructure.Options;

/// <summary>
/// Options for configuring data storage context service.
/// </summary>
public class DataStorageOptions
{
    /// <summary>
    /// The type of data storage.
    /// </summary>
    public required DataStorageType DataStorageType { get; init; }
    
    /// <summary>
    /// The access path or connection string for the data storage.
    /// </summary>
    public required string DataStorageAccess { get; init; }
}