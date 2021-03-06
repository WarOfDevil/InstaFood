using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InstaFood.DataAccess;
using InstaFood.DataAccess.Data.Repository.IRepository;
using InstaFood.DataAccess.Data.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using InstaFood.Utility;
using System;
using Stripe;
using InstaFood.DataAccess.Data.Initializer;

namespace InstaFood.WebUI
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
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("InstaFoodConnectionString")));

			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddDefaultTokenProviders()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddSingleton<IEmailSender, EmailSender>();

			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IDbInitializer, DbInitializer>();

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(10);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

			//services.AddMvc(options => options.EnableEndpointRouting = false)
			//	.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
			services.AddRazorPages();

			services.AddControllersWithViews().AddRazorRuntimeCompilation();

			services.AddAuthentication().AddFacebook(facebookOptions =>
			{
				facebookOptions.AppId = Configuration.GetSection("FacebookApp")["AppId"];
				facebookOptions.AppSecret = Configuration.GetSection("FacebookApp")["AppSecret"];
			});

			services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
			{
				microsoftOptions.ClientId = Configuration.GetSection("MicrosoftApp")["ClientId"];
				microsoftOptions.ClientSecret = Configuration.GetSection("MicrosoftApp")["SecretId"];
			});

			services.ConfigureApplicationCookie(options =>
			{

				options.LoginPath = $"/Identity/Account/Login";

				options.LogoutPath = $"/Identity/Account/Logout";

				options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSession();

			dbInitializer.Initialize();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});
			//app.UseMvc();

			StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
		}
	}
}
