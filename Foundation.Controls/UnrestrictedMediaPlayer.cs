using Foundation.Controls.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Controls
{
    public sealed class UnrestrictedMediaPlayer : MediaPlayerElement, IMultimediaControl
    {
        private uint _fileWidth;
        private uint _fileHeight;

        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }

        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(UnrestrictedMediaPlayer), new PropertyMetadata(null, OnUriSourceChanged));


        public UnrestrictedMediaPlayer()
        {
            Unloaded += UnrestrictedMediaPlayer_Unloaded;
        }

        private void UnrestrictedMediaPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Source = null;
        }

        private static async void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as UnrestrictedMediaPlayer;

            var uri = e.NewValue as Uri;
            var file = await StorageFile.GetFileFromPathAsync(uri.LocalPath);

            var videoProps = await file.Properties.GetVideoPropertiesAsync();
            that._fileWidth = videoProps.Width;
            that._fileHeight = videoProps.Height;

            that.Source = MediaSource.CreateFromStorageFile(file);
        }

        public async Task<Size> GetExpectedSize()
        {
            if (MediaPlayer == null)
                return new Size(_fileWidth, _fileHeight);

            double aw = 0, ah = 0;
            var w = (double)MediaPlayer.PlaybackSession.NaturalVideoWidth;
            var h = (double)MediaPlayer.PlaybackSession.NaturalVideoHeight;
            var ar = w / h;
            aw = ar * ActualHeight;
            ah = ActualHeight;

            await Task.CompletedTask;
            return new Size(aw, ah);
        }

        public void Activate()
        {
            if (MediaPlayer == null)
                return;

            MediaPlayer.Play();
        }

        public void Deactivate()
        {
            MediaPlayer.Pause();
            MediaPlayer.PlaybackSession.Position = TimeSpan.FromMilliseconds(0);
        }
    }
}
