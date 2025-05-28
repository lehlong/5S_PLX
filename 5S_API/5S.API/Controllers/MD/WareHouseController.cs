using Common;
using Microsoft.AspNetCore.Mvc;
using PLX5S.API.AppCode.Enum;
using PLX5S.API.AppCode.Extensions;
using PLX5S.BUSINESS.Dtos.MD;
using PLX5S.BUSINESS.Services.MD;

namespace PLX5S.API.Controllers.MD
{
    
    [Route("api/[controller]")]
[ApiController]
public class WareHouseController(IWarehouseService service) : ControllerBase
{
    public readonly IWarehouseService _service = service;

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
                transferObject.Data = result;
                transferObject.MessageObject.MessageType = MessageType.Error;
            transferObject.GetMessage("0001", _service);
        }
        return Ok(transferObject);
    }
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] BaseMdFilter filter)
    {
        var transferObject = new TransferObject();
        var result = await _service.GetAll(filter);
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
    public async Task<IActionResult> Insert([FromBody] WarehouseDto time)
    {
        var transferObject = new TransferObject();

        await _service.Insert(time);

        if (_service.Status)
        {
            //transferObject.Data = result;
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
    public async Task<IActionResult> Update([FromBody] WarehouseDto time)
    {
        var transferObject = new TransferObject();
        await _service.Update(time);
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
        await _service.Delete(id);
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
        [HttpGet("GetATVSV")]
        public async Task<IActionResult> GetATVSV([FromQuery] string headerId)
        {
            var transferObject = new TransferObject();
            var result = await _service.GetATVSV(headerId);
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
    }
}
