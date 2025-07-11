using FluentValidation.AspNetCore;
using Htsoft.Uttt.Edi.Aplication.Interfaces;
using Htsoft.Uttt.Edi.Aplication.Interfaces.Loging;
using Htsoft.Uttt.Edi.Aplication.Interfaces.Services;
using Htsoft.Uttt.Edi.Aplication.Validators;
using Htsoft.Uttt.Edi.Infraestructura.Middleware;
using Htsoft.Uttt.Edi.Infraestructura.Mongo;
using Htsoft.Uttt.Edi.Infraestructura.Repositories;
using Htsoft.Uttt.Edi.Infraestructura.Services;
using Htsoft.Uttt.Edi.Infraestructura.Services.Logging;
using Microsoft.AspNetCore.Http.Features;
using NLog;

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

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = long.MaxValue; // Permite archivos grandes
    });

    builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));
    builder.Services.AddSingleton<MongoDbContext>();
    builder.Services.AddScoped<IEdiRepository, EdiRepository>();
    ///builder.Services.AddScoped<ExceptionHandlingMiddleware>();
    builder.Services.AddScoped(typeof(ILoggingService<>), typeof(LoggingService<>));
    builder.Services.AddScoped<IExcelUploadService, ExcelUploadService>();


    var app = builder.Build();
    var context = app.Services.GetRequiredService<MongoDbContext>();
    if (!context.IsConnectionOk())
    {
        logger.Error("No se pudo conectar a MongoDB.");
    }
    else
    {
        logger.Info("Conexi�n a MongoDB exitosa.");
    }
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseExceptionHandlingMiddleware();
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
