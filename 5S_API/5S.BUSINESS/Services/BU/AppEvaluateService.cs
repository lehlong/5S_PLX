using AutoMapper;
using Common;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE;
using PLX5S.CORE.Entities.AD;
using PLX5S.CORE.Entities.BU;
using PLX5S.CORE.Statics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Serialization;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface IAppEvaluateService : IGenericService<TblBuEvaluateHeader, EvaluateHeaderDto>
    {
        Task<TieuChiDto> BuildDataTreeForApp(string kiKhaoSatId, string storeId);
        Task<List<TieuChiDto>> GetAllTieuChiLeaves(string kiKhaoSatId, string doiTuongId);
        Task<EvaluateModel> BuildInputEvaluate(string kiKhaoSatId, string doiTuongId, string deviceId);
        Task<object> UploadFile(IFormFile request);
        Task InsertEvaluate(EvaluateModel data);
        Task InsertEvaluate2(EvaluateModel data);
        Task<TblBuEvaluateImage> HandelFile(TblBuEvaluateImage request);
        Task<EvaluateModel> GetResultEvaluate(string code);
        Task TinhTongLanCham(TblBuPoint point);
        Task<List<TblBuPoint>> GetPointStore(string kiKhaoSatid, string surveyId);
        Task<List<TblBuNotification>> GetNotification();
        Task HandlePointStore(EvaluateFilter param);
        Task HandlePointStore2(EvaluateFilter param);
        Task<HomeModel> GetDataHome(string userName);
        Task<List<TblBuEvaluateImage>> UploadFileOffline(List<FileModel> files);
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
            var lstAllTieuChi = await _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId != "-1" && x.IsDeleted != true).OrderBy(x => x.OrderNumber).ToListAsync();
            var lstBlack = _dbContext.TblBuCriteriaExcludedObject.Where(x => x.IsDeleted != true && x.DoiTuongId == doiTuongId && lstAllTieuChi.Select(x => x.Code).Contains(x.TieuChiCode)).ToList();
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
                var checkBlack = lstBlack.FirstOrDefault(x => x.TieuChiCode == menu.Code);
                if (checkBlack == null)
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
                        DiemTieuChi = _dbContext.TblBuTinhDiemTieuChi.Where(x => x.TieuChiCode == menu.Code).OrderByDescending(x => x.Diem).ToList(),
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
                var lstBlack = _dbContext.TblBuCriteriaExcludedObject.Where(x => x.IsDeleted != true && x.DoiTuongId == doiTuongId && tieuChi.Select(x => x.Code).Contains(x.TieuChiCode)).ToList();
                var lstDiem = _dbContext.TblBuTinhDiemTieuChi.Where(x => tieuChi.Select(x => x.Code).Contains(x.TieuChiCode)).OrderBy(x => x.MoTa).ToList();
                var lstTieuChiLeaves = new List<TieuChiDto>();
                foreach (var item in tieuChi)
                {
                    var checkBlack = lstBlack.FirstOrDefault(x => x.TieuChiCode == item.Code);
                    if (checkBlack == null)
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
                        DeviceId = deviceId
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

                string folder = GetFolderByExtension();

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

        public async Task<object> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                string extension = Path.GetExtension(file.FileName).ToLower();
                string folder = GetFolderByExtension();

                // Tạo tên file
                string physicalFileName = BuildFileName(file.FileName, extension);
                //string newFileName = $"{Guid.NewGuid()}{extension}";
                string thumbFileName = $"{Guid.NewGuid()}_thumb{extension}";

                // Đường dẫn vật lý
                string fullFolderPath = Path.Combine(folder.Replace("/", "\\"));

                if (!Directory.Exists(fullFolderPath))
                    Directory.CreateDirectory(fullFolderPath);

                // Đường dẫn file gốc
                string filePath = Path.Combine(fullFolderPath, physicalFileName);

                // Ghi file gốc
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string thumbPath = null;

                bool isImage = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" }.Contains(extension);
                // === TẠO THUMBNAIL nếu là file ảnh ===
                if (isImage)
                {
                    try
                    {
                        thumbPath = Path.Combine(fullFolderPath, thumbFileName);

                        using (var img = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream()))
                        {
                            img.Mutate(x => x
                                .Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
                                {
                                    Size = new SixLabors.ImageSharp.Size(100, 100),
                                    Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop
                                })
                            );

                            await img.SaveAsync(thumbPath);
                        }
                    }
                    catch { }
                }

                // Trả về URL public
                //string baseUrl = $"{Request.Scheme}://{Request.Host}/";

                return new TblBuEvaluateImage
                {
                    Code = Guid.NewGuid().ToString(),
                    FilePath = $"{fullFolderPath}/{physicalFileName}",
                    FileName = file.FileName,
                    NameThumbnail = thumbFileName,
                    PathThumbnail = thumbPath != null ? $"{fullFolderPath}/{thumbFileName}" : null,
                    Type = extension,
                    KinhDo = 0,
                    ViDo = 0
                };
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }
        public async Task<List<TblBuEvaluateImage>> UploadFileOffline(List<FileModel> files)
        {
            var result = new List<TblBuEvaluateImage>();

            try
            {
                foreach (var file in files)
                {
                    if (file == null || file.file == null || file.file.Length == 0)
                        continue;

                    string code = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    string folder = GetFolderByExtension();

                    // --- Tạo tên file ---
                    string physicalFileName = BuildFileName(file.FileName, extension);
                    string thumbFileName = $"{code}_thumb{extension}";

                    // --- Đường dẫn thư mục ---
                    string fullFolderPath = Path.Combine(folder.Replace("/", "\\"));

                    if (!Directory.Exists(fullFolderPath))
                        Directory.CreateDirectory(fullFolderPath);

                    // --- Đường dẫn file gốc ---
                    string filePath = Path.Combine(fullFolderPath, physicalFileName);

                    // --- Ghi file gốc ---
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.file.CopyToAsync(stream);
                    }

                    string thumbPath = Path.Combine(fullFolderPath, thumbFileName);

                    // --- Chỉ tạo thumbnail nếu là file ảnh ---
                    bool isImage = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" }.Contains(extension);

                    if (isImage)
                    {
                        try
                        {
                            // Reset stream để load lại ảnh
                            //file.file.ContentDisposition = 0;

                            using (var img = await SixLabors.ImageSharp.Image.LoadAsync(file.file.OpenReadStream()))
                            {
                                img.Mutate(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
                                {
                                    Size = new SixLabors.ImageSharp.Size(100, 100),
                                    Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop
                                }));

                                await img.SaveAsync(thumbPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Xóa file thumb nếu lỗi
                            if (System.IO.File.Exists(thumbPath))
                                System.IO.File.Delete(thumbPath);

                            Console.WriteLine("Thumbnail error: " + ex.Message);
                        }
                    }

                    result.Add(new TblBuEvaluateImage
                    {
                        Code = code,
                        FileName = file.FileName,
                        FilePath = $"{fullFolderPath}/{physicalFileName}",
                        NameThumbnail = isImage ? thumbFileName : null,
                        PathThumbnail = isImage ? $"{fullFolderPath}/{thumbFileName}" : null,
                        Type = extension,
                        KinhDo = ToDecimal(file.KinhDo),
                        ViDo = ToDecimal(file.ViDo),
                        TieuChiCode = file.TieuChiCode,
                        EvaluateHeaderCode = file.EvaluateHeaderCode
                    });
                }

                await _dbContext.TblBuEvaluateImage.AddRangeAsync(result);
                await _dbContext.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                // Nếu lỗi → rollback file
                foreach (var item in result)
                {
                    string filePath = item.FilePath?.Replace("/", "\\");
                    string thumbPath = item.PathThumbnail?.Replace("/", "\\");

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);

                    if (System.IO.File.Exists(thumbPath))
                        System.IO.File.Delete(thumbPath);
                }

                this.Status = false;
                this.Exception = ex;
                return null;
            }
        }

        private string BuildFileName(string originalFileName, string ext)
        {
            var name = Path.GetFileNameWithoutExtension(originalFileName);

            // loại ký tự không hợp lệ
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            string time = DateTime.Now.ToString("yyyyMMddHHmmss");

            return $"{name}_{time}{ext}";
        }

        private decimal? ToDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Replace comma with dot to avoid culture issues
            value = value.Replace(",", ".");

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;

            return null;
        }

        private string GetFolderByExtension()
        {
            var folderName = DateTime.Now.ToString("yyyyMM");
            return Path.Combine("Uploads", folderName);
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
            catch (Exception ex)
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


        public async Task TinhTongLanCham(TblBuPoint point)
        {
            try
            {
                var diem = _dbContext.TblBuPoint.FirstOrDefault(x => x.DoiTuongId == point.DoiTuongId && x.KiKhaoSatId == point.KiKhaoSatId && x.SurveyId == point.SurveyId);
                if (diem == null)
                {
                    point.Code = Guid.NewGuid().ToString();
                    point.Description = point.Description;
                    await _dbContext.TblBuPoint.AddAsync(point);
                }
                else
                {
                    diem.Point = point.Point;
                    diem.Description = point.Description;
                    diem.Length = point.Length;
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.Status = false;
                //return null;
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
                }

                var lstImage = new List<TblBuEvaluateImage>();

                var lstHeader = _dbContext.TblBuEvaluateHeader.Where(x =>
                                    x.KiKhaoSatId == data.Header.KiKhaoSatId &&
                                    x.DoiTuongId == data.Header.DoiTuongId &&
                                    x.IsActive == true).ToList();
                var header = lstHeader.OrderByDescending(x => x.Order).FirstOrDefault();


                bool dot1 = lstHeader.Any(x =>
                    x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) &&
                    x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7));

                bool dot2 = lstHeader.Any(x =>
                    x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 8) &&
                    x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 15));

                bool dot3 = lstHeader.Any(x =>
                    x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 16) &&
                    x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 23));

                if (data.Header.ChucVuId == "CHT" || data.Header.ChucVuId == "TK")
                {
                    if (((dateNow.Day >= 08 && dateNow.Day <= 15) || (dateNow.Day >= 16 && dateNow.Day <= 23)) && !dot1)
                    {
                        data.Header.IsActive = false;
                    }
                    else if ((!dot1 && !dot3) && (dateNow.Day > 23))
                    {
                        data.Header.IsActive = false;
                    }
                }
                else if (data.Header.ChucVuId == "ATVSV" && dateNow.Day >= 16 && !dot2)
                {
                    data.Header.IsActive = false;
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


        //CHT, TK : 1 - 7
        // avtsv : 8 - 15
        //CHT, TK : 16 - 23
        // atvsv : 24 - end
        public async Task InsertEvaluate2(EvaluateModel data)
        {
            try
            {
                var now = DateTime.Now;
                //var now = new DateTime(2025, 12, 20);

                var user = _dbContext.TblAdAccount.FirstOrDefault(x => x.UserName == data.Header.AccountUserName);
                if (user.ChucVuId == null)
                {
                    Status = false;
                    MessageObject.Message = "Bạn chưa được phân chức vụ!!";
                    return;
                }
                // 2️⃣ Lấy order lần chấm
                int order = _dbContext.TblBuEvaluateHeader
                            .Count(x =>
                                x.KiKhaoSatId == data.Header.KiKhaoSatId &&
                                x.DoiTuongId == data.Header.DoiTuongId);

                // 3️⃣ Gán thông tin hệ thống
                data.Header.ChucVuId = user.ChucVuId;
                data.Header.Order = order + 1;
                data.Header.Name = $"Lần chấm thứ {(order + 1).ToString()}";
                data.Header.UpdateDate = now;

                // 4️⃣ Insert
                await _dbContext.TblBuEvaluateHeader.AddAsync(data.Header);
                await _dbContext.TblBuEvaluateImage.AddRangeAsync(data.LstImages);
                await _dbContext.TblBuEvaluateValue.AddRangeAsync(data.LstEvaluate);

                await _dbContext.SaveChangesAsync();
                Status = true;
            }
            catch (Exception ex)
            {
                Status = false;
                MessageObject.MessageDetail = "Chấm điểm thất bại!";
                Exception = ex;
            }
        }


        public async Task HandlePointStore(EvaluateFilter param)
        {
            try
            {
                var lstEvaHeader = await _dbContext.TblBuEvaluateHeader.Where(x => x.KiKhaoSatId == param.KiKhaoSatId && x.DoiTuongId == param.DoiTuongId).ToListAsync();
                var lstUser = _dbContext.TblAdAccount.Where(u => param.LstData.Contains(u.UserName)).ToList();
                var checkViPham = lstEvaHeader.Any(x => x.IsActive == false);
                var dateNow = DateTime.Now;
                decimal tongChuyenVien = 0;
                decimal tong = 1;
                decimal tongPointUser = 0;
                decimal count = 0;
                decimal countChuyenVien = 0;

                if (checkViPham)
                {
                    tong = 0;
                }
                else
                {
                    foreach (var item in lstUser)
                    {
                        if (item.ChucVuId == "CHT" || item.ChucVuId == "TK")
                        {
                            bool dot1 = lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList().Any(x =>
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7));

                            bool dot3 = lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList().Any(x =>
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 16) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 23));

                            if ((dateNow.Day < 16 && dot1) || (dateNow.Day >= 16 && dot1 && dot3))
                            {
                                foreach (var i in lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList())
                                {
                                    tongPointUser = tongPointUser + i.Point;
                                    count++;
                                }
                            }
                            else
                            {
                                tong = 0;
                                break;
                            }

                        }
                        else if (item.ChucVuId == "ATVSV")
                        {
                            bool dot2 = lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList().Any(x =>
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 8) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 15));

                            bool dot4 = lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList().Any(x =>
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 24));

                            if (dateNow.Day <= 7 || (dateNow.Day >= 8 && dateNow.Day <= 23 && dot2) || (dateNow.Day >= 24 && dot4 && dot2))
                            {
                                foreach (var i in lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList())
                                {
                                    tongPointUser = tongPointUser + i.Point;
                                    count++;
                                }
                            }
                            else
                            {
                                tong = 0;
                                break;
                            }
                        }
                        else
                        {
                            foreach (var i in lstEvaHeader.Where(x => x.AccountUserName == item.UserName).ToList())
                            {
                                tongChuyenVien = tongChuyenVien + i.Point;
                                countChuyenVien++;
                            }
                        }

                    }
                    if (count == 0 || countChuyenVien == 0)
                    {
                        tong = 0;
                    }
                    else
                    {
                        tongChuyenVien = tongChuyenVien / (countChuyenVien);
                        tongPointUser = tongPointUser / count;

                        tong = (tong * (tongPointUser + tongChuyenVien)) / 2;
                    }
                }
                await TinhTongLanCham(new TblBuPoint
                {
                    Code = "",
                    DoiTuongId = param.DoiTuongId,
                    KiKhaoSatId = param.KiKhaoSatId,
                    Length = countChuyenVien + count,
                    SurveyId = param.SurveyId,
                    Point = tong,
                });
            }
            catch (Exception ex)
            {
                this.Status = false;
            }
        }

        public async Task HandlePointStore2(EvaluateFilter param)
        {
            var now = DateTime.Now;
            string description = "";
            decimal tongDiem = 1;

            var headers = await _dbContext.TblBuEvaluateHeader
                .Where(x => x.KiKhaoSatId == param.KiKhaoSatId &&
                            x.DoiTuongId == param.DoiTuongId &&
                            x.IsActive == true)
                .ToListAsync();

            var users = await _dbContext.TblAdAccount
                .Where(u => param.LstData.Contains(u.UserName))
                .ToListAsync();

            var rolesInStore = users.Select(x => x.ChucVuId).Distinct().ToList();


            // =============================
            // 1) XÁC ĐỊNH NHÓM QUẢN LÝ
            // =============================
            List<string> groupRoles = new();

            if (rolesInStore.Contains(RoleIds.CHT))
                groupRoles.Add(RoleIds.CHT);
            else if (rolesInStore.Contains(RoleIds.TK))
                groupRoles.Add(RoleIds.TK);

            groupRoles.Add(RoleIds.ATVSV);

            // =============================
            // 3) KIỂM TRA CHUYÊN QUẢN (CQ)
            // =============================
            var cqUsers = users.Where(u => u.ChucVuId == RoleIds.CQ).ToList();
            var headerCQ = headers.Any(x => x.ChucVuId == RoleIds.CQ);
            if (!headerCQ)
            {
                if (cqUsers.Any())
                {
                    tongDiem = 0;
                    description += "Chuyên quản Chưa chấm | ";
                }
                else
                {
                    description += "Thiếu chức vụ Chuyên quản | ";
                }

            }


            // =============================
            // 2) KIỂM TRA NHÓM QUẢN LÝ + ATVSV
            // =============================
            foreach (var roleId in groupRoles)
            {
                var roleUsers = users.Where(u => u.ChucVuId == roleId).ToList();

                if (CheckViolationByGroup(roleId, headers, now, out var reason))
                {
                    tongDiem = 0;
                    description += reason;
                    goto TINH_DIEM;
                }
                else
                {
                    description += reason;
                }
            }



        TINH_DIEM:

            if (tongDiem > 0)
                tongDiem = TinhDiemTong(headers);

            await TinhTongLanCham(new TblBuPoint
            {
                KiKhaoSatId = param.KiKhaoSatId,
                DoiTuongId = param.DoiTuongId,
                SurveyId = param.SurveyId,
                Point = tongDiem,
                Description = description,
                Length = headers.Count
            });
        }


        private decimal TinhDiemTong(List<TblBuEvaluateHeader> headers)
        {
            var diemQuanLy = headers
                .Where(x => DotChamHelper.BatBuocTheoChucVu.ContainsKey(x.ChucVuId))
                .Select(x => x.Point)
                .DefaultIfEmpty(0)
                .Average();

            var diemChuyenVien = headers
                .Where(x => !DotChamHelper.BatBuocTheoChucVu.ContainsKey(x.ChucVuId))
                .Select(x => x.Point)
                .DefaultIfEmpty(0)
                .Average();

            if (diemQuanLy == 0 && diemChuyenVien == 0)
                return 0;

            return (diemQuanLy + diemChuyenVien) / 2;
        }

        private bool CheckViolationByGroup(string roleId, List<TblBuEvaluateHeader> allHeaders, DateTime now, out string description)
        {
            description = string.Empty;

            // Không có rule → không vi phạm
            if (!DotChamHelper.BatBuocTheoChucVu.TryGetValue(roleId, out var batBuocDots))
                return false;

            var nowDot = DotChamHelper.GetDot(now);

            // Những đợt bắt buộc đã qua
            var requiredDots = batBuocDots.Where(d => d <= nowDot).ToList();

            // Lấy tất cả headers của role này (một hoặc nhiều người)
            var headersOfRole = allHeaders
                .Where(x => x.ChucVuId == roleId && x.UpdateDate != null)
                .ToList();

            // Danh sách đợt đã có người trong nhóm chấm
            var dotsDaCham = headersOfRole
                .Select(x => DotChamHelper.GetDot(x.UpdateDate.Value))
                .Distinct()
                .ToList();

            // Kiểm tra thiếu đợt nào
            var thieuDots = requiredDots
                .Where(d => !dotsDaCham.Contains(d))
                .ToList();

            if (thieuDots.Any())
            {
                var roleName = DotChamHelper.GetRoleName(roleId);
                var dotNames = thieuDots.Select(d => $"Đợt {DotChamHelper.ToNumber(d)}");

                description = $"{roleName} chấm thiếu: {string.Join(", ", dotNames)} | ";
                return true;
            }

            return false;
        }


        public async Task<List<TblBuPoint>> GetPointStore(string kiKhaoSatid, string surveyId)
        {
            try
            {
                var lstPointStore = new List<TblBuPoint>();
                lstPointStore = await _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == kiKhaoSatid && x.SurveyId == surveyId).ToListAsync();
                return lstPointStore;
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }


        public async Task<HomeModel> GetDataHome(string userName)
        {
            try
            {
                var result = new HomeModel();
                var now = DateTime.Now;
                //var now = new DateTime(2026, 01, 8);
                var user = _dbContext.TblAdAccount.FirstOrDefault(x => x.UserName == userName);
                var dot = GetDotInfo(now, user.ChucVuId);

                var lstSurvey = _dbContext.TblBuSurveyMgmt.OrderBy(x => x.Id).ToList();
                var lstKy = _dbContext.TblBuKiKhaoSat.Where(x => x.TrangThaiKi == "2" && x.EndDate.Month == now.Month).ToList();

                var lstHeader = _dbContext.TblBuEvaluateHeader.Where(x => x.AccountUserName == userName && lstKy.Select(x => x.Id).Contains(x.KiKhaoSatId)).ToList();
                var lstChamDiem = _dbContext.TblBuInputChamDiem.Where(x => x.UserName == userName && lstKy.Select(x => x.Id).Contains(x.KiKhaoSatId)).ToList();
                var lstPoint = _dbContext.TblBuPoint.Where(x => lstKy.Select(x => x.Id).Contains(x.KiKhaoSatId)).ToList();

                var lstInDoiTuong = _dbContext.TblBuInputDoiTuong.Where(x => lstSurvey.Select(x => x.Id).Contains(x.SurveyMgmtId)).ToList();
                var lstStore = _dbContext.tblMdStore.OrderBy(x => x.Id).ToList();
                var lstWareHouse = _dbContext.TblMdWareHouse.OrderBy(x => x.Id).ToList();

                foreach (var i in lstChamDiem)
                {
                    var ky = lstKy.FirstOrDefault(x => x.Id == i.KiKhaoSatId);
                    var survey = lstSurvey.FirstOrDefault(x => x.Id == ky.SurveyMgmtId);
                    var doiTuongName = survey.DoiTuongId == "DT1"
                            ? lstStore.FirstOrDefault(x => x.Id == i.DoiTuongId).Name
                            : survey.DoiTuongId == "DT2"
                                ? lstWareHouse.FirstOrDefault(x => x.Id == i.DoiTuongId).Name
                                : "";
                    var doiTuongId = lstInDoiTuong.FirstOrDefault(x => x.DoiTuongId == i.DoiTuongId && ky.SurveyMgmtId == x.SurveyMgmtId).Id;
                    var isScore = dot.Dot == 0 ? false : !lstHeader.Any(x => x.UpdateDate >= dot.FDate && x.UpdateDate <= dot.EDate && x.DoiTuongId == doiTuongId);
                    var scored = dot.Dot == 0 ? false : lstHeader.Any(x => x.UpdateDate >= dot.FDate && x.UpdateDate <= dot.EDate && x.DoiTuongId == doiTuongId);

                    result.LstDoiTuong.Add(new DoiTuong()
                    {
                        Id = doiTuongId,
                        Name = doiTuongName,
                        KiKhaoSatCode = ky.Code,
                        KiKhaoSatId = ky.Id,
                        KiKhaoSatName = ky.Name,
                        FDate = ky.StartDate,
                        EndDate = ky.EndDate,
                        Type = survey.DoiTuongId,
                        Point = lstPoint.FirstOrDefault(y => y.DoiTuongId == lstInDoiTuong.FirstOrDefault(x => x.DoiTuongId == i.DoiTuongId).Id)?.Point ?? 0,
                        IsScore = isScore,
                        TimeText = FormatToMonthYear(ky.EndDate),
                        Description = GetChamDiemStatus(ky.EndDate, now, scored, user.ChucVuId),
                        Scored = scored,
                        LstChamDiem = [userName]
                    });
                }

                result.LanCham = lstHeader.Count();
                result.ChuaCham = dot.Dot == 0 ? 0 : result.LstDoiTuong.Count(x => x.IsScore == true);

                return result;
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }

        public static DotInfo GetDotInfo(DateTime date, string? chucVuId = null)
        {
            var dot = DotChamHelper.GetDot2(date, chucVuId);     // dùng hàm enum DotCham có sẵn

            // fDate, eDate theo đợt
            var fDate = dot switch
            {
                DotCham.Dot1 => new DateTime(date.Year, date.Month, 1),
                DotCham.Dot2 => new DateTime(date.Year, date.Month, 8),
                DotCham.Dot3 => new DateTime(date.Year, date.Month, 16),
                DotCham.Dot4 => new DateTime(date.Year, date.Month, 24),
                _ => new DateTime(date.Year, date.Month, 1)
            };

            var eDate = dot switch
            {
                DotCham.Dot1 => new DateTime(date.Year, date.Month, 7),
                DotCham.Dot2 => new DateTime(date.Year, date.Month, 15),
                DotCham.Dot3 => new DateTime(date.Year, date.Month, 23),
                DotCham.Dot4 => new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)),
                _ => new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month))
            };

            return new DotInfo((int)dot, fDate, eDate);
        }

        public string FormatToMonthYear(DateTime dateStr)
        {
            var month = dateStr.Month.ToString("00");
            var year = dateStr.Year;

            return $"T{month}/{year}";
        }

        public string GetChamDiemStatus(DateTime eDate, DateTime now, bool scored, string chucVuId)
        {
            DateTime date = eDate;
            //DateTime now = new DateTime(2026, 1, 8);
            // nếu muốn dùng ngày hiện tại thực tế:

            int currentMonth = now.Month;
            int currentYear = now.Year;
            int dateMonth = date.Month;
            int dateYear = date.Year;
            int dateDay = now.Day;

            if (scored)
                return "Đã chấm";

            if (dateMonth != currentMonth || dateYear != currentYear)
                return "Ngoài thời gian chấm";

            // CHT, TK, ATVSV
            if (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK || chucVuId == RoleIds.ATVSV)
            {
                // CHT + TK → ngày 01–07
                if (dateDay >= 1 && dateDay <= 7 &&
                    (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK))
                {
                    return $"Trong thời gian (01-07/{currentMonth:00})";
                }

                // CHT + TK → ngày 16–23
                if (dateDay >= 16 && dateDay <= 23 &&
                    (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK))
                {
                    return $"Trong thời gian (15-23/{currentMonth:00})";
                }

                // ATVSV → ngày 08–15
                if (dateDay >= 8 && dateDay <= 15 && chucVuId == RoleIds.ATVSV)
                {
                    return $"Trong thời gian (08-15/{currentMonth:00})";
                }

                // ATVSV → ngày 24–30
                if (dateDay >= 24 && chucVuId == RoleIds.ATVSV)
                {
                    return $"Trong thời gian (24-30/{currentMonth:00})";
                }

                return "Ngoài thời gian chấm";
            }
            else
            {
                return "Trong thời gian chấm";
            }
        }
        //public async Task<HomeModel> CountViPham(HomeModel data, string chucVuId, DateTime now)
        //{
        //    var dot = DotChamHelper.GetDot2(now, chucVuId);     // dùng hàm enum DotCham có sẵn
        //    if (chucVuId == RoleIds.CQ || dot == DotCham.Dot1)
        //    {
        //        return data;
        //    }

        //    if((dot == DotCham.Dot2 || dot == DotCham.Dot3) && (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK))
        //    {
        //        data.LanCham >= data.LstDoiTuong.Count()
        //            ? return data
        //            : return
        //    }

        //    return null;
        //}

        public async Task<HomeModel> HandleLanCham(HomeModel data, string userName)
        {
            try
            {
                //IsScore == true => chưa chấm

                //var dateNow = new DateTime(2025, 9, 27);
                var dateNow = DateTime.Now;
                var user = _dbContext.TblAdAccount.FirstOrDefault(x => x.UserName == userName);
                var lstEvaHeader = await _dbContext.TblBuEvaluateHeader.Where(x => x.AccountUserName == userName && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1)).ToListAsync();

                if (dateNow.Day <= 7)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id && x.KiKhaoSatId == item.KiKhaoSatId && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) && x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7)))
                        {
                            item.IsScore = false;
                        }
                        else
                        {
                            if (user.ChucVuId != "ATVSV")
                            {
                                item.IsScore = true;
                                data.ChuaCham = data.ChuaCham + 1;
                            }
                            else
                            {
                                item.IsScore = false;
                            }
                        }
                    }
                }
                else if (dateNow.Day > 7 && dateNow.Day < 15)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        // nếu không có bản ghi nào của thủ trưởng chấm trong ngày 7 - 15
                        if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id && x.KiKhaoSatId == item.KiKhaoSatId && x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) && x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7)))
                        {
                            if (user.ChucVuId == "CHT" || user.ChucVuId == "TK")
                            {
                                item.IsScore = true;
                                data.ViPham = data.ViPham + 1;
                                data.ChuaCham = data.ChuaCham + 1;
                            }
                            else
                            {
                                item.IsScore = false;
                            }
                        }
                        else
                        {
                            item.IsScore = true;
                            data.ChuaCham = data.ChuaCham + 1;

                        }
                    }
                }
                else if (dateNow.Day <= 23 && dateNow.Day >= 15)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        if (user.ChucVuId == "CHT" || user.ChucVuId == "TK")
                        {
                            if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id &&
                                x.KiKhaoSatId == item.KiKhaoSatId &&
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7)))
                            {
                                item.IsScore = true;
                                data.ChuaCham = data.ChuaCham + 1;
                            }
                            else
                            {
                                data.ViPham = data.ViPham + 1;
                                data.ChuaCham = data.ChuaCham + 1;
                                item.IsScore = false;
                            }
                        }
                        else if (user.ChucVuId == "ATVSV")
                        {
                            if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id &&
                                x.KiKhaoSatId == item.KiKhaoSatId &&
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 8) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 15)))
                            {
                                item.IsScore = false;
                            }
                            else
                            {
                                data.ViPham = data.ViPham + 1;
                                item.IsScore = false;
                            }

                        }
                        else
                        {
                            if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id &&
                                x.KiKhaoSatId == item.KiKhaoSatId &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 15)))
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
                }
                else if (dateNow.Day > 23)
                {
                    foreach (var item in data.LstDoiTuong)
                    {
                        if (user.ChucVuId == "CHT" || user.ChucVuId == "TK")
                        {
                            if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id &&
                                x.KiKhaoSatId == item.KiKhaoSatId && ((
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 1) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 7)) || (
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 15) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 23))
                                )))
                            {
                                item.IsScore = false;
                            }
                            else
                            {
                                data.ViPham = data.ViPham + 1;
                                item.IsScore = false;
                            }
                        }
                        else if (user.ChucVuId == "ATVSV")
                        {
                            if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id &&
                                x.KiKhaoSatId == item.KiKhaoSatId &&
                                x.UpdateDate >= new DateTime(dateNow.Year, dateNow.Month, 8) &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 15)))
                            {
                                item.IsScore = true;
                                data.ChuaCham = data.ChuaCham + 1;
                            }
                            else
                            {
                                data.ViPham = data.ViPham + 1;
                                data.ChuaCham = data.ChuaCham + 1;
                                item.IsScore = false;
                            }

                        }
                        else
                        {
                            if (lstEvaHeader.Any(x => x.DoiTuongId == item.Id &&
                                x.KiKhaoSatId == item.KiKhaoSatId &&
                                x.UpdateDate <= new DateTime(dateNow.Year, dateNow.Month, 23)))
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
                }

                return data;
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
            }
        }



    }
}