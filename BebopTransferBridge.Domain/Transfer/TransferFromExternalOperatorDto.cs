namespace BebopTransferBridge.Domain.Transfer; 

using System.Text.Json.Serialization;

public class TransferFromExternalOperatorDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("address")]
    public string Address { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("callbackUrl")]
    public string CallbackUrl { get; set; }
    [JsonPropertyName("files")]
    public FileTransferDto[] Files { get; set; }
}
