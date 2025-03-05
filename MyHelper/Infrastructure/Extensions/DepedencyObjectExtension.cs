using System.Windows;
using System.Windows.Media;

namespace MyHelper.Infrastructure.Extensions
{
    static class DepedencyObjectExtension
    {
        public static DependencyObject FindVisualRoot(this DependencyObject element)
        {
            var root = VisualTreeHelper.GetParent(element);
            if (root is null) return element;
            return root.FindVisualRoot();
        }

        public static T? FindVisualParent<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            if (obj is null) return default;
            var target = obj;
            do
            {
                target = VisualTreeHelper.GetParent(target);
            } while (target != null && target is not T);
            return target as T;
        }
    }
}
