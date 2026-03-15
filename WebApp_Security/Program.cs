using Microsoft.AspNetCore.Authorization;
using WebApp_Security.Authorization;

var builder = WebApplication.CreateBuilder(args);

// 向依赖注入容器中添加 Razor Pages 服务
builder.Services.AddRazorPages();
// -------------------- 认证 Authentication --------------------
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", option =>
{
    option.Cookie.Name = "MyCookieAuth";
    option.LoginPath = "/Account/Login";
    option.ExpireTimeSpan = TimeSpan.FromSeconds(200);
});
// -------------------- 授权 Authorization --------------------
// 添加授权策略（Policy）
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly",policy=>policy.RequireClaim("Admin"))
    .AddPolicy("HRManagerOnly", policy => policy.RequireClaim("Department", "HR").RequireClaim("Manager").Requirements.Add(new HRManagerProbationRequirement(3)))
    .AddPolicy("MustBelongToHRDepartment", policy => policy.RequireClaim("Department","HR"));

// -------------------- HttpClient --------------------
builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7078");
});

// -------------------- 自定义授权处理器 --------------------
builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationHandler>();


builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// -------------------- HTTP 请求管道配置 --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
