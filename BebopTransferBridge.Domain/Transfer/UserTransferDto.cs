namespace BebopTransferBridge.Domain.Transfer;

using System.ComponentModel.DataAnnotations;

public class UserTransferDto
{
    [Required]
    public string OperatorId { get; set; }
}
