using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Models.Mcs;

namespace ReplicaEmrApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        RequestedOrientation = ScreenOrientation.Portrait;

        WeakReferenceMessenger.Default.Register<GetCertRequest>(this, async (sender, message) =>
        {
            var builder = new Android.Net.Uri.Builder();
            builder.Scheme(message.Scheme)
            .AppendQueryParameter("operation", message.Operation)
            .AppendQueryParameter("account", message.Account)
            .AppendQueryParameter("password", message.Password)
            .AppendQueryParameter("hospital_code", message.HostialCode);

            var uri = builder.Build();
            var intent = new Intent(Intent.ActionView, uri);

            try
            {
                StartActivityForResult(intent, ActivityRequestCodeEnum.GetCertRequestCode.GetHashCode());
            }
            catch (Exception ex)
            { }
        });
        WeakReferenceMessenger.Default.Register<SignRequest>(this, async (sender, message) =>
        {
            var builder = new Android.Net.Uri.Builder();
            builder.Scheme(message.Scheme)
            .AppendQueryParameter("operation", message.Operation)
            .AppendQueryParameter("account", message.Account)
            .AppendQueryParameter("password", message.Password)
            .AppendQueryParameter("data", message.Data)
            .AppendQueryParameter("hash_flag", message.HashFlag)
            .AppendQueryParameter("hospital_code", message.HostialCode);

            var uri = builder.Build();
            var intent = new Intent(Intent.ActionView, uri);

            try
            {
                StartActivityForResult(intent, ActivityRequestCodeEnum.SignRequestCode.GetHashCode());
            }
            catch (Exception ex)
            { }
        });
        WeakReferenceMessenger.Default.Register<SignBatchRequest>(this, async (sender, message) =>
        {
            var builder = new Android.Net.Uri.Builder();
            builder.Scheme(message.Scheme)
            .AppendQueryParameter("operation", message.Operation)
            .AppendQueryParameter("account", message.Account)
            .AppendQueryParameter("password", message.Password)
            .AppendQueryParameter("data", message.Data)
            .AppendQueryParameter("hash_flag", message.HashFlag)
            .AppendQueryParameter("hospital_code", message.HostialCode);

            var uri = builder.Build();
            var intent = new Intent(Intent.ActionView, uri);

            try
            {
                StartActivityForResult(intent, ActivityRequestCodeEnum.SignBatchRequestCode.GetHashCode());
            }
            catch (Exception ex)
            { }
        });

    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        #region 取得憑證
        if (requestCode == ActivityRequestCodeEnum.GetCertRequestCode.GetHashCode())
        {
            if (resultCode == Result.Ok)
            {
                var bundle = data.Extras;
                var customResult = new GetCertResponse
                {
                    Code = bundle.GetInt("code").ToString(),
                    Idno = bundle.GetString("idno"),
                    Message = bundle.GetString("message")
                };

                WeakReferenceMessenger.Default.Send(customResult);
            }
            else
            {
                //回傳失敗訊息
                var customResult = new GetCertResponse
                {
                    Code = "-1",
                    Message = "請確認是否安裝行動憑證App"
                };

                WeakReferenceMessenger.Default.Send(customResult);
            }
        }
        #endregion
        #region 簽章
        if (requestCode == ActivityRequestCodeEnum.SignRequestCode.GetHashCode())
        {
            if (resultCode == Result.Ok)
            {
                var bundle = data.Extras;
                var customResult = new SignResponse
                {
                    Code = bundle.GetInt("code").ToString(),
                    Idno = bundle.GetString("idno"),
                    Message = bundle.GetString("message")
                };

                WeakReferenceMessenger.Default.Send(customResult);
            }
            else
            {
                //TODO: 顯示錯誤訊息並且秀在螢幕上
                //WeakReferenceMessenger.Default.Send(new GetCertResponse(false));
            }
        }
        #endregion
        #region 批次簽章
        if (requestCode == ActivityRequestCodeEnum.SignBatchRequestCode.GetHashCode())
        {
            if (resultCode == Result.Ok)
            {
                var bundle = data.Extras;
                var customResult = new SignBatchResponse
                {
                    Code = bundle.GetInt("code").ToString(),
                    Idno = bundle.GetString("idno"),
                    Message = bundle.GetString("message")
                };

                WeakReferenceMessenger.Default.Send(customResult);
            }
            else
            {
                //TODO: 顯示錯誤訊息並且秀在螢幕上
                //WeakReferenceMessenger.Default.Send(new GetCertResponse(false));
            }
        }
        #endregion

    }
}
