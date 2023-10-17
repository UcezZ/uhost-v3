using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Uhost.Core;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Middleware;
using Uhost.Core.Providers;
using Uhost.Core.Services.FileService;
using Uhost.Core.Services.Graylog;
using Uhost.Core.Services.HangfireScheduler;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.RestClient;
using Uhost.Core.Services.Role;
using Uhost.Core.Services.User;
using Uhost.Web.Filters;
using Uhost.Web.Middleware;
using Uhost.Web.Services.Auth;

namespace Uhost.Web
{
    /// <summary>
    /// Конфигурация приложения
    /// </summary>
    public class Startup
    {
        /// <inheritdoc cref="Startup"/>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <inheritdoc cref="IConfiguration"/>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Services configuration
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddControllers();// options => options.EnableEndpointRouting = false);

            services.AddHangfire(e => e.UsePostgreSqlStorage(CoreSettings.SqlConnectionString));

            services.AddDbContext<PostgreSqlDbContext>(e => e.UseNpgsql(CoreSettings.SqlConnectionString));
            services.AddDbContext<PostgreSqlLogDbContext>(e => e.UseNpgsql(CoreSettings.SqlLogConnectionString));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRestClientService, RestClientService>();
            services.AddScoped<IGraylogService, GraylogService>();

            services.AddSingleton<IHangfireSchedulerService, HangfireSchedulerService>();

            // for NLog
            services.AddHttpContextAccessor();

            // Кодировка
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Проверка прав
            services.AddScoped<IAuthorizationHandler, RightAuthHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, HasRightPolicyProvider>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultTransformer>();

            // Redis
            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(CoreSettings.RedisConfig));
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(CoreSettings.RedisConfig);

            if (LocalEnvironment.IsDev)
            {
                // Swagger
                services.AddSwaggerGen(options =>
                {
                    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    options.SwaggerDoc("v2", new OpenApiInfo
                    {
                        Version = "v2",
                        Title = "uHost v3 API",
                        Description = "uHost v3 API"
                    });

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                    });

                    options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme()
                                {
                                    Reference = new OpenApiReference()
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                new List<string>()
                            }
                        }
                    );

                    var webXml = Path.Combine(AppContext.BaseDirectory, $"{typeof(WebSettings).Assembly.GetName().Name}.xml");
                    var coreXml = Path.Combine(AppContext.BaseDirectory, $"{typeof(CoreSettings).Assembly.GetName().Name}.xml");
                    options.IncludeXmlComments(webXml, includeControllerXmlComments: true);
                    options.IncludeXmlComments(coreXml);
                    options.OperationFilter<IgnorePropertyFilter>();
                    options.SchemaFilter<SwaggerIgnoreFilter>();
                    options.OperationFilter<SwaggerFileUploadFilter>();
                });
            }

            // JWT Bearer
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new DummyTokenHandler());
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = WebSettings.Jwt.Issuer,
                    ValidAudience = WebSettings.Jwt.Audience,
                    IssuerSigningKey = WebSettings.Jwt.SecurityKey
                };
            });
        }

        /// <summary>
        /// App configuration
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.AllowCredentials();
                    options.SetIsOriginAllowed(e => true);
                });

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Uhost v3");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseExceptionHandler("/error");
            app.UseFileServer();
            app.UseRouting();

            app.UseMiddleware<ThrottleMiddleware>();
            app.UseMiddleware<SentryLegacyMiddleware>();

            app.UseHangfireDashboard("/hangfire", WebSettings.HangfireDashboardOptions);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(e => e.MapDefaultControllerRoute());

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
