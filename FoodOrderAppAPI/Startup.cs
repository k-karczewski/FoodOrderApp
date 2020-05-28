using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodOrderApp.Data.DataContext;
using FoodOrderApp.Data.Repositories;
using FoodOrderApp.Data.UnitOfWork;
using FoodOrderApp.Interfaces.Repositories;
using FoodOrderApp.Interfaces.Services;
using FoodOrderApp.Interfaces.UnitOfWork;
using FoodOrderApp.Models.PizzaModels;
using FoodOrderApp.Models.PizzaModels.PriceModels;
using FoodOrderApp.Models.UserModels;
using FoodOrderApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FoodOrderAppAPI
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
            services.AddControllers().AddNewtonsoftJson(options =>
                                                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddDbContext<FoodOrderContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IFoodOrderRepository<>), typeof(FoodOrderRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPizzaService, PizzaService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<ISignUpService, SignUpService>();

            IdentityBuilder identityBuilder = services.AddIdentityCore<UserModel>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole<int>), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<FoodOrderContext>();
            identityBuilder.AddUserManager<UserManager<UserModel>>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
