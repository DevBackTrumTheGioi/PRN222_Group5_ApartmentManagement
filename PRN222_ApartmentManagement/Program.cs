using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Repositories.Implementations;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;
using PRN222_ApartmentManagement.Services.Implementations;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/ForgotPassword");
    options.Conventions.AllowAnonymousToPage("/Account/Inactive");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
    options.Conventions.AllowAnonymousToPage("/Account/VerifyPhone");
    options.Conventions.AllowAnonymousToPage("/Admin/SeedData");
    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
    options.Conventions.AuthorizeFolder("/BQL_Manager", "BQLManagerOnly");
    options.Conventions.AuthorizeFolder("/BQL_Staff", "BQLStaffOnly");
    options.Conventions.AuthorizeFolder("/Resident", "ResidentOnly");
    options.Conventions.AuthorizeFolder("/BQT_Head", "BQTHeadOnly");
    options.Conventions.AuthorizeFolder("/BQT_Member", "BQTMemberOnly");
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("BQLManagerOnly", policy => policy.RequireRole("BQL_Manager"));
    options.AddPolicy("BQLStaffOnly", policy => policy.RequireRole("BQL_Staff"));
    options.AddPolicy("ResidentOnly", policy => policy.RequireRole("Resident"));
    options.AddPolicy("BQTHeadOnly", policy => policy.RequireRole("BQT_Head"));
    options.AddPolicy("BQTMemberOnly", policy => policy.RequireRole("BQT_Member"));
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = jwtSettings["Issuer"],
    ValidateAudience = true,
    ValidAudience = jwtSettings["Audience"],
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = tokenValidationParameters;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.HttpContext.Items.TryGetValue(AuthCookieHelper.AccessTokenCookieName, out var refreshedToken) &&
                    refreshedToken is string accessTokenFromRefresh &&
                    !string.IsNullOrWhiteSpace(accessTokenFromRefresh))
                {
                    context.Token = accessTokenFromRefresh;
                    return Task.CompletedTask;
                }

                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrWhiteSpace(authHeader) &&
                    authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader["Bearer ".Length..].Trim();
                    return Task.CompletedTask;
                }

                var accessTokenCookie = context.Request.Cookies[AuthCookieHelper.AccessTokenCookieName];
                if (!string.IsNullOrWhiteSpace(accessTokenCookie))
                {
                    context.Token = accessTokenCookie;
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    context.Fail("Invalid token payload.");
                    return;
                }

                var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApartmentDbContext>();
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
                if (user == null || !user.IsActive)
                {
                    context.Fail("User is inactive.");
                }
            },
            OnChallenge = context =>
            {
                if (IsBrowserPageRequest(context.Request))
                {
                    context.HandleResponse();
                    AuthCookieHelper.ClearAuthCookies(context.HttpContext);

                    var returnUrl = context.Request.Path + context.Request.QueryString;
                    var loginUrl = string.IsNullOrWhiteSpace(returnUrl)
                        ? "/Account/Login"
                        : $"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";

                    context.Response.Redirect(loginUrl);
                }

                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                if (IsBrowserPageRequest(context.Request))
                {
                    context.Response.Redirect("/Account/AccessDenied");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApartmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IFaceAuthService, FaceAuthService>();
builder.Services.AddScoped<IInvoiceManagementService, InvoiceManagementService>();
builder.Services.AddScoped<IPaymentManagementService, PaymentManagementService>();
builder.Services.AddScoped<IFinancialReportService, FinancialReportService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddHttpClient();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
builder.Services.AddScoped<IAmenityBookingRepository, AmenityBookingRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IApartmentServiceRepository, ApartmentServiceRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractMemberRepository, ContractMemberRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceDetailRepository, InvoiceDetailRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRequestAttachmentRepository, RequestAttachmentRepository>();
builder.Services.AddScoped<IResidentCardRepository, ResidentCardRepository>();
builder.Services.AddScoped<IResidentApartmentRepository, ResidentApartmentRepository>();
builder.Services.AddScoped<IServicePriceRepository, ServicePriceRepository>();
builder.Services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVisitorRepository, VisitorRepository>();
builder.Services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IResidentCardService, ResidentCardService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApartmentDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    await DbInitializer.InitializeAsync(context, logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseMiddleware<JwtCookieRefreshMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await app.RunAsync();

static bool IsBrowserPageRequest(HttpRequest request)
{
    if (request.Headers.ContainsKey("Authorization"))
    {
        return false;
    }

    if (request.Headers.TryGetValue("X-Requested-With", out var requestedWith) &&
        string.Equals(requestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    var acceptHeader = request.Headers.Accept.ToString();
    return string.IsNullOrWhiteSpace(acceptHeader) ||
           acceptHeader.Contains("text/html", StringComparison.OrdinalIgnoreCase) ||
           acceptHeader.Contains("*/*", StringComparison.OrdinalIgnoreCase);
}
