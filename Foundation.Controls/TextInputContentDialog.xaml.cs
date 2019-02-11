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

namespace Foundation.Controls
{
    public sealed partial class TextInputContentDialog : ContentDialog
    {
        public string Text { get; set; }

        public TextInputContentDialog()
        {
            InitializeComponent();
            Loaded += TextInputContentDialog_Loaded;
        }

        private void TextInputContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (Text == null)
                return;

            tbText.Text = Text;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = (sender as TextBox).Text;
        }
    }
}
