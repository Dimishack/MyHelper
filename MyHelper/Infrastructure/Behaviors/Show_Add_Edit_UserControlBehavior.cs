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

        protected override void OnDetaching()
        {
            if(_showAnimation is not null) _showAnimation.Completed -= Animation_Completed;
            _textBox = null;
            _showAnimation = null;
            _hideAnimation = null;
            base.OnDetaching();
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
            if (_hideAnimation is null)
            {
                _hideAnimation = new()
                {
                    To = 0.0,
                    Duration = TimeSpan.FromMilliseconds(700),
                    AccelerationRatio = 0.5,
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                };
                if (_hideAnimation.CanFreeze) _hideAnimation.Freeze();
            }
            AssociatedObject.BeginAnimation(Grid.HeightProperty, _hideAnimation);
        }


        private void ShowUserControl()
        {
            if(_showAnimation is null)
            {
                _showAnimation = new()
                {
                    To = MaxHeight,
                    Duration = TimeSpan.FromMilliseconds(700),
                    AccelerationRatio = 0.5,
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
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
