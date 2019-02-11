using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Foundation.Controls
{
    [TemplatePart(Name = nameof(PART_HeaderGrid), Type = typeof(Grid))]
    [TemplatePart(Name = nameof(PART_HeaderPresenter), Type = typeof(ContentPresenter))]
    [TemplatePart(Name = nameof(PART_ContentPresenter), Type = typeof(ContentPresenter))]
    [TemplatePart(Name = nameof(PART_ContentRow), Type = typeof(RowDefinition))]
    [TemplatePart(Name = nameof(PART_HeaderRow), Type = typeof(RowDefinition))]
    [TemplatePart(Name = nameof(PART_HeightTarget), Type = typeof(Rectangle))]
    public sealed class ContentExpander : Control
    {
        #region Dependency Props


        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ContentExpander), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ContentExpander), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ContentExpander), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ContentExpander), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentRowHeightProperty =
            DependencyProperty.Register("ContentRowHeight", typeof(double), typeof(ContentExpander), new PropertyMetadata(0d));

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ContentExpander), new PropertyMetadata(false, OnIsExpandedChanged));

        public static readonly DependencyProperty HasHeaderProperty =
            DependencyProperty.Register("HasHeader", typeof(bool), typeof(ContentExpander), new PropertyMetadata(true));

        public static readonly DependencyProperty ForceContentExpandProperty =
            DependencyProperty.Register("ForceContentExpand", typeof(bool), typeof(ContentExpander), new PropertyMetadata(false));


        #endregion

        #region Fields

        private Storyboard _heightAnimation;
        private DoubleAnimation _heightDoubleAnimation;
        private DoubleAnimation _opacityDoubleAnimation;

        #endregion

        #region Props

        public bool ForceContentExpand
        {
            get { return (bool)GetValue(ForceContentExpandProperty); }
            set { SetValue(ForceContentExpandProperty, value); }
        }

        public bool HasHeader
        {
            get { return (bool)GetValue(HasHeaderProperty); }
            set { SetValue(HasHeaderProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public double ContentRowHeight
        {
            get { return (double)GetValue(ContentRowHeightProperty); }
            set { SetValue(ContentRowHeightProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion

        #region Template Parts

        public Rectangle PART_HeightTarget { get; set; }
        public Grid PART_HeaderGrid { get; private set; }
        public ContentPresenter PART_HeaderPresenter { get; private set; }
        public ContentPresenter PART_ContentPresenter { get; private set; }
        public RowDefinition PART_ContentRow { get; private set; }
        public RowDefinition PART_HeaderRow { get; private set; }

        #endregion

        public ContentExpander()
        {
            DataContext = this;
            DefaultStyleKey = typeof(ContentExpander);

        }

        private void InitializeAnimations()
        {
            _heightAnimation = new Storyboard();

            var easingFunction = new CubicEase();
            var duration = new Duration(TimeSpan.FromMilliseconds(800));
            var duration2 = new Duration(TimeSpan.FromMilliseconds(800));

            _heightDoubleAnimation = new DoubleAnimation
            {
                EasingFunction = easingFunction,
                Duration = duration,
                EnableDependentAnimation = true,

            };

            _opacityDoubleAnimation = new DoubleAnimation
            {
                EasingFunction = easingFunction,
                BeginTime = TimeSpan.FromMilliseconds(800),
                Duration = duration2,
                EnableDependentAnimation = true,
            };


            Storyboard.SetTarget(_heightDoubleAnimation, PART_ContentRow);
            Storyboard.SetTargetProperty(_heightDoubleAnimation, "MaxHeight");

            Storyboard.SetTarget(_opacityDoubleAnimation, PART_ContentPresenter);
            Storyboard.SetTargetProperty(_opacityDoubleAnimation, "Opacity");

            _heightAnimation.Children.Add(_heightDoubleAnimation);
            _heightAnimation.Children.Add(_opacityDoubleAnimation);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_HeaderGrid = GetTemplateChild(nameof(PART_HeaderGrid)) as Grid;
            PART_HeaderPresenter = GetTemplateChild(nameof(PART_HeaderPresenter)) as ContentPresenter;
            PART_ContentPresenter = GetTemplateChild(nameof(PART_ContentPresenter)) as ContentPresenter;
            PART_ContentRow = GetTemplateChild(nameof(PART_ContentRow)) as RowDefinition;
            PART_HeaderRow = GetTemplateChild(nameof(PART_HeaderRow)) as RowDefinition;
            PART_HeightTarget = GetTemplateChild(nameof(PART_HeightTarget)) as Rectangle;

            PART_HeaderGrid.Tapped += PART_HeaderGrid_Tapped;
            PART_ContentPresenter.LayoutUpdated += PART_ContentPresenter_LayoutUpdated;

            InitializeAnimations();

            EnsureExpansion();
        }

        private void PART_ContentPresenter_LayoutUpdated(object sender, object e)
        {
            if (ForceContentExpand)
                EnsureExpansion();
        }

        private void EnsureExpansion()
        {
            if (IsExpanded)
            {
                PART_ContentPresenter.Measure(new Size(9000, 9000));
                PART_ContentRow.MaxHeight = PART_ContentPresenter.DesiredSize.Height;
            }
            else
            {
                PART_ContentRow.MaxHeight = 0;
            }

            if (!HasHeader)
            {
                PART_HeaderRow.MinHeight = 0;
                PART_HeaderRow.Height = new GridLength(0, GridUnitType.Pixel);
            }
        }

        private void PART_HeaderGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        private async static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as ContentExpander;

            if (that.PART_ContentPresenter == null)
                return;

            bool skipHack = false;

    GOAEGEN:
            that.PART_ContentPresenter.Measure(new Size(9000, 9000));

            double target = 0;
            double from = 0;

            double otarget = 0;
            double ofrom = 0;

            if (that.IsExpanded)
            {
                target = that.PART_ContentPresenter.DesiredSize.Height;
                from = 0;

                that.PART_ContentPresenter.Opacity = 0;

                otarget = 1;
                ofrom = 0;
            }
            else
            {
                target = 0;
                from = that.PART_ContentPresenter.DesiredSize.Height;

                otarget = 0;
                ofrom = 1;
            }

            //that._heightDoubleAnimation.From = from;
            that._heightDoubleAnimation.To = target;

            that._opacityDoubleAnimation.From = ofrom;
            that._opacityDoubleAnimation.To = otarget;

            //that._heightAnimation.Stop();
            that._heightAnimation.Begin();

            if (!skipHack)
            {
                await Task.Delay(75);
                skipHack = true;
                goto GOAEGEN;
            }

            that.EnsureExpansion();
        }
    }
}
