namespace Shared.JaaS;


public class JaaSOptions
{
    
    public string AppId { get; set; } = string.Empty;

    public string ApiKeyId { get; set; } = string.Empty;

    public string PrivateKeyPem { get; set; } = string.Empty;

    public string Domain { get; set; } = "8x8.vc";

    public int EarlyJoinMinutes { get; set; } = 15;
}

