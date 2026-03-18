# ASP.NET Core 身份认证与授权 示例

这是一个教学示例仓库，包含两个主要子项目：

- `WebApp_Security`：基于 Razor Pages 的示例（Cookie Authentication、声明`Claims`、策略`Policy`、自定义 Authorization Handler、Session、IHttpClientFactory）。
- `WebAPI_Security`：基于 Controllers 的示例（JWT 验证、JwtBearer、Policy 授权）。

本仓库适合用于学习 ASP.NET Core 中的认证与授权实现以及生产环境下的若干安全注意事项。

----------------

## 目录

- 项目结构
- 快速开始
- 配置说明
- 本地运行（开发）
- 示例请求（curl）
- 关键实现文件
- 安全与生产建议
- 贡献与许可证

----------------

## 项目结构（概览）

- `WebApp_Security/` — Razor Pages 示例（前端登录、Cookie、Policy、Session、调用 WebAPI）。
- `WebAPI_Security/` — Web API 示例（JWT 签发与验证、Policy 授权）。
- `WebAPP/` — 另一个示例应用（参考）。

----------------

## 快速开始（本地开发）

先决条件

- .NET SDK 10 或更高（项目目标 `net10.0`）。

克隆并在开发模式下运行（示例端口以项目配置为准）：

```powershell
git clone <repo-url>
cd WebApp_Security
dotnet restore
# 启动 WebAPI
dotnet run --project WebAPI_Security --urls "https://localhost:7078"
# 在另一个终端启动 WebApp
dotnet run --project WebApp_Security --urls "https://localhost:7210"
```

请以 `launchSettings.json` 或运行输出的 URL 为准。

----------------

## 配置说明

- `SecretKey`（用于 `WebAPI_Security` 的对称签名 Key）：必须至少 32 字符，且不要将真实秘钥提交到仓库。开发时可使用 `appsettings.Development.json` 或 `dotnet user-secrets` 存储：

    ```powershell
    dotnet user-secrets set "SecretKey" "your_32+_char_secret_here" --project WebAPI_Security
    ```

- `Cookie`（在 `WebApp_Security` 中配置）：生产环境请设置 `Cookie.SecurePolicy = CookieSecurePolicy.Always`、适当的 `SameSite` 策略与 `HttpOnly` 为 true。

----------------

## 本地运行（开发流程）

1. 配置 `SecretKey`（见上）。
2. 启动 `WebAPI_Security`，再启动 `WebApp_Security`。
3. 打开浏览器访问前端，使用示例登录（或生成含示例 Claim 的 JWT），前端将调用 API 示范受保护资源访问。

----------------

## 示例请求（curl）

- 公共接口（需要先获取Token）：

```bash
curl https://localhost:7078/WeatherForecast
```

- 受保护接口（需要 Bearer Token）：

```bash
curl -H "Authorization: Bearer {your_jwt_token}" https://localhost:7078/WeatherForecast/secure
```

示例 Claims（JWT）

```json
{
  "Name": "admin",
  "Email": "admin@example.com",
  "Admin": "true"
}
```

生成 Token 可使用项目内工具、`JwtSecurityTokenHandler`、`Postman` 或线上工具（仅用于测试）。确保生成时使用与验证端一致的 `SecretKey`。

----------------

## 关键实现文件（参考）

- `WebApp_Security/Authorization/Credential.cs`
- `WebApp_Security/Authorization/JwtToken.cs`
- `WebApp_Security/Authorization/HRManagerProbationRequirement.cs`
- `WebAPI_Security/Controllers/WeatherForecastController.cs`
- 配置相关见各项目的 `Program.cs` 和 `appsettings*.json`

（在需要时可加上具体文件链接与行号，便于定位实现）

----------------

## 安全与生产建议（必须阅读）

- 绝不将 `SecretKey` 或任何敏感凭据提交到源码控制；生产环境请使用密钥管理服务（例如 Azure Key Vault、AWS Secrets Manager、HashiCorp Vault）。
- 在生产中强制 HTTPS，并为 Cookie 设置 `Secure`、`HttpOnly`、合适的 `SameSite`。
- SecretKey 最小长度建议 32 字符并定期轮换；尽量使用托管的密钥轮换方案。
- 设计合理的 JWT 过期与刷新策略，不要长期使用无限期 Token；`ClockSkew = TimeSpan.Zero` 会严格验证到期时间，注意客户端时间误差。
- 使用 `IHttpClientFactory` 管理 HTTP 客户端，配置超时与重试策略以防止资源耗尽。
- 对敏感操作和鉴权失败进行审计日志记录，但避免在日志中输出敏感信息（如完整 Token）。

----------------

## 贡献与许可证

- 欢迎 Fork 和 PR。建议将演示凭据替换为真实认证（Identity/外部登录）、将内存存储改为数据库（EF Core）、并完善输入校验与错误处理。
- 本仓库采用 MIT 许可证。

----------------

