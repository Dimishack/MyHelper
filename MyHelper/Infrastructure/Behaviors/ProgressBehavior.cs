using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MyHelper.Infrastructure.Behaviors
{
    class ProgressBehavior : Behavior<ProgressBar>
    {
        private DoubleAnimation? _animation;

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
                _animation = null;
                AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            }
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProgressBehavior), new PropertyMetadata(0.0, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ProgressBehavior behavior) behavior.ProgressAnimation();
        }
        private void ProgressAnimation()
        {
            if (AssociatedObject is null)
                return;
            if (_animation is null)
            {
                _animation = new DoubleAnimation
                {
                    To = Progress,
                    Duration = TimeSpan.FromMilliseconds(300),
                    DecelerationRatio = 0.8
                };
            }
            else _animation.To = Progress;
            AssociatedObject.BeginAnimation(ProgressBar.ValueProperty, _animation);
        }
    }
}
