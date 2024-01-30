using Amazon.Runtime.Internal.Transform;
using FlightPlanApi.Auth;
using FlightPlanApi.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(setUpaction =>
{
    setUpaction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
    setUpaction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
    setUpaction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
    setUpaction.Filters.Add(new AuthorizeFilter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddSwaggerGen(setUpAction =>
{
    setUpAction.SwaggerDoc("flightplan", new()
    {
        Title = "Flight Plan Api",
        Version = "v3",
        Description = "A description of the api that provides a flight plans info and route to update, create, and delete them"
    });

    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFullPathFile = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);

    setUpAction.IncludeXmlComments(xmlFullPathFile);

    //Authentication setUp
    setUpAction.AddSecurityDefinition("BasicAuthentication", new()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "basic authentication to protect the api -not the best approach-",
        In = ParameterLocation.Header
    });

    setUpAction.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BasicAuthentication"
                }
            },
            new List<string>()
        }
    });

});
builder.Services.AddScoped<IDatabaseAdapter, MongoDbDatabase>();
builder.Services.AddAuthentication("BasicAuthentication").AddScheme
    <AuthenticationSchemeOptions, BasicAuthenticationHandler>
    ("BasicAuthentication", null);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/flightplan/swagger.json", "Flight Plan API");
    });
    app.UseCors("DevelopmentPolicy");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
