using Foundation.Core.Data;
using GhostCore.Math;
using GhostCore.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

namespace Foundation.Core.ViewModels
{
    public class ContentAreaViewModel : HostedLayoutItemViewModel
    {
        private ContentItemViewModel _content;
        private int _color;
        private RelayCommand _clearContentCommand;

        public int Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged(nameof(Color)); }
        }

        public bool HasContent
        {
            get { return ModelAs<ContentArea>().HasContent; }
            set { ModelAs<ContentArea>().HasContent = value; OnPropertyChanged(nameof(HasContent)); }
        }

        public ContentItemViewModel Content
        {
            get { return _content; }
            set
            {
                _content = value;
                ModelAs<ContentArea>().Content = _content?.Model;
                OnPropertyChanged(nameof(Content));
                OnPropertyChanged(nameof(PopupCloseButtonVisibility));
            }
        }

        public bool IsPopup
        {
            get
            {
                return ModelAs<ContentArea>().IsPopup;
            }
            set
            {
                ModelAs<ContentArea>().IsPopup = value;
                OnPropertyChanged(nameof(IsPopup));
                OnPropertyChanged(nameof(PopupCloseButtonVisibility));
            }
        }

        public bool IsLightDismiss
        {
            get { return ModelAs<ContentArea>().IsLightDismiss; }
            set
            {
                ModelAs<ContentArea>().IsLightDismiss = value;
                OnPropertyChanged(nameof(IsLightDismiss));
                OnPropertyChanged(nameof(PopupCloseButtonVisibility));
            }
        }

        public ContentAreaState State
        {
            get { return ModelAs<ContentArea>().State; }
            set { ModelAs<ContentArea>().State = value; OnPropertyChanged(nameof(State)); }
        }

        public List<Vector2D> Shape
        {
            get { return ModelAs<ContentArea>().Shape; }
            set { ModelAs<ContentArea>().Shape = value; OnPropertyChanged(nameof(Shape)); }
        }

        public Visibility PopupCloseButtonVisibility => (IsPopup && !IsLightDismiss && Content != null) ? Visibility.Visible : Visibility.Collapsed;

        public RelayCommand ClearContent
        {
            get { return _clearContentCommand; }
            set { _clearContentCommand = value; OnPropertyChanged(nameof(ClearContent)); }
        }


        public ContentAreaViewModel(HostedLayoutItem model)
            : base(model)
        {
            _clearContentCommand = new RelayCommand(OnClearContent);

            var contentArea = ModelAs<ContentArea>();
            if (contentArea.HasContent)
            {
                var contentVm = ViewModelMapper.GetViewModel(contentArea.Content);
                if (contentVm == null)
                {
                    contentVm = ContentItemViewModel.CreateItem(contentArea.Content);
                }

                Content = (ContentItemViewModel)contentVm;
            }
        }

        private void OnClearContent(object obj)
        {
            Content = null;
        }

        public override HostedLayoutItemViewModel Clone()
        {
            var oldContent = ModelAs<ContentArea>();

            var newContent = new ContentArea()
            {
                Content = oldContent.Content,
                HasContent = oldContent.HasContent,
                Height = oldContent.Height,
                IsLightDismiss = oldContent.IsLightDismiss,
                IsPopup = oldContent.IsPopup,
                Name = oldContent.Name,
                Shape = oldContent.Shape?.ToList(),
                State = oldContent.State,
                Width = oldContent.Width,
                X = oldContent.X,
                Y = oldContent.Y
            };

            var newVm = new ContentAreaViewModel(newContent);

            return newVm;
        }
    }
}
