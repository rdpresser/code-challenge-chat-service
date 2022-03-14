using Jobsity.CodeChallenge.Chat.UI.Configurations.Extensions;
using Jobsity.CodeChallenge.Chat.UI.Data;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.Hubs;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jobsity.CodeChallenge.Chat.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //https://andrewlock.net/creating-custom-password-validators-for-asp-net-core-identity-2/#:~:text=Passwords%20must%20be%20at%20least,0%27%2D%279%27)
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));

            ConfigureAppProviders(services);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDbChat"));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();

            // ASP.NET HttpContext dependency
            services.AddHttpContextAccessor();
            // NET core http client factory
            services.AddHttpClient();

            services.RegisterServices();

            ConfigureMQServices(services);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub"); // Add the ChatHub to the configuration.
            });
        }

        private static void ConfigureMQServices(IServiceCollection services)
        {
            services.AddHostedService<RabbitMqEventConsumer<MqErrorSettingsProvider>>();
            services.AddHostedService<RabbitMqEventConsumer<MqChatClientSettingsProvider>>();
        }

        private void ConfigureAppProviders(IServiceCollection services)
        {
            services.Configure<MbHostSettingsProvider>(Configuration.GetSection("RabbitSettings"));
            services.Configure<MqErrorSettingsProvider>(x => x.GetSettingsProvider(Configuration, "RabbitServiceQueues"));
            services.Configure<MqChatClientSettingsProvider>(x => x.GetSettingsProvider(Configuration, "RabbitServiceQueues"));
        }
    }
}
