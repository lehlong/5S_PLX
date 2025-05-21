using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.CORE.Entities.BU;
using PLX5S.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PLX5S.BUSINESS.Services.BU.KikhaosatService;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace PLX5S.BUSINESS.Services.BU
{

    public interface IKikhaosatService : IGenericService<TblBuKiKhaoSat, KiKhaoSatDto>
    {
        //Task<IList<SurveyMgmtDto>> GetAll(BaseMdFilter filter);
        //Task<byte[]> Export(BaseMdFilter filter);
        Task Insert(KiKhaoSatDto data);
        Task<List<TblBuInputStore>> GetallData(string headerId);
        Task<List<TblBuInputChamDiem>> Getchamdiem(string kiKhaoSatId);
        Task UpdateData(KiKhaoSatDto data);
        Task DeleteData(string id);

    }
    public class KikhaosatService(AppDbContext dbContext, IMapper mapper) : GenericService<TblBuKiKhaoSat, KiKhaoSatDto>(dbContext, mapper), IKikhaosatService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblBuKiKhaoSat.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.SurveyMgmtId.ToString().Contains(filter.KeyWord) || x.Name.Contains(filter.KeyWord));
                  
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
        public class Dataks
        {
            public KiKhaoSatDto KhaoSat { get; set; }
            public List<TblBuInputChamDiem> ChamDiemList { get; set; }
        }
        public async Task<List<TblBuInputChamDiem>> Getchamdiem(string kiKhaoSatId)
        {
            try
            {
                var chamDiemList = await _dbContext.TblBuInputChamDiem
                     .Include(x => x.Store)
                    .Where(x => x.KiKhaoSatId == kiKhaoSatId)
                     .ToListAsync();



                var chamDiemDtoList = _mapper.Map<List<TblBuInputChamDiem>>(chamDiemList);

                return chamDiemDtoList;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }


        public async Task Insert(KiKhaoSatDto data)
        {
            try
            {
                var lsttc = new List<TblBuTieuChi>();
                var lstDtc = new List<TblBuTinhDiemTieuChi>();
                if (data.kicopy.IsNullOrEmpty()) {
                    var tree = new TblBuTieuChi()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = data.Name,
                        IsGroup = true,
                        PId = "-1",
                        KiKhaoSatId = data.Code,
                        IsImg = false,
                        OrderNumber = 1,
                        Report = "-",
                        IsDeleted = false
                    };
                    _dbContext.TblBuTieuChi.Add(tree);
                }
                else
                {
                   
                    var tcks = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == data.kicopy).ToList();
                    foreach (var item in tcks)
                    {
                        var code = Guid.NewGuid().ToString();
                        var itemtc = new TblBuTieuChi()
                        {
                            Id=code,
                            Name = item.Name,
                            IsGroup = item.IsGroup,
                            PId = item.PId,
                            KiKhaoSatId = data.Code,
                            IsImg = item.IsImg,
                            OrderNumber = item.OrderNumber,
                            Report = item.Report,
                            IsDeleted = false,
                            ChiChtAtvsv = item.ChiChtAtvsv,
                            NumberImg = item.NumberImg,
                        };
                        lsttc.Add(itemtc);
                        if (!item.DiemTieuChi.IsNullOrEmpty())
                        {
                            foreach(var dtc in item.DiemTieuChi)
                            {
                                var diemtc = new TblBuTinhDiemTieuChi()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    MoTa = dtc.MoTa,
                                    TieuChiId=code,
                                    IsActive=true,
                                    Diem=dtc.Diem
                                };
                                lstDtc.Add(diemtc);
                            }
                            
                        }

                        _dbContext.TblBuTinhDiemTieuChi.AddRange(lstDtc);

                    }
                    _dbContext.TblBuTieuChi.AddRange(lsttc);
                   
                }
                    var khaosatdata = new TblBuKiKhaoSat()
                    {
                        Code = data.Code,
                        SurveyMgmtId = data.SurveyMgmtId,
                        Name = data.Name,
                        IsActive = true,
                        StartDate = data.StartDate,
                        EndDate = data.EndDate,
                        Des = data.Des,
                        IsDeleted = false

                    };
                var lstChamDiem = new List<TblBuInputChamDiem>();
                foreach (var item in data.Chamdiemlst)
                {
                    foreach (var i in item.NguoiChamDiem)
                    {
                        var chamdiem = new TblBuInputChamDiem()
                        {
                            Id = Guid.NewGuid().ToString(),
                            StoreId = item.storeId,
                            KiKhaoSatId = data.Code,
                            UserName = i,
                            IsActive = true,
                            IsDeleted=false,
                        };
                        lstChamDiem.Add(chamdiem);
                    }
                   
                }
                _dbContext.TblBuInputChamDiem.AddRange(lstChamDiem);

                _dbContext.TblBuKiKhaoSat.Add(khaosatdata);

                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }
        public async Task UpdateData(KiKhaoSatDto data)
        {
            try
            {
               
                var khaosatdata = new TblBuKiKhaoSat()
                {
                    Code = data.Code,
                    SurveyMgmtId = data.SurveyMgmtId,
                    Name = data.Name,
                    IsActive = true,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    Des = data.Des,

                };
                var lstdel = _dbContext.TblBuInputChamDiem.Where(x => x.KiKhaoSatId == data.Code).ToList();
                foreach (var item in lstdel)
                {
                    item.IsDeleted = true;
                }
                var lsttc = new List<TblBuTieuChi>();
                var lstDtc = new List<TblBuTinhDiemTieuChi>();
                if (!data.kicopy.IsNullOrEmpty())
                {
                    //var lstchamdiem

                    var tcks = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == data.kicopy).ToList();
                    foreach (var item in tcks)
                    {
                        var code = Guid.NewGuid().ToString();
                        var itemtc = new TblBuTieuChi()
                        {
                            Id = code,
                            Name = item.Name,
                            IsGroup = item.IsGroup,
                            PId = item.PId,
                            KiKhaoSatId = data.Code,
                            IsImg = item.IsImg,
                            OrderNumber = item.OrderNumber,
                            Report = item.Report,
                            IsDeleted = false,
                            ChiChtAtvsv = item.ChiChtAtvsv,
                            NumberImg = item.NumberImg,
                        };
                        lsttc.Add(itemtc);
                        if (!item.DiemTieuChi.IsNullOrEmpty())
                        {
                            foreach (var dtc in item.DiemTieuChi)
                            {
                                var diemtc = new TblBuTinhDiemTieuChi()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    MoTa = dtc.MoTa,
                                    TieuChiId = code,
                                    IsActive = true,
                                    Diem = dtc.Diem
                                };
                                lstDtc.Add(diemtc);
                            }

                        }

                        _dbContext.TblBuTinhDiemTieuChi.AddRange(lstDtc);

                    }
                    _dbContext.TblBuTieuChi.AddRange(lsttc);

                }
                _dbContext.TblBuInputChamDiem.UpdateRange(lstdel);
                var lstChamDiem = new List<TblBuInputChamDiem>();
                
                foreach (var item in data.Chamdiemlst)
                {
                    foreach (var i in item.NguoiChamDiem)
                    {
                        var chamdiem = new TblBuInputChamDiem()
                        {
                            Id = Guid.NewGuid().ToString(),
                            StoreId = item.storeId,
                            KiKhaoSatId = data.Code,
                            UserName = i,
                            IsActive = true,
                            IsDeleted=false
                        };
                        lstChamDiem.Add(chamdiem);

                    }

                }
                
               
                _dbContext.TblBuKiKhaoSat.Update(khaosatdata);
                _dbContext.TblBuInputChamDiem.AddRange(lstChamDiem);

                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }
        public async Task DeleteData(string id)
        {
            try
            {
                var data = await _dbContext.TblBuKiKhaoSat.FindAsync(id);
                var Ncd = await _dbContext.TblBuInputChamDiem.Where(x => x.KiKhaoSatId == id).ToListAsync();
                var TC = await _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == id).ToListAsync();

                if (data != null)
                {
                    _dbContext.TblBuInputChamDiem.RemoveRange(Ncd);
                    _dbContext.TblBuKiKhaoSat.Remove(data);
                    _dbContext.TblBuTieuChi.RemoveRange(TC);

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }
        public class ListdataKikhaosat
        {
          public List<InputStoreDto> InputStore { get; set; }
         public List<NameUser> NameUsers { get; set; }

        }
        public class NameUser
        {
            public string UserName { get; set; }
            public string Fullname { get; set; }
         }

        public async Task<List<TblBuInputStore>> GetallData( string headerId)
        {

            try
            {

                var data = await _dbContext.TblBuInputStore.Include(x => x.Store).Where(x => x.SurveyMgmtId == headerId).ToListAsync();
                 return  data;

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }
        
        

    }
}