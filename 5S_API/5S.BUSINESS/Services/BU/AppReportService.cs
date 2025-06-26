
using AutoMapper;
using Common;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface IAppReportService : IGenericService<TblBuEvaluateHeader, EvaluateHeaderDto>
    {
        Task<List<KetQuaChamDiem>> KetQuaChamDiem(FilterReport filterReport);
        Task<List<ThoiGianChamDiem>> ThoiGianChamDiem(FilterReport filterReport);
        Task<List<ThoiGianChamDiem>> ThietBiChamDiem(FilterReport filterReport);
        Task<List<TheoKhungThoiGian>> TheoKhungThoiGian(FilterReport filterReport);
        Task<List<TongHopYKienDeXuat>> TongHopYKienDeXuat(FilterReport filterReport);
    }

    public class AppReportService : GenericService<TblBuEvaluateHeader, EvaluateHeaderDto>, IAppReportService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IKikhaosatService _kiKhaoSatService;
        private readonly ITieuChiService _tieuChiService;

        public AppReportService(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment environment, IKikhaosatService KiKhaoSatService, ITieuChiService TieuChiService) : base(dbContext, mapper)
        {
            _environment = environment;
            _kiKhaoSatService = KiKhaoSatService;
            _tieuChiService = TieuChiService;
        }

        public async Task<List<KetQuaChamDiem>> KetQuaChamDiem(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstInStore = inputKy.Result.lstInputStore.ToList();
                var lstPoint = _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();

                if (!string.IsNullOrWhiteSpace(filterReport.InstoreId))
                {
                    lstInStore = lstInStore.Where(x => x.Id == filterReport.InstoreId).ToList();
                }

                var result = new List<KetQuaChamDiem>();

                foreach (var item in lstInStore)
                {
                    var report = new KetQuaChamDiem()
                    {
                        stt = item.StoreId,
                        StoreName= item.Name,
                        Length = lstPoint.FirstOrDefault(x => x.InStoreId == item.Id)?.Length ?? 0,
                        point = lstPoint.FirstOrDefault(x => x.InStoreId == item.Id)?.Point ?? 0,
                    };

                    result.Add(report);
                }

                return result;
            }
            catch (Exception ex) 
            {
                return null;
            }
        }



        public async Task<List<ThoiGianChamDiem>> ThoiGianChamDiem(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstInStore = inputKy.Result.lstInputStore.ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.IsActive == true && x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstPoint = _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();

                if (!string.IsNullOrWhiteSpace(filterReport.InstoreId))
                {
                    lstInStore = lstInStore.Where(x => x.Id == filterReport.InstoreId).ToList();
                }

                var result = new List<ThoiGianChamDiem>();

                foreach (var item in lstInStore)
                {
                    var report = new ThoiGianChamDiem()
                    {
                        stt = item.StoreId,
                        StoreName = item.Name,
                        Cht = lstEvaHeader.Where(x => x.StoreId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " + x.UpdateDate).ToList(),
                        Atvsv = lstEvaHeader.Where(x => x.StoreId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => "L" + (index + 1) + " "+ x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                        ChuyenGia = lstEvaHeader.Where(x => x.StoreId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => "L" + (index + 1) + " "+ x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                        Point = lstPoint.FirstOrDefault(x => x.InStoreId == item.Id)?.Point ?? 0,
                    };

                    result.Add(report);
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<ThoiGianChamDiem>> ThietBiChamDiem(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstInStore = inputKy.Result.lstInputStore.ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.IsActive == true && x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstPoint = _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var device = _dbContext.tblMdDevice.AsQueryable().ToList();

                if (!string.IsNullOrWhiteSpace(filterReport.InstoreId))
                {
                    lstInStore = lstInStore.Where(x => x.Id == filterReport.InstoreId).ToList();
                }

                var result = new List<ThoiGianChamDiem>();


                foreach (var item in lstInStore)
                {
                   
                    var report = new ThoiGianChamDiem()
                    {

                        stt = item.StoreId,
                        StoreName = item.Name,
                        Cht = lstEvaHeader.Where(x => x.StoreId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " +((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "") ).ToList(),
                        Atvsv = lstEvaHeader.Where(x => x.StoreId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => "L" + (index + 1) + " " + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "")).ToList(),
                        ChuyenGia = lstEvaHeader.Where(x => x.StoreId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => "L" + " " +(index + 1) + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "")).ToList(),
                        Point = lstPoint.FirstOrDefault(x => x.InStoreId == item.Id)?.Point ?? 0,
                    };

                    result.Add(report);
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TheoKhungThoiGian>> TheoKhungThoiGian(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstInStore = inputKy.Result.lstInputStore.ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstPoint = _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();

                if (!string.IsNullOrWhiteSpace(filterReport.InstoreId))
                {
                    lstInStore = lstInStore.Where(x => x.Id == filterReport.InstoreId).ToList();
                }
                var result = new List<TheoKhungThoiGian>();

                foreach (var item in lstInStore)
                {
                    var report = new TheoKhungThoiGian()
                    {
                        stt = item.StoreId,
                        StoreName = item.Name,
                        Cht_T = lstEvaHeader.Count(x => x.IsActive == true  && x.StoreId == item.Id && x.ChucVuId == "CHT"),
                        Cht_N = lstEvaHeader.Count(x => x.IsActive == false && x.StoreId == item.Id && x.ChucVuId == "CHT"),
                        Atvsv_T = lstEvaHeader.Count(x => x.IsActive == true && x.StoreId == item.Id && x.ChucVuId == "ATVSV"),
                        Atvsv_N = lstEvaHeader.Count(x => x.IsActive == false && x.StoreId == item.Id && x.ChucVuId == "ATVSV"),
                        ChuyenGia = lstEvaHeader.Count(x => x.StoreId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT"),
                        Point = lstPoint.FirstOrDefault(x => x.InStoreId == item.Id)?.Point ?? 0,
                    };

                    result.Add(report);
                }
                return result;

            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }




        public async Task<List<TongHopYKienDeXuat>> TongHopYKienDeXuat(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstTieuChi = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId && x.IsGroup == false).ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstEvaValue = _dbContext.TblBuEvaluateValue.Where(x => x.FeedBack != "").ToList();
                var lstPoint = _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstInStore = inputKy.Result.lstInputStore.ToList();



                if (!string.IsNullOrWhiteSpace(filterReport.InstoreId))
                {
                    lstInStore = lstInStore.Where(x => x.Id == filterReport.InstoreId).ToList();
                }
                var result = new List<TongHopYKienDeXuat>();


                foreach (var i in lstPoint)
                {
                    var store = lstInStore.FirstOrDefault(x => x.Id == i.InStoreId);
                    var lstHeader = lstEvaHeader.Where(x => x.StoreId == i.InStoreId).ToList();

                    var lstDeXuat = new List<LstTieuChiDeXuat>(); 
                    foreach (var item in lstHeader)
                    {
                        var ab = lstEvaValue.Where(x => x.EvaluateHeaderCode == item.Code).Select(x => new LstTieuChiDeXuat()
                        {
                            TieuChi = lstTieuChi.FirstOrDefault(b => b.Code == x.TieuChiCode).Name,
                            DeXuat = x.FeedBack,
                            CanBo = item.AccountUserName,
                            ChucVu = item.ChucVuId,
                            ThoiGian = i.UpdateDate?.ToString("HH:mm dd/MM/yyyy")
                        }).ToList();

                        lstDeXuat.AddRange(ab);
                    }

                    var a = new TongHopYKienDeXuat()
                    {
                        stt = store.StoreId,
                        StoreName = store.Name,
                        lstTieuChiDeXuat = lstDeXuat
                    };
                    result.Add(a);
                }

                return result;
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }
    }
}