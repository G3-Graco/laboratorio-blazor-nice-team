using APP.Components;
using APP.Data.Servicios;
using BlazorServerAuthenticationAndAuthorization.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

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

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_blazor";
        options.LoginPath = "/iniciarsesion";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(7);
        options.AccessDeniedPath = "/no-autorizado";
    });


builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
provider.GetRequiredService<CustomAuthenticationStateProvider>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();



app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();
