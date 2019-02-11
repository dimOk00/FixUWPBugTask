using GhostCore.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    public class DialMenuControlViewModel : ViewModelBase<DialMenuDefinition>
    {
        private SectionViewModel _currentSection;
        private bool _isPreviousSectionButtonVisible;
        private bool _isNextSectionButtonVisible;
        private Uri _currentMedia;
        private Uri _sectionTransitionVideo;
        private bool _isSubMenuVisible;

        public DialMenuControlViewModel(DialMenuDefinition dialMenuDefinition) : base(dialMenuDefinition)
        {
            Sections = dialMenuDefinition.Sections.Select(s => new SectionViewModel(s)).ToList();
            SetCurrentSection(Sections.FirstOrDefault());
            MenuItemViewModel.FontSize = Model.MenuItemFontSize;
            SubMenuItemViewModel.FontSize = Model.SubMenuFontSize;
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

        public double SubMenuBackgroundBlur => Model.SubMenuBackgroundBlur;
        public int HostedWidth => Model.HostedWidth;
        public int HostedHeight => Model.HostedHeight;
        public bool IsDialPressToCancelMediaEnabled => Model.IsDialPressToCancelMediaEnabled;
        public Uri IntroMedia => string.IsNullOrEmpty(Model?.IntroMedia) ? null : new Uri(Model.IntroMedia, UriKind.Absolute);

        public Uri CurrentMedia
        {
            get => _currentMedia;
            set
            {
                _currentMedia = value;
                OnPropertyChanged(nameof(CurrentMedia));
            }
        }

        public Uri SectionTransitionVideo
        {
            get => _sectionTransitionVideo;
            set
            {
                _sectionTransitionVideo = value;
                OnPropertyChanged(nameof(SectionTransitionVideo));
            }
        }

        public List<SectionViewModel> Sections { get; }
        public SectionViewModel CurrentSection
        {
            get => _currentSection;
            set
            {
                _currentSection = value;
                OnPropertyChanged(nameof(CurrentSection));
            }
        }

        public SectionViewModel NextSection
        {
            get;
            set;
        }

        public SectionViewModel PreviousSection
        {
            get;
            set;
        }

        public bool IsPreviousSectionButtonVisible
        {
            get => _isPreviousSectionButtonVisible;
            set
            {
                _isPreviousSectionButtonVisible = value;
                OnPropertyChanged(nameof(ShouldHideNextAndPreviousButtons));
                OnPropertyChanged(nameof(IsPreviousSectionButtonVisible));
            }
        }

        public bool IsNextSectionButtonVisible
        {
            get => _isNextSectionButtonVisible;
            set
            {
                _isNextSectionButtonVisible = value;
                OnPropertyChanged(nameof(ShouldHideNextAndPreviousButtons));
                OnPropertyChanged(nameof(IsNextSectionButtonVisible));
            }
        }

        public bool ShouldHideNextAndPreviousButtons => !(IsNextSectionButtonVisible || IsPreviousSectionButtonVisible);

        public void Previous()
        {
            if (IsNextSectionButtonVisible)
            {
                // Go to last menu item and hide next button
                CurrentSection.GoToLastMenuItem();
                IsNextSectionButtonVisible = false;
            }
            else if (CurrentSection.GoToPreviousMenuItem())
            {
                // Show the previous button and deselect menu item
                IsPreviousSectionButtonVisible = true;
            }
        }

        public void Next()
        {
            if (IsPreviousSectionButtonVisible)
            {
                // Go to first menu item and hide previous button
                CurrentSection.GoToFirstMenuItem();
                IsPreviousSectionButtonVisible = false;
            }
            else if (CurrentSection.GoToNextMenuItem())
            {
                // Show next button
                IsNextSectionButtonVisible = true;
            }
        }

        public void Select()
        {
            if (IsNextSectionButtonVisible)
            {
                // go to next section
                var transitionVideoPath = CurrentSection.TransitionToNextSectionVideo;
                SectionTransitionVideo = transitionVideoPath;
                IsNextSectionButtonVisible = false;
                var nextSection = GetNextSection();
                nextSection.GoToFirstMenuItem();
                SetCurrentSection(nextSection);
            }
            else if (IsPreviousSectionButtonVisible)
            {
                // go to previous section
                var transitionVideoPath = CurrentSection.TransitionToPreviousSectionVideo;
                SectionTransitionVideo = transitionVideoPath;
                IsPreviousSectionButtonVisible = false;
                var previousSection = GetPreviousSection();
                previousSection.GoToLastMenuItem();
                SetCurrentSection(previousSection);
            }
            else
            {
                // Show media
                var mediaToDisplay = CurrentSection.Select();
                CurrentMedia = mediaToDisplay;
                IsSubMenuVisible = Sections.Any(s => s.IsSubMenuVisible);
            }
        }

        private void SetCurrentSection(SectionViewModel viewModel)
        {
            foreach (var section in Sections)
            {
                section.IsCurrentSection = false;
            }

            if (viewModel != null)
            {
                viewModel.IsCurrentSection = true;
            }

            CurrentSection = viewModel;
            PreviousSection = GetPreviousSection();
            NextSection = GetNextSection();
        }

        private SectionViewModel GetPreviousSection()
        {
            var index = Sections.IndexOf(CurrentSection);
            index = index > 0 ? index - 1 : Sections.Count - 1;
            return Sections[index];
        }

        private SectionViewModel GetNextSection()
        {
            var index = Sections.IndexOf(CurrentSection);
            index = index < Sections.Count - 1 ? index + 1 : 0;
            return Sections[index];
        }
    }
}
