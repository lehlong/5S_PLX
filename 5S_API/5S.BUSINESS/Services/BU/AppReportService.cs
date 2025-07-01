
using AutoMapper;
using Common;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
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
        Task<string> ExportExcel(string ReportName, FilterReport filterReport);
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
                var result = new List<KetQuaChamDiem>();
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
                if(filterReport.SurveyId == "DT1")
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
                            Name= item.Name,
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
                            Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " + x.UpdateDate).ToList(),
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
                            Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => "L" + (index + 1) + " " + x.UpdateDate).ToList(),
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
                var lstPoint = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();
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
                                CanBo = item.AccountUserName,
                                ChucVu = item?.ChucVuId,
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
                                CanBo = item.AccountUserName,
                                ChucVu = item.ChucVuId,
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

        public async Task<string> ExportExcel(string ReportName, FilterReport filterReport)
        {
            try
            {

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


                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
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
                        ExcelNPOIExtention.SetCellValueNumber(row, 3, i.point , styles.Number);
                        ExcelNPOIExtention.SetCellValueText(row, 4, "", styles.Text);
                        ExcelNPOIExtention.SetCellValueText(row, 5, "", styles.Text);

                        startIndex++;
                    }
                }

                else if (ReportName == "ChamTheoThietBi")
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


                        Text = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Left, true),
                        TextRight = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Right, true),
                        TextBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Left, true),
                        TextCenter = ExcelNPOIExtention.SetCellStyleText(workbook, false, HorizontalAlignment.Center, false),
                        TextCenterBold = ExcelNPOIExtention.SetCellStyleText(workbook, true, HorizontalAlignment.Center, false),
                        Number = ExcelNPOIExtention.SetCellStyleNumber(workbook, false, HorizontalAlignment.Right, true),
                        NumberBold = ExcelNPOIExtention.SetCellStyleNumber(workbook, true, HorizontalAlignment.Right, true),
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
                                Cht = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "CHT").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                Atvsv = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId == "ATVSV").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                ChuyenGia = lstEvaHeader.Where(x => x.DoiTuongId == item.Id && x.ChucVuId != "ATVSV" && x.ChucVuId != "CHT").Select((x, index) => ((device.FirstOrDefault(y => y.Id == x.DeviceId)?.MainDevice == true) ? "Chính" : "Khác")).ToList(),
                                Point = lstPoint.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                            };

                            result.Add(report);
                        }
                    }

                }
                else if(ReportName == "ChamTheokhungThoiGian")
                {

                }
                else if (ReportName == "ChamTheoYkienDeXuat")
                {

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
                {
                    this.Status = false;
                    return null;
                }
            }
        }



    }
}