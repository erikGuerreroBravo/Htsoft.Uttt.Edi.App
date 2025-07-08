using Htsoft.Uttt.Edi.Aplication.Interfaces;
using Htsoft.Uttt.Edi.Aplication.Interfaces.Services;
using Htsoft.Uttt.Edi.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace Htsoft.Uttt.Edi.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {
        private readonly IExcelUploadService _service;
        private readonly IEdiRepository _repository;

        public ExcelUploadController(IExcelUploadService service, IEdiRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var data = await _service.LoadExcelAsync(file);
            await _repository.BulkInsertAsync(data.Select(dto => dto.ToEntity()));
            return Ok(new { message = "Carga exitosa", total = data.Count });
        }
    }
}
