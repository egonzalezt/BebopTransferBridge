namespace BebopTransferBridge.Domain.Transfer; 

using System.ComponentModel.DataAnnotations;

public class UserTransfer
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string OperatorId { get; set; }
    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }
}
