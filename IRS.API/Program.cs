
using System.Security.Claims;
using System.Text;
using System.Transactions;
using IRS.BLL.Managers.AccountManager.Auth;
using IRS.DAL.Database;
using IRS.DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SAFQA.BLL.Managers.AccountManager.Email_Sender;

namespace IRS.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<IRS_Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IRS_Context>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IAuthUser, AuthUser>();

            var jwtSettings = builder.Configuration.GetSection("JWT");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings["ValidIssuer"],
                    ValidAudience = jwtSettings["ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<UserManager<User>>();

                        var userId = context.Principal
                            .FindFirstValue(ClaimTypes.NameIdentifier);

                        if (string.IsNullOrEmpty(userId))
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);

                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var securityStamp = context.Principal
                            .FindFirstValue("SecurityStamp");

                        if (string.IsNullOrEmpty(securityStamp) ||
                            user.SecurityStamp != securityStamp)
                        {
                            context.Fail("Token expired due to security stamp change");
                        }
                    }
                };
            });

            builder.Services.AddDbContext<IRS_Context>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,         // √ﬁ’Ï ⁄œœ „Õ«Ê·«  ≈⁄«œ… «·« ’«·
                            maxRetryDelay: TimeSpan.FromSeconds(10), // Êﬁ  «·«‰ Ÿ«— »Ì‰ «·„Õ«Ê·« 
                            errorNumbersToAdd: null   // ·Ê ⁄«Ì“  Õœœ √—ﬁ«„ errors „⁄Ì‰…
                        );
                    }
                )
            );

            builder.Services.AddHostedService<ExpiredOtpCleanupService>();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();
            

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            async Task SeedRolesAsync(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "ADMIN", "CITIZEN", "AUTHORITY" };

                foreach (var roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }

            await SeedRolesAsync(app);

            await app.RunAsync();
        }
    }
}
