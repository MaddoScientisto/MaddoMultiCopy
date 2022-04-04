using Microsoft.AspNetCore.Components.WebView.Maui;
using Multicopy.MAUI.Data;
using Multicopy.MAUI.Services;

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
builder.Services.AddTransient<IFolderPicker, MaddoImageViewer.Platforms.MacCatalyst.FolderPicker>();;
#endif

		builder.Services.AddBlazorWebView();
		builder.Services.AddSingleton<WeatherForecastService>();

		return builder.Build();
	}
}
