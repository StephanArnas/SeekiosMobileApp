using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SeekiosApp.UWP.UserControls
{
    public sealed partial class MapSeekiosMarkerOverlay : UserControl
    {
        public string SeekiosName
        {
            get { return (string)GetValue(SeekiosNameProperty); }
            set { SetValue(SeekiosNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeekiosName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeekiosNameProperty =
            DependencyProperty.Register("SeekiosName", typeof(string), typeof(MapSeekiosMarkerOverlay), new PropertyMetadata(string.Empty));

        public string SeekiosDate
        {
            get { return (string)GetValue(SeekiosDateProperty); }
            set { SetValue(SeekiosDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeekiosDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeekiosDateProperty =
            DependencyProperty.Register("SeekiosDate", typeof(string), typeof(MapSeekiosMarkerOverlay), new PropertyMetadata(string.Empty));


        public MapSeekiosMarkerOverlay(string seekiosName, string seekiosDate)
        {
            InitializeComponent();
            SeekiosName = seekiosName;
            SeekiosDate = seekiosDate;

        }
    }
}
