﻿using System;
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
    public class SendEmailTriger
    {
        private readonly ILogger _logger;
        private readonly BlobContainerClient _copyContainerClient;
        private readonly ISendGridClient sendGridClient;
        private readonly EmailOptions emailOptions;

        public SendEmailTriger(ILoggerFactory loggerFactory, IAzureClientFactory<BlobServiceClient> blobClientFactory, IOptions<EmailOptions> emailOptions, ISendGridClient sendGridClient)
        {
            _copyContainerClient = blobClientFactory. CreateClient("Sender").GetBlobContainerClient("apidocx");
            _logger = loggerFactory.CreateLogger<SendEmailTriger>();

            this.sendGridClient = sendGridClient;
            this.emailOptions = emailOptions.Value;
        }


        [Function("SendEmailTriger")]
        public async Task RunAsync([BlobTrigger("apidocx/{name}", Connection = "" )] String content,string name)
        {
            var file = _copyContainerClient.GetBlobClient(name);
            BlobProperties blobProp = await file.GetPropertiesAsync();
            blobProp.Metadata.TryGetValue("email", out string email );

            var message = new SendGridMessage
            {
                From = new EmailAddress("reenbit.testing.task.by.stotskyi@gmail.com","Andrii Stotskyi"),
                Subject = "File Successfully Uploaded: Access Details Inside",
                PlainTextContent = $"I hope this message finds you well!I wanted to let you know that the file you uploaded to our blob storage has been successfully processed. You can access it using the link below. Just a heads up, the link will remain active for the next hour, so make sure to grab your file within that time frame 1 hour: {await CreateServiceSASBlob(file)}" 
            };


            message.AddTo(new EmailAddress(email, "Testing User"));
            var response = await sendGridClient.SendEmailAsync(message);
            _logger.LogInformation(response.Body.ToString(), response.Headers, response.StatusCode);
        }
        public static async Task<string> CreateServiceSASBlob( BlobClient blobClient,  string storedPolicyName = null)
        {
            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }
                return  blobClient.GenerateSasUri(sasBuilder).ToString();

            }
            return null;
           
        }
    }
}