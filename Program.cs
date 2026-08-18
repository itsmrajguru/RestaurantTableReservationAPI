using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantTableReservationAPI.Data;
using RestaurantTableReservationAPI.Filters;
using RestaurantTableReservationAPI.Repositories;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services;
using RestaurantTableReservationAPI.Services.Interfaces;

//the app builder registers services into the DI
var builder = WebApplication.CreateBuilder(args);

//app builder says to the asp.NET core, that i'm using these controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories for Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IRestaurantConfigRepository, RestaurantConfigRepository>();
builder.Services.AddScoped<IOperatingHoursRepository, OperatingHoursRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
builder.Services.AddScoped<IRestaurantConfigService, RestaurantConfigService>();
builder.Services.AddScoped<IOperatingHoursService, OperatingHoursService>();
builder.Services.AddScoped<RestaurantTableReservationAPI.Services.Validation.IReservationValidationEngine, RestaurantTableReservationAPI.Services.Validation.ReservationValidationEngine>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure Authentication & JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Restaurant Table Reservation API",
        Version = "v1",
        Description = "REST API Developed by Mangesh❤️"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name="Authorization",
        Type=Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme="Bearer",
        BearerFormat="JWT",
        In=Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description="**Authorization Configuration:**\n\nPlease enter ONLY your JWT token below. You no longer need to type the word 'Bearer'.\n\nDepending on what you are testing, use an **Admin** token or a **Customer** token."
    });

    // Add our custom operation filter to show roles in Swagger UI
    options.OperationFilter<SwaggerRoleOperationFilter>();

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference=new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type=Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//this actually builds the app
var app = builder.Build();

// Run DB migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    //enable swagger as raw json
    app.UseSwagger();
    //enable swagger ui page
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Table Reservation API v1");
        options.RoutePrefix = string.Empty;
    });
}

//if the app is not running on localhost, redirect to https
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//connect http routes to controllers
app.MapControllers();

app.Run();
