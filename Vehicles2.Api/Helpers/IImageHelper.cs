using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Vehicles2.Api.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);

        string UploadImage(byte[] pictureArray, string folder);

        Task<string> UploadImage2Async(Stream imageFile, string folder);
    }
}