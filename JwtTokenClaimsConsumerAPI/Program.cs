using JWTClaimsExtractor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtClaimsExtractorServices(builder.Configuration);
var app = builder.Build();

app.UseJwtExceptionMiddleware();
app.UseExceptionHandler("/error");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (builder.Configuration["TEST_MODE"] == "JWT_BEARER")
{
    app.UseAuthentication();
}
if (builder.Configuration["TEST_MODE"] == "JWT_EXCEPTION_MIDDLEWARE")
{
    app.UseJwtTokenClaimsMiddleware();
}
app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();