using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ChatClient.Services
{
    public static class AvatarService
    {
        public static string BitmapImageToBase64(BitmapImage image)
        {
            if (image == null) return null;
            using (var ms = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static BitmapImage? Base64ToBitmapImage(string base64)
        {
            if (base64 == null)
                return null;

            var bytes = Convert.FromBase64String(base64);
            var bitmap = new BitmapImage();
            using (var ms = new MemoryStream(bytes)) 
            {
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            return bitmap;
        }
    }
}
