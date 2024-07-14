namespace AES256Encryption.FA.Models;

public class StorageSettings
{
    public const string EncryptionSettings = nameof(EncryptionSettings);
    public const string DecryptionSettings = nameof(DecryptionSettings);

    public string Container { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
}
