using Foundation.Core.Data;
using GhostCore.Math;
using GhostCore.MVVM;
using GhostCore.UWP.AutoFormGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Core.ViewModels
{
    public class InteractableItemViewModel : HostedLayoutItemViewModel
    {
        private readonly static Array _actionSoureStatic = Enum.GetValues(typeof(HostedLayoutAction));
        private ContentAreaViewModel _contentAreaTarget;
        private ContentItemViewModel _itemTarget;
        private HostedLayoutPageViewModel _pageTarget;
        private RelayCommand _clearContentCommand;
        private RelayCommand _importStringCommand;
        private string _polygonString;

        public Array ActionSource => _actionSoureStatic;

        public virtual string ItemTitle => "Interactable Item";

        public HostedLayoutAction Action
        {
            get { return ModelAs<InteractableItem>().Action; }
            set { ModelAs<InteractableItem>().Action = value; OnPropertyChanged(nameof(Action)); }
        }

        public int HighlightColor
        {
            get { return ModelAs<InteractableItem>().HighlightColor; }
            set { ModelAs<InteractableItem>().HighlightColor = value; OnPropertyChanged(nameof(HighlightColor)); }
        }

        public ContentAreaViewModel ContentAreaTarget
        {
            get { return _contentAreaTarget; }
            set
            {
                _contentAreaTarget = value;
                ModelAs<InteractableItem>().ContentAreaTarget = (ContentArea)_contentAreaTarget?.Model;
                OnPropertyChanged(nameof(ContentAreaTarget));
            }
        }

        public ContentItemViewModel ItemTarget
        {
            get { return _itemTarget; }
            set
            {
                _itemTarget = value;
                ModelAs<InteractableItem>().ItemTarget = _itemTarget?.Model;
                OnPropertyChanged(nameof(ItemTarget));
            }
        }

        public HostedLayoutPageViewModel PageTarget
        {
            get { return _pageTarget; }
            set
            {
                _pageTarget = value;
                ModelAs<InteractableItem>().PageTargetName = _pageTarget?.Name;
                ModelAs<InteractableItem>().PageTarget = _pageTarget?.Model;
                OnPropertyChanged(nameof(PageTarget));
            }
        }


        public bool IsNonStandardHitArea
        {
            get { return ModelAs<InteractableItem>().IsNonStandardHitArea; }
            set { ModelAs<InteractableItem>().IsNonStandardHitArea = value; OnPropertyChanged(nameof(IsNonStandardHitArea)); }
        }
        public List<Vector2D> Shape
        {
            get { return ModelAs<InteractableItem>().Shape; }
            set { ModelAs<InteractableItem>().Shape = value; OnPropertyChanged(nameof(Shape)); }
        }

        public string PolygonString
        {
            get { return _polygonString; }
            set { _polygonString = value; OnPropertyChanged(nameof(PolygonString)); }
        }

        public RelayCommand ClearContent
        {
            get { return _clearContentCommand; }
            set { _clearContentCommand = value; OnPropertyChanged(nameof(ClearContent)); }
        }

        public RelayCommand ImportStringCommand
        {
            get { return _importStringCommand; }
            set { _importStringCommand = value; OnPropertyChanged(nameof(ImportStringCommand)); }
        }

        public InteractableItemViewModel(HostedLayoutItem model)
            : base(model)
        {
            _clearContentCommand = new RelayCommand(OnClearContent);
            _importStringCommand = new RelayCommand(OnImportString);

            var ii = ModelAs<InteractableItem>();

            if (ii.ContentAreaTarget != null)
            {
                _contentAreaTarget = (ContentAreaViewModel)ViewModelMapper.GetViewModel(ii.ContentAreaTarget);
                if (_contentAreaTarget == null)
                    _contentAreaTarget = new ContentAreaViewModel(ii.ContentAreaTarget);
            }

            if (ii.ItemTarget != null)
            {
                _itemTarget = (ContentItemViewModel)ViewModelMapper.GetViewModel(ii.ItemTarget);
                if (_itemTarget == null)
                    _itemTarget = ContentItemViewModel.CreateItem(ii.ItemTarget);
            }

            if (ii.PageTarget != null)
            {
                _pageTarget = (HostedLayoutPageViewModel)ViewModelMapper.GetViewModel(ii.PageTarget);
                if (_pageTarget == null)
                    _pageTarget = new HostedLayoutPageViewModel(ii.PageTarget);
            }
        }

        private void OnImportString(object obj)
        {
            if (string.IsNullOrWhiteSpace(_polygonString))
            {
                PolygonString = "Invalid String";
                return;
            }

            var split = _polygonString.Split(' ');
            if (split.Length % 2 != 0)
            {
                PolygonString = "Invalid String";
                return;
            }

            if (Shape == null)
                Shape = new List<Vector2D>();
            else
                Shape.Clear();

            double x = 0;
            double y = 0;
            for (int i = 0; i < split.Length; i += 2)
            {
                x = double.Parse(split[i]);
                y = double.Parse(split[i + 1]);

                var pt = new Vector2D(x, y);
                Shape.Add(pt);
            }


            PolygonString = "String Imported";
        }

        private void OnClearContent(object obj)
        {
            ItemTarget = null;
        }

        public override HostedLayoutItemViewModel Clone()
        {
            var oldContent = ModelAs<InteractableItem>();

            var newContent = new InteractableItem()
            {
                Height = oldContent.Height,
                Name = oldContent.Name,
                Shape = oldContent.Shape.ToList(),
                Width = oldContent.Width,
                X = oldContent.X,
                Y = oldContent.Y,
                Action = oldContent.Action,
                ContentAreaTarget = null,
                HighlightColor = oldContent.HighlightColor,
                IsNonStandardHitArea = oldContent.IsNonStandardHitArea,
                ItemTarget = oldContent.ItemTarget,
                PageTarget = oldContent.PageTarget
            };

            var newVm = new InteractableItemViewModel(newContent);

            return newVm;
        }
    }
}
