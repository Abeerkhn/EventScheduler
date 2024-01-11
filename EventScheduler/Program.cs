using EventScheduler;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using EventScheduler.RabbitMQ;
//using Microsoft.OpenApi.Models;
using EventScheduler.Services;
using MediatR;
using System.Text;
using System.Text.Json.Serialization;
using System.Reflection;
using EventScheduler.Services;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using EventScheduler.Data;
using Hangfire;
using Hangfire.PostgreSql;
using EventScheduler.RabbitMQ;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
/*builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<EventContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<UserJoinEventContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
*/
builder.Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostGreConnection")));

builder.Services.AddDbContext<EventContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostGreConnection")));

builder.Services.AddDbContext<UserJoinEventContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostGreConnection")));

builder.Services.AddScoped<IEventRepository,EventRepository>(provider => {
    var dbContext = provider.GetRequiredService<EventContext>();
    var userJoinEventDbContext = provider.GetRequiredService<UserJoinEventContext>();
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var userContext = provider.GetRequiredService<UserContext>();
    var rabbitMQService = provider.GetRequiredService<RabbitMQ_Service>();
    var serviceProvider = provider.GetRequiredService<IServiceProvider>();

    return new EventRepository(dbContext, userJoinEventDbContext, httpClientFactory, userContext, rabbitMQService,serviceProvider);
});
builder.Services.AddScoped<RabbitMQ_Service>();
builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });

/*builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
   //        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
       });
*/

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my secret top key")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
 builder.Services.AddAutoMapper(typeof(Program));
// Configure authorization
builder.Services.AddAuthorization();
// Mediator
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddHttpClient();


// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});
// HangFire Configurations
builder.Services.AddHangfire(config =>
{
    //   config.UseSqlServerStorage(builder.Configuration.GetConnectionString("PostGreConnection"));
    //config.UseNpgsqlStorage(builder.Configuration.GetConnectionString("PostGreConnection"));
    config.UseStorage(new PostgreSqlStorage(builder.Configuration.GetConnectionString("PostGreConnection")));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Title");
        options.RoutePrefix = "swagger";
    });
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(builder =>
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseHangfireDashboard();
//app.UseRabbitMQDashboard();
app.UseHangfireServer();


RecurringJob.AddOrUpdate<EventRepository>(repo => repo.ExecuteGetNearEventsJobWrapper(), Cron.Minutely);



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
