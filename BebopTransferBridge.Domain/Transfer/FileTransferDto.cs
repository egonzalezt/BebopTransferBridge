namespace BebopTransferBridge.Domain.Transfer;

using System.Text.Json.Serialization;

public class FileTransferDto
{
    [JsonPropertyName("documentTitle")]
    public string DocumentTitle { get; set; }
    [JsonPropertyName("urlDocument")]
    public string UrlDocument { get; set; }
}
