using Common;
using Microsoft.AspNetCore.Mvc;
using PLX5S.API.AppCode.Enum;
using PLX5S.BUSINESS.Dtos.BU;
//using PlX5S.BUSINESS.Services.BU;
using PLX5S.BUSINESS.Services.BU;
using PLX5S.API.AppCode.Extensions;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE.Entities.BU;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PLX5S.API.Controllers.BU
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppReportController(IAppReportService service) : ControllerBase
    {
        public readonly IAppReportService _service = service;
        
        [HttpGet("KetQuaChamDiem")]
        public async Task<IActionResult> KetQuaChamDiem([FromQuery] FilterReport filterReport)
        {
            var transferObject = new TransferObject();
            var result = await _service.KetQuaChamDiem(filterReport);

            if (_service.Status)
            {
                transferObject.Data = result;
                transferObject.Status = true;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0001", _service);
            }
            return Ok(transferObject);
        }


    }
}