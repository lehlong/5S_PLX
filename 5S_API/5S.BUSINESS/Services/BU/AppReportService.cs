
using Aspose.Words.Tables;
using AutoMapper;
using Common;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.BUSINESS.Extentions;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
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
        Task<List<BaoCaoHinhAnh>> BaoCaoHinhAnh(FilterReport filterReport);
        Task<string> ExportExcel(string ReportName, FilterReport filterReport);
    }

    public class AppReportService : GenericService<TblBuEvaluateHeader, EvaluateHeaderDto>, IAppReportService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IKikhaosatService _kiKhaoSatService;
        private readonly ITieuChiService _tieuChiService;
        private readonly IConfiguration _config;

        public AppReportService(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment environment, IKikhaosatService KiKhaoSatService, ITieuChiService TieuChiService, IConfiguration configuration) : base(dbContext, mapper)
        {
            _environment = environment;
            _kiKhaoSatService = KiKhaoSatService;
            _tieuChiService = TieuChiService;
            _config = configuration;
        }

        public async Task<List<KetQuaChamDiem>> KetQuaChamDiem(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var result = new List<KetQuaChamDiem>();
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                if (filterReport.SurveyId == "DT1")
                {
                    var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var item in lstDoiTuong)
                    {
                        var report = new KetQuaChamDiem()
                        {
                            stt = item.StoreId,
                            Name = item.Name,
                            Length = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Length ?? 0,
                            point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };

                        result.Add(report);
                    }
                }
                else if (filterReport.SurveyId == "DT2")
                {
                    var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var item in lstDoiTuong)
                    {
                        var report = new KetQuaChamDiem()
                        {
                            stt = item.Id,
                            Name = item.Name,
                            Length = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Length ?? 0,
                            point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };
                        
                        result.Add(report);
                    }

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
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.IsActive == true && x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var result = new List<ThoiGianChamDiem>();
                if (filterReport.SurveyId == "DT1")
                {
                    var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var item in lstDoiTuong)
                    {
                        var report = new ThoiGianChamDiem()
                        {
                            stt = item.StoreId,
                            Name = item.Name,
                            Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " + x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                            Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => "L" + (index + 1) + " " + x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                            ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => "L" + (index + 1) + "  " + x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                            Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };

                        result.Add(report);
                    }
                }
                else if (filterReport.SurveyId == "DT2")
                {
                    var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();
                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var item in lstDoiTuong)
                    {
                        var report = new ThoiGianChamDiem()
                        {
                            stt = item.WareHouseId,
                            Name = item.Name,
                            Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " + x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                            Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => "L" + (index + 1) + " " + x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                            ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => "L" + (index + 1) + "  " + x.UpdateDate?.ToString("HH:mm dd-MM-yyyy")).ToList(),
                            Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };

                        result.Add(report);
                    }
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
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.IsActive == true && x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var device = _dbContext.tblMdDevice.AsQueryable().ToList();
                var result = new List<ThoiGianChamDiem>();
                if (filterReport.SurveyId == "DT1")
                {
                    var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }


                    foreach (var item in lstDoiTuong)
                    {
                        var report = new ThoiGianChamDiem()
                        {
                            stt = item.StoreId,
                            Name = item.Name,
                            Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                            Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => "L" + (index + 1) + " " + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                            ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => "L" + (index + 1) + " " + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                            Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };

                        result.Add(report);
                    }
                }
                else if (filterReport.SurveyId == "DT2")
                {
                    var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }


                    foreach (var item in lstDoiTuong)
                    {
                        var report = new ThoiGianChamDiem()
                        {
                            stt = item.WareHouseId,
                            Name = item.Name,
                            Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                            Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => "L" + (index + 1) + " " + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                            ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => "L" + (index + 1) + " " + ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                            Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };

                        result.Add(report);
                    }
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
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var result = new List<TheoKhungThoiGian>();
                if (filterReport.SurveyId == "DT1")
                {
                    var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var item in lstDoiTuong)
                    {
                        var report = new TheoKhungThoiGian()
                        {
                            stt = item.StoreId,
                            Name = item.Name,
                            Cht_T = lstEvaHeader.Count(x => x.IsActive == true && x.DoiTuongId == item.Id && x.ChucVuId == "CHT"),
                            Cht_N = lstEvaHeader.Count(x => x.IsActive == false && x.DoiTuongId == item.Id && x.ChucVuId == "CHT"),
                            Atvsv_T = lstEvaHeader.Count(x => x.IsActive == true && x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV"),
                            Atvsv_N = lstEvaHeader.Count(x => x.IsActive == false && x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV"),
                            ChuyenGia = lstEvaHeader.Count(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT"),
                            Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };

                        result.Add(report);
                    }
                }
                else if (filterReport.SurveyId == "DT2")
                {
                    var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var item in lstDoiTuong)
                    {
                        var report = new TheoKhungThoiGian()
                        {
                            stt = item.WareHouseId,
                            Name = item.Name,
                            Cht_T = lstEvaHeader.Count(x => x.IsActive == true && x.DoiTuongId == item.Id && x.ChucVuId == "CHT"),
                            Cht_N = lstEvaHeader.Count(x => x.IsActive == false && x.DoiTuongId == item.Id && x.ChucVuId == "CHT"),
                            Atvsv_T = lstEvaHeader.Count(x => x.IsActive == true && x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV"),
                            Atvsv_N = lstEvaHeader.Count(x => x.IsActive == false && x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV"),
                            ChuyenGia = lstEvaHeader.Count(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT"),
                            Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                        };

                        result.Add(report);
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

        public async Task<List<TongHopYKienDeXuat>> TongHopYKienDeXuat(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstTieuChi = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId && x.IsGroup == false).ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstEvaValue = _dbContext.TblBuEvaluateValue.Where(x => x.FeedBack != "").ToList();
                var lstChucVu = _dbContext.tblMdChucVu.OrderBy(x => x.Id).ToList();
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstAccount = _dbContext.TblAdAccount.OrderBy(x => x.UserName).ToList();
                var result = new List<TongHopYKienDeXuat>();

                if (filterReport.SurveyId == "DT1")
                {
                    var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var i in lstPoint)
                    {
                        var store = lstDoiTuong.FirstOrDefault(x => x.Id == i.DoiTuongId);
                        var lstHeader = lstEvaHeader.Where(x => x.DoiTuongId == i.DoiTuongId).ToList();

                        var lstDeXuat = new List<LstTieuChiDeXuat>();
                        foreach (var item in lstHeader)
                        {
                            var ab = lstEvaValue.Where(x => x.EvaluateHeaderCode == item.Code).Select(x => new LstTieuChiDeXuat()
                            {
                                TieuChi = lstTieuChi.FirstOrDefault(b => b.Code == x.TieuChiCode)?.Name,
                                DeXuat = x.FeedBack,
                                CanBo = lstAccount.FirstOrDefault(x => x.UserName == item.AccountUserName)?.FullName,
                                ChucVu = lstChucVu.FirstOrDefault(x => x.Id == item?.ChucVuId)?.Name,
                                ThoiGian = i.UpdateDate?.ToString("HH:mm dd/MM/yyyy")
                            }).ToList();

                            lstDeXuat.AddRange(ab);
                        }

                        var a = new TongHopYKienDeXuat()
                        {
                            stt = store.StoreId,
                            Name = store.Name,
                            lstTieuChiDeXuat = lstDeXuat
                        };
                        result.Add(a);
                    }
                }
                else if (filterReport.SurveyId == "DT2")
                {
                    var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var i in lstPoint)
                    {
                        var store = lstDoiTuong.FirstOrDefault(x => x.Id == i.DoiTuongId);
                        var lstHeader = lstEvaHeader.Where(x => x.DoiTuongId == i.DoiTuongId).ToList();

                        var lstDeXuat = new List<LstTieuChiDeXuat>();
                        foreach (var item in lstHeader)
                        {
                            var ab = lstEvaValue.Where(x => x.EvaluateHeaderCode == item.Code).Select(x => new LstTieuChiDeXuat()
                            {
                                TieuChi = lstTieuChi.FirstOrDefault(b => b.Code == x.TieuChiCode).Name,
                                DeXuat = x.FeedBack,
                                CanBo = lstAccount.FirstOrDefault(x => x.UserName == item.AccountUserName)?.FullName,
                                ChucVu = lstChucVu.FirstOrDefault(x => x.Id == item?.ChucVuId)?.Name,
                                ThoiGian = i.UpdateDate?.ToString("HH:mm dd/MM/yyyy")
                            }).ToList();

                            lstDeXuat.AddRange(ab);
                        }

                        var a = new TongHopYKienDeXuat()
                        {
                            stt = store.WareHouseId,
                            Name = store.Name,
                            lstTieuChiDeXuat = lstDeXuat
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

        //Báo cáo hình ảnh
        public async Task<List<BaoCaoHinhAnh>> BaoCaoHinhAnh(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstTieuChi = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId && x.IsGroup == false).ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstEvaValue = _dbContext.TblBuEvaluateImage.Where(x => x.FilePath != "").ToList();
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                var lstChucVu = _dbContext.tblMdChucVu.OrderBy(x => x.Id).ToList();
                var lstAccount = _dbContext.TblAdAccount.OrderBy(x => x.UserName).ToList();
                var result = new List<BaoCaoHinhAnh>();

                if (filterReport.SurveyId == "DT1")
                {
                    var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var i in lstPoint)
                    {
                        var store = lstDoiTuong.FirstOrDefault(x => x.Id == i.DoiTuongId);
                        var lstHeader = lstEvaHeader.Where(x => x.DoiTuongId == i.DoiTuongId).ToList();

                        var lstHinhAnh = new List<LstHinhAnhBaoCao>();
                        foreach (var item in lstHeader)
                        {
                            var ab = lstEvaValue.Where(x => x.EvaluateHeaderCode == item.Code).Select(x => new LstHinhAnhBaoCao()
                            {
                                TieuChi = lstTieuChi.FirstOrDefault(b => b.Code == x.TieuChiCode)?.Name,
                                HinhAnh = x.FilePath,
                                Thumbnail = x.PathThumbnail,
                                CanBo = lstAccount.FirstOrDefault(x => x.UserName == item.AccountUserName)?.FullName,
                                ChucVu = lstChucVu.FirstOrDefault(x => x.Id == item?.ChucVuId)?.Name,
                                ThoiGian = i.UpdateDate?.ToString("HH:mm dd/MM/yyyy")
                            }).ToList();

                            lstHinhAnh.AddRange(ab);
                        }

                        var a = new BaoCaoHinhAnh()
                        {
                            stt = store.StoreId,
                            Name = store.Name,
                            lstHinhAnhBaoCao = lstHinhAnh
                        };
                        result.Add(a);
                    }
                }
                else if (filterReport.SurveyId == "DT2")
                {
                    var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();

                    if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                    {
                        lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                    }

                    foreach (var i in lstPoint)
                    {
                        var store = lstDoiTuong.FirstOrDefault(x => x.Id == i.DoiTuongId);
                        var lstHeader = lstEvaHeader.Where(x => x.DoiTuongId == i.DoiTuongId).ToList();

                        var lstHinhAnh = new List<LstHinhAnhBaoCao>();
                        foreach (var item in lstHeader)
                        {
                            var ab = lstEvaValue.Where(x => x.EvaluateHeaderCode == item.Code).Select(x => new LstHinhAnhBaoCao()
                            {
                                TieuChi = lstTieuChi.FirstOrDefault(b => b.Code == x.TieuChiCode).Name,
                                HinhAnh = x.FilePath,
                                Thumbnail = x.PathThumbnail,
                                CanBo = lstAccount.FirstOrDefault(x => x.UserName == item.AccountUserName)?.FullName,
                                ChucVu = lstChucVu.FirstOrDefault(x => x.Id == item?.ChucVuId)?.Name,
                                ThoiGian = i.UpdateDate?.ToString("HH:mm dd/MM/yyyy")
                            }).ToList();

                            lstHinhAnh.AddRange(ab);
                        }

                        var a = new BaoCaoHinhAnh()
                        {
                            stt = store.WareHouseId,
                            Name = store.Name,
                            lstHinhAnhBaoCao = lstHinhAnh
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
        public static ICellStyle SetCellStyleTextCenter(IWorkbook workbook, bool isBold, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, bool wrapText)
        {
            ICellStyle style = workbook.CreateCellStyle();
            IFont font = workbook.CreateFont();
            font.IsBold = isBold;
            font.FontName = "Arial"; // Hoặc font bạn muốn sử dụng
            style.SetFont(font);

            // Thiết lập căn ngang và dọc
            style.Alignment = horizontalAlignment;
            style.VerticalAlignment = verticalAlignment;

            // Tùy chọn ngắt dòng
            style.WrapText = wrapText;

            return style;
        }

        public async Task<string> ExportExcel(string ReportName, FilterReport filterReport)
        {
            try
            {
                void SetStyleForMergedRegion(ISheet sheet, int rowStart, int rowEnd, int colStart, int colEnd, ICellStyle style)
                {
                    for (int row = rowStart; row <= rowEnd; row++)
                    {
                        var sheetRow = sheet.GetRow(row) ?? sheet.CreateRow(row);
                        for (int col = colStart; col <= colEnd; col++)
                        {
                            var cell = sheetRow.GetCell(col) ?? sheetRow.CreateCell(col);
                            cell.CellStyle = style;

                        }
                    }
                }

                IWorkbook workbook = new XSSFWorkbook();


                if (ReportName == "KetQuaChamDiem")
                {
                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "KetQuaChamDiem.xlsx");

                    if (!File.Exists(templatePath))
                    {
                        throw new FileNotFoundException("Không tìm thấy template Excel.", templatePath);
                    }

                    using var file = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    workbook = new XSSFWorkbook(file);

                    var styles = new
                    {
                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true, wrap: true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = SetCellStyleTextCenter(workbook, true, HorizontalAlignment.Center, VerticalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
                        DecimalNumber = ExcelNPOIExtention.SetCellStyleDecimalNumber(workbook, false, HorizontalAlignment.Right, true)
                    };

                    var data = await KetQuaChamDiem(filterReport);
                    var sheet = workbook.GetSheetAt(0);
                    var startIndex = 1;
                    foreach (var i in data)
                    {
                        var row = sheet.GetRow(startIndex) ?? sheet.CreateRow(startIndex);

                        ExcelNPOIExtention.SetCellValueText(row, 0, i.stt, styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, 1, i.Name, styles.Text);
                        ExcelNPOIExtention.SetCellValueNumber(row, 2, i.Length, styles.Number);
                        ExcelNPOIExtention.SetCellValueNumber(row, 3, i.point, styles.Number);
                        ExcelNPOIExtention.SetCellValueText(row, 4, "", styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, 5, "", styles.Text);

                        startIndex++;
                    }
                }

                else if (ReportName == "ChamTheoThietBi")
                {
                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "ChamDiemTheoThietbi.xlsx");

                    if (!File.Exists(templatePath))
                    {
                        throw new FileNotFoundException("Không tìm thấy template Excel.", templatePath);
                    }

                    using var file = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    workbook = new XSSFWorkbook(file);

                    var styles = new
                    {
                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true, wrap: true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = SetCellStyleTextCenter(workbook, true, HorizontalAlignment.Center, VerticalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
                        DecimalNumber = ExcelNPOIExtention.SetCellStyleDecimalNumber(workbook, false, HorizontalAlignment.Right, true)
                    };

                    var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                    var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.IsActive == true && x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                    var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                    var device = _dbContext.tblMdDevice.AsQueryable().ToList();
                    var result = new List<ThoiGianChamDiem>();
                    if (filterReport.SurveyId == "DT1")
                    {
                        var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                        if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                        {
                            lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                        }


                        foreach (var item in lstDoiTuong)
                        {
                            var report = new ThoiGianChamDiem()
                            {
                                stt = item.StoreId,
                                Name = item.Name,
                                Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                            };

                            result.Add(report);
                        }
                    }
                    else if (filterReport.SurveyId == "DT2")
                    {
                        var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();

                        if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                        {
                            lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                        }


                        foreach (var item in lstDoiTuong)
                        {
                            var report = new ThoiGianChamDiem()
                            {
                                stt = item.WareHouseId,
                                Name = item.Name,
                                Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                            };

                            result.Add(report);
                        }
                    }

                    var sheet = workbook.GetSheetAt(0);
                    var row1 = sheet.GetRow(0) ?? sheet.CreateRow(0);
                    var row2 = sheet.GetRow(1) ?? sheet.CreateRow(1);
                    var row3 = sheet.GetRow(2) ?? sheet.CreateRow(2);
                    var maxCellCHt = result.MaxBy(x => x.Cht?.Count ?? 0).Cht.Count() == 0 ? 1 : result.MaxBy(x => x.Cht?.Count ?? 0).Cht.Count();
                    var maxCellATVSV = result.MaxBy(x => x.Atvsv?.Count ?? 0).Atvsv.Count() == 0 ? 1 : result.MaxBy(x => x.Atvsv?.Count ?? 0).Atvsv.Count();
                    var maxCellTCG5s = result.MaxBy(x => x.ChuyenGia?.Count ?? 0).ChuyenGia.Count() == 0 ? 1 : result.MaxBy(x => x.ChuyenGia?.Count ?? 0).ChuyenGia.Count();



                    //header
                    ExcelNPOIExtention.SetCellValueText(row1, 0, "STT", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, 1, "TÊN ĐƠN VỊ", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, 2, "SỐ LẦN CHẤM ĐIỂM TRONG KỲ", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, "TỔNG", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, "ĐIỂM", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, "XẾP LOẠI", styles.Text);
                    //
                    ExcelNPOIExtention.SetCellValueText(row2, 2, "CHT", styles.TextCenterBold);
                    ExcelNPOIExtention.SetCellValueText(row2, maxCellCHt + 2, "ATVSV", styles.TextCenterBold);
                    ExcelNPOIExtention.SetCellValueText(row2, maxCellATVSV + maxCellCHt + 2, "TỔ CHUYÊN GIA, BAN 5S", styles.TextCenterBold);

                    // meage
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 0, 0));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 1, 1));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4));

                    // Áp dụng cho các vùng merge
                    SetStyleForMergedRegion(sheet, 0, 2, 0, 0, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, 1, 1, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, styles.TextCenterBold);

                    if (maxCellCHt > 1)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 2, maxCellCHt + 1));
                        SetStyleForMergedRegion(sheet, 1, 1, 2, maxCellCHt + 1, styles.TextCenterBold);
                    }
                    if (maxCellCHt > 1)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, maxCellCHt + 2, maxCellCHt + maxCellATVSV + 1));
                        SetStyleForMergedRegion(sheet, 1, 1, maxCellCHt + 2, maxCellCHt + maxCellATVSV + 1, styles.TextCenterBold);
                    }
                    if (maxCellTCG5s > 1)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, maxCellCHt + maxCellATVSV + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1));
                        SetStyleForMergedRegion(sheet, 1, 1, maxCellCHt + maxCellATVSV + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1, styles.TextCenterBold);
                    }

                    //
                    for (int i = 0; i < maxCellCHt; i++)
                    {

                        ExcelNPOIExtention.SetCellValueText(row3, 2 + i, $"Lần {i + 1}", styles.TextCenterBold);
                    }
                    for (int i = 0; i < maxCellATVSV; i++)
                    {

                        ExcelNPOIExtention.SetCellValueText(row3, 2 + maxCellCHt + i, $"Lần {i + 1}", styles.TextCenterBold);
                    }
                    for (int i = 0; i < maxCellTCG5s; i++)
                    {
                        ExcelNPOIExtention.SetCellValueText(row3, 2 + maxCellCHt + maxCellATVSV + i, $"Lần {i + 1}", styles.TextCenterBold);
                    }

                    var startIndex = 3;

                    foreach (var i in result)
                    {
                        var row = sheet.GetRow(startIndex) ?? sheet.CreateRow(startIndex);

                        ExcelNPOIExtention.SetCellValueText(row, 0, i.stt, styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, 1, i.Name, styles.Text);
                        for (int e = 0; e < maxCellCHt; e++)
                        {
                            string value = e < i.Cht.Count() ? i.Cht[e] : "";
                            ExcelNPOIExtention.SetCellValueText(row, 2 + e, value, styles.Number);
                        }
                        for (int f = 0; f < maxCellATVSV; f++)
                        {
                            string value = f < i.Atvsv.Count() ? i.Atvsv[f] : "";
                            ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + 2 + f, value, styles.Number);
                        }
                        for (int f = 0; f < maxCellTCG5s; f++)
                        {
                            string value = f < i.ChuyenGia.Count() ? i.ChuyenGia[f] : "";
                            ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + maxCellATVSV + 2 + f, value, styles.Number);
                        }
                        ExcelNPOIExtention.SetCellValueNumber(row, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, i.Point, styles.Number);
                        ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, i.Cht.Count() + i.Atvsv.Count() + i.ChuyenGia.Count(), styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, i.Cht.Count() + i.Atvsv.Count() + i.ChuyenGia.Count() > 80 ? "Tốt" : i.Cht.Count() + i.Atvsv.Count() + i.ChuyenGia.Count() <= 30 ? "Kém" : "Khá", styles.Text);

                        startIndex++;
                    }
                }

                else if (ReportName == "ChamTheokhungThoiGian")
                {
                    var data = await TheoKhungThoiGian(filterReport);
                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "Chamdiemtheokhungthoigian.xlsx");

                    if (!File.Exists(templatePath))
                    {
                        throw new FileNotFoundException("Không tìm thấy template Excel.", templatePath);
                    }

                    using var file = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    workbook = new XSSFWorkbook(file);

                    var styles = new
                    {
                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = SetCellStyleTextCenter(workbook, true, HorizontalAlignment.Center, VerticalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
                        DecimalNumber = ExcelNPOIExtention.SetCellStyleDecimalNumber(workbook, false, HorizontalAlignment.Right, true)
                    };

                    var sheet = workbook.GetSheetAt(0);
                    var startIndex = 3;
                    foreach (var i in data)
                    {
                        var row = sheet.GetRow(startIndex) ?? sheet.CreateRow(startIndex);

                        ExcelNPOIExtention.SetCellValueText(row, 0, i.stt, styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, 1, i.Name, styles.Text);
                        ExcelNPOIExtention.SetCellValueNumber(row, 2, i.Cht_T ?? 0, styles.DecimalNumber);
                        ExcelNPOIExtention.SetCellValueNumber(row, 3, i.Cht_N ?? 0, styles.DecimalNumber);
                        ExcelNPOIExtention.SetCellValueNumber(row, 4, i.Atvsv_T, styles.DecimalNumber);
                        ExcelNPOIExtention.SetCellValueNumber(row, 5, i.Atvsv_N, styles.DecimalNumber);
                        ExcelNPOIExtention.SetCellValueNumber(row, 6, i.ChuyenGia, styles.DecimalNumber);
                        ExcelNPOIExtention.SetCellValueNumber(row, 7, i.Cht_T + i.Cht_N + i.Atvsv_N + i.Atvsv_T + i.ChuyenGia, styles.DecimalNumber);
                        ExcelNPOIExtention.SetCellValueNumber(row, 8, i.Point, styles.DecimalNumber);
                        ExcelNPOIExtention.SetCellValueText(row, 9, i.Point > 80 ? "Tốt" : i.Point <= 30 ? "kém" : "Khá", styles.Text);

                        startIndex++;
                    }


                }

                else if (ReportName == "ChamTheoYkienDeXuat")
                {
                    var data = await TongHopYKienDeXuat(filterReport);
                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "YKienDeXuat.xlsx");

                    if (!File.Exists(templatePath))
                    {
                        throw new FileNotFoundException("Không tìm thấy template Excel.", templatePath);
                    }

                    using var file = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    workbook = new XSSFWorkbook(file);

                    var styles = new
                    {
                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true, wrap: true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = SetCellStyleTextCenter(workbook, true, HorizontalAlignment.Center, VerticalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
                        DecimalNumber = ExcelNPOIExtention.SetCellStyleDecimalNumber(workbook, false, HorizontalAlignment.Right, true)
                    };

                    var sheet = workbook.GetSheetAt(0);
                    var startIndex = 1;
                    var merge = 0;
                    foreach (var i in data)
                    {
                        var row = sheet.GetRow(startIndex) ?? sheet.CreateRow(startIndex);
                        ExcelNPOIExtention.SetCellValueText(row, 0, i.stt, styles.Text);
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startIndex, startIndex + i.lstTieuChiDeXuat.Count() - 1, 0, 0));
                        SetStyleForMergedRegion(sheet, startIndex, startIndex + i.lstTieuChiDeXuat.Count() - 1, 0, 0, styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, 1, i.Name, styles.Text);
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startIndex, startIndex + i.lstTieuChiDeXuat.Count() - 1, 1, 1));
                        SetStyleForMergedRegion(sheet, startIndex, startIndex + i.lstTieuChiDeXuat.Count() - 1, 1, 1, styles.Text);

                        var startrow = startIndex;
                        foreach (var item in i.lstTieuChiDeXuat)
                        {

                            var rowitem = sheet.GetRow(startrow) ?? sheet.CreateRow(startrow);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 2, item.TieuChi, styles.Text);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 3, item.DeXuat, styles.Text);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 4, item.CanBo, styles.Text);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 5, item.ChucVu, styles.Text);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 6, item.ThoiGian, styles.Text);
                            startrow++;

                        }
                        startIndex = startIndex + i.lstTieuChiDeXuat.Count();
                        merge++;
                    }
                }

                else if (ReportName == "ChamTheoHinhAnh")
                {
                    var data = await BaoCaoHinhAnh(filterReport);
                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "ChamDiemHinhAnh.xlsx");

                    if (!File.Exists(templatePath))
                    {
                        throw new FileNotFoundException("Không tìm thấy template Excel.", templatePath);
                    }

                    using var file = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    workbook = new XSSFWorkbook(file);

                    var styles = new
                    {
                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true, wrap: true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = SetCellStyleTextCenter(workbook, true, HorizontalAlignment.Center, VerticalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
                        DecimalNumber = ExcelNPOIExtention.SetCellStyleDecimalNumber(workbook, false, HorizontalAlignment.Right, true)
                    };

                    var sheet = workbook.GetSheetAt(0);
                    var _urlFile = _config["UrlFile"];

                    var startIndex = 1;
                    var merge = 0;

                    using var httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromSeconds(30);

                    foreach (var i in data)
                    {
                        var row = sheet.GetRow(startIndex) ?? sheet.CreateRow(startIndex);

                        ExcelNPOIExtention.SetCellValueText(row, 0, i.stt, styles.Text);
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startIndex, startIndex + i.lstHinhAnhBaoCao.Count() - 1, 0, 0));
                        SetStyleForMergedRegion(sheet, startIndex, startIndex + i.lstHinhAnhBaoCao.Count() - 1, 0, 0, styles.Text);

                        ExcelNPOIExtention.SetCellValueText(row, 1, i.Name, styles.Text);
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startIndex, startIndex + i.lstHinhAnhBaoCao.Count() - 1, 1, 1));
                        SetStyleForMergedRegion(sheet, startIndex, startIndex + i.lstHinhAnhBaoCao.Count() - 1, 1, 1, styles.Text);

                        var startrow = startIndex;
                        foreach (var item in i.lstHinhAnhBaoCao)
                        {
                            var rowitem = sheet.GetRow(startrow) ?? sheet.CreateRow(startrow);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 2, item.TieuChi, styles.Text);

                            byte[] imgBytes = null;
                            // Tải ảnh từ URL
                            imgBytes = await httpClient.GetByteArrayAsync(_urlFile + item.HinhAnh);
                            // Chèn ảnh vào cell nếu tải được
                            if (imgBytes != null && imgBytes.Length > 0)
                            {
                                InsertImageIntoCell((XSSFWorkbook)workbook, sheet, startrow, 3, imgBytes);
                            }

                            ExcelNPOIExtention.SetCellValueText(rowitem, 4, item.CanBo, styles.Text);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 5, item.ChucVu, styles.Text);
                            ExcelNPOIExtention.SetCellValueText(rowitem, 6, item.ThoiGian, styles.Text);
                            startrow++;

                        }
                        startIndex = startIndex + i.lstHinhAnhBaoCao.Count();
                        merge++;
                    }
                }

                else if (ReportName == "ChamTheoThoigian")
                {
                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", "ChamDiemTheoThietbi.xlsx");

                    if (!File.Exists(templatePath))
                    {
                        throw new FileNotFoundException("Không tìm thấy template Excel.", templatePath);
                    }

                    using var file = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    workbook = new XSSFWorkbook(file);

                    var styles = new
                    {
                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true, wrap: true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = SetCellStyleTextCenter(workbook, true, HorizontalAlignment.Center, VerticalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
                        DecimalNumber = ExcelNPOIExtention.SetCellStyleDecimalNumber(workbook, false, HorizontalAlignment.Right, true)
                    };

                    var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                    var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.IsActive == true && x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                    var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                    var device = _dbContext.tblMdDevice.AsQueryable().ToList();
                    var result = new List<ThoiGianChamDiem>();
                    if (filterReport.SurveyId == "DT1")
                    {
                        var lstDoiTuong = inputKy.Result.lstInputStore.ToList();

                        if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                        {
                            lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                        }


                        foreach (var item in lstDoiTuong)
                        {
                            var report = new ThoiGianChamDiem()
                            {
                                stt = item.StoreId,
                                Name = item.Name,

                                Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT")
                                .Select((x, index) => (x.UpdateDate?.ToString("HH:mm dd/MM/yyyy") ?? ""))
                                .ToList(),
                                Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV")
                                .Select((x, index) => (x.UpdateDate?.ToString("HH:mm dd/MM/yyyy") ?? ""))
                            .ToList(),
                                ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT")
                            .Select((x, index) => (x.UpdateDate?.ToString("HH:mm dd/MM/yyyy") ?? ""))
                         .ToList(),
                                Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0

                            };

                            result.Add(report);
                        }
                    }
                    else if (filterReport.SurveyId == "DT2")
                    {
                        var lstDoiTuong = inputKy.Result.lstInputWareHouse.ToList();

                        if (!string.IsNullOrWhiteSpace(filterReport.DoiTuongId))
                        {
                            lstDoiTuong = lstDoiTuong.Where(x => x.Id == filterReport.DoiTuongId).ToList();
                        }


                        foreach (var item in lstDoiTuong)
                        {
                            var report = new ThoiGianChamDiem()
                            {
                                stt = item.WareHouseId,
                                Name = item.Name,
                                Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT")
                                .Select((x, index) => (x.UpdateDate?.ToString("HH:mm dd/MM/yyyy") ?? ""))
                                .ToList(),
                                Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV")
                                .Select((x, index) => (x.UpdateDate?.ToString("HH:mm dd/MM/yyyy") ?? ""))
                            .ToList(),
                                ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT")
                            .Select((x, index) => (x.UpdateDate?.ToString("HH:mm dd/MM/yyyy") ?? ""))
                         .ToList(),
                                Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0
                            };

                            result.Add(report);
                        }
                    }

                    var sheet = workbook.GetSheetAt(0);
                    var row1 = sheet.GetRow(0) ?? sheet.CreateRow(0);
                    var row2 = sheet.GetRow(1) ?? sheet.CreateRow(1);
                    var row3 = sheet.GetRow(2) ?? sheet.CreateRow(2);
                    var maxCellCHt = result.MaxBy(x => x.Cht?.Count ?? 0).Cht.Count() == 0 ? 1 : result.MaxBy(x => x.Cht?.Count ?? 0).Cht.Count();
                    var maxCellATVSV = result.MaxBy(x => x.Atvsv?.Count ?? 0).Atvsv.Count() == 0 ? 1 : result.MaxBy(x => x.Atvsv?.Count ?? 0).Atvsv.Count();
                    var maxCellTCG5s = result.MaxBy(x => x.ChuyenGia?.Count ?? 0).ChuyenGia.Count() == 0 ? 1 : result.MaxBy(x => x.ChuyenGia?.Count ?? 0).ChuyenGia.Count();



                    //header
                    ExcelNPOIExtention.SetCellValueText(row1, 0, "STT", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, 1, "TÊN ĐƠN VỊ", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, 2, "SỐ LẦN CHẤM ĐIỂM TRONG KỲ", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, "TỔNG", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, "ĐIỂM", styles.Text);
                    ExcelNPOIExtention.SetCellValueText(row1, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, "XẾP LOẠI", styles.Text);
                    //
                    ExcelNPOIExtention.SetCellValueText(row2, 2, "CHT", styles.TextCenterBold);
                    ExcelNPOIExtention.SetCellValueText(row2, maxCellCHt + 2, "ATVSV", styles.TextCenterBold);
                    ExcelNPOIExtention.SetCellValueText(row2, maxCellATVSV + maxCellCHt + 2, "TỔ CHUYÊN GIA, BAN 5S", styles.TextCenterBold);

                    // meage
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 0, 0));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 1, 1));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4));
                    //

                    // row rowend cot so cot


                    // Áp dụng cho các vùng merge
                    SetStyleForMergedRegion(sheet, 0, 2, 0, 0, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, 1, 1, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, styles.TextCenterBold);
                    SetStyleForMergedRegion(sheet, 0, 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, styles.TextCenterBold);

                    if (maxCellCHt > 1)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 2, maxCellCHt + 1));
                        SetStyleForMergedRegion(sheet, 1, 1, 2, maxCellCHt + 1, styles.TextCenterBold);
                    }
                    if (maxCellCHt > 1)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, maxCellCHt + 2, maxCellCHt + maxCellATVSV + 1));
                        SetStyleForMergedRegion(sheet, 1, 1, maxCellCHt + 2, maxCellCHt + maxCellATVSV + 1, styles.TextCenterBold);
                    }
                    if (maxCellTCG5s > 1)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, maxCellCHt + maxCellATVSV + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1));
                        SetStyleForMergedRegion(sheet, 1, 1, maxCellCHt + maxCellATVSV + 2, maxCellCHt + maxCellATVSV + maxCellTCG5s + 1, styles.TextCenterBold);
                    }

                    //
                    for (int i = 0; i < maxCellCHt; i++)
                    {

                        ExcelNPOIExtention.SetCellValueText(row3, 2 + i, $"Lần {i + 1}", styles.TextCenterBold);
                    }
                    for (int i = 0; i < maxCellATVSV; i++)
                    {

                        ExcelNPOIExtention.SetCellValueText(row3, 2 + maxCellCHt + i, $"Lần {i + 1}", styles.TextCenterBold);
                    }
                    for (int i = 0; i < maxCellTCG5s; i++)
                    {
                        ExcelNPOIExtention.SetCellValueText(row3, 2 + maxCellCHt + maxCellATVSV + i, $"Lần {i + 1}", styles.TextCenterBold);
                    }

                    // data

                    var startIndex = 3;

                    foreach (var i in result)
                    {
                        var row = sheet.GetRow(startIndex) ?? sheet.CreateRow(startIndex);

                        ExcelNPOIExtention.SetCellValueText(row, 0, i.stt, styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, 1, i.Name, styles.Text);
                        for (int e = 0; e < maxCellCHt; e++)
                        {
                            string value = e < i.Cht.Count() ? i.Cht[e] : "";
                            ExcelNPOIExtention.SetCellValueText(row, 2 + e, value, styles.Number);
                        }
                        for (int f = 0; f < maxCellATVSV; f++)
                        {
                            string value = f < i.Atvsv.Count() ? i.Atvsv[f] : "";
                            ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + 2 + f, value, styles.Number);
                        }
                        for (int f = 0; f < maxCellTCG5s; f++)
                        {
                            string value = f < i.ChuyenGia.Count() ? i.ChuyenGia[f] : "";
                            ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + maxCellATVSV + 2 + f, value, styles.Number);
                        }
                        ExcelNPOIExtention.SetCellValueNumber(row, maxCellCHt + maxCellATVSV + maxCellTCG5s + 2, i.Point, styles.Number);
                        ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + maxCellATVSV + maxCellTCG5s + 3, i.Cht.Count() + i.Atvsv.Count() + i.ChuyenGia.Count(), styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, maxCellCHt + maxCellATVSV + maxCellTCG5s + 4, i.Cht.Count() + i.Atvsv.Count() + i.ChuyenGia.Count() > 80 ? "Tốt" : i.Cht.Count() + i.Atvsv.Count() + i.ChuyenGia.Count() <= 30 ? "Kém" : "Khá", styles.Text);

                        startIndex++;
                    }


                }

                else
                {
                    this.Status = false;

                    return null;
                }

                var folderPath = Path.Combine($"Uploads/Excel");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var fileName = $"{ReportName}_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";
                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName);
                using var outFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                workbook.Write(outFile);

                return $"{folderPath}/{fileName}";
            }
            catch (Exception ex)
            {
                Exception = ex;
                this.Status = false;
                return null;
            }
        }
        public static void InsertImageIntoCell(XSSFWorkbook workbook, ISheet sheet, int row, int col, byte[] imageBytes)
        {
            // Xác định loại ảnh tự động
            var pictureType = GetPictureType(imageBytes);
            int pictureIdx = workbook.AddPicture(imageBytes, pictureType);

            var helper = workbook.GetCreationHelper();
            var drawing = sheet.CreateDrawingPatriarch();
            var anchor = helper.CreateClientAnchor();

            // Set vị trí và kích thước
            anchor.Col1 = col;
            anchor.Row1 = row;
            anchor.Col2 = col + 1;
            anchor.Row2 = row + 1;

            // Căn giữa ảnh trong cell
            anchor.Dx1 = Units.ToEMU(5); // padding left 5 pixels
            anchor.Dy1 = Units.ToEMU(5); // padding top 5 pixels
            anchor.Dx2 = Units.ToEMU(-5); // padding right 5 pixels
            anchor.Dy2 = Units.ToEMU(-5); // padding bottom 5 pixels

            anchor.AnchorType = AnchorType.MoveAndResize;

            var pict = drawing.CreatePicture(anchor, pictureIdx);

            // Không dùng Resize(1.0) vì nó có thể làm méo ảnh
            // Thay vào đó, set kích thước row/column phù hợp
            sheet.GetRow(row).Height = (short)(100 * 20); // 100 pixels
            sheet.SetColumnWidth(col, 15 * 256); // 15 ký tự
        }

        // Helper method để xác định loại ảnh
        private static PictureType GetPictureType(byte[] imageBytes)
        {
            if (imageBytes.Length < 4) return PictureType.PNG;

            // Check PNG signature
            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return PictureType.PNG;

            // Check JPEG signature
            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                return PictureType.JPEG;

            return PictureType.PNG; // Default
        }

    }
}