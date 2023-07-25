
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // see ChangeFieldValueBeforeReachingController
    options.ModelBinderProviders.Insert(0, new StackOverflow.Answers.AspNet.WebApi.ChangeFieldValueBeforeReachingController.ChangeFieldValueModelBinderProvider());
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
