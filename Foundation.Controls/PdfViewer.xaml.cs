using Foundation.Controls.Abstract;
using GhostCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

namespace Foundation.Controls
{
    public sealed partial class PdfViewer : UserControl, IMultimediaControl
    {
        private PdfDocument _currentPdfDoc;

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        internal bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public ObservableCollection<BitmapImage> ListViewSource
        {
            get { return (ObservableCollection<BitmapImage>)GetValue(ListViewSourceProperty); }
            set { SetValue(ListViewSourceProperty, value); }
        }

        public double RenderWidth
        {
            get { return (double)GetValue(RenderWidthProperty); }
            set { SetValue(RenderWidthProperty, value); }
        }

        public static readonly DependencyProperty RenderWidthProperty =
            DependencyProperty.Register("RenderWidth", typeof(double), typeof(PdfViewer), new PropertyMetadata((double)3840));

        public static readonly DependencyProperty ListViewSourceProperty =
            DependencyProperty.Register("ListViewSource", typeof(ObservableCollection<BitmapImage>), typeof(PdfViewer), new PropertyMetadata(null));

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(PdfViewer), new PropertyMetadata(true));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(PdfViewer), new PropertyMetadata(null, OnSourceChanged));

        public PdfViewer()
        {
            ListViewSource = new ObservableCollection<BitmapImage>();
            InitializeComponent();
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as PdfViewer;
            var uri = e.NewValue as Uri;

            that.OnSourChanged(uri);
        }

        private async void OnSourChanged(Uri uri)
        {
            try
            {
                pnlStatus.Visibility = Visibility.Collapsed;
                IsLoading = true;
                var f = await StorageFile.GetFileFromPathAsync(uri.LocalPath);

                _currentPdfDoc = await PdfDocument.LoadFromFileAsync(f);
                if (_currentPdfDoc == null)
                    return;

                for (uint i = 0; i < _currentPdfDoc.PageCount; i++)
                {
                    lblLiveStatus.Text = $"Loading Page {i + 1} out of {_currentPdfDoc.PageCount}";

                    var page = _currentPdfDoc.GetPage(i);
                    var pageData = new InMemoryRandomAccessStream();

                    var aspectRatio = page.Size.Width / page.Size.Height;
                    var size = new Size(RenderWidth, RenderWidth / aspectRatio);
                    await page.RenderToStreamAsync(pageData, new PdfPageRenderOptions
                    {
                        BackgroundColor = Colors.White,
                        DestinationWidth = (uint)size.Width,
                        DestinationHeight = (uint)size.Height
                    });

                    var bimg = new BitmapImage();
                    await bimg.SetSourceAsync(pageData);

                    ListViewSource.Add(bimg);

                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                lblStatus.Text = ex.ToString();
                pnlStatus.Visibility = Visibility.Visible;
            }
        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView == false)
            {
                e.DestinationItem.Item = e.SourceItem.Item;
            }
        }

        public async Task<Size> GetExpectedSize()
        {
            //var ws = (itm as PDFContentItemViewModel).WindowSettings;
            //aw = ws.DefaultWidth;
            //ah = ws.DefaultHeight;

            // TODO figure out how to get the correct size for this
            await Task.CompletedTask;
            return new Size(940, 540);
        }

        public void Activate()
        {
            //Do nothing
        }

        public void Deactivate()
        {
            //Do nothing
        }
    }
}
