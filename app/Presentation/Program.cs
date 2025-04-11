using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Infrastructure.Services;
using Infrastructure.Repository;
using MudBlazor.Services;
using Core.Data.Mail;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Authorization;
using Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Ajouter les services au conteneur.
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();

// Ajout de l'accès au HttpContext
builder.Services.AddHttpContextAccessor();

// Configurer l'authentification par cookies
// Ajout de l'authentification par cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";    
        options.LogoutPath = "/logout";  
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Désactivé pour tester en local, utilise Always en production
        options.Cookie.SameSite = SameSiteMode.Lax;  // Permet au cookie d'être accepté dans plus de cas
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireProfileComplete", policy =>
    {
        // Exige le claim "ProfileComplete" = "true"
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("ProfileComplete", "true");
    });
});

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();


// Ajouter les services personnalisés
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPrefSexService, PrefSexService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ILikeService, LikeService>();

builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

// Ajouter les repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository>(sp => new SessionRepository(connectionString));
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IPrefSexRepository, PrefSexRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();

builder.Services.AddSingleton<INotificationRepository, NotificationRepository>();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

builder.Services.AddSingleton<IProfileImageStateService ,ProfileImageStateService>();

var app = builder.Build();

// Configuration du pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<Presentation.Middlewares.SessionValidationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Mapper les contrôleurs après l'authentification/autorisation
app.MapControllers();

// Remplacer UseEndpoints par des enregistrements de route de haut niveau
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();