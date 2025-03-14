using Globomantics.Domain.Models;
using Globomantics.Infrastructure.Data;
using Globomatics.Infrastructure.Repositories;
using Globomatics.Web.Attributes;
using Globomatics.Web.Constraints;
using Globomatics.Web.Filters;
using Globomatics.Web.Repositories;
using Globomatics.Web.Transformers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
    options.LoginPath = "/Login";
});

builder.Services.AddSession();

builder.Services.AddRouting(options =>
{
    options.ConstraintMap["validateSlug"] = typeof(SlugConstraint);
    options.ConstraintMap["slugTransform"] = typeof(SlugParameterTransformer);
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<TimerFilter>(); 
    options.ValueProviderFactories.Add(new SessionValueProviderFactory());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<GlobomanticsContext>(ServiceLifetime.Scoped);

builder.Services.AddTransient<IStateRepository, SessionStateRepository>();
builder.Services.AddTransient<IRepository<Customer>, CustomerRepository>();
builder.Services.AddTransient<IRepository<Product>, ProductRepository>();
builder.Services.AddTransient<IRepository<Order>, OrderRepository>();
builder.Services.AddTransient<IRepository<Cart>, CartRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();

builder.Services.AddScoped<TimerFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseSession();

//app.MapControllerRoute(
//    name: "ticketDetailsRoute",
//    defaults: new { action = "TicketDetails", controller = "Home" },
//    pattern: "/details/{productId}/{slug?}");   


app.MapControllerRoute(
    name: "administrationDefault",
    defaults: new { controller = "Home"},
    pattern: "{area:exists}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "administration",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GlobomanticsContext>();
    GlobomanticsContext.CreateInitialDatabase(context);
}
app.Run();