using Newtonsoft.Json;
using Prism.Ioc;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;
using ReplicaEmrApp.ViewModels;
using ReplicaEmrApp.Views;
using ShareResource.Models;
using System.Text;

namespace ReplicaEmrApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            var exceptionRecord = new ExceptionRecord();
            exceptionRecord.Exception = e.ExceptionObject.ToString();
            exceptionRecord.Platform = "MAUI";
            exceptionRecord.Model = "MAUI";
            exceptionRecord.Version = "MAUI";
            exceptionRecord.UserId = "MAUI";

            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Task.Run(() =>
            {
                List<ExceptionRecord> datas = null;
                var task = StorageJSONService<List<ExceptionRecord>>
                .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename).Result;

                datas.Add(exceptionRecord);

                StorageJSONService<List<ExceptionRecord>>
                .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename, datas).Wait();
            }).Wait();
        };

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UsePrism(prism =>
            {

                prism.RegisterTypes(container =>
                      {
                          container.RegisterSingleton<GlobalObject>();
                          container.Register<LoginService>();
                          container.Register<ReportCodeService>();
                          container.Register<ReportDetailService>();
                          container.Register<ExceptionService>();
                          container.RegisterForNavigation<MainPage, MainPageViewModel>();
                          container.RegisterForNavigation<SplashPage, SplashPageViewModel>();
                          container.RegisterForNavigation<LoginPage, LoginPageViewModel>();
                          container.RegisterForNavigation<HomePage, HomePageViewModel>();
                          container.RegisterForNavigation<ReportDetailPage, ReportDetailPageViewModel>();
                          container.RegisterForNavigation<ReportContentPage, ReportContentPageViewModel>();
                          container.RegisterForNavigation<NaviPage, NaviPageViewModel>();
                          container.RegisterForNavigation<MDPage, MDPageViewModel>();
                      })
                     .OnInitialized(() =>
                      {
                          // Do some initializations here
                      })
                     .OnAppStart(async navigationService =>
                     {
                         // Navigate to First page of this App
                         var result = await navigationService
                         .NavigateAsync("SplashPage");
                         if (!result.Success)
                         {
                             System.Diagnostics.Debugger.Break();
                         }
                     });
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("materialdesignicons-webfont.ttf", "material");
            });

        return builder.Build();
    }
}
