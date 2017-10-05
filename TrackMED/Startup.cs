using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;

using System.Net;

//using TrackMED.Data;
using TrackMED.Models;
using TrackMED.Services;
using WilderBlog.Logger;
using WilderBlog.Services;

namespace TrackMED
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            
            EmailConnectionInfo info = new EmailConnectionInfo()
            {
                EmailSubject = "Test Serilog",
                ToEmail = "jul_soriano@yahoo.com",

                /*
                FromEmail = "elijahpne@gmail.com",
                MailServer = "smtp.gmail.com", // "smtp.live.com"
                NetworkCredentials = new NetworkCredential("elijahpne@gmail.com", "Proverb16:6"),
                Port = 587,
                */

                FromEmail = "jul_soriano@hotmail.com",
                MailServer = "smtp.live.com", // "smtp.live.com"
                NetworkCredentials = new NetworkCredential("jul_soriano@hotmail.com", "acts15:23hot"),
                Port = 587, // 465
            };
            

            // Or This: See https://github.com/serilog/serilog/wiki/Getting-Started
            var uriMongoDB = Configuration.GetValue<string>("MongoSettings:mongoconnection");

            // http://sourcebrowser.io/Browse/serilog/serilog/src/Serilog.Sinks.Email/LoggerConfigurationEmailExtensions.cs
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                //.WriteTo.LiterateConsole()
                //.WriteTo.RollingFile("logs\\trackmed-{Date}.txt")
                .WriteTo.Email(info, restrictedToMinimumLevel: LogEventLevel.Warning)   // https://github.com/serilog/serilog/wiki/Configuration-Basics#overriding-per-sink
                //.WriteTo.Seq("http://localhost:5341/")
                .WriteTo.MongoDBCapped(uriMongoDB, collectionName: "logsTrackMED")  // https://github.com/serilog/serilog-sinks-mongodb
                .CreateLogger();

            Log.Warning("Houston we have a problem");
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Add our Config object so it can be injected; needs "Microsoft.Extensions.Options.ConfigurationExtensions": "1.0.0"
            // See http://stackoverflow.com/questions/36893326/read-a-value-from-appsettings-json-in-1-0-0-rc1-final
            services.Configure<Settings>(Configuration.GetSection("MongoSettings"));

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add model services
            services.AddSingleton<IEntityService<ActivityType>, EntityService<ActivityType>>();
            services.AddSingleton<IEntityService<Category>, EntityService<Category>>();
            services.AddSingleton<IEntityService<Classification>, EntityService<Classification>>();
            services.AddSingleton<IEntityService<Component>, EntityService<Component>>();
            services.AddSingleton<IEntityService<Deployment>, EntityService<Deployment>>();
            services.AddSingleton<IEntityService<Description>, EntityService<Description>>();
            services.AddSingleton<IEntityService<EquipmentActivity>, EntityService<EquipmentActivity>>();
            services.AddSingleton<IEntityService<Event>, EntityService<Event>>();
            services.AddSingleton<IEntityService<Location>, EntityService<Location>>();
            services.AddSingleton<IEntityService<Manufacturer>, EntityService<Manufacturer>>();
            services.AddSingleton<IEntityService<Model>, EntityService<Model>>();
            services.AddSingleton<IEntityService<Model_Manufacturer>, EntityService<Model_Manufacturer>>();
            services.AddSingleton<IEntityService<Owner>, EntityService<Owner>>();
            services.AddSingleton<IEntityService<ProviderOfService>, EntityService<ProviderOfService>>();
            services.AddSingleton<IEntityService<Status>, EntityService<Status>>();
            services.AddSingleton<IEntityService<SystemsDescription>, EntityService<SystemsDescription>>();
            services.AddSingleton<IEntityService<SystemTab>, EntityService<SystemTab>>();
            
            /*
            var builder = new ContainerBuilder();
            builder.RegisterType<EntityService<Category>>()
                 .As<IEntityService<Category>>()
                 .InstancePerDependency();
            builder.RegisterType<EntityService<Classification>>()
                 .As<IEntityService<Classification>>()
                 .InstancePerDependency();
            builder.RegisterType<EntityService<Component>>()
                .As<IEntityService<Component>>()
                .InstancePerDependency();
            builder.RegisterType<EntityService<Description>>()
                .As<IEntityService<Description>>()
                .InstancePerDependency();
            builder.RegisterType<EntityService<Event>>()
                .As<IEntityService<Event>>()
                .InstancePerDependency();
            builder.RegisterType<EntityService<Location>>()
                 .As<IEntityService<Location>>()
                 .InstancePerDependency();
            builder.RegisterType<EntityService<Manufacturer>>()
                 .As<IEntityService<Manufacturer>>()
                 .InstancePerDependency();
            builder.RegisterType<EntityService<Model>>()
                 .As<IEntityService<Model>>()
                 .InstancePerDependency();
            builder.RegisterType<EntityService<Model_Manufacturer>>()
                 .As<IEntityService<Model_Manufacturer>>()
                 .InstancePerDependency();
            builder.RegisterType<EntityService<Owner>>()
                 .As<IEntityService<Owner>>()
                 .InstancePerDependency();
            //builder.RegisterType<EntityService<ServiceProvider>>()
            //     .As<IEntityService<ServiceProvider>>()
            //     .InstancePerDependency();
            builder.RegisterType<EntityService<Status>>()
                 .As<IEntityService<Status>>()
                 .InstancePerDependency();
            builder.RegisterType<EntityService<SystemTab>>()
                .As<IEntityService<SystemTab>>()
                .InstancePerDependency();
            builder.RegisterType<EntityService<SystemsDescription>>()
                .As<IEntityService<SystemsDescription>>()
                .InstancePerDependency();
            */
            //builder.Populate(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //loggerFactory.AddEmail(, LogLevel.Critical);

            // See https://www.towfeek.se/2016/06/structured-logging-with-aspnet-core-using-serilog-and-seq/
            loggerFactory.AddSerilog();     // requires Serilog.Extensions,Logging

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
           
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
