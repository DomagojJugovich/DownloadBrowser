using FileBrowser;
using FileBrowser.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;


/* **************** DEV MODE FOR IIS CONFIG *******************************************
    <aspNetCore processPath=".\FileBrowser.exe" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" >
	   <environmentVariables>
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Development" />
      </environmentVariables>
	  </aspNetCore>
 */

var builder = WebApplication.CreateBuilder(args);

var appSettingsPath = Path.Combine(builder.Environment.ContentRootPath, "appsettings.json");
Users.init(builder.Configuration, appSettingsPath);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<FolderListService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
