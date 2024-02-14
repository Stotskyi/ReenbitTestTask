using System;
using System.IO;
using System.Net.Mail;
using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailSenderTriger
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly BlobContainerClient _copyContainerClient;
        private readonly ISendGridClient sendGridClient;
        private readonly EmailOptions emailOptions;
        public Function1(ILoggerFactory loggerFactory, IAzureClientFactory<BlobServiceClient> blobClientFactory, IOptions<EmailOptions> emailOptions, ISendGridClient sendGridClient)
        {
            _copyContainerClient = blobClientFactory. CreateClient("Sender").GetBlobContainerClient("apidocx");
            _logger = loggerFactory.CreateLogger<Function1>();

            this.sendGridClient = sendGridClient;
            this.emailOptions = emailOptions.Value;
        }


        [Function("Function1")]
        public async Task RunAsync([BlobTrigger("apidocx/{name}", Connection = "" )] String content,string name)
        {
            var file = _copyContainerClient.GetBlobClient(name);
            BlobProperties blobProp = await file.GetPropertiesAsync();
            blobProp.Metadata.TryGetValue("email", out string email );

            var message = new SendGridMessage
            {
                From = emailOptions.From,
                Subject = emailOptions.Subject,
                PlainTextContent = $"Hello, secure with sas token for only 5 minutes {await CreateServiceSASBlob(file)}" 
            };


            message.AddTo(new EmailAddress(email));
            var response = await sendGridClient.SendEmailAsync(message);
    
        }
        public static async Task<string> CreateServiceSASBlob( BlobClient blobClient,  string storedPolicyName = null)
        {
            // Check if BlobContainerClient object has been authorized with Shared Key
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one day
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5);
                    sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

                return sasURI.ToString();
            }
            else
            {
                // Client object is not authorized via Shared Key
                return null;
            }
        }
    }
}