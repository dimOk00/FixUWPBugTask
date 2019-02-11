using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public sealed partial class DialMenuMediaControl : UserControl
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly Dictionary<string, Action<Uri>> _showMediaActions;
        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();
        private Task _transitionInTask;
        private Task _transitionOutTask;
        bool _isShowingMedia;

        #region ImageTimeoutMilliseconds DP
        public int ImageTimeoutMilliseconds
        {
            get { return (int)GetValue(ImageTimeoutMillisecondsProperty); }
            set { SetValue(ImageTimeoutMillisecondsProperty, value); }
        }

        public static readonly DependencyProperty ImageTimeoutMillisecondsProperty =
            DependencyProperty.Register("ImageTimeoutMilliseconds", typeof(int), typeof(DialMenuMediaControl), new PropertyMetadata(6000));
        #endregion

        #region MediaUriSource DP

        public Uri MediaUriSource
        {
            get { return (Uri)GetValue(MediaUriSourceProperty); }
            set { SetValue(MediaUriSourceProperty, value); }
        }

        public static readonly DependencyProperty MediaUriSourceProperty =
            DependencyProperty.Register("MediaUriSource", typeof(Uri), typeof(DialMenuMediaControl), new PropertyMetadata(null, OnMediaUriSourceChanged));

        private static async void OnMediaUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisObject = (DialMenuMediaControl)d;
            thisObject.HideAllMedia();
            if (e.NewValue != null)
            {
                var uri = (Uri)e.NewValue;
                var localPath = uri.LocalPath;
                var file = await StorageFile.GetFileFromPathAsync(localPath);
                var showMediaAction = thisObject._showMediaActions[file.ContentType];
                showMediaAction(uri);
                thisObject.TransitionIn();
            }
        }

        #endregion

        public DialMenuMediaControl()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += OnDispatcherTimerTick;
            _showMediaActions = new Dictionary<string, Action<Uri>>
            {
                { "image/png", ShowImage },
                { "image/jpeg", ShowImage },
                { "video/mp4", ShowVideo }
            };

            this.InitializeComponent();
        }

        public bool IsShowingMedia => _isShowingMedia
            || (!_transitionInTask?.IsCompleted ?? false)
            || (!_transitionOutTask?.IsCompleted ?? false);

        private void ShowImage(Uri uri)
        {
            _isShowingMedia = true;
            ImageControl.Opacity = 1;
            ImageControl.UriSource = uri;
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(ImageTimeoutMilliseconds);
            _dispatcherTimer.Start();
        }

        private void ShowVideo(Uri uri)
        {
            _isShowingMedia = true;
            VideoControl.Opacity = 1;
            if (VideoControl.MediaPlayer == null)
            {
                VideoControl.SetMediaPlayer(_mediaPlayer);
            }

            VideoControl.MediaPlayer.Source = null;
            VideoControl.UriSource = uri;
            VideoControl.MediaPlayer.PlaybackSession.PositionChanged -= OnPlaybackPosChanged;
            VideoControl.MediaPlayer.PlaybackSession.PositionChanged += OnPlaybackPosChanged;
            VideoControl.Activate();
        }

        public void HideMedia()
        {
            if ((_transitionInTask?.IsCompleted ?? true)
            && (_transitionOutTask?.IsCompleted ?? true))
            {
                if (VideoControl?.MediaPlayer?.PlaybackSession != null)
                {
                    VideoControl.MediaPlayer.PlaybackSession.PositionChanged -= OnPlaybackPosChanged;
                }
                 
                _dispatcherTimer?.Stop();
                TransitionOut();
                _isShowingMedia = false;
            }
        }

        private async void OnPlaybackPosChanged(MediaPlaybackSession sender, object args)
        {
            if ((sender.NaturalDuration - sender.Position) < TimeSpan.FromMilliseconds(1500))
            {
                _mediaPlayer.PlaybackSession.PositionChanged -= OnPlaybackPosChanged;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    TransitionOut();
                    _isShowingMedia = false;
                });
            }
        }

        private void OnDispatcherTimerTick(object sender, object e)
        {
            HideMedia();
        }

        private void TransitionIn()
        {
            var opacityTask = OpacityGrid
                .Fade(0, 0).Then()
                .Fade(1, 1500, delay: 500)
                .StartAsync();
            var blurTask = BlurGrid
                .Blur(50, 1000, easingType: EasingType.Quadratic, easingMode: Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut).Then()
                .Blur(0, 1000, easingType: EasingType.Quadratic, easingMode: Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut)
                .StartAsync();

            _transitionInTask = Task.WhenAll(opacityTask, blurTask);
        }

        private void TransitionOut()
        {
            var opacityTask = OpacityGrid
                .Fade(1, 0).Then()
                .Fade(1, 500).Then()
                .Fade(0, 1500)
                .StartAsync();
            var blurTask = BlurGrid
                .Blur(50, 1000, easingType: EasingType.Quadratic, easingMode: Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut).Then()
                .Blur(0, 1000, easingType: EasingType.Quadratic, easingMode: Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut)
                .StartAsync();

            _transitionOutTask = Task.WhenAll(opacityTask, blurTask).ContinueWith(async (t) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if (VideoControl?.MediaPlayer != null)
                        {
                            VideoControl.Deactivate();
                            ImageControl.UriSource = null;
                        }
                    });
            });
        }

        private void HideAllMedia()
        {
            VideoControl.Opacity = 0;
            ImageControl.Opacity = 0;
        }
    }
}
