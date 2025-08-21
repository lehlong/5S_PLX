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

        [HttpPost("InsertTreeGroup")]
        public async Task<IActionResult> InsertTreeGroup([FromBody] TblBuTieuChi data)
        {
            var transferObject = new TransferObject();
            await _service.InsertTreeGroup(data);
            if (_service.Status)
            {
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

        [HttpPost("InsertTreeLeaves")]
        public async Task<IActionResult> InsertTreeLeaves([FromBody] TieuChiDto data)
        {
            var transferObject = new TransferObject();
            await _service.InsertTreeLeaves(data);
            if (_service.Status)
            {
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

        [HttpGet("GetLeaves")]
        public async Task<IActionResult> GetLeaves([FromQuery] string pId, string kiKhaoSatId)
        {
            var transferObject = new TransferObject();
            var result = _service.getLeaves(pId, kiKhaoSatId);
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
        [HttpGet("CheckLeaves")]
        public async Task<IActionResult> CheckLeaves([FromQuery] string pId, string kiKhaoSatId)
        {

            var transferObject = new TransferObject();
            var result = await _service.CheckLeaves(pId, kiKhaoSatId);
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

        [HttpPut("UpdateLeaves")]
        public async Task<IActionResult> UpdateLeaves([FromBody] TieuChiDto data)
        {
            var transferObject = new TransferObject();
            await _service.updateLeaves(data);
            if (_service.Status)
            {
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
        [HttpPut("UpdateTreeGroup")]
        public async Task<IActionResult> UpdateTreeGroup([FromBody] TieuChiDto data)
        {
            var transferObject = new TransferObject();
            await _service.updateTreeGroup(data);
            if (_service.Status)
            {
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
        [HttpPut("DeleteTreeGroup")]
        public async Task<IActionResult> DeleteTreeGroup([FromBody] TieuChiDto data)
        {
            var transferObject = new TransferObject();
            await _service.deleteTreeGroup(data);
            if (_service.Status)
            {
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
        [HttpPut("UpdateOrderTree")]
        public async Task<IActionResult> UpdateOrderTree([FromBody] TieuChiDto data)
        {
            var transferObject = new TransferObject();
            await _service.UpdateOrderTree(data);
            if (_service.Status)
            {
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

        [HttpPut("UpdateOrderLeaves")]
        public async Task<IActionResult> UpdateOrderLeaves([FromBody] List<TieuChiDto> data)
        {
            var transferObject = new TransferObject();
            await _service.UpdateOrderLeaves(data);
            if (_service.Status)
            {
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
