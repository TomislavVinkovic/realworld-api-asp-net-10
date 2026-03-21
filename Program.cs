using System.Text;
using System.Text.Json.Serialization;
using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.Services;
using dotnet_api_tutorial.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using dotnet_api_tutorial.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add JWT authentication setup
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };

        // Add "Token token" support
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Token "))
                {
                    context.Token = authHeader["Token ".Length..].Trim();
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AutoValidationFilter>();
}).AddJsonOptions(options =>
    {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add database support
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IHttpContextService, HttpContextService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Learn more about configuring OpenAPI at 
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/openapi/v1.json", "API v1");
	});
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();

app.Run();
