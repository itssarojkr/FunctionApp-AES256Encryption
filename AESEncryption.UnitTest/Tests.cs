using System.Security.Cryptography;
using AESEncryption.Models;

namespace AESEncryption.UnitTest;

[TestClass]
public class Tests
{
    public TestContext TestContext { get; set; }

    private string EncryptedOutputFilePath => @"Samples\EncryptedText_Output.xml";
    private string DecryptedOutputFilePath => @"Samples\PlainText_Ouput.xml";
    private string SamplePlainTextFilePath => @"Samples\PlainText.xml";
    private string SampleEncryptedFilePath => @"Samples\Encrypted.xml";

    [TestMethod]
    public void TestEncryption()
    {
        AESEncryption aesObject = getAESObject();
        Assert.IsNotNull(aesObject);

        if (File.Exists(SamplePlainTextFilePath))
        {
            var fileStream = File.OpenRead(SamplePlainTextFilePath);
            Assert.IsNotNull(fileStream);

            Stream sEncrypted = aesObject.Encrypt(fileStream);
            Assert.IsNotNull(sEncrypted);
            File.WriteAllBytes(EncryptedOutputFilePath, ((MemoryStream)sEncrypted).ToArray());

            TestContext.AddResultFile(EncryptedOutputFilePath);
        }
    }

    [TestMethod]
    public void TestDecryption()
    {
        AESEncryption aesObject = getAESObject();
        Assert.IsNotNull(aesObject);

        if (File.Exists(SampleEncryptedFilePath))
        {
            var fileStream = File.OpenRead(SampleEncryptedFilePath);
            Assert.IsNotNull(fileStream);

            Stream sDecrypted = aesObject.Decrypt(fileStream);
            Assert.IsNotNull(sDecrypted);

            File.WriteAllBytes(DecryptedOutputFilePath, ((MemoryStream)sDecrypted).ToArray());
            TestContext.AddResultFile(DecryptedOutputFilePath);

            string sPlainText = File.ReadAllText(SamplePlainTextFilePath);

            sDecrypted.Seek(0, SeekOrigin.Begin);
            string sDecryptedText = new StreamReader(sDecrypted).ReadToEnd();
            Assert.AreEqual(sDecryptedText, sPlainText);
        }
    }

    private AESEncryption getAESObject()
    {
        Aes aesAlgorithm = Aes.Create();
        aesAlgorithm.KeySize = 256;

        AESConfigurationModel config = new()
        {
            Key = TestContext.Properties["AESEncryption:Key"].ToString(),
            IV = TestContext.Properties["AESEncryption:IV"].ToString()
        };

        return new AESEncryption(config);
    }
}