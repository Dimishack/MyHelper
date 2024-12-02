using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace MyHelper.Infrastructure.Behaviors
{
    class BlackoutDatesForStartChallengeBehavior : Behavior<DatePicker>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        protected override void OnDetaching()
        {
            CleanUp();
            base.OnDetaching();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
            => CleanUp();

        private void CleanUp()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime dateToday = DateTime.Today;
            AssociatedObject.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, dateToday.AddDays(-1)));
            int currentYear = AssociatedObject.DisplayDateStart.HasValue
                ? AssociatedObject.DisplayDateStart.Value.Year
                : dateToday.Year;
            int lastYear = AssociatedObject.DisplayDateEnd.HasValue
                ? AssociatedObject.DisplayDateEnd.Value.Year
                : dateToday.Year + 1;
            while (currentYear <= lastYear)
            {
                AssociatedObject.BlackoutDates.Add(new CalendarDateRange(new DateTime(currentYear, 12, 2), new DateTime(currentYear, 12, 31)));
                currentYear++;
            }
        }
    }
}
