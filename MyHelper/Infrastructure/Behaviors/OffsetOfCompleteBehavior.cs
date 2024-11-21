using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyHelper.Infrastructure.Behaviors
{
    class OffsetOfCompleteBehavior : Behavior<GradientStop>
    {
        private DoubleAnimation? _animation = null;

        protected override void OnDetaching()
        {
            _animation = null;
            base.OnDetaching();
        }
        public double OffsetOfComplete
        {
            get { return (double)GetValue(OffsetOfCompleteProperty); }
            set { SetValue(OffsetOfCompleteProperty, value); }
        }

        public static readonly DependencyProperty OffsetOfCompleteProperty =
            DependencyProperty.Register("OffsetOfComplete", typeof(double), typeof(OffsetOfCompleteBehavior), new PropertyMetadata(2.0, OnOffsetChanged));

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OffsetOfCompleteBehavior behavior) behavior.OffsetAnimation();
        }
        private void OffsetAnimation()
        {
            if (_animation is null)
            {
                _animation = new()
                {
                    To = OffsetOfComplete,
                    Duration = TimeSpan.FromMilliseconds(300),
                    DecelerationRatio = 0.8
                }; 
            }
            else _animation.To = OffsetOfComplete;
            AssociatedObject.BeginAnimation(GradientStop.OffsetProperty, _animation);
        }
    }
}
