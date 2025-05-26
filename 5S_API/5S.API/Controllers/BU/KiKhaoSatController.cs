using Common;
using PLX5S.API.AppCode.Enum;
using PLX5S.API.AppCode.Extensions;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.BUSINESS.Services.BU;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLX5S.CORE.Entities.BU;
using NPOI.SS.Formula.Functions;
using PLX5S.BUSINESS.Models;

namespace PLX5S.API.Controllers.BU
{
    [Route("api/[controller]")]
    [ApiController]
    public class KiKhaoSatController(IKikhaosatService service) : ControllerBase
    {
        public readonly IKikhaosatService _service = service;

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] BaseFilter filter)
        {
            var transferObject = new TransferObject();
            var result = await _service.Search(filter);
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

        [HttpGet("BuildObjCreate")]
        public async Task<IActionResult> BuildObjCreate([FromQuery] string id)
        {
            var transferObject = new TransferObject();
            var result = await _service.BuilObjCreate(id);
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


        [HttpGet("GetInputKiKhaoSat")]
        public async Task<IActionResult> GetInputKiKhaoSat([FromQuery] string idKi)
        {
            var transferObject = new TransferObject();
            var result = await _service.GetInput(idKi);
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



        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] KiKhaoSatModel time)
        {
            var transferObject = new TransferObject();

            await _service.Insert(time);

            if (_service.Status)
            {
                transferObject.Status = true;
                transferObject.MessageObject.MessageType = MessageType.Success;
                transferObject.GetMessage("0100", _service);
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0101", _service);
            }
            return Ok(transferObject);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] KiKhaoSatModel time)
        {
            var transferObject = new TransferObject();
            await _service.UpdateDataInput(time);
            if (_service.Status)
            {
                transferObject.Status = true;
                transferObject.MessageObject.MessageType = MessageType.Success;
                transferObject.GetMessage("0103", _service);
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0104", _service);
            }
            return Ok(transferObject);
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var transferObject = new TransferObject();
            await _service.DeleteData(id);
            if (_service.Status)
            {
                transferObject.Status = true;
                transferObject.MessageObject.MessageType = MessageType.Success;
                transferObject.GetMessage("0105", _service);
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0106", _service);
            }
            return Ok(transferObject);
        }


        [HttpGet("GetInputCopyKy")]
        public async Task<IActionResult> GetInputCopyKy([FromQuery] string kyKhaoSatId)
        {
            var transferObject = new TransferObject();
            var result = await _service.getKyCopy(kyKhaoSatId);
            if (result != null)
            {
                transferObject.Data = result;
                transferObject.Status = true;
                transferObject.MessageObject.MessageType = MessageType.Success;
                transferObject.GetMessage("0100", _service);
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0101", _service);
            }
            return Ok(transferObject);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string kiKhaoSatId)
        {
            var transferObject = new TransferObject();
            var result = '-';
                //await _service.Getchamdiem(kiKhaoSatId);
            if (result != null)
            {
                transferObject.Data = result;
                transferObject.Status = true;
                transferObject.MessageObject.MessageType = MessageType.Success;
                transferObject.GetMessage("0100", _service);
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0101", _service);
            }
            return Ok(transferObject);
        }


    }
}
