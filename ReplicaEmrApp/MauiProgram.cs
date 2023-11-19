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
        IStorageJSONService<List<ExceptionRecord>> storageJSONService = null;

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
                List<ExceptionRecord> datas = storageJSONService
                .ReadFromFileAsync(MagicValueHelper.DataPath,
                MagicValueHelper.ExceptionRecordFilename).Result;

                datas.Add(exceptionRecord);

                storageJSONService
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
                    container.RegisterSingleton<CurrentDeviceInformationService>();
                    container.RegisterSingleton<GlobalObject>();
                    container.Register(typeof(IStorageJSONService<>), typeof(StorageJSONService<>));
                    container.Register(typeof(IStorageUtility), typeof(StorageUtility));
                    container.Register<LoginService>();
                    container.Register<ReportCodeService>();
                    container.Register<ReportDetailService>();
                    container.Register<ExceptionService>();
                    container.RegisterForNavigation<SettingPage, SettingPageViewModel>();
                    container.RegisterForNavigation<MyFirstPage, MyFirstPageViewModel>();
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
                         .NavigateAsync(MagicValueHelper.SplashPage);
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

        var app = builder.Build();
        storageJSONService = app.Services
            .GetService(typeof(IStorageJSONService<List<ExceptionRecord>>))
            as IStorageJSONService<List<ExceptionRecord>>;
        return app;
    }
}
