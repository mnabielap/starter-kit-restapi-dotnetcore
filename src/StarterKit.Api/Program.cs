using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StarterKit.Api.Extensions;
using StarterKit.Api.Middleware;
using StarterKit.Application;
using StarterKit.Infrastructure;
using StarterKit.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Configuration
// (appsettings.json is loaded automatically)

// 2. Register Layers (Dependency Injection)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Register API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation(); 

// 4. Configure Authentication (JWT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["Jwt:Secret"];
    var key = Encoding.UTF8.GetBytes(secretKey!);

    options.MapInboundClaims = false; 

    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, 
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        
        NameClaimType = "sub",
        RoleClaimType = "role"
    };
});

var app = builder.Build();

// 5. Database Migration & Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); 
        Console.WriteLine("--- Database Migration Successful ---");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("--- An error occurred while migrating the database ---");
        Console.Error.WriteLine(ex.Message);
    }
}

// 6. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();