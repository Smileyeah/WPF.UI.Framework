using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF.UI.Framework.Controls
{

    public class ParkTimeSpan : UserControl
    {
        private Grid RootGrid { get; set; }
        private TextBlock TimeSpanLable { get; set; }
        private TextBlock PercentageLable { get; set; }
        private Border LengthBack { get; set; }
        private Border TotalBack { get; set; }

        public ParkTimeSpan()
        {
            RootGrid = new Grid() { HorizontalAlignment = HorizontalAlignment.Left };
            Content = RootGrid;

            TimeSpanLable = new TextBlock();
            PercentageLable = new TextBlock();
            LengthBack = new Border() { HorizontalAlignment = HorizontalAlignment.Right };
            TotalBack = new Border() { HorizontalAlignment = HorizontalAlignment.Left };

            RootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(86, GridUnitType.Star) });
            RootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(466, GridUnitType.Star) });
            RootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Star) });

            RootGrid.UpdateLayout();

            RootGrid.Children.Add(TimeSpanLable);
            RootGrid.Children.Add(PercentageLable);
            RootGrid.Children.Add(TotalBack);
            RootGrid.Children.Add(LengthBack);
            
            TimeSpanLable.SetValue(Grid.ColumnProperty, 0);
            LengthBack.SetValue(Grid.ColumnProperty, 1);
            TotalBack.SetValue(Grid.ColumnProperty, 1);
            PercentageLable.SetValue(Grid.ColumnProperty, 2);

            RootGrid.SetBinding(Grid.WidthProperty,
                new Binding { Path = new PropertyPath(WidthProperty), Source = this });
            RootGrid.SetBinding(Grid.HeightProperty,
                new Binding { Path = new PropertyPath(HeightProperty), Source = this });

            TotalBack.SetBinding(Border.BackgroundProperty,
                new Binding() { Path = new PropertyPath(SpanBackgroundProperty), Source = this });
            TotalBack.SetBinding(Border.CornerRadiusProperty,
                new Binding() { Path = new PropertyPath(SpanCornerRadiusProperty), Source = this });
            TotalBack.SetBinding(Border.HeightProperty,
                new Binding() { Path = new PropertyPath(InnerHeightProperty), Source = this });
            TotalBack.SetBinding(Border.MarginProperty,
                new Binding() { Path = new PropertyPath(InnerMarginProperty), Source = this });

            LengthBack.SetBinding(Border.BackgroundProperty,
                new Binding() { Path = new PropertyPath(SpanActiveFillProperty), Source = this });
            LengthBack.SetBinding(Border.CornerRadiusProperty,
                new Binding() { Path = new PropertyPath(SpanCornerRadiusProperty), Source = this });
            LengthBack.SetBinding(Border.HeightProperty,
                new Binding() { Path = new PropertyPath(InnerHeightProperty), Source = this });
            LengthBack.SetBinding(Border.MarginProperty,
                new Binding() { Path = new PropertyPath(InnerMarginProperty), Source = this });

            SetCurrentValue(SpanBackgroundProperty, new SolidColorBrush(Color.FromRgb(129, 136, 146)));
            SetCurrentValue(SpanActiveFillProperty, new SolidColorBrush(Color.FromRgb(244, 188, 37)));
            SetCurrentValue(SpanCornerRadiusProperty, new CornerRadius(6));

            SizeChanged += (sender, arg) =>
            {
                Update();
            };
        }

        private void Update()
        {
            var borderLen = RootGrid.ColumnDefinitions[1].Width.Value - InnerMargin.Left - InnerMargin.Right;

            TotalBack.SetCurrentValue(Border.WidthProperty, borderLen);
            LengthBack.SetCurrentValue(Border.WidthProperty, borderLen * InnerPercent);

            TimeSpanLable.SetCurrentValue(Border.WidthProperty, RootGrid.ColumnDefinitions[0].Width.Value);

            TimeSpanLable.SetCurrentValue(TextBlock.TextProperty, ParkTimeSpanText);
            PercentageLable.SetCurrentValue(TextBlock.TextProperty, InnerPercent.ToString("P0"));
        }

        public Brush SpanBackground
        {
            get { return (Brush)GetValue(SpanBackgroundProperty); }
            set { SetValue(SpanBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SpanBackgroundProperty =
            DependencyProperty.Register("SpanBackground", typeof(Brush), typeof(ParkTimeSpan), new PropertyMetadata(default(Brush)));


        public Brush SpanActiveFill
        {
            get { return (Brush)GetValue(SpanActiveFillProperty); }
            set { SetValue(SpanActiveFillProperty, value); }
        }

        public static readonly DependencyProperty SpanActiveFillProperty =
            DependencyProperty.Register("SpanActiveFill", typeof(Brush), typeof(ParkTimeSpan), new PropertyMetadata(default(Brush)));


        public CornerRadius SpanCornerRadius
        {
            get { return (CornerRadius)GetValue(SpanCornerRadiusProperty); }
            set { SetValue(SpanCornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty SpanCornerRadiusProperty =
            DependencyProperty.Register("SpanCornerRadius", typeof(CornerRadius), typeof(ParkTimeSpan), new PropertyMetadata(default(CornerRadius)));


        public double InnerPercent
        {
            get { return (double)GetValue(InnerPercentProperty); }
            set { SetValue(InnerPercentProperty, value); }
        }

        public static readonly DependencyProperty InnerPercentProperty =
            DependencyProperty.Register("InnerPercent", typeof(double), typeof(ParkTimeSpan), new PropertyMetadata(0D));


        public string ParkTimeSpanText
        {
            get { return (string)GetValue(ParkTimeSpanTextProperty); }
            set { SetValue(ParkTimeSpanTextProperty, value); }
        }

        public static readonly DependencyProperty ParkTimeSpanTextProperty =
            DependencyProperty.Register("ParkTimeSpanText", typeof(string), typeof(ParkTimeSpan), new PropertyMetadata(default(string)));

        public double InnerHeight
        {
            get { return (double)GetValue(InnerHeightProperty); }
            set { SetValue(InnerHeightProperty, value); }
        }

        public static readonly DependencyProperty InnerHeightProperty =
            DependencyProperty.Register("InnerHeight", typeof(double), typeof(ParkTimeSpan), new PropertyMetadata(0D));


        public Thickness InnerMargin
        {
            get { return (Thickness)GetValue(InnerMarginProperty); }
            set { SetValue(InnerMarginProperty, value); }
        }

        public static readonly DependencyProperty InnerMarginProperty =
            DependencyProperty.Register("InnerMargin", typeof(Thickness), typeof(ParkTimeSpan), new PropertyMetadata(default(Thickness)));
    }
}
