using System.ComponentModel.DataAnnotations;
using Merrsoft.MerrMail.Domain.Enums;

namespace Merrsoft.MerrMail.Domain.Options;

public class DataStorageOptions
{
    [Required] public required DataStorageType DatabaseType { get; set; }
    [Required] public required string DatabaseAccess { get; set; }
}