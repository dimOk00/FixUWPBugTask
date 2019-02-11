using Foundation.Controls.Managers;
using Foundation.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Foundation.Controls
{
    [TemplatePart(Name = nameof(PART_Pivot), Type = typeof(Pivot))]
    public sealed class GalleryControl : Control
    {
        private readonly AutoInferManager _autoInferManager;
        private ObservableCollection<ContentItemViewModel> _source;

        public Pivot PART_Pivot { get; set; }

        public GalleryContentItemViewModel ViewModel
        {
            get { return (GalleryContentItemViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(GalleryControl), new PropertyMetadata(null, OnSelectedItemChanged));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(GalleryContentItemViewModel), typeof(GalleryControl), new PropertyMetadata(null, OnViewModelChanged));


        public GalleryControl()
        {
            _source = new ObservableCollection<ContentItemViewModel>();
            DefaultStyleKey = typeof(GalleryControl);
            _autoInferManager = new AutoInferManager(_source);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Pivot = GetTemplateChild(nameof(PART_Pivot)) as Pivot;

            PART_Pivot.ItemsSource = _source;
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as GalleryControl;
            that.OnViewModelChanged(e.NewValue as GalleryContentItemViewModel);
        }
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private async void OnViewModelChanged(GalleryContentItemViewModel galleryContentItemViewModel)
        {
            if (galleryContentItemViewModel.AutoInferFromPath)
            {
                await _autoInferManager.AutoInferFromPath(galleryContentItemViewModel.Path.LocalPath, subfolderAsCategories: true);
                foreach (var item in _source)
                {
                    item.Parent = galleryContentItemViewModel;
                    item.WindowSettings.InheritFrom(galleryContentItemViewModel.WindowSettings);
                }
            }
            else
            {
                //i intentionally didn't set _source to keep reference separate
                foreach (var vm in galleryContentItemViewModel.Children)
                {
                    _source.Add(vm);
                }
            }

        }
    }
}
