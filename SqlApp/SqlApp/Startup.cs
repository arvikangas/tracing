using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Bson;
using OpenTelemetry.Exporter.Prometheus;
using OpenTelemetry.Trace;
using SqlApp.Database;

namespace SqlApp
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
            services.AddDbContext<SqlDbContext>();

            services.AddOpenTelemetryTracing(
            (builder) =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetTextCommandContent = true;
                        options.Enrich = SqlEnrich;
                    })
                    .AddJaegerExporter();

                builder.SetSampler(new MySampler());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SqlDbContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            dataContext.Database.Migrate();

        }

        void SqlEnrich(Activity activity, string s, object o)
        {
            var command = o as SqlCommand;
            if(command is null)
            {
                return;
            }

            if(command.Parameters.Count == 0)
            {
                return;
            }

            var counter = 0;
            foreach (SqlParameter parameter in command.Parameters)
            {
                activity.AddTag($"db.parameter[{counter}].name", parameter.ParameterName);
                activity.AddTag($"db.parameter[{counter}].value", parameter.SqlValue.ToString());
                counter++;
            }
        }
    }
}
