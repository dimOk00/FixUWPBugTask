using System;
using GhostCore.MVVM;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class SectionViewModel : ViewModelBase<Section>
    {
        private MenuItemViewModel _selectedMenuItem;
        private bool _isCurrentSection;
        private bool _isSubMenuVisible;

        public SectionViewModel(Section section) : base(section)
        {
            MenuItems = section.MenuItems.Select(m => new MenuItemViewModel(m)).ToList();
            SetCurrentMenuItem(MenuItems.FirstOrDefault());
        }

        public string Title => Model.Title;
        public Uri BackgroundVideo => Model.BackgroundVideo;
        public Uri TransitionToPreviousSectionVideo => Model.TransitionToPreviousSectionVideo;
        public Uri TransitionToNextSectionVideo => Model.TransitionToNextSectionVideo;
        public List<MenuItemViewModel> MenuItems { get; }

        public bool IsCurrentSection
        {
            get => _isCurrentSection;
            set
            {
                if (_isCurrentSection != value)
                {
                    _isCurrentSection = value;
                    OnPropertyChanged(nameof(IsCurrentSection));
                }
            }
        }

        public bool IsSubMenuVisible
        {
            get => _isSubMenuVisible;
            set
            {
                if (_isSubMenuVisible != value)
                {
                    _isSubMenuVisible = value;
                    OnPropertyChanged(nameof(IsSubMenuVisible));
                }
            }
        }

        public MenuItemViewModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            private set
            {
                _selectedMenuItem = value;
                OnPropertyChanged(nameof(SelectedMenuItem));
            }
        }

        internal bool GoToPreviousMenuItem()
        {
            bool alreadyOnFirstMenuItem = false;
            if (SelectedMenuItem != null && SelectedMenuItem.IsSubMenuVisible)
            {
                // Go to previous submenu item
                SelectedMenuItem.GoToPreviousSubMenuItem();
            }
            else if (SelectedMenuItem != null)
            {
                // Attempt to go to previous menu item
                var curIndex = MenuItems.IndexOf(SelectedMenuItem);
                curIndex--;
                if (curIndex >= 0)
                {
                    SetCurrentMenuItem(MenuItems[curIndex]);
                }
                else
                {
                    // Could not go to previous menu item as we are already on the first
                    SetCurrentMenuItem(null);
                    alreadyOnFirstMenuItem = true;
                }
            }

            return alreadyOnFirstMenuItem;
        }

        internal bool GoToNextMenuItem()
        {
            bool alreadyOnLastMenuItem = false;
            if (SelectedMenuItem != null && SelectedMenuItem.IsSubMenuVisible)
            {
                // Go to next submenu item
                SelectedMenuItem.GoToNextSubMenuItem();
            }
            else if (SelectedMenuItem != null)
            {
                // Attempt to go to next menu item
                var curIndex = MenuItems.IndexOf(SelectedMenuItem);
                curIndex++;
                if (curIndex < MenuItems.Count)
                {
                    SetCurrentMenuItem(MenuItems[curIndex]);
                }
                else
                {
                    // Could not go to next menu item as we are already on the last
                    SetCurrentMenuItem(null);
                    alreadyOnLastMenuItem = true;
                }
            }

            return alreadyOnLastMenuItem;
        }

        internal Uri Select()
        {
            var media = SelectedMenuItem?.Select() ?? null;
            if (SelectedMenuItem?.IsSubMenuVisible ?? false)
            {
                foreach (var menuItem in MenuItems)
                {
                    menuItem.IsVisible = menuItem == SelectedMenuItem;
                }
            }
            else
            {
                foreach (var menuItem in MenuItems)
                {
                    menuItem.IsVisible = true;
                }
            }

            IsSubMenuVisible = MenuItems.Any(m => m.IsSubMenuVisible);

            return media;
        }

        internal void GoToFirstMenuItem()
        {
            SetCurrentMenuItem(MenuItems.FirstOrDefault());
        }

        internal void GoToLastMenuItem()
        {
            SetCurrentMenuItem(MenuItems.LastOrDefault());
        }

        private void SetCurrentMenuItem(MenuItemViewModel menuItemViewModel)
        {
            foreach (var menuItem in MenuItems)
            {
                menuItem.IsCurrentMenuItem = false;
            }

            if (menuItemViewModel != null)
            {
                menuItemViewModel.IsCurrentMenuItem = true;
            }

            SelectedMenuItem = menuItemViewModel;
        }
    }
}
