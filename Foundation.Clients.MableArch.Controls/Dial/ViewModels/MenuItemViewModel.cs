using GhostCore.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class MenuItemViewModel : ViewModelBase<MenuItem>
    {
        private SubMenuItemViewModel _currentSubMenuItem;
        private bool _isSubMenuVisible;
        private bool _isCurrentMenuItem;
        private bool _isVisible = true;

        public MenuItemViewModel(MenuItem menuItem) : base(menuItem)
        {
            SubMenuItems = menuItem.SubMenuItems.Select(s => new SubMenuItemViewModel(s)).ToList();
            if (SubMenuItems.Any())
            {
                SubMenuItems.Add(new SubMenuItemViewModel(SubMenuItem.Back));
            }
            _currentSubMenuItem = SubMenuItems.FirstOrDefault();
        }

        public static int FontSize { get; set; }
        public string Title => Model.Title;
        public int PosX => Model.PosX;
        public int PosY => Model.PosY;
        public Uri Media => Model.Media;
        public bool IsCurrentMenuItem
        {
            get => _isCurrentMenuItem;
            set
            {
                if (_isCurrentMenuItem != value)
                {
                    _isCurrentMenuItem = value;
                    OnPropertyChanged(nameof(IsCurrentMenuItem));
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        public bool HasSubMenu => SubMenuItems?.Any() ?? false;
        public List<SubMenuItemViewModel> SubMenuItems { get; }
        
        public bool IsSubMenuVisible
        {
            get => _isSubMenuVisible;
            set
            {
                _isSubMenuVisible = value;
                OnPropertyChanged(nameof(IsSubMenuVisible));
            }
        }

        internal void GoToPreviousSubMenuItem()
        {
            var curIndex = SubMenuItems.IndexOf(_currentSubMenuItem);
            curIndex--;
            curIndex = Math.Max(0, curIndex);
            SetCurrentSubMenuItem(SubMenuItems[curIndex]);
        }

        internal void GoToNextSubMenuItem()
        {
            var curIndex = SubMenuItems.IndexOf(_currentSubMenuItem);
            curIndex++;
            curIndex = Math.Min(SubMenuItems.Count - 1, curIndex);
            SetCurrentSubMenuItem(SubMenuItems[curIndex]);
        }

        private void SetCurrentSubMenuItem(SubMenuItemViewModel subMenuItemViewModel)
        {
            foreach (var subMenu in SubMenuItems)
            {
                subMenu.IsCurrentSubMenuItem = false;
            }

            if (subMenuItemViewModel != null)
            {
                subMenuItemViewModel.IsCurrentSubMenuItem = true;
            }

            _currentSubMenuItem = subMenuItemViewModel;
        }

        internal Uri Select()
        {
            Uri mediaToShow = null;
            if (HasSubMenu && !IsSubMenuVisible)
            {
                SetCurrentSubMenuItem(SubMenuItems.FirstOrDefault());
                IsSubMenuVisible = true;
            }
            else if (_currentSubMenuItem?.Model == SubMenuItem.Back)
            {
                IsSubMenuVisible = false;
                SetCurrentSubMenuItem(null);
            }
            else if (_currentSubMenuItem != null)
            {
                mediaToShow = _currentSubMenuItem.Media;
            }
            else
            {
                mediaToShow = Media;
            }

            return mediaToShow;
        }
    }
}
