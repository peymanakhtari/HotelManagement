using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};

// Add services to the container.
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.SlidingExpiration = true;
        options.LoginPath = "/User/Login";
    });

var wwwrootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
var dataProtectionPath = Path.Combine(wwwrootPath, "DataProtection");

builder.Services.AddDataProtection()
    .SetApplicationName("hotelsimorgh")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseDeveloperExceptionPage();
    //app.UseExceptionHandler("/Home/Error");
    //app.UseHsts();
}

app.UseStaticFiles();

app.UseCookiePolicy(cookiePolicyOptions);


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Reserve}/{action=ReservesTable}/{id?}");



app.MapRazorPages();
app.MapDefaultControllerRoute();



app.Run();
