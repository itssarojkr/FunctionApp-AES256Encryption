using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AES256Encryption.FA.Models;
using Azure.Storage.Blobs;
using AESEncryption;

namespace AES256Encryption.FA;

public class Function
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly StorageSettings _encryptionOptions;
    private readonly StorageSettings _decryptionOptions;
    private readonly IAESEncryption _encryptionClient;
    private readonly ILogger<Function> _logger;

    public Function(BlobServiceClient blobService, IAESEncryption encryption, IOptionsSnapshot<StorageSettings> settings, ILogger<Function> logger)
    {
        _encryptionClient = encryption;
        _blobServiceClient = blobService;
        _logger = logger;

        _encryptionOptions = settings.Get(StorageSettings.EncryptionSettings);
        _decryptionOptions = settings.Get(StorageSettings.DecryptionSettings);
    }

    [Function("Encrypt")]
    public async Task Encrypt(
            [BlobTrigger("%EncryptionSettings:Container%/%EncryptionSettings:Source%/{name}")] Stream stream,
            string name
        )
    {
        try
        {
                Stream sEncryptedStream = _encryptionClient.Encrypt(stream);
                await WriteBlobContent(sEncryptedStream, $"Encrypted_{name}", _encryptionOptions);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [Function("Decrypt")]
    public async Task Decrypt(
            [BlobTrigger("%DecryptionSettings:Container%/%DecryptionSettings:Source%/{name}")] Stream stream,
            string name
        )
    {
        try
        {
            Stream sEncryptedStream = _encryptionClient.Decrypt(stream);
            await WriteBlobContent(sEncryptedStream, $"Decrypted_{name}", _decryptionOptions);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task WriteBlobContent(Stream content, string filename, StorageSettings settings)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(settings.Container);
        BlobClient blobClient = containerClient.GetBlobClient($"{settings.Destination}/{filename}");
        content.Seek(0, SeekOrigin.Begin);
        await blobClient.UploadAsync(content);
    }

}
