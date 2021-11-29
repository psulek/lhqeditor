var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
// LHQ: Register generated strings model and localizator, those classes was generated from 'Strings.lhq' file into folder 'Resources'
services.AddTypedStringsLocalizer<StringsModelLocalizer, StringsModel>();

services.AddControllersWithViews()
    .AddMvcTypedStringsLocalizer(); // LHQ: Configure MVC related things (like data annotations) to work with generated strings localizer.


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("en-GB"),
    new CultureInfo("en"),

    new CultureInfo("sk"),
    new CultureInfo("sk-SK")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-GB"),
    // Formatting numbers, dates, etc.
    SupportedCultures = supportedCultures,
    // UI strings that we have localized.
    SupportedUICultures = supportedCultures
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// LHQ: Use previously registered typed strings localizer.
app.UseTypedStringsLocalizer<StringsModelLocalizer, StringsModel>();

app.Run();