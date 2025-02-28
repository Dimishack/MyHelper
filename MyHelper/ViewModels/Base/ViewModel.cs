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

        protected virtual void DepedencyProperites([CallerMemberName] string? propertyName = null)
        {
            var viewModelType = this.GetType();
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
                            var isMoveToTreeAttribute = property.GetCustomAttribute<IsMoveToTreeAttribute>();
                            if (isMoveToTreeAttribute is not null)
                                DepedencyProperites(property.Name);
                            break;
                        }
                    }
                }
            }
        }
    }
}
