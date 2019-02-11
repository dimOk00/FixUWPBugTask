using Foundation.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Threading.Tasks;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Foundation.Clients.MableArch.Controls.Dial
{
    [TemplatePart(Name = nameof(PART_SectionsItemsControl), Type = typeof(ItemsControl))]
    [TemplatePart(Name = nameof(PART_DialMenuMediaControl), Type = typeof(DialMenuMediaControl))]
    [TemplatePart(Name = nameof(PART_VideoSwitchControl), Type = typeof(VideoSwitchControl))]
    [TemplatePart(Name = nameof(PART_PreviousSectionTextBlock), Type = typeof(TextBlock))]
    [TemplatePart(Name = nameof(PART_NextSectionTextBlock), Type = typeof(TextBlock))]

    public sealed class DialMenuControl : Control
    {
        private RadialController _radialController;
        private RadialControllerMenuItem _myRadialMenuItem;
        private ItemsControl PART_SectionsItemsControl { get; set; }
        private DialMenuMediaControl PART_DialMenuMediaControl { get; set; }
        private VideoSwitchControl PART_VideoSwitchControl { get; set; }
        private TextBlock PART_PreviousSectionTextBlock { get; set; }
        private TextBlock PART_NextSectionTextBlock { get; set; }

        #region ViewModel DP
        public DialMenuControlViewModel ViewModel
        {
            get { return (DialMenuControlViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(DialMenuControlViewModel), typeof(DialMenuControl), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = ((DialMenuControl)d);
            var newVm = e.NewValue as DialMenuControlViewModel;
            thisControl.DataContext = newVm;
            if (thisControl.PART_DialMenuMediaControl != null)
            {
                thisControl.PART_DialMenuMediaControl.MediaUriSource = thisControl.ViewModel?.IntroMedia;
            }
        }
        #endregion

        #region CurrentSection DP
        private SectionViewModel CurrentSection
        {
            get { return (SectionViewModel)GetValue(CurrentSectionProperty); }
            set { SetValue(CurrentSectionProperty, value); }
        }

        private static readonly DependencyProperty CurrentSectionProperty =
            DependencyProperty.Register("CurrentSection", typeof(SectionViewModel), typeof(DialMenuControl), new PropertyMetadata(null, OnCurrentSectionChanged));

        private static void OnCurrentSectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = d as DialMenuControl;
            if (thisControl.PART_SectionsItemsControl != null)
            {
                var oldContainer = thisControl.PART_SectionsItemsControl.ContainerFromItem(e.OldValue);
                if (oldContainer != null || e.OldValue != null)
                {
                    thisControl.PART_VideoSwitchControl.PlaybackFinished -= thisControl.OnTransitionVideoFinished;
                    thisControl.PART_VideoSwitchControl.PlaybackFinished += thisControl.OnTransitionVideoFinished;
                    thisControl.PART_VideoSwitchControl.IsLooping = false;
                    thisControl.PART_VideoSwitchControl.Source = thisControl.SectionTransitionVideo;
                    var oldSectionControl = oldContainer.FindDescendant<DialMenuSectionControl>();
                    oldSectionControl?.Hide();
                }
                else
                {
                    // The is the first section to be displayed so skip the transition video
                    thisControl.PlayVideoForCurrentSection();
                }

                var newContainer = thisControl.PART_SectionsItemsControl.ContainerFromItem(e.NewValue);
                var newSectionControl = newContainer?.FindDescendant<DialMenuSectionControl>();
                newSectionControl?.Show();

                Task.Delay(1000).ContinueWith(async (t) =>
                {
                    await thisControl.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        thisControl.PART_NextSectionTextBlock.Text = thisControl.ViewModel.NextSection.Title;
                        thisControl.PART_PreviousSectionTextBlock.Text = thisControl.ViewModel.PreviousSection.Title;
                    });
                });
            }
        }

        private void PlayVideoForCurrentSection()
        {
            PART_VideoSwitchControl.IsLooping = true;
            PART_VideoSwitchControl.Source = CurrentSection.BackgroundVideo;
        }

        private void OnTransitionVideoFinished(object sender, EventArgs e)
        {
            PART_VideoSwitchControl.PlaybackFinished -= OnTransitionVideoFinished;
            PlayVideoForCurrentSection();
        }
        #endregion

        #region SectionTransitionVideo DP
        private Uri SectionTransitionVideo
        {
            get { return (Uri)GetValue(SectionTransitionVideoProperty); }
            set { SetValue(SectionTransitionVideoProperty, value); }
        }

        private static readonly DependencyProperty SectionTransitionVideoProperty =
            DependencyProperty.Register("SectionTransitionVideo", typeof(Uri), typeof(DialMenuControl), new PropertyMetadata(null));
        #endregion SectionTransitionVideo DP

        #region CurrentMedia DP
        private Uri CurrentMedia
        {
            get { return (Uri)GetValue(CurrentMediaProperty); }
            set { SetValue(CurrentMediaProperty, value); }
        }

        private static readonly DependencyProperty CurrentMediaProperty =
            DependencyProperty.Register("CurrentMedia", typeof(Uri), typeof(DialMenuControl), new PropertyMetadata(null, OnCurrentMediaChanged));

        private static void OnCurrentMediaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisObject = (DialMenuControl)d;
            thisObject.PART_DialMenuMediaControl.MediaUriSource = (Uri)e.NewValue;
        }
        #endregion

        #region IsSubMenuDisplay DP
        private bool IsSubMenuDisplayed
        {
            get { return (bool)GetValue(IsSubMenuDisplayedProperty); }
            set { SetValue(IsSubMenuDisplayedProperty, value); }
        }

        private static readonly DependencyProperty IsSubMenuDisplayedProperty =
            DependencyProperty.Register("IsSubMenuDisplayed", typeof(bool), typeof(DialMenuControl), new PropertyMetadata(false, OnIsSubMenuDisplayedChanged));

        private static void OnIsSubMenuDisplayedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = (DialMenuControl)d;
            if ((bool)e.NewValue)
            {
                thisControl.PART_VideoSwitchControl.Blur(thisControl.ViewModel.SubMenuBackgroundBlur).Start();
            }
            else
            {
                thisControl.PART_VideoSwitchControl.Blur(0).Start();
            }
        }
        #endregion

        public DialMenuControl()
        {
            Loaded += DialMenuControl_Loaded;

            _radialController = RadialController.CreateForCurrentView();
            _myRadialMenuItem = RadialControllerMenuItem.CreateFromKnownIcon("The App", RadialControllerMenuKnownIcon.InkColor);
            _radialController.Menu.Items.Add(_myRadialMenuItem);
            _radialController.RotationChanged += OnRadialController_RotationChanged;
            _radialController.ButtonClicked += OnRadialController_ButtonClicked;
            var config = RadialControllerConfiguration.GetForCurrentView();
            config.SetDefaultMenuItems(new RadialControllerSystemMenuItemKind[] { });
            config.IsMenuSuppressed = true;
            this.DefaultStyleKey = typeof(DialMenuControl);
        }

        public void Select()
        {
            if (PART_DialMenuMediaControl.IsShowingMedia)
            {
                if (ViewModel.IsDialPressToCancelMediaEnabled)
                {
                    PART_DialMenuMediaControl.HideMedia();
                }
            }
            else
            { 
                ViewModel.Select();
            }
        }

        public void Next()
        {
            ViewModel.Next();
        }

        public void Previous()
        {
            ViewModel.Previous();
        }

        private void OnRadialController_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            Select();
        }

        private void OnRadialController_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (args.RotationDeltaInDegrees > 0)
            {
                Next();
            }
            else
            {
                Previous();
            }
        }

        private async void DialMenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            var binding = new Binding
            {
                Path = new PropertyPath("CurrentSection")
            };

            this.SetBinding(CurrentSectionProperty, binding);

            var currentMediaBinding = new Binding
            {
                Path = new PropertyPath("CurrentMedia")
            };

            this.SetBinding(CurrentMediaProperty, currentMediaBinding);

            var sectionTransitionVideoBinding = new Binding
            {
                Path = new PropertyPath("SectionTransitionVideo")
            };

            this.SetBinding(SectionTransitionVideoProperty, sectionTransitionVideoBinding);

            var isSubMenuVisibleBinding = new Binding
            {
                Path = new PropertyPath("IsSubMenuVisible")
            };

            this.SetBinding(IsSubMenuDisplayedProperty, isSubMenuVisibleBinding);

            if(PART_DialMenuMediaControl != null)
            {
                PART_DialMenuMediaControl.MediaUriSource = ViewModel?.IntroMedia;
            }

            await Task.Delay(1000);
            _radialController.Menu.SelectMenuItem(_myRadialMenuItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_SectionsItemsControl = GetTemplateChild(nameof(PART_SectionsItemsControl)) as ItemsControl;
            PART_DialMenuMediaControl = GetTemplateChild(nameof(PART_DialMenuMediaControl)) as DialMenuMediaControl;
            PART_VideoSwitchControl = GetTemplateChild(nameof(PART_VideoSwitchControl)) as VideoSwitchControl;
            PART_PreviousSectionTextBlock = GetTemplateChild(nameof(PART_PreviousSectionTextBlock)) as TextBlock;
            PART_NextSectionTextBlock = GetTemplateChild(nameof(PART_NextSectionTextBlock)) as TextBlock;
        }
    }
}
