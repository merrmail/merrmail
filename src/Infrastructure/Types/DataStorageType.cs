namespace MerrMail.Infrastructure.Types;

/// <summary>
/// Enum representing the types of data storage supported by MerrMail.
/// </summary>
public enum DataStorageType
{
    /// <summary>
    /// SQLite database storage.
    /// </summary>
    Sqlite,

    /// <summary>
    /// CSV file storage.
    /// </summary>
    Csv,
}