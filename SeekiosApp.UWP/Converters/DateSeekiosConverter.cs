using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using SeekiosApp.Extension;

namespace SeekiosApp.UWP.Converters
{
    public class DateSeekiosConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int idSeekios = 0;
            int.TryParse(value.ToString(), out idSeekios);
            var seekios = SeekiosApp.App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == idSeekios);
            if (seekios != null && seekios.LastKnownLocation_dateLocationCreation.HasValue)
            {
                return seekios.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
            }
            else return App.ResourceLoader.GetString("DateSeekios_NoPosition");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
