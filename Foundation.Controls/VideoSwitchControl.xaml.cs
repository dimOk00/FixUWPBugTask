using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Controls
{
    /// <summary>
    /// Seamless switch between different videos
    /// </summary>
    public sealed partial class VideoSwitchControl : UserControl
    {
        public event EventHandler PlaybackFinished;
        private bool _isPlayerOne;

        #region Dependency Properties
        public object Source
        {
            get => (object)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(VideoSwitchControl), new PropertyMetadata(null, OnSourceChanged));

        public double PlaybackRate
        {
            get => (double)GetValue(PlaybackRateProperty);
            set => SetValue(PlaybackRateProperty, value);
        }

        public static readonly DependencyProperty PlaybackRateProperty =
            DependencyProperty.Register(nameof(PlaybackRate), typeof(double), typeof(VideoSwitchControl), new PropertyMetadata(1.0));

        public bool IsLooping
        {
            get { return (bool)GetValue(IsLoopingProperty); }
            set { SetValue(IsLoopingProperty, value); }
        }

        public static readonly DependencyProperty IsLoopingProperty =
            DependencyProperty.Register("IsLooping", typeof(bool), typeof(VideoSwitchControl), new PropertyMetadata(false));
        #endregion

        public VideoSwitchControl()
        {
            this.InitializeComponent();
        }

        private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = d as VideoSwitchControl;
            IMediaPlaybackSource source = null;
            if (e.NewValue is IMediaPlaybackSource mediaPlaybackSource)
            {
                source = mediaPlaybackSource;
            }
            else if (e.NewValue is string filePath)
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    var file = await StorageFile.GetFileFromPathAsync(filePath);
                    if (file != null)
                    {
                        source = MediaSource.CreateFromStorageFile(file);
                    }
                }
            }
            else if (e.NewValue is Uri uri)
            {
                var file = await StorageFile.GetFileFromPathAsync(uri.LocalPath);
                source = MediaSource.CreateFromStorageFile(file);
            }

            thisControl?.ChangeSource(source);
        }

        private void ChangeSource(IMediaPlaybackSource newSource)
        {
            MediaPlayerElement newPlayer = !_isPlayerOne ? Player1 : Player2;

            newPlayer.MediaPlayer.MediaOpened -= OnMediaOpened;
            newPlayer.MediaPlayer.MediaOpened += OnMediaOpened;
            newPlayer.MediaPlayer.IsLoopingEnabled = IsLooping;
            newPlayer.Source = newSource;
        }

        private async void OnMediaOpened(MediaPlayer sender, object args)
        {
            sender.MediaOpened -= OnMediaOpened;
            MediaPlayerElement oldPlayer = _isPlayerOne ? Player1 : Player2;
            MediaPlayerElement newPlayer = !_isPlayerOne ? Player1 : Player2;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    newPlayer.MediaPlayer.PlaybackSession.PlaybackRate = PlaybackRate;

                    // Can't deterministically wait until correct time so hacking in a delay
                    await Task.Delay(250);
                    newPlayer.Opacity = 1;
                    oldPlayer.Opacity = 0;
                    oldPlayer.MediaPlayer.Pause();
                    newPlayer.MediaPlayer.MediaEnded -= OnMediaEnded;
                    newPlayer.MediaPlayer.MediaEnded += OnMediaEnded;
                    newPlayer.MediaPlayer.Play();
                    oldPlayer.Source = null;
                    _isPlayerOne = !_isPlayerOne;
                });
        }

        private async void OnMediaEnded(MediaPlayer sender, object args)
        {
            sender.MediaEnded -= OnMediaEnded;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PlaybackFinished?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
