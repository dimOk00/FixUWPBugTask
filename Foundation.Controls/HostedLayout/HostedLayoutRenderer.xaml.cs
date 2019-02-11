using Foundation.Controls.Imaging;
using Foundation.Core.Data;
using Foundation.Core.ViewModels;
using GhostCore.Math;
using GhostCore.MVVM.Messaging;
using GhostCore.UWP.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Foundation.Controls
{
    public sealed partial class HostedLayoutRenderer : UserControl
    {
        private EventBus _menuSelectedItemBus;
        private Dictionary<HostedLayoutAction, Action<InteractableItemViewModel>> _hostedLayoutActions;
        private Dictionary<string, List<ToggleItemViewModel>> _toggleItemGroups;

        public HostedLayoutViewModel ViewModel
        {
            get { return (HostedLayoutViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public HostedLayoutPageViewModel SelectedPage
        {
            get { return (HostedLayoutPageViewModel)GetValue(SelectedPageProperty); }
            set { SetValue(SelectedPageProperty, value); }
        }

        public static readonly DependencyProperty SelectedPageProperty =
            DependencyProperty.Register("SelectedPage", typeof(HostedLayoutPageViewModel), typeof(HostedLayoutRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(HostedLayoutViewModel), typeof(HostedLayoutRenderer), new PropertyMetadata(null, OnViewModelChanged));


        public HostedLayoutRenderer()
        {
            InitializeComponent();

            _toggleItemGroups = new Dictionary<string, List<ToggleItemViewModel>>();
            _menuSelectedItemBus = EventBusManager.Instance.GetOrCreateBus(nameof(ContentItemViewModel.MenuSelectedItemBus));
            _hostedLayoutActions = new Dictionary<HostedLayoutAction, Action<InteractableItemViewModel>>
            {
                { HostedLayoutAction.NavigateToPage, OnNavigateToPage },
                { HostedLayoutAction.OpenContentToArea, OnOpenContentToArea },
                { HostedLayoutAction.OpenContentToWindow, OnOpenContentToWindow },
                { HostedLayoutAction.OpenPopup, OnOpenPopup },
                { HostedLayoutAction.CloseContent, OnCloseContent }
            };
        }


        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as HostedLayoutRenderer;
            var vm = e.NewValue as HostedLayoutViewModel;
            that.SelectedPage = vm.Pages[0];

            foreach (var item in that.SelectedPage.Items)
            {
                if (item is ToggleItemViewModel ivm)
                {
                    if (ivm.DefaultState == ToggleItemState.Selected)
                    {
                        that.HandleInteractiveItemHit(ivm);
                    }

                    InitToggleItemGroups(that, ivm);
                }
            }
        }

        private void RootGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            CallbackOnHit(e, BeginHighlightItem);
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CallbackOnHit(e, HandleInteractiveItemHit, EndHighlightItem);
        }

        private bool CallbackOnHit(PointerRoutedEventArgs e, params Action<InteractableItemViewModel>[] callbacks)
        {
            var pt = e.GetCurrentPoint(RootGrid);
            var pos = pt.Position;
            var target = new Vector2D(pos.X, pos.Y);

            var interactableItems = from x in SelectedPage.Items
                                    where x is InteractableItemViewModel
                                    select x as InteractableItemViewModel;

            bool isHit = false;

            var lightDismissPopups = from x in SelectedPage.Items
                                     where (x is ContentAreaViewModel dd) && dd.IsPopup && dd.IsLightDismiss
                                     select x as ContentAreaViewModel;

            foreach (var item in lightDismissPopups)
            {
                item.Content = null;
            }

            foreach (var item in interactableItems)
            {
                if (item.IsNonStandardHitArea)
                {
                    isHit = MathUtils.IsInPolygon(item.Shape.ToArray(), target);
                }
                else
                {
                    var rectStart = new Vector2D(item.X, item.Y);
                    var rectEnd = new Vector2D(item.X + item.Width, item.Y + item.Height);
                    isHit = MathUtils.IsPointInsideBoundingBox(rectStart, rectEnd, target);
                }

                if (isHit)
                {
                    foreach (var callback in callbacks)
                    {
                        callback(item);
                    }
                    break;
                }
            }


            return isHit;
        }

        private void HandleInteractiveItemHit(InteractableItemViewModel vm)
        {
            var callback = _hostedLayoutActions[vm.Action];
            callback(vm);

            HandleToggleItemGroups(vm);
        }

        private void BeginHighlightItem(InteractableItemViewModel obj)
        {
            if (obj.Shape == null)
                return;

            var points = new PointCollection();

            foreach (var point in obj.Shape)
            {
                points.Add(new Point(point.X, point.Y));
            }

            var poly = new Polygon
            {
                Fill = (Brush)IntToSolidColorBrushConverter.Instance.Convert(obj.HighlightColor, typeof(SolidColorBrush), null, "en"),
                Points = points,
            };

            pnlHighlightCanvas.Children.Add(poly);
        }

        private void EndHighlightItem(InteractableItemViewModel obj)
        {
            pnlHighlightCanvas.Children.Clear();
        }

        private void OnCloseContent(InteractableItemViewModel obj)
        {
            obj.ContentAreaTarget.Content = null;

            if (obj is ToggleItemViewModel t)
            {
                if (t.DefaultState == ToggleItemState.Normal)
                {
                    t.Action = HostedLayoutAction.OpenContentToArea;
                }
            }
        }
        private void OnNavigateToPage(InteractableItemViewModel obj)
        {
            if (!ViewModel.Pages.Contains(obj.PageTarget))
                Debug.Log("WARNING: page target not found in view model pages collection");

            SelectedPage = obj.PageTarget;
        }
        private void OnOpenContentToWindow(InteractableItemViewModel obj)
        {
            _menuSelectedItemBus.Publish(obj.ItemTarget, this);
        }
        private void OnOpenPopup(InteractableItemViewModel obj)
        {
            obj.ItemTarget.Parent = obj.ContentAreaTarget;
            obj.ContentAreaTarget.Content = null;
            obj.ContentAreaTarget.Content = obj.ItemTarget;
        }
        private void OnOpenContentToArea(InteractableItemViewModel obj)
        {
            obj.ItemTarget.Parent = obj.ContentAreaTarget;
            obj.ContentAreaTarget.Content = obj.ItemTarget;

            if (obj is ToggleItemViewModel t)
            {
                if (t.DefaultState == ToggleItemState.Selected)
                {
                    t.Action = HostedLayoutAction.CloseContent;
                }
            }
        }


        private static void InitToggleItemGroups(HostedLayoutRenderer that, ToggleItemViewModel ivm)
        {
            if (ivm.ToggleGroup != null)
            {
                if (that._toggleItemGroups.ContainsKey(ivm.ToggleGroup))
                {
                    that._toggleItemGroups[ivm.ToggleGroup].Add(ivm);
                }
                else
                {
                    that._toggleItemGroups.Add(ivm.ToggleGroup, new List<ToggleItemViewModel>() { ivm });
                }
            }
        }

        private void HandleToggleItemGroups(InteractableItemViewModel item)
        {
            if (item is ToggleItemViewModel ivm)
            {
                if (ivm.ToggleGroup != null)
                {
                    var group = _toggleItemGroups[ivm.ToggleGroup];
                    foreach (var x in group)
                    {
                        if (x == ivm)
                            continue;

                        x.DefaultState = ToggleItemState.Normal;
                    }

                }
            }
        }

    }
}
