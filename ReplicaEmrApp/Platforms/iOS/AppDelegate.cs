using CommunityToolkit.Mvvm.Messaging;
using Foundation;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Models.Mcs;
using System.Diagnostics;
using UIKit;

namespace ReplicaEmrApp;

[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate
{
    public static ActivityRequestCodeEnum requestCode;
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        var urlComponents = new NSUrlComponents(url, false);
        var queryItems = urlComponents?.QueryItems;
        var code = queryItems?.FirstOrDefault(x => x.Name == "code")?.Value;
        var basicId = queryItems?.FirstOrDefault(x => x.Name == "basicId")?.Value;
        var message = queryItems?.FirstOrDefault(x => x.Name == "message")?.Value;

        #region 取得憑證
        if (requestCode == ActivityRequestCodeEnum.GetCertRequestCode)
        {
            var customResult = new GetCertResponse
            {
                Code = code,
                Idno = basicId,
                Message = message
            };

            WeakReferenceMessenger.Default.Send(customResult);
        }
        #endregion
        #region 取得登入者資訊
        if (requestCode == ActivityRequestCodeEnum.GetUserDeviceCode)
        {
            var customResult = new UserDeviceResponse
            {
                Code = code,
                BasicId = basicId,
                Message = message
            };

            WeakReferenceMessenger.Default.Send(customResult);

        }
        #endregion
        #region 簽章
        if (requestCode == ActivityRequestCodeEnum.SignRequestCode)
        {
            var customResult = new SignResponse
            {
                Code = code,
                Idno = basicId,
                Message = message
            };

            WeakReferenceMessenger.Default.Send(customResult);

        }
        #endregion
        #region 批次簽章
        if (requestCode == ActivityRequestCodeEnum.SignBatchRequestCode)
        {
            var customResult = new SignBatchResponse
            {
                Code = code,
                Idno = basicId,
                Message = message
            };

            WeakReferenceMessenger.Default.Send(customResult);

        }
        #endregion

        return base.OpenUrl(application, url, options);
    }
}
