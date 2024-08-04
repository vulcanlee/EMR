# 擴充與客製化 Prism-MAUI 的專案樣板

會用到的指令為

* 切換到工作目錄

d:

cd D:\Vulcan\GitHub\Prism-MAUI-Template\src

* 產生出 NuGet 套件

d:\nuget.exe pack D:\Vulcan\GitHub\Prism-MAUI-Template\Vulcan.Maui.Template.nuspec -outputdirectory d:\temp

* 安裝此 NuGet 套件 - 此專案範本

Install template

dotnet new install d:\temp\Vulcan.Maui.Template.0.2.5.nupkg

* 使用此專案範本，產生一個新的專案

# Create a new empty project of Prism.Maui

dotnet new Vulcan-Maui -o d:\Temp\ReplicaEmrApp

# Create a new full project of Prism.Maui (has flyout menu)

dotnet new Full-Maui -o d:\Temp\ReplicaEmrApp

* 建立 View / ViewModel 項目

# Create a new View and ViewModel (don't append Page to view-name parameter)

dotnet new MVVMItem  --namespace ReplicaEmrApp --view-name ExceptionList

# Create a new View and ViewModel for Prism MAUI Project (don't append Page to view-name parameter)

dotnet new MVVMPrismItem --namespace MyApp --view-name ExceptionDetail

* 移除此專案範本

dotnet new uninstall Vulcan.Maui.Template
