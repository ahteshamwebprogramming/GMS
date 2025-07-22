using Dapper;
using GMS.Core.Repository;
using GMS.Infrastructure.Helper;
using GMS.Infrastructure.Models.AppParameters;
using GMS.Infrastruture.Helper;
using GMS.Services;
using GMS.Services.Configurations;
using GMS.Services.DBContext;
using GMS.Services.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddControllers().AddControllersAsServices();

builder.Services.AddScoped<DapperDBContext>();
builder.Services.AddScoped<DapperEHRMSDBContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddHttpContextAccessor();
SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());
SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());

builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<ClientInfo>(builder.Configuration.GetSection("ClientInfo"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Account/ErrorMessage";
            options.LoginPath = "/Account/Login";
        });

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//For Sessions
app.UseSession();
//For Sessions end
app.MapRazorPages();
app.UseMiddleware<ClientIpMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
});
app.Run();


