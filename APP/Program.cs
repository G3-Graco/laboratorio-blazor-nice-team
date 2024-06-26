using APP.Components;
using APP.Data.Servicios;
using Tewr.Blazor.FileReader;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBootstrap();

builder.Services.AddScoped<UsuarioServicio>();
builder.Services.AddScoped<ClienteServicio>();
builder.Services.AddScoped<CuentaServicio>();
builder.Services.AddScoped<PrestamoServicio>();
builder.Services.AddScoped<MovimientoServicio>();
builder.Services.AddScoped<PagoServicio>();
builder.Services.AddScoped<CuotaServicio>();
builder.Services.AddScoped<DocumentoServicio>();

builder.Services.AddFileReaderService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();
