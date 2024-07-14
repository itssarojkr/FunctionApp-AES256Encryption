namespace AESEncryption;

public interface IAESEncryption
{
    Stream Encrypt(Stream stream);

    Stream Decrypt(Stream stream);
}
