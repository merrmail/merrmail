namespace MerrMail.Domain.Types;

/// <summary>
/// Specifies the label types for email threads.
/// </summary>
public enum LabelType
{
    /// <summary>
    /// Indicates a high priority label.
    /// </summary>
    High,
    
    /// <summary>
    /// Indicates a low priority label.
    /// </summary>
    Low,
    
    /// <summary>
    /// Email threads for no-reply emails and email threads that are sent by the host.
    /// </summary>
    Other,
    
    /// <summary>
    /// Indicates no specific label.
    /// </summary>
    None,
}