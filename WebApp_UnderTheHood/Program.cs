using Microsoft.AspNetCore.Authorization;
using WebApp_UnderTheHood.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", opts =>
{
    opts.Cookie.Name = "MyCookieAuth";
    opts.LoginPath = "/Accounts/Login";
    opts.AccessDeniedPath = "/Accounts/Default";
    opts.ExpireTimeSpan = TimeSpan.FromSeconds(30);
});

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("RequireAdminRole", 
        policy => policy.RequireClaim("Admin"));
    opts.AddPolicy("MustBelongToHR", policy => 
        policy.RequireClaim("Department", "HR"));
    opts.AddPolicy("HRManagerOnly", policy => policy
        .RequireClaim("Department", "HR")
        .RequireClaim("Manager")
        .Requirements.Add(new HrManagerProbationRequirement(3)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HrManagerProbationRequirementHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();