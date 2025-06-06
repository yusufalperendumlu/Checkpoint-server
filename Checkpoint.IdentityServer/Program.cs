using Checkpoint.IdentityServer;
using Checkpoint.IdentityServer.BackgroundJobs;
using Checkpoint.IdentityServer.Consumers;
using Checkpoint.IdentityServer.Data;
using Checkpoint.IdentityServer.Policies;
using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Shared;
using Shared.Hash;
using Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://localhost:7253",
            ValidAudience = "https://localhost:5000",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Hashing.Hash("checkpointsecret"))
        };
    });
builder.Services.AddServices();
builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthorization(configure =>
{
    configure.AddPolicy("Add", configureBuilder =>
    configureBuilder.AddRequirements(new AddRequirement()));
});



builder.Services.AddDbContext<IdentityDbContext>(y => y.UseNpgsql(builder.Configuration.GetConnectionString("checkpoint")));
builder.Services.AddMassTransit<IBus>(configure =>
{
    configure.AddConsumer<AnalysisNotAvgEventConsumer>();
    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetSection("AmqpConf")["Host"], config =>
        {
            config.Username(builder.Configuration.GetSection("AmqpConf")["Username"]!);
            config.Password(builder.Configuration.GetSection("AmqpConf")["Password"]!);

        });
        configurator.ReceiveEndpoint(QueueConfigurations.Checkpoint_Api_AnalysisNotAvgTime_Identity, cnf => cnf.ConfigureConsumer<AnalysisNotAvgEventConsumer>(context));
    });

});
builder.Services.AddHangfire(y => y.UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapScalarApiReference();


app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<RegisterOutboxJob>("registerOutbox", y => y.ExecuteJob(CancellationToken.None), "*/15 * * * * *");


app.MapOpenApi();


app.UseHttpsRedirection();


app.UseAuthentication();
app.UseMiddleware<AdminCheck>();
app.UseAuthorization();

app.MapControllers();

app.Run();
