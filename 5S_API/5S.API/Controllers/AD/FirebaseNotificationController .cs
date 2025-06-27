using Common;
using Dtos.AD;
using Microsoft.AspNetCore.Mvc;
using PLX5S.API.AppCode.Enum;
using Services.AD;

namespace PLX5S.API.Controllers.AD
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseNotificationController : ControllerBase
    {
        private readonly IFirebaseNotificationService _firebaseService;

        public FirebaseNotificationController(IFirebaseNotificationService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification([FromBody] FirebaseNotificationDto notification)
        {
            var transferObject = new TransferObject();
            var result = await _firebaseService.SendNotificationAsync(notification);

            if (result.Success)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.MessageObject.Message = result.Message;
            }

            return Ok(transferObject);
        }

        [HttpPost("SendToTopic")]
        public async Task<IActionResult> SendToTopic([FromBody] SendToTopicRequest request)
        {
            var transferObject = new TransferObject();
            var result = await _firebaseService.SendToTopicAsync(request.Topic, request.Title, request.Body, request.Data);

            if (result.Success)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.MessageObject.Message = result.Message;
            }

            return Ok(transferObject);
        }

        [HttpPost("SendToToken")]
        public async Task<IActionResult> SendToToken([FromBody] SendToTokenRequest request)
        {
            var transferObject = new TransferObject();
            var result = await _firebaseService.SendToTokenAsync(request.Token, request.Title, request.Body);

            if (result.Success)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.MessageObject.Message = result.Message;
            }

            return Ok(transferObject);
        }

        [HttpGet("TestConnection")]
        public async Task<IActionResult> TestConnection()
        {
            var transferObject = new TransferObject();
            var result = await _firebaseService.TestConnectionAsync();

            if (result.Success)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.MessageObject.Message = result.Message;
            }

            return Ok(transferObject);
        }

        [HttpPost("SendSampleFromServer")]
        public async Task<IActionResult> SendSampleFromServer()
        {
            // Gửi thông báo đến topic "news"
            var result = await _firebaseService.SendToTopicAsync(
                "news", // topic
                "Thông báo từ server .NET Core", // title
                "Đây là nội dung gửi từ code C#" // body
            );

            var transferObject = new TransferObject();
            if (result.Success)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.MessageObject.Message = result.Message;
            }
            return Ok(transferObject);
        }
        [HttpPost("SendToTopicTest")]
        public async Task<IActionResult> SendToTopicTest([FromBody] SendToTopicTestRequest request)
        {
            var transferObject = new TransferObject();
            var result = await _firebaseService.SendToTopicTestAsync(request.Title, request.Body);

            if (result.Success)
            {
                transferObject.Data = result;
            }
            else
            {
                transferObject.Status = false;
                transferObject.MessageObject.MessageType = MessageType.Error;
                transferObject.MessageObject.Message = result.Message;
            }

            return Ok(transferObject);
        }
    }
}