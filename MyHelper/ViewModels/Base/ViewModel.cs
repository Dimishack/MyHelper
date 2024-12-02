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

        protected virtual void PropertiesChanged(ViewModel viewModel,[CallerMemberName] string? propertyName = null)
        {
            var viewModelType = viewModel.GetType();
            foreach (PropertyInfo property in viewModelType.GetProperties())
            {
                var depedencyAttribute = property.GetCustomAttribute<DependencyOnAttribute>();
                if (depedencyAttribute is not null)
                {
                    foreach (string prop in depedencyAttribute.PropertiesName)
                    {
                        if(!string.IsNullOrWhiteSpace(prop) && prop == propertyName)
                        {
                            OnPropertyChanged(property.Name);
                            LinkPropertiesChanged(viewModelType, property.Name);
                            break;
                        }
                    } 
                }
            }
        }

        private void LinkPropertiesChanged(Type viewModelType, string propertyName)
        {
            var propertyWithChanged = viewModelType.GetProperty(propertyName);
            if (propertyWithChanged is not null)
            {
                var changedPropertiesWithAttribute = propertyWithChanged.GetCustomAttribute<ChangesWithPropertiesAttribute>();
                if (changedPropertiesWithAttribute != null)
                {
                    foreach (var changedProp in changedPropertiesWithAttribute.PropertiesName)
                    {
                        if (!string.IsNullOrWhiteSpace(changedProp.Key))
                            OnPropertyChanged(changedProp.Key);
                        if(changedProp.Value)
                            LinkPropertiesChanged(viewModelType, changedProp.Key);
                    }
                }
            }
        }
    }
}
