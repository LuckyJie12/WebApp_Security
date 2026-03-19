# WebApp_Security

## 项目简介

WebApp_Security 是一个基于 **ASP.NET Core Razor Pages** 的 Web 应用示例，主要用于演示 **认证（Authentication）**、**授权（Authorization）** 和 **安全策略（Policy-based Authorization）** 的实现。

该项目通过 **Cookie Authentication** 实现用户登录状态管理，并结合 **声明**`Claims`和 **策略**`Policy`控制用户访问权限。同时使用 **HttpClient** 调用外部 WebAPI，并通过 **Session** 管理部分用户状态。

------

## 技术栈

- ASP.NET Core Razor Pages
- Cookie Authentication
- Policy-based Authorization
- Claims-based Identity
- HttpClientFactory
- Session State
- Dependency Injection

------

## 项目主要功能

### 1. 用户认证（Authentication）

项目使用 **Cookie Authentication** 进行身份验证。

配置：

```csharp
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromSeconds(200);
});
```

功能说明：

- 登录成功后，服务器生成认证 Cookie。
- 浏览器保存 Cookie 用于后续请求。
- 如果访问受保护页面但未登录，系统会自动重定向到：

```
/Account/Login
```

Cookie 有效期为：

```
200 秒
```

------

### 2. 用户授权（Authorization）

项目采用 **Policy-based Authorization**。

配置：

```csharp
builder.Services.AddAuthorizationBuilder()
```

系统定义了三个访问策略。

------

#### Policy 1：AdminOnly

```csharp
.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"))
```

访问要求：

用户必须拥有 **Admin Claim**。

示例：

```
Admin = true
```

------

#### Policy 2：HRManagerOnly

```csharp
.AddPolicy("HRManagerOnly", policy =>
    policy.RequireClaim("Department", "HR")
          .RequireClaim("Manager")
          .Requirements.Add(new HRManagerProbationRequirement(3)))
```

访问要求：

用户必须同时满足以下条件：

1. Department = HR
2. 具有 Manager Claim
3. 通过自定义授权规则 `HRManagerProbationRequirement`

该规则用于判断：

```
HR Manager 入职是否超过 3 个月
```

------

#### Policy 3：MustBelongToHRDepartment

```csharp
.AddPolicy("MustBelongToHRDepartment",
    policy => policy.RequireClaim("Department","HR"));
```

访问要求：

用户必须属于 **HR 部门**。

------

### 3. 自定义授权处理器

项目实现了一个自定义 Authorization Handler：

```
HRManagerProbationHandler
```

注册方式：

```csharp
builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationHandler>();
```

作用：

判断 HR Manager 是否满足试用期要求。

例如：

```C#
public class HRManagerProbationHandler : AuthorizationHandler<HRManagerProbationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == "Department" && c.Value == "HR") &&
            context.User.HasClaim(c => c.Type == "Manager" && c.Value == "true") &&
            context.User.HasClaim(c => c.Type == "DepartmentDate"))
        {
            var departmentDateClaim = context.User.FindFirst(c => c.Type == "DepartmentDate");
            if (DateTime.TryParse(departmentDateClaim?.Value, out var departmentDate))
            {
                var monthsInDepartment = ((DateTime.Now - departmentDate).Days) / 30;
                if (monthsInDepartment >= requirement.ProbationMonths)
                {
                    context.Succeed(requirement);
                }
            }
        }
        return Task.CompletedTask;
    }
}
```

如果入职时间超过策略设置的月份数，则授权成功。

------

### 4. HttpClient 调用 WebAPI

项目使用 **IHttpClientFactory** 调用外部 Web API。

配置：

```csharp
builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7078");
});
```

功能：

WebApp → 调用 WebAPI 获取数据，例如：

```
WeatherForecast API
```

优点：

- 避免 Socket Exhaustion
- 统一管理 HTTP 请求
- 支持 DI 注入

------

### 5. Session 管理

项目启用了 **Session State**：

```csharp
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
```

配置说明：

| 配置        | 说明                             |
| ----------- | -------------------------------- |
| HttpOnly    | Cookie 不允许 JS 访问            |
| IsEssential | 即使用户未同意 Cookie 也允许使用 |
| IdleTimeout | 30 分钟无操作自动过期            |

启用 Session Middleware：

```csharp
app.UseSession();
```

------

### 6. HTTP 请求管道

项目使用以下 Middleware Pipeline：

```
HTTPS Redirection
        ↓
Routing
        ↓
Authentication
        ↓
Authorization
        ↓
Session
        ↓
Razor Pages
```

关键代码：

```csharp
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
```

注意：

Authentication 必须在 Authorization 之前。

------

### 7. 静态资源

项目通过以下方式提供静态文件：

```csharp
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
```

支持资源：

```
CSS
JavaScript
Images
Fonts
```

------

# WebAPI_Security

## 项目简介

WebAPI_Security 是一个基于 **ASP.NET Core WebAPI** 的示例项目，演示如何使用 **JWT（JSON Web Token）认证** 和 **Policy-based 授权** 来保护 API 接口。

项目特点：

- 支持 JWT 登录和验证
- 支持基于**声明** `Claims` 的 **策略**`Policy` 授权
- 使用 **Symmetric Key** 签名 JWT
- 使用 **ASP.NET Core Controllers** 构建 API

------

## 技术栈

- ASP.NET Core 10+
- JSON Web Token (JWT)
- JwtBearer Authentication
- Policy-based Authorization
- Symmetric Security Key

## 依赖Nugget包

- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.IdentityModel.JsonWebTokens

------

## 项目主要功能

### 1. JWT 认证（Authentication）

项目通过 **JwtBearer** 中间件验证请求中的 JWT Token。

配置代码：

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]!)
        ),
        ClockSkew = TimeSpan.Zero
    };
});
```

说明：

- **IssuerSigningKey**: 使用 `appsettings.json` 中配置的 SecretKey（必须 ≥32 字符）
- **ValidateLifetime = true**: 验证 Token 是否过期
- **ClockSkew = 0**: 不允许额外时间偏差，严格验证过期时间
- 默认 **AuthenticateScheme/ChallengeScheme** 都是 JwtBearer

> 如果客户端请求缺少或无效 JWT，会返回 401 Unauthorized。

------

### 2. Policy 授权（Authorization）

项目定义了一个简单的 Policy：

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
});
```

- **AdminOnly**: 只有 Token 中包含 **Admin Claim** 的用户可以访问
- 在 Controller 上使用：

```csharp
[Authorize(Policy = "AdminOnly")]
[HttpGet("secure-data")]
public IActionResult GetSecureData()
{
    return Ok("This is admin only data");
}
```

------

### 3. 中间件配置（Pipeline）

请求处理顺序：

```text
HTTPS Redirection
        ↓
Authentication (JwtBearer)
        ↓
Authorization (Policy)
        ↓
Controller
```

对应代码：

```csharp
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

⚠️ **顺序很重要**：`UseAuthentication()` 必须在 `UseAuthorization()` 之前。

------

### 4. SecretKey 配置

选择项目右键选择**管理用户机密**中配置：

```json
{
  "SecretKey": "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
}
```

- 必须 ≥32 字符，符合 HmacSha256 安全要求
- Token 生成时和验证时都要用同一个 Key

------

### 5. Controller 示例

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { "Sunny", "Cloudy", "Rainy" });

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("secure")]
    public IActionResult GetSecure() => Ok("This is Admin only data");
}
```

- 普通接口：无需 Token，可直接访问
- 受保护接口：需要 JWT 且包含 Admin Claim

------

### 6. 请求示例

#### 获取普通数据

```http
GET https://localhost:7078/WeatherForecast
```

无需 Token，返回天气数据列表。

------

#### 获取 Admin 数据

```http
GET https://localhost:7078/WeatherForecast/secure
Authorization: Bearer {your_jwt_token_here}
```

- Token 必须包含 `Admin` Claim
- 否则返回 401 Unauthorized

------

### 7. Token 生成（与 WebApp_Security 配合）

WebAPI 期望使用 **HmacSha256** 对称密钥签名的 JWT：

- SecretKey 必须与 WebApp_Security 中生成 Token 时使用的一致
- Claims 示例：

```json
{
  "Name": "admin",
  "Email": "admin@example.com",
  "Admin": "true"
}
```

- 生成 Token 可使用 `JsonWebTokenHandler` 或 `JwtSecurityTokenHandler`

------

# WebAPP

## 依赖Nugget包

- Microsoft.AspNetCore.Identity.ui package
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.Design

