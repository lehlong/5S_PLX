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


        [HttpGet("ThoiGianChamDiem")]
        public async Task<IActionResult> ThoiGianChamDiem([FromQuery] FilterReport filterReport)
        {
            var transferObject = new TransferObject();
            var result = await _service.ThoiGianChamDiem(filterReport);

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
        [HttpGet("ThietBiChamDiem")]
        public async Task<IActionResult> ThietBiChamDiem([FromQuery] FilterReport filterReport)
        {
            var transferObject = new TransferObject();
            var result = await _service.ThietBiChamDiem(filterReport);

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




        [HttpGet("TheoKhungThoiGian")]
        public async Task<IActionResult> TheoKhungThoiGian([FromQuery] FilterReport filterReport)
        {
            var transferObject = new TransferObject();
            var result = await _service.TheoKhungThoiGian(filterReport);

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


        [HttpGet("TongHopYKienDeXuat")]
        public async Task<IActionResult> TongHopYKienDeXuat([FromQuery] FilterReport filterReport)
        {
            var transferObject = new TransferObject();
            var result = await _service.TongHopYKienDeXuat(filterReport);

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


        [HttpGet("BaoCaoHinhAnh")]
        public async Task<IActionResult> BaoCaoHinhAnh([FromQuery] FilterReport filterReport)
        {
            var transferObject = new TransferObject();
            var result = await _service.BaoCaoHinhAnh(filterReport);

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


        [HttpPost("ExportExcel")]
        public async Task<IActionResult> ExportExcel([FromQuery] string ReportName, [FromBody] FilterReport filterReport)
        {
            var transferObject = new TransferObject();
            var result = await _service.ExportExcel(ReportName, filterReport);

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