using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Font = Microsoft.Maui.Font;

namespace ReplicaEmrApp.Helpers
{
    public class SnackbarHelper
    {
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
            TimeSpan duration = TimeSpan.FromSeconds(2);

            var snackbar = Snackbar.Make(text, null, "", duration, snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }
    }
}
