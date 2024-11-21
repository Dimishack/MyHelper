using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyHelper.Infrastructure.Behaviors
{
    internal class ShowMainUserControlBehavior : Behavior<UserControl>
    {
        private DoubleAnimation? _translateTransform_Y_Animation;
        private DoubleAnimation? _opacityAnimation;
        private readonly TimeSpan _duration = TimeSpan.FromMilliseconds(300);
        protected override void OnAttached()
        {
            AssociatedObject.RenderTransform = new TranslateTransform(0,0);
            AssociatedObject.Loaded += UserControl_Loaded;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= UserControl_Loaded;
            _translateTransform_Y_Animation = null;
            _opacityAnimation = null;
            base.OnDetaching();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(_translateTransform_Y_Animation is null)
            {
                _translateTransform_Y_Animation = new()
                {
                    From = -10,
                    To = 0,
                    AccelerationRatio = 0.9,
                    Duration = _duration,
                };
                if (_translateTransform_Y_Animation.CanFreeze) _translateTransform_Y_Animation.Freeze();
            }
            if (_opacityAnimation is null)
            {
                _opacityAnimation = new()
                {
                    From = 0,
                    To = 1,
                    Duration = _duration,
                };
                if(_opacityAnimation.CanFreeze) _opacityAnimation.Freeze();
            }
            AssociatedObject.RenderTransform.BeginAnimation(TranslateTransform.YProperty, _translateTransform_Y_Animation);
            AssociatedObject.BeginAnimation(UserControl.OpacityProperty, _opacityAnimation);
        }

    }
}
