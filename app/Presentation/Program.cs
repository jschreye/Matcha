using Core.Interfaces;
using Infrastructure.Services;
using Infrastructure.Model;
using Core.Repository;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Ajouter les services personnalisés
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();

// Ajouter les models personnalisés
builder.Services.AddScoped<IUserRepository, UserModel>();

var app = builder.Build();

// Configuration du pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("Error/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();