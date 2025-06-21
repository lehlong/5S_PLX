
using AutoMapper;
using Common;
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
                var lstInStore = inputKy.Result.lstInputStore.ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstEvaValue = _dbContext.TblBuEvaluateValue.Where(x => x.FeedBack != "").ToList();
                var lstPoint = _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();

                if (!string.IsNullOrWhiteSpace(filterReport.InstoreId))
                {
                    lstInStore = lstInStore.Where(x => x.Id == filterReport.InstoreId).ToList();
                }
                var result = new List<TongHopYKienDeXuat>();


                foreach (var i in lstEvaHeader)
                {
                    var lstValue = lstEvaValue.Where(x => x.EvaluateHeaderCode == i.Code).ToList();
                    foreach (var v in lstValue)
                    {
                        var a = new TongHopYKienDeXuat()
                        {
                            stt = lstInStore.FirstOrDefault(x => x.Id == i.StoreId).Id,
                            StoreName = lstInStore.FirstOrDefault(x => x.Id == i.StoreId).Name,
                            lstTieuChiDeXuat = lstValue.Select(x => new LstTieuChiDeXuat()
                            {
                                TieuChi = lstTieuChi.FirstOrDefault(b => b.Code == x.TieuChiCode).Name,
                                DeXuat = x.FeedBack,
                                CanBo = i.AccountUserName,
                                ChucVu = i.ChucVuId,
                                ThoiGian = i.UpdateDate?.ToString("HH:mm dd/MM/yyyy")
                            }).ToList() ?? null,
                        };
                        result.Add(a);
                    }
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