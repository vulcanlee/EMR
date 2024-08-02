using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Prism.Ioc;

using Newtonsoft.Json;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;
using ReplicaEmrApp.ViewModels;
using ReplicaEmrApp.Views;
using ShareResource.Models;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Maui.LifecycleEvents;
using CommunityToolkit.Mvvm.Messaging;
using ReplicaEmrApp.Models.Mcs;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;

#if ANDROID
using ReplicaEmrApp;
#endif

#if IOS
using ReplicaEmrApp.Platforms.iOS;
#endif

namespace ReplicaEmrApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            var currentDeviceInformationService = ServiceProviderHelper.Current
            .GetService<CurrentDeviceInformationService>();
            var globalObject = ServiceProviderHelper.Current
            .GetService<GlobalObject>();
            var exceptionRecord = new ExceptionRecord();
            exceptionRecord.Exception = JsonConvert.SerializeObject(e.ExceptionObject);
            exceptionRecord.Platform = currentDeviceInformationService.Current.Platform;
            exceptionRecord.Model = currentDeviceInformationService.Current.Model;
            exceptionRecord.Version = currentDeviceInformationService.Current.VersionString;
            exceptionRecord.UserId = globalObject?.UserName;
            exceptionRecord.CreateAt = DateTime.Now;
            exceptionRecord.Message = (e.ExceptionObject as Exception)?.Message;

            var storageJSONService = ServiceProviderHelper.Current
            .GetService<IStorageJSONService<List<ExceptionRecord>>>();
            List<ExceptionRecord> datas = storageJSONService
            .ReadFromFileAsync(MagicValueHelper.DataPath,
            MagicValueHelper.ExceptionRecordFilename).Result;

            datas.Add(exceptionRecord);

            storageJSONService
            .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename, datas).Wait();

            Task.Run(() =>
            {
                var storageJSONService = ServiceProviderHelper.Current
                 .GetService<IStorageJSONService<List<ExceptionRecord>>>();
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
            .UseMauiCommunityToolkit()
            .UsePrism(prism =>
            {
                prism.RegisterTypes(container =>
                {
                    container.RegisterSingleton<CurrentDeviceInformationService>();
                    container.RegisterSingleton<GlobalObject>();
                    container.RegisterSingleton<UnsignReportStorageService>();
                    container.Register(typeof(IStorageJSONService<>), typeof(StorageJSONService<>));
                    container.Register(typeof(IStorageUtility), typeof(StorageUtility));
                    container.Register<LoginService>();
                    container.Register<CheckSessionService>();
                    container.Register<UnsignService>();
                    container.Register<DocumentReportService>();
                    container.Register<FailLogService>();
                    container.Register<SignatureAddService>();
                    container.Register<ExceptionService>();
                    container.Register<ConfigService>();
                    container.Register<ParameterService>();
                    container.Register<TenantService>();
                    container.Register<IDeviceService, DeviceService>();
                    container.RegisterInstance<IDeviceInfo>(DeviceInfo.Current);
                    container.RegisterForNavigation<SettingPage, SettingPageViewModel>();
                    container.RegisterForNavigation<SplashPage, SplashPageViewModel>();
                    container.RegisterForNavigation<LoginPage, LoginPageViewModel>();
                    container.RegisterForNavigation<HomePage, HomePageViewModel>();
                    container.RegisterForNavigation<ReportDetailPage, ReportDetailPageViewModel>();
                    container.RegisterForNavigation<ReportContentPage, ReportContentPageViewModel>();
                    container.RegisterForNavigation<FailLogPage, FailLogPageViewModel>();
                    container.RegisterForNavigation<NaviPage, NaviPageViewModel>();
                })
                     .OnInitialized(() =>
                     {
                         // Do some initializations here
                     })
                     .CreateWindow(async navigationService =>
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

                var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("ReplicaEmrApp.appsettings.json");

        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();


        builder.Configuration.AddConfiguration(config);

#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            //handler.PlatformView
            //.SetBackgroundColor(Android.Graphics.Color.White);
            handler.PlatformView.BackgroundTintList =
            Android.Content.Res.ColorStateList
            .ValueOf(Android.Graphics.Color.White);
        });
#endif

        var app = builder.Build();
        return app;
    }
}
