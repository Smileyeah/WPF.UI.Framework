using LiveCharts.Wpf.Points;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPF.UI.Framework.Control
{
    public class AngularGaugeWithPointer : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AngularGaugeWithPointer"/> class.
        /// </summary>
        public AngularGaugeWithPointer()
        {
            Canvas = new Canvas();
            Content = Canvas;

            PieBack = new PieSlice();
            Pie = new PieSlice();
            MeasureTextBlock = new TextBlock();
            LeftLabel = new TextBlock();
            RightLabel = new TextBlock();

            StickRotateTransform = new RotateTransform(180);
            Stick = new Path
            {
                Data = Geometry.Parse("m0,90 a5,5 0 0 0 20,0 l-8,-88 a2,2 0 0 0 -4 0 z"),
                Fill = Brushes.CornflowerBlue,
                Stretch = Stretch.Fill,
                RenderTransformOrigin = new Point(0.5, 0.9),
                RenderTransform = StickRotateTransform
            };
            Canvas.Children.Add(Stick);
            Panel.SetZIndex(Stick, 1);

            Canvas.Children.Add(PieBack);
            Canvas.Children.Add(Pie);
            Canvas.Children.Add(MeasureTextBlock);
            Canvas.Children.Add(RightLabel);
            Canvas.Children.Add(LeftLabel);

            Panel.SetZIndex(MeasureTextBlock, 1);
            Panel.SetZIndex(RightLabel, 1);
            Panel.SetZIndex(LeftLabel, 1);

            Panel.SetZIndex(PieBack, 0);
            Panel.SetZIndex(Pie, 1);

            Canvas.SetBinding(WidthProperty,
                new Binding { Path = new PropertyPath(WidthProperty), Source = this });
            Canvas.SetBinding(HeightProperty,
                new Binding { Path = new PropertyPath(HeightProperty), Source = this });

            PieBack.SetBinding(Shape.FillProperty,
                new Binding { Path = new PropertyPath(GaugeBackgroundProperty), Source = this });
            PieBack.SetBinding(Shape.StrokeThicknessProperty,
                new Binding { Path = new PropertyPath(StrokeThicknessProperty), Source = this });
            PieBack.SetBinding(Shape.StrokeProperty,
                new Binding { Path = new PropertyPath(StrokeProperty), Source = this });
            PieBack.SetBinding(RenderTransformProperty,
                new Binding { Path = new PropertyPath(GaugeRenderTransformProperty), Source = this });

            Pie.SetBinding(Shape.StrokeThicknessProperty,
                new Binding { Path = new PropertyPath(StrokeThicknessProperty), Source = this });
            Pie.SetBinding(RenderTransformProperty,
                new Binding { Path = new PropertyPath(GaugeRenderTransformProperty), Source = this });
            Pie.Stroke = Brushes.Transparent;

            Stick.SetCurrentValue(Shape.FillProperty, new SolidColorBrush(Color.FromRgb(69, 90, 100)));

            SetCurrentValue(GaugeBackgroundProperty, new SolidColorBrush(Color.FromRgb(21, 101, 191)) { Opacity = .1 });
            SetCurrentValue(StrokeThicknessProperty, 0d);
            SetCurrentValue(StrokeProperty, new SolidColorBrush(Color.FromRgb(222, 222, 222)));

            SetCurrentValue(FromColorProperty, Color.FromRgb(100, 180, 245));
            SetCurrentValue(ToColorProperty, Color.FromRgb(21, 101, 191));

            SetCurrentValue(MinHeightProperty, 20d);
            SetCurrentValue(MinWidthProperty, 20d);

            SetCurrentValue(AnimationsSpeedProperty, TimeSpan.FromMilliseconds(800));

            MeasureTextBlock.FontWeight = FontWeights.Bold;

            IsNew = true;

            SizeChanged += (sender, args) =>
            {
                IsChartInitialized = true;
                Update();
            };
        }

        #region Properties

        private Canvas Canvas { get; set; }
        private PieSlice PieBack { get; set; }
        private PieSlice Pie { get; set; }
        private Path Stick { get; set; }
        private RotateTransform StickRotateTransform { get; set; }
        private TextBlock MeasureTextBlock { get; set; }
        private TextBlock LeftLabel { get; set; }
        private TextBlock RightLabel { get; set; }
        private bool IsNew { get; set; }
        private bool IsChartInitialized { get; set; }

        /// <summary>
        /// The gauge active fill property
        /// </summary>
        public static readonly DependencyProperty GaugeActiveFillProperty = DependencyProperty.Register(
            "GaugeActiveFill", typeof(Brush), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets the gauge active fill, if this property is set, From/to color properties interpolation will be ignored
        /// </summary>
        /// <value>
        /// The gauge active fill.
        /// </value>
        public Brush GaugeActiveFill
        {
            get { return (Brush)GetValue(GaugeActiveFillProperty); }
            set { SetValue(GaugeActiveFillProperty, value); }
        }

        /// <summary>
        /// The labels visibility property
        /// </summary>
        public static readonly DependencyProperty LabelsVisibilityProperty = DependencyProperty.Register(
            "LabelsVisibility", typeof(Visibility), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Visibility)));
        /// <summary>
        /// Gets or sets the labels visibility.
        /// </summary>
        /// <value>
        /// The labels visibility.
        /// </value>
        public Visibility LabelsVisibility
        {
            get { return (Visibility)GetValue(LabelsVisibilityProperty); }
            set { SetValue(LabelsVisibilityProperty, value); }
        }

        /// <summary>
        /// The gauge render transform property
        /// </summary>
        public static readonly DependencyProperty GaugeRenderTransformProperty = DependencyProperty.Register(
            "GaugeRenderTransform", typeof(Transform), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Transform)));
        /// <summary>
        /// Gets or sets the gauge render transform.
        /// </summary>
        /// <value>
        /// The gauge render transform.
        /// </value>
        public Transform GaugeRenderTransform
        {
            get { return (Transform)GetValue(GaugeRenderTransformProperty); }
            set { SetValue(GaugeRenderTransformProperty, value); }
        }

        /// <summary>
        /// The wedge property
        /// </summary>
        public static readonly DependencyProperty WedgeProperty = DependencyProperty.Register(
            "Wedge", typeof(double), typeof(AngularGaugeWithPointer),
            new PropertyMetadata(300d, UpdateCallback));
        /// <summary>
        /// Gets or sets the opening angle in the gauge
        /// </summary>
        public double Wedge
        {
            get { return (double)GetValue(WedgeProperty); }
            set { SetValue(WedgeProperty, value); }
        }

        /// <summary>
        /// From property
        /// </summary>
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
            "From", typeof(double), typeof(AngularGaugeWithPointer), new PropertyMetadata(0d, UpdateCallback));
        /// <summary>
        /// Gets or sets the value where the gauge starts
        /// </summary>
        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        /// <summary>
        /// To property
        /// </summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            "To", typeof(double), typeof(AngularGaugeWithPointer), new PropertyMetadata(1d, UpdateCallback));
        /// <summary>
        /// Gets or sets the value where the gauge ends
        /// </summary>
        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        /// <summary>
        /// The value property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(double), ValueChangedCallback));
        /// <summary>
        /// Gets or sets the current value of the gauge
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// The inner radius property
        /// </summary>
        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            "InnerRadius", typeof(double?), typeof(AngularGaugeWithPointer), new PropertyMetadata(null, UpdateCallback));
        /// <summary>
        /// Gets o sets inner radius
        /// </summary>
        public double? InnerRadius
        {
            get { return (double?)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        /// <summary>
        /// The stroke property
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(Brush), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets stroke, the stroke is the brush used to draw the gauge border.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }


        /// <summary>
        /// The stroke thickness property
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(double)));
        /// <summary>
        /// Gets or sets stroke brush thickness
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// To color property
        /// </summary>
        public static readonly DependencyProperty ToColorProperty = DependencyProperty.Register(
            "ToColor", typeof(Color), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Color), UpdateCallback));
        /// <summary>
        /// Gets or sets the color when the current value equals to min value, any value between min and max will use an interpolated color.
        /// </summary>
        public Color ToColor
        {
            get { return (Color)GetValue(ToColorProperty); }
            set { SetValue(ToColorProperty, value); }
        }

        /// <summary>
        /// From color property
        /// </summary>
        public static readonly DependencyProperty FromColorProperty = DependencyProperty.Register(
            "FromColor", typeof(Color), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Color), UpdateCallback));
        /// <summary>
        /// Gets or sets the color when the current value equals to max value, any value between min and max will use an interpolated color.
        /// </summary>
        public Color FromColor
        {
            get { return (Color)GetValue(FromColorProperty); }
            set { SetValue(FromColorProperty, value); }
        }

        /// <summary>
        /// The gauge background property
        /// </summary>
        public static readonly DependencyProperty GaugeBackgroundProperty = DependencyProperty.Register(
            "GaugeBackground", typeof(Brush), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets the gauge background
        /// </summary>
        public Brush GaugeBackground
        {
            get { return (Brush)GetValue(GaugeBackgroundProperty); }
            set { SetValue(GaugeBackgroundProperty, value); }
        }

        /// <summary>
        /// The animations speed property
        /// </summary>
        public static readonly DependencyProperty AnimationsSpeedProperty = DependencyProperty.Register(
            "AnimationsSpeed", typeof(TimeSpan), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(TimeSpan)));
        /// <summary>
        /// G3ts or sets the gauge animations speed
        /// </summary>
        public TimeSpan AnimationsSpeed
        {
            get { return (TimeSpan)GetValue(AnimationsSpeedProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        /// <summary>
        /// The disablea animations property
        /// </summary>
        public static readonly DependencyProperty DisableAnimationsProperty = DependencyProperty.Register(
            "DisableAnimations", typeof(bool), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(bool)));
        /// <summary>
        /// Gets or sets whether the chart is animated
        /// </summary>
        public bool DisableAnimations
        {
            get { return (bool)GetValue(DisableAnimationsProperty); }
            set { SetValue(DisableAnimationsProperty, value); }
        }

        /// <summary>
        /// The label formatter property
        /// </summary>
        public static readonly DependencyProperty LabelFormatterProperty = DependencyProperty.Register(
            "LabelFormatter", typeof(Func<double, string>), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Func<double, string>)));
        /// <summary>
        /// Gets or sets the label formatter, a label formatter takes a double value, and return a string, e.g. val => val.ToString("C");
        /// </summary>
        public Func<double, string> LabelFormatter
        {
            get { return (Func<double, string>)GetValue(LabelFormatterProperty); }
            set { SetValue(LabelFormatterProperty, value); }
        }

        /// <summary>
        /// The high font size property
        /// </summary>
        public static readonly DependencyProperty HighFontSizeProperty = DependencyProperty.Register(
            "HighFontSize", typeof(double?), typeof(AngularGaugeWithPointer), new PropertyMetadata(null));
        /// <summary>
        /// Gets o sets the label size, if this value is null then it will be automatically calculated, default is null.
        /// </summary>
        public double? HighFontSize
        {
            get { return (double?)GetValue(HighFontSizeProperty); }
            set { SetValue(HighFontSizeProperty, value); }
        }

        /// <summary>
        /// The ticks step property
        /// </summary>
        public static readonly DependencyProperty TicksStepProperty = DependencyProperty.Register(
            "TicksStep", typeof(double), typeof(AngularGaugeWithPointer),
            new PropertyMetadata(double.NaN, UpdateCallback));
        /// <summary>
        /// Gets or sets the separation between every tick
        /// </summary>
        public double TicksStep
        {
            get { return (double)GetValue(TicksStepProperty); }
            set { SetValue(TicksStepProperty, value); }
        }
        
        /// <summary>
        /// The ticks stroke thickness property
        /// </summary>
        public static readonly DependencyProperty TicksStrokeThicknessProperty = DependencyProperty.Register(
            "TicksStrokeThickness", typeof(double), typeof(AngularGaugeWithPointer), new PropertyMetadata(2d));

        /// <summary>
        /// Gets or sets the ticks stroke thickness.
        /// </summary>
        /// <value>
        /// The ticks stroke thickness.
        /// </value>
        public double TicksStrokeThickness
        {
            get { return (double)GetValue(TicksStrokeThicknessProperty); }
            set { SetValue(TicksStrokeThicknessProperty, value); }
        }
        
        /// <summary>
        /// The ticks foreground property
        /// </summary>
        public static readonly DependencyProperty TicksForegroundProperty = DependencyProperty.Register(
            "TicksForeground", typeof(Brush), typeof(AngularGaugeWithPointer), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets the ticks foreground
        /// </summary>
        public Brush TicksForeground
        {
            get { return (Brush)GetValue(TicksForegroundProperty); }
            set { SetValue(TicksForegroundProperty, value); }
        }

        /// <summary>
        /// The labels step property
        /// </summary>
        public static readonly DependencyProperty LabelsStepProperty = DependencyProperty.Register(
            "LabelsStep", typeof(double), typeof(AngularGaugeWithPointer),
            new PropertyMetadata(double.NaN, UpdateCallback));
        /// <summary>
        /// Gets or sets the separation between every label
        /// </summary>
        public double LabelsStep
        {
            get { return (double)GetValue(LabelsStepProperty); }
            set { SetValue(LabelsStepProperty, value); }
        }

        #endregion

        private static void ValueChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var ag = (AngularGaugeWithPointer)o;

            ag.Update();
        }

        private static void UpdateCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var gauge = (AngularGaugeWithPointer)dependencyObject;

            gauge.Update();
        }

        private void Update()
        {
            if (!IsChartInitialized) return;

            Func<double, string> defFormatter = x => x.ToString(CultureInfo.InvariantCulture);

            var completed = (Value - From) / (To - From);

            var t = 0d;

            if (double.IsNaN(completed) || double.IsInfinity(completed))
            {
                completed = 0;
            }

            completed = completed > 1 ? 1 : (completed < 0 ? 0 : completed);

            var angle = Wedge > 360 ? 360 : (Wedge < 0 ? 0 : Wedge);

            var fromAlpha = (360 - Wedge) * .5;
            var toAlpha = 360 - fromAlpha;

            LeftLabel.Visibility = Visibility.Hidden;
            RightLabel.Visibility = Visibility.Hidden;

            double r, top;
            
            r = ActualWidth > ActualHeight ? ActualHeight : ActualWidth;
            r = r / 2 - 2 * t;
            top = ActualHeight / 2;

            if (r < 0) r = 1;

            PieBack.Radius = r;
            PieBack.InnerRadius = InnerRadius ?? r * .6;
            PieBack.RotationAngle = 270;
            PieBack.WedgeAngle = angle;

            Pie.Radius = PieBack.Radius;
            Pie.InnerRadius = PieBack.InnerRadius;
            Pie.RotationAngle = PieBack.RotationAngle;

            Canvas.SetLeft(PieBack, ActualWidth / 2);
            Canvas.SetTop(PieBack, top);
            Canvas.SetLeft(Pie, ActualWidth / 2);
            Canvas.SetTop(Pie, top);

            Canvas.SetTop(LeftLabel, top);
            Canvas.SetTop(RightLabel, top);
            Canvas.SetRight(LeftLabel, ActualWidth / 2 + (r + PieBack.InnerRadius) / 2 - LeftLabel.ActualWidth / 2);
            Canvas.SetRight(RightLabel, ActualWidth / 2 - (r + PieBack.InnerRadius) / 2 - RightLabel.ActualWidth / 2);

            MeasureTextBlock.FontSize = HighFontSize ?? Pie.InnerRadius * .4;
            MeasureTextBlock.Text = (Value / 100).ToString("P");
            MeasureTextBlock.SetCurrentValue(ForegroundProperty,GaugeActiveFill);
            MeasureTextBlock.UpdateLayout();
            Canvas.SetTop(MeasureTextBlock, top * 2 - MeasureTextBlock.ActualHeight * 1.3);
            Canvas.SetLeft(MeasureTextBlock, ActualWidth / 2 - MeasureTextBlock.ActualWidth / 2);

            var interpolatedColor = new Color
            {
                R = LinearInterpolation(FromColor.R, ToColor.R),
                G = LinearInterpolation(FromColor.G, ToColor.G),
                B = LinearInterpolation(FromColor.B, ToColor.B),
                A = LinearInterpolation(FromColor.A, ToColor.A)
            };

            if (IsNew)
            {
                Pie.Fill = new SolidColorBrush(FromColor);
                Pie.WedgeAngle = 0;
            }

            if (DisableAnimations)
            {
                Pie.WedgeAngle = completed * angle;
            }
            else
            {
                Pie.BeginAnimation(PieSlice.WedgeAngleProperty, new DoubleAnimation(completed * angle, AnimationsSpeed));
            }

            var d = ActualWidth < ActualHeight ? ActualWidth : ActualHeight;

            Stick.Height = d * .5 * .8;
            Stick.Width = Stick.Height * .2;

            Canvas.SetLeft(Stick, ActualWidth * .5 - Stick.Width * .5);
            Canvas.SetTop(Stick, ActualHeight * .5 - Stick.Height * .9);

            var ticksHi = d * .35;
            var ticksHj = d * .3;
            var labelsHj = d * .3;

            #region 画Tick刻度

            var ts = double.IsNaN(TicksStep) ? DecideInterval((To - From) / 5) : TicksStep;
            if (ts / (From - To) > 300)
                throw new InvalidOperationException("TicksStep property is too small compared with the range in " +
                                              "the gauge, to avoid performance issues, please increase it.");

            for (var i = From; i <= To; i += ts)
            {
                var alpha = LinearInterpolation(fromAlpha, toAlpha, From, To, i) + 90;

                var tick = new Line
                {
                    X1 = ActualWidth * .5 + ticksHi * Math.Cos(alpha * Math.PI / 180),
                    X2 = ActualWidth * .5 + ticksHj * Math.Cos(alpha * Math.PI / 180),
                    Y1 = ActualHeight * .5 + ticksHi * Math.Sin(alpha * Math.PI / 180),
                    Y2 = ActualHeight * .5 + ticksHj * Math.Sin(alpha * Math.PI / 180)
                };
                Canvas.Children.Add(tick);
                tick.SetBinding(Shape.StrokeProperty,
                    new Binding { Path = new PropertyPath(TicksForegroundProperty), Source = this });
                tick.SetBinding(Shape.StrokeThicknessProperty,
                    new Binding { Path = new PropertyPath(TicksStrokeThicknessProperty), Source = this });
            }

            #endregion

            #region 画Lable刻度
            var ls = double.IsNaN(LabelsStep) ? DecideInterval((To - From) / 5) : LabelsStep;
            if (ls / (From - To) > 300)
                throw new InvalidOperationException("LabelsStep property is too small compared with the range in " +
                                              "the gauge, to avoid performance issues, please increase it.");

            for (var i = From; i <= To; i += ls)
            {
                var alpha = LinearInterpolation(fromAlpha, toAlpha, From, To, i) + 90;

                var tick = new Line
                {
                    X1 = ActualWidth * .5 + ticksHi * Math.Cos(alpha * Math.PI / 180),
                    X2 = ActualWidth * .5 + labelsHj * Math.Cos(alpha * Math.PI / 180),
                    Y1 = ActualHeight * .5 + ticksHi * Math.Sin(alpha * Math.PI / 180),
                    Y2 = ActualHeight * .5 + labelsHj * Math.Sin(alpha * Math.PI / 180)
                };

                Canvas.Children.Add(tick);
                var label = new TextBlock
                {
                    Text = (LabelFormatter ?? defFormatter)(i)
                };

                Canvas.Children.Add(label);
                label.UpdateLayout();
                Canvas.SetLeft(label, alpha < 270
                    ? tick.X2
                    : (Math.Abs(alpha - 270) < 4
                        ? tick.X2 - label.ActualWidth * .5
                        : tick.X2 - label.ActualWidth));
                Canvas.SetTop(label, tick.Y2);
                tick.SetBinding(Shape.StrokeProperty,
                    new Binding { Path = new PropertyPath(TicksForegroundProperty), Source = this });
                tick.SetBinding(Shape.StrokeThicknessProperty,
                    new Binding { Path = new PropertyPath(TicksStrokeThicknessProperty), Source = this });
            }
            #endregion

            MoveStick();


            if (GaugeActiveFill == null)
            {
                ((SolidColorBrush)Pie.Fill).BeginAnimation(SolidColorBrush.ColorProperty,
                    new ColorAnimation(interpolatedColor, AnimationsSpeed));
            }
            else
            {
                Pie.Fill = GaugeActiveFill;
            }
            IsNew = false;
        }
        
        private static double DecideInterval(double minimum)
        {
            var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

            var residual = minimum / magnitude;
            double tick;
            if (residual > 5)
                tick = 10 * magnitude;
            else if (residual > 2)
                tick = 5 * magnitude;
            else if (residual > 1)
                tick = 2 * magnitude;
            else
                tick = magnitude;

            return tick;
        }

        private byte LinearInterpolation(double from, double to)
        {
            var p1 = new Point(From, from);
            var p2 = new Point(To, to);

            var deltaX = p2.X - p1.X;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var m = (p2.Y - p1.Y) / (deltaX == 0 ? double.MinValue : deltaX);

            var v = Value > To ? To : (Value < From ? From : Value);

            return (byte)(m * (v - p1.X) + p1.Y);
        }
        
        private static double LinearInterpolation(double fromA, double toA, double fromB, double toB, double value)
        {
            var p1 = new Point(fromB, fromA);
            var p2 = new Point(toB, toA);

            var deltaX = p2.X - p1.X;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var m = (p2.Y - p1.Y) / (deltaX == 0 ? double.MinValue : deltaX);

            return m * (value - p1.X) + p1.Y;
        }

        private void MoveStick()
        {
            Wedge = Wedge > 360 ? 360 : (Wedge < 0 ? 0 : Wedge);

            var fromAlpha = (360 - Wedge) * .5;
            var toAlpha = 360 - fromAlpha;

            var alpha = LinearInterpolation(fromAlpha, toAlpha, From, To, Value) + 180;

            if (DisableAnimations)
            {
                StickRotateTransform.Angle = alpha;
            }
            else
            {
                StickRotateTransform.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(alpha, AnimationsSpeed));
            }
        }
    }
}
