using System.ComponentModel.DataAnnotations;
using Merrsoft.MerrMail.Domain.Enums;

namespace Merrsoft.MerrMail.Domain.Options;

public class DataStorageOptions
{
    [Required] public required DataStorageType DataStorageType { get; set; }
    [Required] public required string DataStorageAccess { get; set; }
}