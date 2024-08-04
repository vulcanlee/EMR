using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace ReplicaEmrApp.Helpers
{
    public class SnackbarHelper
    {
        static ISnackbar SnackbarCache = null;
        public static async Task Show(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromRgba("#00A9B4FF"),
                TextColor = Color.FromRgba("#E3F6FAFF"),
                //TextColor = Colors.Orange,
                CornerRadius = new CornerRadius(10),
                Font = Font.SystemFontOfSize(14),
                CharacterSpacing = 0.5,
            };

            string text = $"{message}";
            TimeSpan duration = TimeSpan.FromSeconds(3);

            SnackbarCache = Snackbar.Make(text, null, "", duration, snackbarOptions);

            await SnackbarCache.Show(cancellationTokenSource.Token);
        }

        public static async Task Dismiss()
        {
            if (SnackbarCache == null)
            {
                return;
            }
            await SnackbarCache.Dismiss();
            SnackbarCache = null;
        }
    }
}
