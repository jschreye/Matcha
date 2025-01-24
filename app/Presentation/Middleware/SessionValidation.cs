using Core.Interfaces;

namespace Presentation.Middlewares
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Injecte IUserService comme paramètre de méthode pour le résoudre dans le scope de la requête
        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            if (context.Request.Cookies.TryGetValue("SessionToken", out var sessionToken))
            {
                var isValid = await userService.ValidateSessionAsync(sessionToken);
                if (!isValid)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
            await _next(context);
        }
    }
}