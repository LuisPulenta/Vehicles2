using System.IO;

namespace Vehicles2.Common.Helpers
{
    public interface IFilesHelper
    {
        byte[] ReadFully(Stream input);
        bool UploadPhoto(MemoryStream stream, string folder, string name);
    }
}