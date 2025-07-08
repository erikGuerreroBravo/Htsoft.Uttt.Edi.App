using Htsoft.Uttt.Edi.Aplication.Interfaces;
using Htsoft.Uttt.Edi.Aplication.Interfaces.Services;
using Htsoft.Uttt.Edi.Domain;
using Htsoft.Uttt.Edi.Infraestructura.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace Htsoft.Uttt.Edi.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {
        
        private readonly IEdiRepository? _repository;
        private readonly IExcelUploadService _excelUploadService;

        public ExcelUploadController(IExcelUploadService excelUploadService, IEdiRepository? repository)
        {
            _excelUploadService = excelUploadService;
            _repository = repository;
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadArchivoAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo Excel inválido o vacío.");

            try
            {
                var ediModels = await _excelUploadService.ReadExcelAsync(file, cancellationToken);

                if (ediModels == null || !ediModels.Any())
                    return BadRequest("No se encontraron registros válidos en el archivo.");

                // Aquí podrías insertar en MongoDB, si lo deseas:
                // foreach (var item in ediModels)
                //     await _ediRepository.InsertAsync(item, cancellationToken);

                return Ok(new
                {
                    Message = "Carga de datos exitosa",
                    Count = ediModels.Count,
                    Items = ediModels
                });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "La operación fue cancelada por el cliente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error procesando el archivo: {ex.Message}");
            }
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcelAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo Excel inválido o vacío.");

            try
            {
                var ediModels = await _excelUploadService.ReadExcelAsync(file, cancellationToken);

                if (ediModels == null || !ediModels.Any())
                    return BadRequest("No se encontraron registros válidos en el archivo.");

                int registrosInsertados = 0;
                int maxConcurrency = 10; // Puedes ajustar a 5, 10, 20 según recursos
                var semaphore = new SemaphoreSlim(maxConcurrency);
                var tasks = new List<Task>();

                foreach (var entity in ediModels)
                {

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    await semaphore.WaitAsync(cancellationToken);

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await _repository!.InsertAsync(entity);
                            Interlocked.Increment(ref registrosInsertados); // Thread-safe counter
                        }
                        catch (Exception ex)
                        {
                            // Aquí puedes loggear el error por entidad si quieres
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellationToken);

                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);

                return Ok(new
                {
                    Message = "Carga y guardado exitoso.",
                    Total = ediModels.Count,
                    Insertados = registrosInsertados,
                    Fallidos = ediModels.Count - registrosInsertados
                });
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "La operación fue cancelada por el cliente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error general al procesar el archivo: {ex.Message}");
            }
        }
    }
}
