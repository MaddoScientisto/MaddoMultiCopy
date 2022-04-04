using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Multicopy.MAUI.Data;
using Multicopy.MAUI.Services;
using Multicopy.MAUI.Services.Impl;

namespace Multicopy.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.RegisterBlazorMauiWebView()
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

#if WINDOWS
		builder.Services.AddTransient<IFolderPicker, Multicopy.MAUI.Platforms.Windows.FolderPicker>();
#elif MACCATALYST
builder.Services.AddTransient<IFolderPicker, Multicopy.MAUI.Platforms.MacCatalyst.FolderPicker>();;
#endif

		builder.Services
	.AddBlazorise(options =>
	{
		options.Immediate = true;
	})
	.AddBootstrap5Providers()
	.AddFontAwesomeIcons();

		builder.Services.AddBlazorWebView();
		builder.Services.AddSingleton<WeatherForecastService>();
		builder.Services.AddTransient<ICopyService, CopyService>();


		builder.Services.AddBlazoredLocalStorage();

		return builder.Build();
	}
}
