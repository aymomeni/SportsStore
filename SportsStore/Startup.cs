using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SportsStore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SportsStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddTransient<IProductRepository, EFProductRepository>();
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp)); // related to shopping cart
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // related to shopping cart
            //services.AddTransient<IProductRepository, FakeProductRepository>(); // if a controller needs an implementation of the IProductRepository it should receive an instance of the FakeProductRepository class
            services.AddMvc();
            services.AddMemoryCache(); // for sessions (state of the session is stored in memory)
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage(); // details are shown when exception occurs during development phase
            app.UseStatusCodePages(); // adds imple message to HTTP responses
            app.UseStaticFiles(); // enables serving static files from the wwwroot folder
            app.UseSession(); // associates requests with sessions when http requests arrive from the client
            app.UseMvc(routes => {

                routes.MapRoute(
                    name:null,
                    template:"{category}/Page{productPage:int}",
                    defaults: new {controller="Product",action="List"}
                );

                routes.MapRoute(
                    name:null,
                    template:"Page{productPage:int}",
                    defaults: new {controller="Product", action="List", productPage=1}
                );

                routes.MapRoute(
                    name:null,
                    template:"{category}",
                    defaults: new {controller="Product", action="List", productPage=1}
                );

                routes.MapRoute(
                    name:null,
                    template:"",
                    defaults: new {controller="Product",action="List", productPage=1});

                routes.MapRoute(name: null, template: "{controller}/{action}/{id?}");
            });

            SeedData.EnsurePopulated(app);
        }
    }
}
