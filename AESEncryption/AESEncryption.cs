using AESEncryption.Models;
using System.Security.Cryptography;

namespace AESEncryption;

public class AESEncryption : IAESEncryption
{
    private readonly Aes _aesAlg;
    private Stream _outputStream;
    public bool _disposed;

    public AESEncryption(AESConfigurationModel config)
    {
        _aesAlg = Aes.Create();
        _aesAlg.Padding = PaddingMode.PKCS7;
        _aesAlg.Key = Convert.FromBase64String(config.Key);
        _aesAlg.IV = Convert.FromBase64String(config.IV);

        _outputStream = new MemoryStream();
    }

    public Stream Decrypt(Stream stream)
    {
        try
        {
            ICryptoTransform decryptor = _aesAlg.CreateDecryptor();
            using CryptoStream csDecrypt = new(stream, decryptor, CryptoStreamMode.Read);
            csDecrypt.CopyTo(_outputStream);
            return _outputStream;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Stream Encrypt(Stream stream)
    {
        try
        {
            ICryptoTransform encryptor = _aesAlg.CreateEncryptor();
            using CryptoStream csEncrypt = new(stream, encryptor, CryptoStreamMode.Read);
            csEncrypt.CopyTo(_outputStream);
            return _outputStream;
        }
        catch (Exception)
        {
            throw;
        }
    }

    #region  IDisposable members
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) _aesAlg?.Dispose();

        _disposed = true;
    }
    #endregion

}