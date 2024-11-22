using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MyHelper.Infrastructure.Behaviors
{
    class ProgressBehavior : Behavior<ProgressBar>
    {
        private Storyboard? _storyboard;

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
                if (_storyboard != null)
                {
                    _storyboard.Stop();
                    _storyboard.Children.Clear();
                    _storyboard = null;
                }
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
            if(_storyboard is null)
            {
                _storyboard = new Storyboard();
                Storyboard.SetTargetProperty(_storyboard, new PropertyPath(ProgressBar.ValueProperty));
                Storyboard.SetTarget(_storyboard, AssociatedObject);
            }
            var animation = new DoubleAnimation
            {
                To = Progress,
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 0.8
            };
            _storyboard.Children.Clear();
            _storyboard.Children.Add(animation);
            _storyboard.Begin();
        }
    }
}
