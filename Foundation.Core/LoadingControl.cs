using Foundation.Core.Data;
using Foundation.Core.IO;
using Foundation.Networking;
using GhostCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Foundation.Core
{
    public sealed class LoadingControl : Control
    {
        #region Events


        public event EventHandler LoadFinished;
        public event EventHandler<Exception> LoadFailed;

        #endregion

        #region Fields


        private GlobalSettings _globalSettings;

        #endregion

        public LoadingControl()
        {
            DefaultStyleKey = typeof(LoadingControl);
        }

        #region Loading Logic

        public async Task LoadAppData(string knownPath = null, bool loadDebugJson = false)
        {
            try
            {
                if (knownPath == null)
                {
                    try
                    {
                        var sfolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("AppRootFolder");
                        knownPath = sfolder.Path;
                    }
                    catch (Exception)
                    {
                    }
                }

                var folderPicker = new FolderPicker
                {
                    SuggestedStartLocation = PickerLocationId.Desktop
                };
                folderPicker.FileTypeFilter.Add("*");

                StorageFolder folder = null;
                if (knownPath == null)
                    folder = await folderPicker.PickSingleFolderAsync();
                else
                    folder = await StorageFolder.GetFolderFromPathAsync(knownPath);

                if (folder != null)
                {
                    var configFile = await folder.GetFileAsync(loadDebugJson ? "config_debug.json" : "config.json");
                    var configText = await FileIO.ReadTextAsync(configFile);

                    var conv = new JsonPathConverter(folder.Path);

                    var rootConfig = JsonConvert.DeserializeObject<RootConfiguration>(configText, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        PreserveReferencesHandling = PreserveReferencesHandling.All,
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        Converters = new List<JsonConverter>() { conv }
                    });

                    _globalSettings = new GlobalSettings() { RootFolder = folder.Path };

                    await ProcessAppConfig(rootConfig);

                    var config = ServiceLocator.Instance.Resolve<RemoteRelayConfiguration>();
                    config.ServerURL = rootConfig.RemoteSettings.ServerURL;

                    await RemoteRelayManager.Instance.Initialize(config);

                    ServiceLocator.Instance.Register(rootConfig);
                    ServiceLocator.Instance.Register(_globalSettings);

                    // Application now has read/write access to all contents in the picked folder
                    // (including other sub-folder contents)
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("AppRootFolder", folder);

                    var localSettings = ApplicationData.Current.LocalSettings;
                    if (localSettings.Values.ContainsKey("AppRootFolder"))
                    {
                        localSettings.Values["AppRootFolder"] = folder.Path;
                    }
                    else
                    {
                    }

                    OnLoadFinished();
                }
            }
            catch (Exception ex)
            {
                OnLoadFailed(ex);
            }
        }

        private async Task ProcessAppConfig(RootConfiguration appConfig)
        {
            foreach (var x in appConfig.Sessions)
            {
                await ProcessFontSettings(x.ApplicationSettings.FontSettings);
            }
        }

        private async Task ProcessFontSettings(FontSettings fontSetings)
        {
            var primaryFontPath = _globalSettings.GetFullPath(fontSetings.PrimaryFontPath);
            var secondaryFontPath = _globalSettings.GetFullPath(fontSetings.SecondaryFontPath);
            var tertiaryFontPath = _globalSettings.GetFullPath(fontSetings.TertiaryFontPath);

            var primaryIconFontPath = _globalSettings.GetFullPath(fontSetings.PrimaryIconFontPath);
            var secondaryIconFontPath = _globalSettings.GetFullPath(fontSetings.SecondaryIconFontPath);
            var tertiaryIconFontPath = _globalSettings.GetFullPath(fontSetings.TertiaryIconFontPath);

            await LoadFont(primaryFontPath, "PrimaryFont");
            await LoadFont(secondaryFontPath, "SecondaryFont");
            await LoadFont(tertiaryFontPath, "TertiaryFont");
            await LoadFont(primaryIconFontPath, "PrimaryIconFont");
            await LoadFont(secondaryIconFontPath, "SecondaryIconFont");
            await LoadFont(tertiaryIconFontPath, "TertiaryIconFont");
        }

        private async Task LoadFont(string filepath, string key)
        {
            if (filepath == null)
                return;

            var file = await StorageFile.GetFileFromPathAsync(filepath);
            var extension = Path.GetExtension(file.Path);

            if (extension == ".ttf" || extension == ".otf")
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("fontcache", CreationCollisionOption.OpenIfExists);
                var importedFile = await file.CopyAsync(folder, file.Name, NameCollisionOption.ReplaceExisting);

                string fontPath = "ms-appdata:///local/fontcache/" + importedFile.Name;

                string fontName = Path.GetFileNameWithoutExtension(file.Path);
                var family = new FontFamily(fontPath + "#" + fontName);

                Application.Current.Resources[key] = family;
            }
        }

        #endregion

        #region Event Triggers

        private void OnLoadFinished()
        {
            if (LoadFinished == null)
                return;

            LoadFinished(this, EventArgs.Empty);
        }
        private void OnLoadFailed(Exception ex)
        {
            if (LoadFailed == null)
                return;

            LoadFailed(this, ex);
        }

        #endregion

    }
}
