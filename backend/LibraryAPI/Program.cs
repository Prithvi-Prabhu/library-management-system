using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

// OData model
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Book>("Books");
modelBuilder.EntitySet<Member>("Members");
modelBuilder.EntitySet<Loan>("Loans");

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    })
    .AddOData(opt =>
        opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
           .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.MapControllers();
app.Run();