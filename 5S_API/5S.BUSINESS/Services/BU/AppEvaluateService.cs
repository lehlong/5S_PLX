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
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface IAppEvaluateService : IGenericService<TblBuEvaluateHeader, EvaluateHeaderDto>
    {
        Task<TieuChiDto> BuildDataTreeForApp(string kiKhaoSatId, string storeId);
        Task<List<TieuChiDto>> GetAllTieuChiLeaves(string kiKhaoSatId, string storeId);
        Task<EvaluateModel> BuildInputEvaluate(string kiKhaoSatId, string storeId,string deviceId);
        Task InsertEvaluate(EvaluateModel data);
        Task<TblBuEvaluateImage> HandelFile(TblBuEvaluateImage request);
        Task<EvaluateModel> GetResultEvaluate(string code);
        Task TinhTongLanCham(TblBuPointStore point);
        Task<List<TblBuPointStore>> GetPointStore(string kiKhaoSatid, string surveyId);
        Task<List<TblBuEvaluateHeader>> FilterLstChamDiem(BaseFilter filter);
    }

    public class AppEvaluateService : GenericService<TblBuEvaluateHeader, EvaluateHeaderDto>, IAppEvaluateService
    {
        private readonly IWebHostEnvironment _environment;

        public AppEvaluateService(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment environment) : base(dbContext, mapper)
        {
            _environment = environment;
        }

        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblBuEvaluateHeader.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.StoreId.Contains(filter.KeyWord));
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


        public async Task<TieuChiDto> BuildDataTreeForApp(string kiKhaoSatId, string storeId)
        {
            var lstNode = new List<TieuChiDto>();
            var node = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId == "-1" && x.IsDeleted != true).FirstOrDefault();
            var lstBlack = _dbContext.TblBuCriteriaExcludedStores.Where(x => x.IsDeleted != true).ToList();
            var lstAllTieuChi = await _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId != "-1" && x.IsDeleted != true).OrderBy(x => x.OrderNumber).ToListAsync();

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
                OrderNumber = 0,
                IsImg = node.IsImg,
                Report = node.Report,
                Expanded = true,
                IsLeaf = false
            };


            lstNode.Add(rootNode);
            foreach (var menu in lstAllTieuChi)
            {
                var checkBack = lstBlack.FirstOrDefault(x => x.TieuChiCode == menu.Code && x.StoreId == storeId);
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

        public async Task<List<TieuChiDto>> GetAllTieuChiLeaves(string kiKhaoSatId, string storeId)
        {
            try
            {
                var tieuChi = _dbContext.TblBuTieuChi.Where(x => x.IsDeleted != true && x.KiKhaoSatId == kiKhaoSatId && x.IsGroup == false).OrderBy(x => x.Id).ToList();
                var lstBlack = _dbContext.TblBuCriteriaExcludedStores.Where(x => x.IsDeleted != true).ToList();
                var lstDiem = _dbContext.TblBuTinhDiemTieuChi.OrderBy(x => x.MoTa).ToList();
                var lstTieuChiLeaves = new List<TieuChiDto>();
                foreach (var item in tieuChi)
                {
                    var checkBack = lstBlack.FirstOrDefault(x => x.TieuChiCode == item.Code && x.StoreId == storeId);
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


        public async Task<EvaluateModel> BuildInputEvaluate(string kiKhaoSatId, string storeId, string deviceId)
        {
            try
            {
                var idStore = _dbContext.TblBuInputStore.FirstOrDefault(x => x.Id == storeId).StoreId;
                var nameStore = _dbContext.tblMdStore.FirstOrDefault(x => x.Id == idStore).Name;
                var lstTieuChi = await GetAllTieuChiLeaves(kiKhaoSatId, storeId);
                var idHeader = Guid.NewGuid().ToString();
                return new EvaluateModel()
                {
                    Header = new TblBuEvaluateHeader()
                    {
                        Code = idHeader,
                        Name = "Bản nháp",
                        Point = 0,
                        Order = 0,
                        StoreId = storeId,
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

        public async Task InsertEvaluate(EvaluateModel data)
        {
            try
            {
                var lstImage = new List<TblBuEvaluateImage>();

                var header = _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == data.Header.KiKhaoSatId && x.StoreId == data.Header.StoreId).OrderByDescending(x => x.Order).FirstOrDefault();
                var number = header != null ? header.Order + 1 : 1;

                var dateNow = DateTime.Now;
                data.Header.IsActive = true;

                if (data.Header.ChucVuId == "CHT" || data.Header.ChucVuId == "ATVSV")
                {
                    if (dateNow.Day > 07 && dateNow.Day < 15 || dateNow.Day > 23)
                    {
                        data.Header.IsActive = false;
                    }
                }
                data.Header.Name = "Lần chấm thứ " + (number).ToString();
                data.Header.Order = number;
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


        public async Task TinhTongLanCham(TblBuPointStore point)
        {
            try
            {
                var diem = _dbContext.TblBuPointStore.FirstOrDefault(x => x.InStoreId == point.InStoreId && x.KiKhaoSatId == point.KiKhaoSatId && x.SurveyId == point.SurveyId);
                if(diem == null)
                {
                    point.Code = Guid.NewGuid().ToString();
                    _dbContext.TblBuPointStore.Add(point);
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

        public async Task<List<TblBuEvaluateHeader>> FilterLstChamDiem(BaseFilter filter)
        {
            try
            {
                var lstEvaHeader = await _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == filter.KeyWord && x.StoreId == filter.SortColumn).ToListAsync();
                var filterLst = lstEvaHeader.Where(x => x.IsActive == false).ToList();

                foreach (var item in filterLst)
                {
                    lstEvaHeader.RemoveAll(x => x.AccountUserName == item.AccountUserName);
                }
                return lstEvaHeader;
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }

        public  async Task<List<TblBuPointStore>> GetPointStore(string kiKhaoSatid, string surveyId)
        {
            try
            {
                var lstPointStore = new List<TblBuPointStore>();
                lstPointStore = await _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == kiKhaoSatid && x.SurveyId == surveyId).ToListAsync();
                return lstPointStore;
            }
            catch(Exception ex)
            {
                this.Status = false;
                return null;
            }
        }


    }
}