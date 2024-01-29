using FlightPlanApi.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(setUpaction =>
{
    setUpaction.ReturnHttpNotAcceptable = true;
    setUpaction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
    setUpaction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
    setUpaction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDatabaseAdapter, MongoDbDatabase>();

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

app.Run();
