using Common;
using Microsoft.AspNetCore.Mvc;
using PLX5S.API.AppCode.Enum;
using PLX5S.BUSINESS.Dtos.BU;
//using PlX5S.BUSINESS.Services.BU;
using PLX5S.BUSINESS.Services.BU;
using PLX5S.API.AppCode.Extensions;

namespace PLX5S.API.Controllers.BU
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppEvaluateController(IAppEvaluateService service) : ControllerBase
    {
        public readonly IAppEvaluateService _service = service;

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


        [HttpGet("BuildDataTreeForApp")]
        public async Task<IActionResult> BuildDataTreeForApp([FromQuery] string kiKhaoSatId, string storeId)
        {
            var transferObject = new TransferObject();
            var result = await _service.BuildDataTreeForApp(kiKhaoSatId, storeId);
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


        [HttpGet("GetAllTieuChiLeaves")]
        public async Task<IActionResult> GetAllTieuChiLeaves([FromQuery] string kiKhaoSatId, string storeId)
        {
            var transferObject = new TransferObject();
            var result = await _service.GetAllTieuChiLeaves(kiKhaoSatId, storeId);
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


        [HttpGet("BuildInputEvaluate")]
        public async Task<IActionResult> BuildInputEvaluate([FromQuery] string kiKhaoSatId, string storeId)
        {
            var transferObject = new TransferObject();
            var result = await _service.BuildInputEvaluate(kiKhaoSatId, storeId);
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


        //[HttpPost("upload-image")]
        //public async Task<IActionResult> UploadImage([FromBody] ImageUploadRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Base64Image))
        //        return BadRequest("Image data is required.");

        //    try
        //    {
        //        // Tách phần base64 data (nếu có định dạng data:image/png;base64,...)
        //        var base64Data = request.Base64Image;
        //        var base64Parts = base64Data.Split(',');
        //        if (base64Parts.Length == 2)
        //        {
        //            base64Data = base64Parts[1];
        //        }

        //        // Giải mã base64 thành mảng byte
        //        byte[] imageBytes = Convert.FromBase64String(base64Data);

        //        // Tạo tên file ngẫu nhiên (vd: GUID + .png)
        //        var fileName = $"{Guid.NewGuid()}.png";

        //        // Đường dẫn lưu file trên server (ví dụ: wwwroot/uploads)
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var filePath = Path.Combine(uploadsFolder, fileName);

        //        // Lưu file
        //        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

        //        // Lưu thông tin vào DB
        //        var imageRecord = new ImageRecord
        //        {
        //            FileName = fileName,
        //            FilePath = $"/uploads/{fileName}", // Đường dẫn để client truy cập (nếu cần)
        //            UploadedAt = DateTime.UtcNow
        //        };

        //        _context.Images.Add(imageRecord);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { Message = "Upload thành công", ImageUrl = imageRecord.FilePath });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi lưu ảnh: {ex.Message}");
        //    }
        //}

    }
}
