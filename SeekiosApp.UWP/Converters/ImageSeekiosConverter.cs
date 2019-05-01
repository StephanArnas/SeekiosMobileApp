using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace SeekiosApp.UWP.Converters
{
    public class ImageSeekiosConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrEmpty(value as string))
            {
                // Return default image
                var bitmapImage = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/DefaultSeekios.png"));
                return bitmapImage;
            }
            else
            {
                // Return seekios image
                var imageBytes = System.Convert.FromBase64String(value as string);
                using (InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream())
                using (DataWriter dataWriter = new DataWriter(memoryStream.GetOutputStreamAt(0)))
                {
                    dataWriter.WriteBytes(imageBytes);
                    dataWriter.StoreAsync().GetResults();
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(memoryStream);
                    return bitmapImage;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
