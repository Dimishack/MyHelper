using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MyHelper.Infrastructure.Behaviors
{
    class Show_Add_Edit_UserControlBehavior : Behavior<Grid>
    {
        private DoubleAnimation? _showAnimation = null;
        private DoubleAnimation? _hideAnimation = null;
        private TextBox? _textBox = null;
        private readonly TimeSpan _duration = TimeSpan.FromMilliseconds(700);
        private readonly double _accelerationRatio = 0.7;
        private readonly IEasingFunction _easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

        protected override void OnAttached()
        {
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
            base.OnAttached();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e) => CleanUp();

        protected override void OnDetaching()
        {
            CleanUp();
            base.OnDetaching();
        }

        private void CleanUp()
        {
            if (AssociatedObject is not null)
            {
                if (_showAnimation is not null)
                {
                    _showAnimation.Completed -= Animation_Completed;
                    AssociatedObject.BeginAnimation(Grid.HeightProperty, null);
                    _showAnimation = null;
                }
                if (_hideAnimation is not null)
                {
                    AssociatedObject.BeginAnimation(Grid.HeightProperty, null);
                    _hideAnimation = null;
                }
                _textBox = null;
                AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            }
        }

        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register("MaxHeight", typeof(double), typeof(Show_Add_Edit_UserControlBehavior), new PropertyMetadata(0.0));



        public bool ManagementShow
        {
            get { return (bool)GetValue(ManagementShowProperty); }
            set { SetValue(ManagementShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ManagementShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ManagementShowProperty =
            DependencyProperty.Register("ManagementShow", typeof(bool), typeof(Show_Add_Edit_UserControlBehavior), new PropertyMetadata(false, OnManagementShowChanged));

        private static void OnManagementShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is Show_Add_Edit_UserControlBehavior behavior)
            {
                if (behavior.ManagementShow) behavior.ShowUserControl();
                else behavior.HideUserControl();
            }
        }

        private void HideUserControl()
        {
            if (AssociatedObject is null) return;

            if (_hideAnimation is null)
            {
                _hideAnimation = new()
                {
                    To = 0.0,
                    Duration = _duration,
                    AccelerationRatio = _accelerationRatio,
                    EasingFunction =_easingFunction,
                };
                if (_hideAnimation.CanFreeze) _hideAnimation.Freeze();
            }
            AssociatedObject.BeginAnimation(Grid.HeightProperty, _hideAnimation);
        }


        private void ShowUserControl()
        {
            if (AssociatedObject is null) return;

            if (_showAnimation is null)
            {
                _showAnimation = new()
                {
                    To = MaxHeight,
                    Duration = _duration,
                    AccelerationRatio = _accelerationRatio,
                    EasingFunction = _easingFunction,
                };
                if(AssociatedObject.FindName("tb_Name") is TextBox textBox)
                {
                    _textBox = textBox;
                    _showAnimation.Completed += Animation_Completed;
                }
                if(_showAnimation.CanFreeze) _showAnimation.Freeze();
            }
            AssociatedObject.BeginAnimation(Grid.HeightProperty, _showAnimation);
        }

        private void Animation_Completed(object? sender, EventArgs e)
        {
            if(_textBox is not null)
            {
                _textBox.Focus();
                _textBox.SelectAll();
            }
        }
    }
}
