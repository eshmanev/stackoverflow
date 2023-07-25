using WebApplication2.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // see ChangeFieldValueBeforeReachingController
    options.ModelBinderProviders.Insert(0, new ChangeFieldValueModelBinderProvider());
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
