using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public class AgeSuccessToastFilter : IAsyncActionFilter
{
    private readonly IAuthorizationService _auth;
    private const string CookieName = "age_ok_toast";   // session one shot marker

    public AgeSuccessToastFilter(IAuthorizationService auth)
    {
        _auth = auth;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var http = ctx.HttpContext;

        // Only if signed in and we haven't shown the toast yet in this session
        if (http.User.Identity?.IsAuthenticated == true &&
            !http.Request.Cookies.ContainsKey(CookieName))
        {
            // Ask authorization system if we pass
            var result = await _auth.AuthorizeAsync(http.User, "Age18+");
            if (result.Succeeded)
            {
                // Show the message
                var tempFactory = http.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                tempFactory.GetTempData(http)["AuthSuccess"] = "Authentication successful";

                // Drop a cookie so we don’t spam the toast on every page
                // Now I know why websites want us to accept cookies
                http.Response.Cookies.Append(CookieName, "1", new CookieOptions
                {
                    IsEssential = true,
                    HttpOnly = false,
                    MaxAge = TimeSpan.FromMinutes(10)
                });
            }
        }

        await next();

        // Optional to clear it immediately after first render, don't need a ton of cookies
        if (http.Request.Cookies.ContainsKey(CookieName))
        {
            http.Response.Cookies.Delete(CookieName);
        }
    }
}
