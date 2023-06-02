using ElectronNET.API;
using GestThéLib.Data.CSV;
using GestThéLib.Data.Database;
using GestThéLib.Data.Electron;
using GestThéLib.Data.PDF;
using Plk.Blazor.DragDrop;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();
builder.Services.AddLocalization();

// Blazor DragDrop library
builder.Services.AddBlazorDragDrop();

// Set the web-host to ElectronNET
builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

// Database Context
builder.Services.AddTransient<DatabaseContext>();

// Radzen Services
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();

// CSV & PDF services
builder.Services.AddScoped<CsvGenerator>();
builder.Services.AddScoped<PdfGenerator>();

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

// Swiss French localization
app.UseRequestLocalization("fr-CH");

// If Electron is active, open the mainWindow
if (HybridSupport.IsElectronActive)
{
    await ElectronHandler.BuildElectronWindow();
}

app.Run();