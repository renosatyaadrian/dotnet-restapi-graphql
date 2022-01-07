using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using TwittorAPI.GraphQL;
using TwittorAPI.Models;

namespace TwittorAPI
{
    public class Startup
    {
        public IConfiguration _config { get; }

        private IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = _config.GetConnectionString("LocalSQLEdge");
            // services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=conferences.db"));
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));
            
            services
                .AddGraphQLServer()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = _env.IsDevelopment())
                .AddQueryType<Query>()
                .AddMutationType<Mutation>();
                // .AddAuthorization();

            services.Configure<TokenSettings>(_config.GetSection("TokenSettings"));
            services.Configure<KafkaSettings>(_config.GetSection("KafkaSettings"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = _config.GetSection("TokenSettings").GetValue<string>("Issuer"),
                        ValidateIssuer = true,
                        ValidAudience = _config.GetSection("TokenSettings").GetValue<string>("Audience"),
                        ValidateAudience = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("TokenSettings").GetValue<string>("Key"))),
                        ValidateIssuerSigningKey = true
                    };
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
