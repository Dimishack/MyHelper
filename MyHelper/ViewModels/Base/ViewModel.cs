using MyHelper.Infrastructure.Attributes;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MyHelper.ViewModels.Base
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string? PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName); 
            return true;
        }



        protected virtual void ChangedWithProperties(ViewModel viewModel, [CallerMemberName] string? propertyName = null)
        {
            var property = viewModel.GetType().GetProperty(propertyName!);
            if (property is not null)
            {
                var attributes = property.GetCustomAttributes<PropertyChangedWithAttribute>();
                if (attributes is not null)
                    foreach (var attribute in attributes)
                        OnPropertyChanged(attribute.PropertyName);
            }
        }

        protected virtual void DepedenciesChanged(ViewModel viewModel,[CallerMemberName] string? propertyName = null)
        {
            foreach (PropertyInfo property in viewModel.GetType().GetProperties())
            {
                var depedencyAttribute = property.GetCustomAttribute<DependencyOnAttribute>();
                if (depedencyAttribute != null && depedencyAttribute.PropertyName == propertyName)
                    OnPropertyChanged(property.Name);
            }
        }
    }
}
