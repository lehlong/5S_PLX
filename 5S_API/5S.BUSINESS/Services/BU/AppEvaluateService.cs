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
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface IAppEvaluateService : IGenericService<TblBuEvaluateHeader, EvaluateHeaderDto>
    {
        Task<TieuChiDto> BuildDataTreeForApp(string kiKhaoSatId, string storeId);
        Task<List<TieuChiDto>> GetAllTieuChiLeaves(string kiKhaoSatId, string doiTuongId);
        Task<EvaluateModel> BuildInputEvaluate(string kiKhaoSatId, string doiTuongId, string deviceId);
        Task InsertEvaluate(EvaluateModel data);
        Task<TblBuEvaluateImage> HandelFile(TblBuEvaluateImage request);
        Task<EvaluateModel> GetResultEvaluate(string code);
        Task TinhTongLanCham(TblBuPoint point);
        Task<List<TblBuPoint>> GetPointStore(string kiKhaoSatid, string surveyId);
        Task <List<TblBuNotification>> GetNotification();
        Task HandlePointStore(EvaluateFilter param);
        Task<HomeModel> GetDataHome(string userName);
    }

    public class AppEvaluateService : GenericService<TblBuEvaluateHeader, EvaluateHeaderDto>, IAppEvaluateService
    {
        private readonly IKikhaosatService _kiKhaoSatService;
        private readonly IWebHostEnvironment _environment;

        public AppEvaluateService(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment environment, IKikhaosatService KiKhaoSatService) : base(dbContext, mapper)
        {
            _environment = environment;
            _kiKhaoSatService = KiKhaoSatService;
        }

        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblBuEvaluateHeader.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.DoiTuongId.Contains(filter.KeyWord));
                }
                if (!string.IsNullOrWhiteSpace(filter.SortColumn))
                {
                    query = query.Where(x => x.KiKhaoSatId.Contains(filter.SortColumn));
                }
                if (filter.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == filter.IsActive);
                }
                return await Paging(query, filter);
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }


        public async Task<TieuChiDto> BuildDataTreeForApp(string kiKhaoSatId, string doiTuongId)
        {
            var lstNode = new List<TieuChiDto>();
            var node = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId == "-1" && x.IsDeleted != true).FirstOrDefault();
            var lstBlack = _dbContext.TblBuCriteriaExcludedObject.Where(x => x.IsDeleted != true).ToList();
            var lstAllTieuChi = await _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId != "-1" && x.IsDeleted != true).OrderBy(x => x.OrderNumber).ToListAsync();
            var indexTC = 0;
            var rootNode = new TieuChiDto()
            {
                Code = node.Code,
                Id = node.Id,
                Key = node.Id,   
                Name = node.Name,
                Title = node.Name,
                PId = node.PId,
                IsGroup = node.IsGroup,
                KiKhaoSatId = node.KiKhaoSatId,
                ChiChtAtvsv = node.ChiChtAtvsv,
                OrderNumber = 0,
                IsImg = node.IsImg,
                Report = node.Report,
                Expanded = true,
                IsLeaf = false
            };


            lstNode.Add(rootNode);
            foreach (var menu in lstAllTieuChi)
            {
                var checkBack = lstBlack.FirstOrDefault(x => x.TieuChiCode == menu.Code && x.DoiTuongId == doiTuongId);
                if (checkBack == null)
                {
                    var node1 = new TieuChiDto()
                    {
                        Code = menu.Code,
                        Id = menu.Id,
                        Key = menu.Id,
                        Name = menu.Name,
                        Title = menu.Name,
                        PId = menu.PId,
                        IsGroup = menu.IsGroup,
                        KiKhaoSatId = menu.KiKhaoSatId,
                        OrderNumber = menu.OrderNumber,
                        IsImg = menu.IsImg,
                        ChiChtAtvsv = menu.ChiChtAtvsv,
                        Report = menu.Report,
                        NumberImg = menu.NumberImg ?? 0,
                        Expanded = true,
                        IsLeaf = false,
                        DiemTieuChi = _dbContext.TblBuTinhDiemTieuChi.Where(x => x.TieuChiCode == menu.Code).ToList(),
                    };
                    lstNode.Add(node1);
                }

            }
            var nodeDict = lstNode.ToDictionary(n => n.Id);
            foreach (var item in lstNode)
            {
                if (item.PId == "-1" || !nodeDict.TryGetValue(item.PId, out TieuChiDto parentNode))
                {
                    continue;
                }

                parentNode.Children ??= [];
                parentNode.Children.Add(item);
            }
            return rootNode;

        }

        public async Task<List<TieuChiDto>> GetAllTieuChiLeaves(string kiKhaoSatId, string doiTuongId)
        {
            try
            {
                var tieuChi = _dbContext.TblBuTieuChi.Where(x => x.IsDeleted != true && x.KiKhaoSatId == kiKhaoSatId && x.IsGroup == false).OrderBy(x => x.CreateDate).ToList();
                var lstBlack = _dbContext.TblBuCriteriaExcludedObject.Where(x => x.IsDeleted != true).ToList();
                var lstDiem = _dbContext.TblBuTinhDiemTieuChi.OrderBy(x => x.MoTa).ToList();
                var lstTieuChiLeaves = new List<TieuChiDto>();
                foreach (var item in tieuChi)
                {
                    var checkBack = lstBlack.FirstOrDefault(x => x.TieuChiCode == item.Code && x.DoiTuongId == doiTuongId);
                    if (checkBack == null)
                    {
                        var leaves = new TieuChiDto()
                        {
                            Code = item.Code,
                            Id = item.Id,
                            PId = item.PId,
                            Name = item.Name,
                            Title = item.Name,
                            IsImg = item.IsImg,
                            Report = item.Report,
                            IsGroup = item.IsGroup,
                            NumberImg = item.NumberImg,
                            KiKhaoSatId = item.KiKhaoSatId,
                            ChiChtAtvsv = item.ChiChtAtvsv,
                            OrderNumber = item.OrderNumber,
                            DiemTieuChi = lstDiem.Where(x => x.TieuChiCode == item.Code).ToList(),
                        };
                        lstTieuChiLeaves.Add(leaves);
                    }
                }

                return lstTieuChiLeaves;
            }
            catch (Exception ex)
            {
                Status = false;
                return new List<TieuChiDto>();
            }

        }


        public async Task<EvaluateModel> BuildInputEvaluate(string kiKhaoSatId, string doiTuongId, string deviceId)
        {
            try
            {
                var lstTieuChi = await GetAllTieuChiLeaves(kiKhaoSatId, doiTuongId);
                var idHeader = Guid.NewGuid().ToString();
                return new EvaluateModel()
                {
                    Header = new TblBuEvaluateHeader()
                    {
                        Code = idHeader,
                        Name = "Bản nháp",
                        Point = 0,
                        Order = 0,
                        DoiTuongId = doiTuongId,
                        IsActive = true,
                        KiKhaoSatId = kiKhaoSatId,
                        CreateDate = DateTime.Now,
                        DeviceId= deviceId
                    },
                    LstEvaluate = lstTieuChi.Select(x => new TblBuEvaluateValue
                    {
                        Code = Guid.NewGuid().ToString(),
                        PointId = "",
                        TieuChiCode = x.Code,
                        FeedBack = "",
                        EvaluateHeaderCode = idHeader,

                    }).ToList(),
                };
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<List<TblBuNotification>> GetNotification()
        {
            try
            {
                return _dbContext.TblBuNotification.OrderByDescending(x => x.CreateDate).ToList();
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }

        public async Task InsertEvaluate(EvaluateModel data)
        {
            try
            {
                var dateNow = DateTime.Now;
                if (_dbContext.TblBuKiKhaoSat.Any(x => x.Id == data.Header.KiKhaoSatId && x.EndDate <= dateNow))
                {
                    this.Status = false;
                    return;
                };

                var lstImage = new List<TblBuEvaluateImage>();

                var lstHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == data.Header.KiKhaoSatId && x.DoiTuongId == data.Header.DoiTuongId).ToList();
                var header = lstHeader.OrderByDescending(x => x.Order).FirstOrDefault();

                    
                bool hasFirstPeriod = lstHeader.Any(x =>
                    x.CreateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) &&
                    x.CreateDate <= new DateTime(dateNow.Year, dateNow.Month, 7));

                bool hasSecondPeriod = lstHeader.Any(x =>
                    x.CreateDate >= new DateTime(dateNow.Year, dateNow.Month, 15) &&
                    x.CreateDate <= new DateTime(dateNow.Year, dateNow.Month, 23));


                if (data.Header.ChucVuId == "CHT" || data.Header.ChucVuId == "ATVSV" || data.Header.ChucVuId == "TK")
                {
                    if (((dateNow.Day > 07 && dateNow.Day < 15) || (dateNow.Day >= 15 && dateNow.Day <= 23)) && !hasFirstPeriod)
                    {
                        data.Header.IsActive = false;
                    }
                    else if ((!hasFirstPeriod && !hasSecondPeriod) && (dateNow.Day > 23))
                    {
                        data.Header.IsActive = false;
                    }
                }
                data.Header.Name = "Lần chấm thứ " + (header != null ? header.Order + 1 : 1).ToString();
                data.Header.Order = header != null ? header.Order + 1 : 1;
                data.Header.UpdateDate = DateTime.Now;
                

                _dbContext.TblBuEvaluateHeader.Add(data.Header);

                foreach (var item in data.LstImages)
                {
                    var a = await HandelFile(item);
                    lstImage.Add(a);
                }

                _dbContext.TblBuEvaluateImage.AddRange(lstImage);
                _dbContext.TblBuEvaluateValue.AddRange(data.LstEvaluate);

                await _dbContext.SaveChangesAsync();
                this.Status = true;
            }
            catch (Exception ex)
            {
                this.Status = false;
            }
        }
        public async Task<TblBuEvaluateImage> HandelFile(TblBuEvaluateImage request)
        {
            if (string.IsNullOrEmpty(request.FilePath))
                return null;

            try
            {
                var base64Data = request.FilePath;
                var base64Parts = base64Data.Split(',');

                if (base64Parts.Length != 2)
                    return null;

                var base64String = base64Parts[1];
                var fileBytes = Convert.FromBase64String(base64String);

                var mimeTypePart = base64Parts[0]; // ví dụ: data:image/jpeg;base64
                string extension = GetExtensionFromMimeType(mimeTypePart);
                if (string.IsNullOrEmpty(extension))
                    return null;

                string folder = GetFolderByExtension(extension);

                // Tạo file chính
                var fileName = !string.IsNullOrEmpty(request.FileName)
                    ? Path.GetFileNameWithoutExtension(request.FileName) + extension
                    : $"{Guid.NewGuid()}{extension}";

                var uploadsFolder = Path.Combine(folder);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                // Nếu là ảnh thì tạo thumbnail
                string thumbPath = null;
                string thumbName = null;
                if (mimeTypePart.Contains("image"))
                {
                    var thumbBytes = GenerateThumbnailBytes(fileBytes, 100, 100); // <-- Tạo ảnh nhỏ
                    thumbName = "thumb_" + fileName;
                    thumbPath = Path.Combine(uploadsFolder, thumbName);
                    await System.IO.File.WriteAllBytesAsync(thumbPath, thumbBytes);
                }

                return new TblBuEvaluateImage()
                {
                    Code = Guid.NewGuid().ToString(),
                    FileName = fileName,
                    FilePath = filePath,
                    Type = request.Type,
                    KinhDo = request.KinhDo,
                    ViDo = request.ViDo,
                    EvaluateHeaderCode = request.EvaluateHeaderCode,
                    TieuChiCode = request.TieuChiCode,
                    PathThumbnail = thumbPath,
                    NameThumbnail = thumbName
                };
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }

        public byte[] GenerateThumbnailBytes(byte[] originalBytes, int maxWidth, int maxHeight)
        {
            using (var inputStream = new MemoryStream(originalBytes))
            using (var image = System.Drawing.Image.FromStream(inputStream))
            {
                int width = image.Width;
                int height = image.Height;

                if (width > height)
                {
                    if (width > maxWidth)
                    {
                        height = (int)(height * ((float)maxWidth / width));
                        width = maxWidth;
                    }
                }
                else
                {
                    if (height > maxHeight)
                    {
                        width = (int)(width * ((float)maxHeight / height));
                        height = maxHeight;
                    }
                }

                using (var thumbnail = new System.Drawing.Bitmap(image, width, height))
                using (var outputStream = new MemoryStream())
                {
                    thumbnail.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png);
                    return outputStream.ToArray();
                }
            }
        }

        public async Task<EvaluateModel> GetResultEvaluate(string code)
        {
            try
            {
                return new EvaluateModel()
                {
                    Header = _dbContext.TblBuEvaluateHeader.FirstOrDefault(x => x.Code == code),
                    LstEvaluate = _dbContext.TblBuEvaluateValue.Where(x => x.EvaluateHeaderCode == code).ToList(),
                    LstImages = _dbContext.TblBuEvaluateImage.Where(x => x.EvaluateHeaderCode == code).ToList()
                };
            }
            catch(Exception ex)
            {
                this.Status = false;
                return null;
            }
        }



        private string GetExtensionFromMimeType(string mime)
        {
            if (mime.Contains("image/jpeg")) return ".jpg";
            if (mime.Contains("image/png")) return ".png";
            if (mime.Contains("image/webp")) return ".webp";
            if (mime.Contains("video/mp4")) return ".mp4";
            if (mime.Contains("video/webm")) return ".webm";
            if (mime.Contains("application/pdf")) return ".pdf";
            if (mime.Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document")) return ".docx";
            if (mime.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")) return ".xlsx";
            if (mime.Contains("application/vnd.ms-excel.sheet.macroEnabled.12")) return ".xlsm";
            if (mime.Contains("application/vnd.openxmlformats-officedocument.presentationml.presentation")) return ".pptx";

            return null; // không hỗ trợ
        }

        private string GetFolderByExtension(string ext)
        {
            if (ext.StartsWith(".jpg") || ext.StartsWith(".png") || ext.StartsWith(".webp"))
                return "Uploads/Images";

            if (ext.StartsWith(".mp4") || ext.StartsWith(".webm"))
                return "Uploads/Videos";

            return "Uploads/Files"; // mặc định cho .docx, .xlsx, .pdf, ...
        }


        public async Task TinhTongLanCham(TblBuPoint point)
        {
            try
            {
                var diem = _dbContext.TblBuPoint.FirstOrDefault(x => x.DoiTuongId == point.DoiTuongId && x.KiKhaoSatId == point.KiKhaoSatId && x.SurveyId == point.SurveyId);
                if(diem == null)
                {
                    point.Code = Guid.NewGuid().ToString();
                    _dbContext.TblBuPoint.Add(point);
                }
                else
                {
                    diem.Point = point.Point;
                    diem.Length = point.Length;
                    _dbContext.Update(diem);
                }
                await _dbContext.SaveChangesAsync();
                this.Status = true;
            }
            catch(Exception ex)
            {
                this.Status = false;
                //return null;
            }
        }

        public async Task HandlePointStore(EvaluateFilter param)
        {
            try
            {
                var lstEvaHeader = await _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == param.KiKhaoSatId && x.DoiTuongId == param.DoiTuongId).ToListAsync();
                var lstUser = _dbContext.TblAdAccount.Where(u => param.LstData.Contains(u.UserName)).ToList();
                var filterLst = lstEvaHeader.Where(x => x.IsActive == false).ToList();
                var dateNow = DateTime.Now;
                decimal tong = 0;

                foreach (var item in filterLst)
                {
                    lstEvaHeader.RemoveAll(x => x.AccountUserName == item.AccountUserName);
                }

                foreach (var item in lstUser)
                {
                    decimal tongPointUser = 0;
                    decimal count = 0;

                    if (item.ChucVuId == "CHT" || item.ChucVuId == "ATVSV" || item.ChucVuId == "TK")
                    {

                        bool hasFirstPeriod = lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList().Any(x =>
                            x.CreateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) &&
                            x.CreateDate <= new DateTime(dateNow.Year, dateNow.Month, 7));

                        bool hasSecondPeriod = lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList().Any(x =>
                            x.CreateDate >= new DateTime(dateNow.Year, dateNow.Month, 15) &&
                            x.CreateDate <= new DateTime(dateNow.Year, dateNow.Month, 23));

                        if (dateNow.Day < 23 && hasFirstPeriod && hasSecondPeriod)
                        {
                            foreach (var i in lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList())
                            {
                                tongPointUser = tongPointUser + i.Point;
                                count++;
                            }
                            tong = tong + ((tongPointUser / 2) / count);
                        }
                        else
                        {
                            tong = 0;
                        }

                    }
                    else
                    {
                        foreach (var i in lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList())
                        {
                            tongPointUser = tongPointUser + i.Point;
                            count++;
                        }
                        tong = tong + ((tongPointUser) / count);
                    }
                }

                await TinhTongLanCham(new TblBuPoint
                {
                    Code = "",
                    DoiTuongId = param.DoiTuongId,
                    KiKhaoSatId = param.KiKhaoSatId,
                    SurveyId = param.SurveyId,
                    Point = tong / param.LstData.Count,
                });

            }
            catch (Exception ex)
            {
                this.Status = false;
            }
        }

        public  async Task<List<TblBuPoint>> GetPointStore(string kiKhaoSatid, string surveyId)
        {
            try
            {
                var lstPointStore = new List<TblBuPoint>();
                lstPointStore = await _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == kiKhaoSatid && x.SurveyId == surveyId).ToListAsync();
                return lstPointStore;
            }
            catch(Exception ex)
            {
                this.Status = false;
                return null;
            }
        }

        public async Task<HomeModel> GetDataHome(string userName)
        {
            try
            {
                var dateNow = DateTime.Now;
                bool isScore = true;
                var result = new HomeModel();
                var lstDoiTuong = new List<DoiTuong>();

                var lstInChamDiem = _dbContext.TblBuInputChamDiem.Where(x => x.UserName == userName && x.IsActive == true).ToList();
                var lstPoint = _dbContext.TblBuPoint.OrderBy(x => x.Code).ToList();
                var lstEvaHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.AccountUserName == userName).ToList();

                var lstInStore = _dbContext.TblBuInputStore.Where(x => x.SurveyMgmtId == "03805572-e6b7-4455-90fe-9b6584eef46f").ToList();
                var lstInWareHouse = _dbContext.TblBuInputWareHouse.Where(x => x.SurveyMgmtId == "16d30d78-0b80-4323-bd86-2498aae676a1").ToList();

                var lstStore = _dbContext.tblMdStore.Where(x => x.IsActive == true).ToList();
                var lstWareHouse = _dbContext.TblMdWareHouse.Where(x => x.IsActive == true).ToList();

                var lstKiKhaoSatStore = _dbContext.TblBuKiKhaoSat.Where(x => x.TrangThaiKi == "2" && (x.SurveyMgmtId == "03805572-e6b7-4455-90fe-9b6584eef46f")).ToList();
                var lstKiKhaoSatKho = _dbContext.TblBuKiKhaoSat.Where(x => x.TrangThaiKi == "2" && (x.SurveyMgmtId == "16d30d78-0b80-4323-bd86-2498aae676a1")).ToList();

                //var checkDate = dateNow.Day <= 7 ? 1 : (dateNow.Day > 7 && dateNow.Day < 23) ? 0 : (dateNow.Day > 7 && dateNow.Day < 23)

                foreach (var i in lstKiKhaoSatStore)
                {
                    foreach (var e in lstInStore)
                    {
                        result.LanCham = result.LanCham + lstEvaHeader.Count(x => x.KiKhaoSatId == i.Id && x.DoiTuongId == e.Id && x.IsActive == true);
                        result.ViPham = result.ViPham + lstEvaHeader.Count(x => x.KiKhaoSatId == i.Id && x.DoiTuongId == e.Id && x.IsActive == false);
                        lstDoiTuong.AddRange(lstInChamDiem.Where(x => x.KiKhaoSatId == i.Id && x.InStoreId == e.Id).Select(x => new DoiTuong
                        {
                            Id = e.Id,
                            Name = lstStore.FirstOrDefault(_x => _x.Id == e.StoreId).Name,
                            FDate = i.StartDate,
                            Type = "DT1",
                            Point = lstPoint.FirstOrDefault(y => y.DoiTuongId == e.Id && y.KiKhaoSatId == i.Id)?.Point ?? 0,
                            KiKhaoSatName = i.Name,
                            KiKhaoSatId = i.Id,

                        }).ToList());
                    }
                }
                foreach (var i in lstKiKhaoSatKho)
                {
                    foreach (var e in lstInWareHouse)
                    {
                        result.LanCham = result.LanCham + lstEvaHeader.Count(x => x.KiKhaoSatId == i.Id && x.DoiTuongId == e.Id && x.IsActive == true);
                        result.ViPham = result.ViPham + lstEvaHeader.Count(x => x.KiKhaoSatId == i.Id && x.DoiTuongId == e.Id && x.IsActive == false);

                        lstDoiTuong.AddRange(lstInChamDiem.Where(x => x.KiKhaoSatId == i.Id && x.InStoreId == e.Id).Select(x => new DoiTuong
                        {
                            Id = e.Id,
                            Name = lstWareHouse.FirstOrDefault(_x => _x.Id == e.WareHouseId).Name,
                            FDate = i.StartDate,
                            Type = "DT2",
                            Point = lstPoint.FirstOrDefault(y => y.DoiTuongId == e.Id && y.KiKhaoSatId == i.Id)?.Point ?? 0,
                            KiKhaoSatName = i.Name,

                            KiKhaoSatId = i.Id,
                        }).ToList());
                    }
                }
                result.LstDoiTuong.AddRange(lstDoiTuong);

                result = await HandleLanCham(result, userName);

                return result;
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }

        public async Task<HomeModel> HandleLanCham(HomeModel data, string userName)
        {
            try
            {
                //var dateNow = new DateTime(2025, 7, 24);
                var dateNow = DateTime.Now;
                var lstEvaHeader = await _dbContext.TblBuEvaluateHeader.Where(x => x.AccountUserName == userName && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1)).ToListAsync();
                
                if (dateNow.Day <= 7)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        if (item.FDate <= new DateTime(dateNow.Year, dateNow.Month, 1) || lstEvaHeader.Any(x => x.DoiTuongId == item.Id && x.KiKhaoSatId == item.KiKhaoSatId && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) && x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7)))
                        {
                            item.IsScore = false;
                        }
                        else
                        {
                            item.IsScore = true;
                            data.ChuaCham = data.ChuaCham + 1;
                        }
                    }
                } 
                else if (dateNow.Day > 7 && dateNow.Day < 15)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        if (item.FDate <= new DateTime(dateNow.Year, dateNow.Month, 1) || lstEvaHeader.Any(x => x.DoiTuongId == item.Id && x.KiKhaoSatId == item.KiKhaoSatId && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) && x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7)))
                        {
                            item.IsScore = false;
                        }
                        else
                        {
                            item.IsScore = false;
                            data.ViPham = data.ViPham + 1;
                        }
                    }
                }
                else if (dateNow.Day <= 23 && dateNow.Day >= 15)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        if (item.FDate <= new DateTime(dateNow.Year, dateNow.Month, 1) || lstEvaHeader.Any(x => x.DoiTuongId == item.Id && x.KiKhaoSatId == item.KiKhaoSatId && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 15) && x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 23)))
                        {
                            item.IsScore = false;
                        }
                        else
                        {
                            item.IsScore = true;
                            data.ChuaCham = data.ChuaCham + 1;
                        }
                    }
                }
                else if (dateNow.Day > 23)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        if (item.FDate <= new DateTime(dateNow.Year, dateNow.Month, 1) || lstEvaHeader.Any(x => x.DoiTuongId == item.Id && x.KiKhaoSatId == item.KiKhaoSatId && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 15) && x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 23)))
                        {
                            item.IsScore = false;
                        }
                        else
                        {
                            item.IsScore = false;
                            data.ViPham = data.ViPham + 1;
                        }
                    }
                }

                return data;
            }
            catch(Exception ex)
            {
                this.Status = false;
                return null;
            }
        }



    }
}