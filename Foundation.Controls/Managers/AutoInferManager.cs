using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Foundation.Controls.Managers
{
    public class AutoInferManager
    {
        protected Dictionary<string, Func<Uri, ContentItemViewModel>> _contentTypeFactory;
        private ObservableCollection<ContentItemViewModel> _source;

        public AutoInferManager(ObservableCollection<ContentItemViewModel> sourceCollection)
        {
            _source = sourceCollection;
            _contentTypeFactory = new Dictionary<string, Func<Uri, ContentItemViewModel>>
            {
                { "image/png", CreateImageContentItem },
                { "image/jpeg", CreateImageContentItem },
                { "video/mp4", CreateVideoContentItem },
                { "application/pdf", CreatePDFContentItem }
            };
        }

        public async Task AutoInferFromPath(object parent, string localPath)
        {
            var sf = await StorageFolder.GetFolderFromPathAsync(localPath);
            var files = await sf.GetFilesAsync();
            foreach (var file in files)
            {
                var fact = _contentTypeFactory[file.ContentType];
                var item = fact(new Uri(file.Path));
                item.Parent = parent;
                _source.Add(item);
            }
        }

        public async Task AutoInferFromPath(string localPath)
        {
            var sf = await StorageFolder.GetFolderFromPathAsync(localPath);
            var files = await sf.GetFilesAsync();
            foreach (var file in files)
            {
                if (!_contentTypeFactory.ContainsKey(file.ContentType))
                    continue;

                var fact = _contentTypeFactory[file.ContentType];
                var item = fact(new Uri(file.Path));
                _source.Add(item);
            }
        }

        public async Task AutoInferFromPath(string localPath, bool subfolderAsCategories)
        {
            if (!subfolderAsCategories)
            {
                await AutoInferFromPath(localPath);
                return;
            }

            var sf = await StorageFolder.GetFolderFromPathAsync(localPath);

            var folders = await sf.GetFoldersAsync();
            foreach (var folder in folders)
            {

                var mmCont = CreateMMContentItem();
                mmCont.DisplayLabel = folder.DisplayName;
                mmCont.Name = folder.DisplayName;                

                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                {
                    if (!_contentTypeFactory.ContainsKey(file.ContentType))
                        continue;

                    var fact = _contentTypeFactory[file.ContentType];
                    var item = fact(new Uri(file.Path));
                    item.Parent = mmCont;
                    mmCont.Children.Add(item);
                }

                _source.Add(mmCont);
            }
        }

        protected ContentItemViewModel CreatePDFContentItem(Uri path)
        {
            return new PDFContentItemViewModel(new PDFContentItem() { Path = path, Type = ContentItemType.PDF });
        }

        protected ContentItemViewModel CreateVideoContentItem(Uri path)
        {
            return new VideoContentItemViewModel(new VideoContentItem() { Path = path, Type = ContentItemType.Video });
        }

        protected ContentItemViewModel CreateImageContentItem(Uri path)
        {
            return new ImageContentItemViewModel(new ImageContentItem() { Path = path, Type = ContentItemType.Image });
        }

        protected MultimediaContentItemViewModel CreateMMContentItem()
        {
            return new MultimediaContentItemViewModel(new MultimediaContentItem() { AutoInferFromPath = false, Type = ContentItemType.MultimediaControl });
        }
    }

}
