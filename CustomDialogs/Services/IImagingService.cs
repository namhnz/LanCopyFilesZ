using System.Threading.Tasks;

namespace CustomDialogs.Services
{
    public interface IImagingService
    {
        Task<ImageModel?> GetImageModelFromDataAsync(byte[]? rawData);

        Task<ImageModel?> GetImageModelFromPathAsync(string filePath, uint thumbnailSize = 64u);
    }
}
