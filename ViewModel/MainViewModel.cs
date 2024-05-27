using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace WeatherApp.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [RelayCommand]
        async Task RequsetLocation()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
                return;

            var status = PermissionStatus.Unknown;
            status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                return;
            }
                    

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>() || Permissions.ShouldShowRationale<LocationAlways>())
            {
                await Shell.Current.DisplayAlert("Needs permissions", "BECAUSE!!!", "OK");
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                await Shell.Current.DisplayAlert("Permission required",
                    "Location permission is required for location scanning. " +
                    "We do not store or use your location at all.", "OK");
        }
    }
}
