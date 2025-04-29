using MyHelper.Services.Interfaces;
using System.Runtime.CompilerServices;

namespace MyHelper.ViewModels.Base
{
    internal class ConnectedViewModel : ViewModel
    {
        protected readonly IPropertyDependency PropertyDependency;

        protected ConnectedViewModel(IPropertyDependency propertyDepenency)
        {
            PropertyDependency = propertyDepenency;
            PropertyDependency.RegisterDependencies(this.GetType());
        }

        protected void OnConnectedPropertyChanged([CallerMemberName] string propertyName = "", bool currentPropertyChanged = false)
        {
            if(currentPropertyChanged) OnPropertyChanged(propertyName);
            foreach (var connectedProperty in PropertyDependency.GetDependentProperties(propertyName))
            {
                OnPropertyChanged(connectedProperty);
                if(PropertyDependency.DependencyContains(connectedProperty))
                    OnConnectedPropertyChanged(connectedProperty);
            }
        }
    }
}
