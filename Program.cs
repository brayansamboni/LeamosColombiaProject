using LeamosColombiaProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<LeamosColombiaProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection")));

builder.Services.AddMailKit(config =>
{
    config.UseMailKit(builder.Configuration.GetSection("Email").Get<MailKitOptions>());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/InicioSesion/IniciarSesion";
        options.AccessDeniedPath = "/InicioSesion/AccessDenied";
        options.LogoutPath = "/InicioSesion/CerrarSesion";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
        options.Events = new CookieAuthenticationEvents

        {
            OnRedirectToLogin = context =>
            {
                context.Response.Redirect(options.LoginPath);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "resetpassword",
    pattern: "InicioSesion/RestablecerContrasena/{token?}",
    defaults: new { controller = "InicioSesion", action = "RestablecerContrasena" }
);
app.MapControllerRoute(
    name: "Perfil",
    pattern: "Perfil/Edit/{id?}",
    defaults: new { controller = "Perfil", action = "Edit" }
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=InicioSesion}/{action=IniciarSesion}/{id?}");

app.Run();
