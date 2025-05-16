using Common;
using Microsoft.AspNetCore.Mvc;
using PLX5S.API.AppCode.Enum;
using PLX5S.BUSINESS.Dtos.BU;
//using PlX5S.BUSINESS.Services.BU;
using PLX5S.BUSINESS.Services.BU;
using PLX5S.API.AppCode.Extensions;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.API.Controllers.BU
{
    [Route("api/[controller]")]
    [ApiController]
    public class TieuChiController(ITieuChiService service) : ControllerBase
    {
        public readonly ITieuChiService _service = service;

        [HttpGet("BuildDataForTree")]
        public async Task<IActionResult> BuildDataForTree([FromQuery] string kiKhaoSatId)
        {
            var transferObject = new TransferObject();
            var result = await _service.BuildDataForTree(kiKhaoSatId);
            if (_service.Status)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0001", _service);
            }
            return Ok(transferObject);
        }

        [HttpGet("InsertTreeGroup")]
        public async Task<IActionResult> InsertTreeGroup([FromQuery] TblBuTieuChi data)
        {
            var transferObject = new TransferObject();
            await _service.InsertTreeGroup(data);
            if (_service.Status)
            {
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0001", _service);
            }
            return Ok(transferObject);
        }

        [HttpGet("InsertTreeLeaves")]
        public async Task<IActionResult> InsertTreeLeaves([FromQuery] TblBuTieuChi data)
        {
            var transferObject = new TransferObject();
            await _service.InsertTreeLeaves(data);
            if (_service.Status)
            {
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
