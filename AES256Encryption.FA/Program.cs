using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using AESEncryption.Models;
using AESEncryption;
using Microsoft.Extensions.Azure;
using AES256Encryption.FA.Models;

namespace AES256Encryption.FA;

public class Program
{
    public static void Main()
    {
        try
        {

            var host = new HostBuilder()
            .ConfigureFunctionsWebApplication()
            .ConfigureAppConfiguration(builder => { })
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration config = hostContext.Configuration;
                services.AddLogging();
                
                var encryptionConfig = config.GetRequiredSection("Encryption").Get<AESConfigurationModel>();
                services.TryAddTransient<IAESEncryption>(e => new AESEncryption.AESEncryption(encryptionConfig));

                services.Configure<StorageSettings>(
                    StorageSettings.EncryptionSettings,
                    config.GetSection(StorageSettings.EncryptionSettings));

                services.Configure<StorageSettings>(
                    StorageSettings.DecryptionSettings,
                    config.GetSection(StorageSettings.DecryptionSettings));

                services.AddAzureClients(clientBuilder =>
                {
                    clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetValue<string>("AzureWebJobsStorage"));
                });
            })
            .Build();
            
            host.Run();
        }
        catch (Exception)
        {
            throw;
        }
    }
}