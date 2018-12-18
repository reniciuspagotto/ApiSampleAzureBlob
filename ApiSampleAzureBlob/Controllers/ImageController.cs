using ApiSampleAzureBlob.Option;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System.IO;
using System.Threading.Tasks;

namespace ApiSampleAzureBlob.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IOptions<StorageConfiguration> _storageConfig = null;

        public ImageController(IOptions<StorageConfiguration> config)
        {
            _storageConfig = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var response = await UploadFileStreamToStorage(stream, file.FileName);
                return Ok(response);
            }
                
        }

        private async Task<string> UploadFileStreamToStorage(Stream fileStream, string fileName)
        {
            var storageCredentials = new StorageCredentials(_storageConfig.Value.AccountName, _storageConfig.Value.AccountKey);
            var storageAccount = new CloudStorageAccount(storageCredentials, true);
            var blobAzure = storageAccount.CreateCloudBlobClient();
            var container = blobAzure.GetContainerReference(_storageConfig.Value.ContainerName);
            var blob = container.GetBlockBlobReference(fileName);

            await blob.UploadFromStreamAsync(fileStream);
            return blob.SnapshotQualifiedStorageUri.PrimaryUri.ToString();
        }
    }
}