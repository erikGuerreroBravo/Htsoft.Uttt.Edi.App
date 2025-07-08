using NLog;
using NLog.Web;
using NLog;
using FluentValidation.AspNetCore;
using Htsoft.Uttt.Edi.Aplication.Validators;
using Htsoft.Uttt.Edi.Infraestructura.Mongo;


var logger = LogManager.Setup()
                       .LoadConfigurationFromFile("nlog.config")
                       .GetCurrentClassLogger();

try
{
    logger.Info("Start la aplicacion");
    var builder = WebApplication.CreateBuilder(args);
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddControllers()
     .AddFluentValidation(fv =>
     {
         fv.RegisterValidatorsFromAssemblyContaining<EdiModelValidator>();
     });

    builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

    builder.Services.AddSingleton<MongoDbContext>();

    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Se detuvo la aplicacion durante la ejecucion.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
