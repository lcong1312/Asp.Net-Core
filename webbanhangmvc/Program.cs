using Microsoft.EntityFrameworkCore;
using webbanhangmvc.Casso;
using webbanhangmvc.Models;
using webbanhangmvc.Repository;
using webbanhangmvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("QlbanValiContext");
builder.Services.AddDbContext<QlbanVaLiContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddScoped<ICassoService, CassoService>();


builder.Services.AddScoped<ILoaiSpRepository, LoaiSpRepository>();
builder.Services.AddScoped<IChietKhauRepository, ChietKhauRepository>();
builder.Services.AddHttpClient();

builder.Services.Configure<VietQRSettings>(builder.Configuration.GetSection("VietQR"));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IVnPayServices, VnPayServices>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
    DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);
    context.Items["CurrentTime"] = currentTime;
    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
