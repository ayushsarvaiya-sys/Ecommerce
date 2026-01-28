using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using ECommerce.Config;

namespace ECommerce.Utils
{
    public interface ICloudinaryService
    {
        string GetCloudName();
        string GetUploadPreset();
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinarySettings _settings;

        public CloudinaryService(IOptions<CloudinarySettings> options)
        {
            _settings = options.Value;
        }

        /// <summary>
        /// Returns the Cloudinary cloud name
        /// </summary>
        public string GetCloudName()
        {
            if (string.IsNullOrEmpty(_settings.CloudName))
            {
                throw new InvalidOperationException("Cloudinary CloudName is not configured");
            }
            return _settings.CloudName;
        }

        /// <summary>
        /// Returns the Cloudinary upload preset for unsigned uploads
        /// </summary>
        public string GetUploadPreset()
        {
            if (string.IsNullOrEmpty(_settings.UploadPreset))
            {
                throw new InvalidOperationException("Cloudinary UploadPreset is not configured");
            }
            return _settings.UploadPreset;
        }
    }
}
