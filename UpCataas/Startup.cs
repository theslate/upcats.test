using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using UpCataas.Models;

namespace UpCataas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Swagger origins
            services.AddCors(options =>
            {
                options.AddPolicy(name: "swagger",
                    builder =>
                    {
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowAnyOrigin();
                    });
            });

            services.AddSingleton<IImageTransformer, CachedImageTransformer>();
            services.AddSingleton<IImageTransformService, ImageTransformService>();
            services.AddSingleton<HttpClient, HttpClient>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<UserManager, UserManager>();
            services.AddControllers(options =>
                options.OutputFormatters.Insert(0, new ImageFormatter()));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("KnownUser", builder => builder.AddRequirements(new KnownUserRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, KnownUserHandler>();

            services.AddSwaggerGen(SwaggerGenConfig);
        }

        private void SwaggerGenConfig(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Straight up JWT Token. Remember to include \"Bearer\"",
            });

            
            options.AddSecurityDefinition("oauth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(MicrosoftAccountDefaults.AuthorizationEndpoint),
                        TokenUrl = new Uri(MicrosoftAccountDefaults.TokenEndpoint),
                        Scopes = new Dictionary<string, string>
                        {
                            { Configuration.GetValue<string>(@"AzureAD:Scopes"), "Cat transform"}
                        }
                    }
                },
                Description = "Get a JWT token from Microsoft"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth"
                        },
                    },
                    new string[] { }
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearer"
                        }
                    },
                    new string[] { }
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("swagger");
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UpCats");                    
                    c.OAuthClientId(Configuration["AzureAD:ClientId"]);
                    c.OAuthScopes("openid");
                    c.OAuthUsePkce();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class KnownUserRequirement : IAuthorizationRequirement
    {
    }
}
