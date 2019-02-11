using Foundation.Core.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    [TemplatePart(Name = nameof(PART_DialMenuControl), Type = typeof(DialMenuControl))]
    public sealed class JsonDialMenuControl : Control
    {
        private DialMenuControlViewModel _viewModel;
        private DialMenuControl PART_DialMenuControl { get; set; }
        #region JsonFileUri DP
        public Uri JsonFileUri
        {
            get { return (Uri)GetValue(JsonFileUriProperty); }
            set { SetValue(JsonFileUriProperty, value); }
        }

        public static readonly DependencyProperty JsonFileUriProperty =
            DependencyProperty.Register("JsonFileUri", typeof(Uri), typeof(JsonDialMenuControl), new PropertyMetadata(null, OnJsonFileUriChanged));

        private static async void OnJsonFileUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            await ((JsonDialMenuControl)d).CreateViewModel(e.NewValue as Uri);
        }
        #endregion

        #region Initialisation
        public JsonDialMenuControl()
        {
            this.DefaultStyleKey = typeof(JsonDialMenuControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_DialMenuControl = GetTemplateChild(nameof(PART_DialMenuControl)) as DialMenuControl;
            PART_DialMenuControl.ViewModel = _viewModel;
        }

        #endregion

        #region Public Methods
        public void Next()
        {
            PART_DialMenuControl?.Next();
        }

        public void Previous()
        {
            PART_DialMenuControl?.Previous();
        }

        public void Select()
        {
            PART_DialMenuControl?.Select();
        }
        #endregion

        #region Private Methods
        private async Task CreateViewModel(Uri jsonFileUri)
        {
            if (jsonFileUri != null)
            {
                var localPath = jsonFileUri.LocalPath;
                var jsonFile = await StorageFile.GetFileFromPathAsync(localPath);
                var json = await FileIO.ReadTextAsync(jsonFile);
                var conv = new JsonPathConverter(Path.GetDirectoryName(jsonFileUri.AbsolutePath));
                var dialMenuDefinition = JsonConvert.DeserializeObject<DialMenuDefinition>(json, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    Converters = new List<JsonConverter>() { conv }
                });
                _viewModel = new DialMenuControlViewModel(dialMenuDefinition);
            }

            if (PART_DialMenuControl != null)
            {
                PART_DialMenuControl.ViewModel = _viewModel;
            }
        }
        #endregion
    }
}
