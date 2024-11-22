using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyHelper.Infrastructure.Behaviors
{
    class OffsetOfCompleteBehavior : Behavior<GradientStop>
    {
        private DoubleAnimation? _animation = null;
        private bool _isCleanUp = false;

        protected override void OnDetaching()
        {
            CleanUp();
            base.OnDetaching();
        }

        public double OffsetOfComplete
        {
            get { return (double)GetValue(OffsetOfCompleteProperty); }
            set { SetValue(OffsetOfCompleteProperty, value); }
        }

        public bool IsCleanUp
        {
            get { return (bool)GetValue(IsCleanUpProperty); }
            set { SetValue(IsCleanUpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CleanUp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCleanUpProperty =
            DependencyProperty.Register("IsCleanUp", typeof(bool), typeof(OffsetOfCompleteBehavior), new PropertyMetadata(false, OnIsCleanUpChanged));

        private static void OnIsCleanUpChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is OffsetOfCompleteBehavior behavior && !behavior._isCleanUp)
            {
                behavior._isCleanUp = true;
                behavior.CleanUp();
            }
        }

        private void CleanUp()
        {
            if (AssociatedObject is null || _isCleanUp) return;

            if (_animation is not null)
            {
                AssociatedObject.BeginAnimation(GradientStop.OffsetProperty, null);
                _animation = null;
            }
        }

        public static readonly DependencyProperty OffsetOfCompleteProperty =
            DependencyProperty.Register("OffsetOfComplete", typeof(double), typeof(OffsetOfCompleteBehavior), new PropertyMetadata(2.0, OnOffsetChanged));

        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OffsetOfCompleteBehavior behavior) behavior.OffsetAnimation();
        }
        private void OffsetAnimation()
        {
            if(AssociatedObject is null || _isCleanUp) return;

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
