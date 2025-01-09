using System.Globalization;

namespace lcpapi.Localization;

public class LocalizationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {        
        var defbybrowser = false;
        var cultureKey = !defbybrowser ? context.Response.Headers["X-Language"].ToString() : context.Request.Headers["Accept-Language"].ToString();

        if (!string.IsNullOrEmpty(cultureKey) && DoesCultureExist(cultureKey))
        {
            var culture = new CultureInfo(cultureKey);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        
        await next(context);
    }

    private static bool DoesCultureExist(string cultureName)
    {
        return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => string.Equals(culture.Name, cultureName,
StringComparison.CurrentCultureIgnoreCase));
    }
}