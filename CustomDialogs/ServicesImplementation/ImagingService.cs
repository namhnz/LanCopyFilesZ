using System.Threading.Tasks;
using CustomDialogs.Models.Imaging;
using CustomDialogs.Services;

namespace CustomDialogs.ServicesImplementation
{
    internal sealed class ImagingService : IImagingService
    {
        public async Task<ImageModel?> GetImageModelFromDataAsync(byte[] rawData)
        {
            return new BitmapImageModel(await BitmapHelper.ToBitmapAsync(rawData));
        }

        public async Task<ImageModel?> GetImageModelFromPathAsync(string filePath, uint thumbnailSize = 64)
        {
            ImageModel? imageModel = null;

            if (await FileThumbnailHelper.LoadIconFromPathAsync(filePath, thumbnailSize, ThumbnailMode.ListView) is byte[] imageBuffer)
            {
                imageModel = await GetImageModelFromDataAsync(imageBuffer);
            }

            return imageModel;
        }
    }
}
