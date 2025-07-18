﻿using Common;
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
        public async Task<IActionResult> BuildDataTreeForApp([FromQuery] string kiKhaoSatId, string doiTuongId)
        {
            var transferObject = new TransferObject();
            var result = await _service.BuildDataTreeForApp(kiKhaoSatId, doiTuongId);
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
        public async Task<IActionResult> GetAllTieuChiLeaves([FromQuery] string kiKhaoSatId, string doiTuongId)
        {
            var transferObject = new TransferObject();
            var result = await _service.GetAllTieuChiLeaves(kiKhaoSatId, doiTuongId);
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


        [HttpGet("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {
            var transferObject = new TransferObject();
            var result = await _service.GetNotification();
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
        public async Task<IActionResult> BuildInputEvaluate([FromQuery] string kiKhaoSatId, string doiTuongId, string deviceId)
        {
            var transferObject = new TransferObject();
            var result = await _service.BuildInputEvaluate(kiKhaoSatId, doiTuongId, deviceId);
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




        [HttpPost("InsertEvaluate")]
        public async Task<IActionResult> InsertEvaluate([FromBody] EvaluateModel data)
        {
            var transferObject = new TransferObject();
            await _service.InsertEvaluate(data);
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



        [HttpGet("GetResultEvaluate")]
        public async Task<IActionResult> GetResultEvaluate([FromQuery] string code)
        {
            var transferObject = new TransferObject();
            var data = await _service.GetResultEvaluate(code);
            if (_service.Status)
            {
                transferObject.Data = data;
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

        [HttpPost("HandlePointStore")]
        public async Task<IActionResult> HandlePointStore([FromBody] EvaluateFilter param)
        {
            var transferObject = new TransferObject();
            await _service.HandlePointStore(param);
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

        [HttpPost("TinhTongLanCham")]
        public async Task<IActionResult> TinhTongLanCham([FromBody] TblBuPoint data)
        {
            var transferObject = new TransferObject();
            await _service.TinhTongLanCham(data);
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


        //[HttpGet("")]
        //public async Task<IActionResult> GetPointStore([FromQuery] string kiKhaoSatId, string surveyId)
        //{
        //    var transferObject = new TransferObject();
        //    var data = await _service.GetPointStore(kiKhaoSatId, surveyId);
        //    if (_service.Status)
        //    {
        //        transferObject.Data = data;
        //        transferObject.Status = true;
        //    }
        //    else
        //    {
        //        transferObject.Status = false;
        //        transferObject.MessageObject.MessageType = MessageType.Error;
        //        transferObject.GetMessage("0001", _service);
        //    }
        //    return Ok(transferObject);
        //}

        [HttpGet("GetPointStore")]
        public async Task<IActionResult> GetPointStore([FromQuery] string kiKhaoSatId, string surveyId)
        {
            var transferObject = new TransferObject();
            var data = await _service.GetPointStore(kiKhaoSatId, surveyId);
            if (_service.Status)
            {
                transferObject.Data = data;
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

        [HttpGet("GetDataHome")]
        public async Task<IActionResult> GetDataHome([FromQuery] string userName)
        {
            var transferObject = new TransferObject();
            var data = await _service.GetDataHome(userName);
            if (_service.Status)
            {
                transferObject.Data = data;
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
        //    
        //}

    }
}
