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

        protected virtual void DepedencyProperites(ViewModel viewModel, [CallerMemberName] string? propertyName = null)
        {
            var viewModelType = viewModel.GetType();
            foreach (PropertyInfo property in viewModelType.GetProperties())
            {
                var depedencyAttribute = property.GetCustomAttribute<DependencyOnAttribute>();
                if (depedencyAttribute is not null)
                {
                    foreach (string prop in depedencyAttribute.PropertiesName)
                    {
                        if (!string.IsNullOrWhiteSpace(prop) && prop == propertyName)
                        {
                            OnPropertyChanged(property.Name);
                            var depenciedProperty = viewModelType.GetProperty(property.Name);
                            var isMoveToTreeAttribute = depenciedProperty.GetCustomAttribute<IsMoveToTreeAttribute>();
                            if (isMoveToTreeAttribute is not null)
                                DepedencyProperites(viewModel, property.Name);
                            break;
                        }
                    }
                }
            }
        }
    }
}
