using JWT.Bearer.Claims.Auth.Extractor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtClaimsExtractorServices(builder.Configuration);
var app = builder.Build();
//app.UseExceptionHandler("/error");

// Configure the HTTP request pipeline.

app.UseRouting();

if (builder.Configuration["TEST_MODE"] == "JWT_BEARER")
{
    app.UseAuthentication();
}

//app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
  //  app.UseSwagger();
 //   app.UseSwaggerUI();
}
if (builder.Configuration["TEST_MODE"] == "JWT_EXCEPTION_MIDDLEWARE")
{
   // app.UseJwtTokenClaimsMiddleware();
}
app.UseHttpsRedirection();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();