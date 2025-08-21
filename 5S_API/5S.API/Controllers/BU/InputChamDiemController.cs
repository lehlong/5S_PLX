//using Common;
//using Microsoft.AspNetCore.Mvc;
//using PLX5S.API.AppCode.Enum;
//using PLX5S.API.AppCode.Extensions;
//using PLX5S.BUSINESS.Dtos.BU;
//using Services.BU;

//namespace PLX5S.API.Controllers.BU
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class InputChamDiemController(IInputChamDiemService service) : ControllerBase
//    {
//        private readonly IInputChamDiemService _service = service;

//        [HttpGet("Search")]
//        public async Task<IActionResult> Search([FromQuery] BaseFilter filter)
//        {
//            var transferObject = new TransferObject();
//            var result = await _service.Search(filter);
//            if (_service.Status)
//            {
//                transferObject.Data = result;
//            }
//            else
//            {
//                transferObject.Status = false;
//                transferObject.MessageObject.MessageType = MessageType.Error;
//                transferObject.GetMessage("0001", _service);
//            }
//            return Ok(transferObject);
//        }

//        [HttpPost("Insert")]
//        public async Task<IActionResult> Insert([FromBody] InputChamDiemDto dto)
//        {
//            var transferObject = new TransferObject();
//            await _service.Insert(dto);
//            if (_service.Status)
//            {
//                transferObject.Status = true;
//                transferObject.MessageObject.MessageType = MessageType.Success;
//                transferObject.GetMessage("0100", _service);
//            }
//            else
//            {
//                transferObject.Status = false;
//                transferObject.MessageObject.MessageType = MessageType.Error;
//                transferObject.GetMessage("0101", _service);
//            }
//            return Ok(transferObject);
//        }

//        [HttpPut("Update")]
//        public async Task<IActionResult> Update([FromBody] InputChamDiemDto dto)
//        {
//            var transferObject = new TransferObject();
//            await _service.Update(dto);
//            if (_service.Status)
//            {
//                transferObject.Status = true;
//                transferObject.MessageObject.MessageType = MessageType.Success;
//                transferObject.GetMessage("0103", _service);
//            }
//            else
//            {
//                transferObject.Status = false;
//                transferObject.MessageObject.MessageType = MessageType.Error;
//                transferObject.GetMessage("0104", _service);
//            }
//            return Ok(transferObject);
//        }

//        [HttpDelete("Delete")]
//        public async Task<IActionResult> Delete([FromQuery] string storeId, [FromQuery] string kiKhaoSatId, [FromQuery] string userName)
//        {
//            var transferObject = new TransferObject();
//            var key = new { StoreId = storeId, KiKhaoSatId = kiKhaoSatId, UserName = userName };
//            await _service.Delete(key);
//            if (_service.Status)
//            {
//                transferObject.Status = true;
//                transferObject.MessageObject.MessageType = MessageType.Success;
//                transferObject.GetMessage("0105", _service);
//            }
//            else
//            {
//                transferObject.Status = false;
//                transferObject.MessageObject.MessageType = MessageType.Error;
//                transferObject.GetMessage("0106", _service);
//            }
//            return Ok(transferObject);
//        }
//    }
//}
