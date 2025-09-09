using Common;
using PLX5S.API.AppCode.Enum;
using PLX5S.API.AppCode.Extensions;
using PLX5S.BUSINESS.Dtos.AD;
using PLX5S.BUSINESS.Filter.AD;
using PLX5S.BUSINESS.Services.AD;
using Microsoft.AspNetCore.Mvc;

namespace PLX5S.API.Controllers.MD
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(IAccountService service) : ControllerBase
    {
        public readonly IAccountService _service = service;

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] AccountFilter filter)
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
                transferObject.GetMessage("2000", _service);
            }
            return Ok(transferObject);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] AccountFilterLite filter)
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

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetDetail(string userName)
        {
            var transferObject = new TransferObject();
            var result = await _service.GetByIdWithRightTree(userName);
            if (_service.Status)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("2000", _service);
            }
            return Ok(transferObject);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] AccountCreateDto account)
        {
            var transferObject = new TransferObject();
            var result = await _service.Add(account);
            if (_service.Status)
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

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] AccountUpdateDto account)
        {
            var transferObject = new TransferObject();
            await _service.Update(account);
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

        [HttpDelete("Delete/{userName}")]
        public async Task<IActionResult> Delete(string userName)
        {
            var transferObject = new TransferObject();
            await _service.Delete(userName);
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

        [HttpPut("UpdateInformation")]
        public async Task<IActionResult> UpdateInformation([FromBody] AccountUpdateInformationDto account)
        {
            var transferObject = new TransferObject();
            await _service.UpdateInformation(account);
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
        public class IdDto 
        {
            public string Id { get; set; }
        }
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword([FromQuery] string username)
        {
            var transferObject = new TransferObject();
            _service.ResetPassword(username);
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
        [HttpGet("GetDeviceByUser")]
        public async Task<IActionResult> GetDeviceByUser([FromQuery] string username)
        {
            var transferObject = new TransferObject();
            var result = await _service.GetDiviceByUser(username);
            if (_service.Status)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.GetMessage("0104", _service);
            }
            return Ok(transferObject);
        }
        [HttpPut("EnableDevice")]
         public async Task<IActionResult> EnableDevice([FromBody] IdDto deviceID)
        {
            var transferObject = new TransferObject();
           await _service.EnableDevice(deviceID.Id);
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
        [HttpPut("MainDevice")]
        public async Task<IActionResult> MainDevice([FromBody] IdDto deviceID)
        {
            var transferObject = new TransferObject();
            await _service.MainDevice(deviceID.Id);
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


        //[HttpGet("ResetAllPassUser")]
        //public async Task<IActionResult> ResetAllPassUser([FromQuery] string username)
        //{
        //    var transferObject = new TransferObject();
        //    await _service.ResetAllPassUser();
        //    if (_service.Status)
        //    {
        //    }
        //    else
        //    {
        //        transferObject.Status = false;
        //        transferObject.MessageObject.MessageType = MessageType.Error;
        //        transferObject.GetMessage("0104", _service);
        //    }
        //    return Ok(transferObject);
        //}
    }
}
