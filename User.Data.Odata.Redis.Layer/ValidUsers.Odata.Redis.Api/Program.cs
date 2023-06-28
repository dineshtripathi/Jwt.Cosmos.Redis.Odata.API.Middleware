using JwtValidUsersOdataRedis.Api.Model;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace JwtValidUsersOdataRedis.Api
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services
                .AddControllers()
                .AddOData(opt => opt.Count()
                    .Filter()
                    .Expand()
                    .Select()
                    .OrderBy()
                    .SetMaxTop(null)
                    .AddRouteComponents("odata", GetEdmModel()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddODataQueryFilter();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

          //  app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.Run();
        }
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<ValidUser>("ValidUsers");
            builder.EntitySet<Department>("Departments");
            builder.EntitySet<Location>("Locations");
            return builder.GetEdmModel();
        }

    }
}