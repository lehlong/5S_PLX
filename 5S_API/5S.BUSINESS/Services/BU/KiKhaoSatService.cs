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
using PLX5S.BUSINESS.Models;

namespace PLX5S.BUSINESS.Services.BU
{

    public interface IKikhaosatService : IGenericService<TblBuKiKhaoSat, KiKhaoSatDto>
    {
        //Task<IList<SurveyMgmtDto>> GetAll(BaseMdFilter filter);
        //Task<byte[]> Export(BaseMdFilter filter);
        Task Insert(KiKhaoSatModel data);
        Task<List<TblBuInputStore>> GetallData(string headerId);
        //Task<List<TblBuInputChamDiem>> Getchamdiem(string kiKhaoSatId);
        Task<KiKhaoSatModel> BuilObjCreate(string surveyMgmtId);
        Task UpdateDataInput(KiKhaoSatModel data);
        Task DeleteData(string id);
        Task<KiKhaoSatModel> GetInput(string idKi);

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

        public async Task<KiKhaoSatModel> BuilObjCreate(string surveyMgmtId)
        {
            var lstInStore = _dbContext.TblBuInputStore.Where(x => x.IsActive == true && x.SurveyMgmtId == surveyMgmtId).ToList();
            
            var kiKhaoSatModel = new KiKhaoSatModel();

            var lstInputStore = new List<InputStore>();

            foreach (var item in lstInStore)
            {
                var store = _dbContext.tblMdStore.Where(x => x.Id == item.StoreId).FirstOrDefault();
                var inStore = new InputStore()
                {
                    Id = store.Id,
                    PhoneNumber = store.PhoneNumber,
                    Name = store.Name,
                    CuaHangTruong = store.CuaHangTruong,
                    NguoiPhuTrach = store.NguoiPhuTrach,
                    ViDo = store.ViDo,
                    KinhDo = store.KinhDo,
                    TrangThaiCuaHang = store.TrangThaiCuaHang,
                    StoreId = store.Id,
                    SurveyMgmtId = surveyMgmtId,
                };
                lstInputStore.Add(inStore);

            }
            return new KiKhaoSatModel()
            {
                KiKhaoSat = new TblBuKiKhaoSat()
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = "",
                    Name = "",
                    Des = "",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    SurveyMgmtId = surveyMgmtId,
                    IsActive = true,
                    IsDeleted = false
                },
                lstInputStore = lstInputStore,
            };

        }

        //public async Task<List<TblBuInputChamDiem>> Getchamdiem(string kiKhaoSatId)
        //{
        //    try
        //    {
        //        var chamDiemList = await _dbContext.TblBuInputChamDiem
        //             .Include(x => x.Store)
        //            .Where(x => x.KiKhaoSatId == kiKhaoSatId)
        //             .ToListAsync();



        //        var chamDiemDtoList = _mapper.Map<List<TblBuInputChamDiem>>(chamDiemList);

        //        return chamDiemDtoList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Status = false;
        //        Exception = ex;
        //        return null;
        //    }
        //}


        public async Task Insert(KiKhaoSatModel data)
        {
            try
            {
                var tree = new TblBuTieuChi()
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = Guid.NewGuid().ToString(),
                    Name = data.KiKhaoSat.Name,
                    IsGroup = true,
                    PId = "-1",
                    KiKhaoSatId = data.KiKhaoSat.Id,
                    IsImg = false,
                    OrderNumber = 1,
                    Report = "-",
                    IsDeleted = false
                };
                _dbContext.TblBuTieuChi.Add(tree);

                _dbContext.TblBuKiKhaoSat.Add(data.KiKhaoSat);
                foreach (var item in data.lstInputStore)
                {
                    foreach (var d in item.LstChamDiem)
                    {
                        _dbContext.TblBuInputChamDiem.Add(new TblBuInputChamDiem
                        {
                            Id = Guid.NewGuid().ToString(),
                            InStoreId = item.Id,
                            IsDeleted = false,
                            IsActive = true,
                            KiKhaoSatId = data.KiKhaoSat.Id,
                            UserName = d
                        });
                    }
                }
                _dbContext.SaveChanges();
                //var khaosatdata = new TblBuKiKhaoSat()
                //{
                //    Code = data.Code,
                //    SurveyMgmtId = data.SurveyMgmtId,
                //    Name = data.Name,
                //    IsActive = true,
                //    StartDate = data.StartDate,
                //    EndDate = data.EndDate,
                //    Des = data.Des,
                //    IsDeleted=false
                //var lsttc = new List<TblBuTieuChi>();
                //var lstDtc = new List<TblBuTinhDiemTieuChi>();
                //if (data.kicopy.IsNullOrEmpty()) {
                //    var tree = new TblBuTieuChi()
                //    {
                //        Id = Guid.NewGuid().ToString(),
                //        Name = data.Name,
                //        IsGroup = true,
                //        PId = "-1",
                //        KiKhaoSatId = data.Code,
                //        IsImg = false,
                //        OrderNumber = 1,
                //        Report = "-",
                //        IsDeleted = false
                //    };
                //    _dbContext.TblBuTieuChi.Add(tree);
                //}
                //else
                //{

                //    var tcks = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == data.kicopy).ToList();
                //    foreach (var item in tcks)
                //    {
                //        var code = Guid.NewGuid().ToString();
                //        var itemtc = new TblBuTieuChi()
                //        {
                //            Id=code,
                //            Name = item.Name,
                //            IsGroup = item.IsGroup,
                //            PId = item.PId,
                //            KiKhaoSatId = data.Code,
                //            IsImg = item.IsImg,
                //            OrderNumber = item.OrderNumber,
                //            Report = item.Report,
                //            IsDeleted = false,
                //            ChiChtAtvsv = item.ChiChtAtvsv,
                //            NumberImg = item.NumberImg,
                //        };
                //        lsttc.Add(itemtc);
                //        //if (!item.DiemTieuChi.IsNullOrEmpty())
                //        //{
                //        //    foreach(var dtc in item.DiemTieuChi)
                //        //    {
                //        //        var diemtc = new TblBuTinhDiemTieuChi()
                //        //        {
                //        //            Id = Guid.NewGuid().ToString(),
                //        //            MoTa = dtc.MoTa,
                //        //            TieuChiId=code,
                //        //            IsActive=true,
                //        //            Diem=dtc.Diem
                //        //        };
                //        //        lstDtc.Add(diemtc);
                //        //    }

                //        //}

                //        _dbContext.TblBuTinhDiemTieuChi.AddRange(lstDtc);

                //    }
                //    _dbContext.TblBuTieuChi.AddRange(lsttc);

                //}
                //    var khaosatdata = new TblBuKiKhaoSat()
                //    {
                //        Code = data.Code,
                //        SurveyMgmtId = data.SurveyMgmtId,
                //        Name = data.Name,
                //        IsActive = true,
                //        StartDate = data.StartDate,
                //        EndDate = data.EndDate,
                //        Des = data.Des,
                //        IsDeleted = false

                //    };
                //var lstChamDiem = new List<TblBuInputChamDiem>();
                //foreach (var item in data.Chamdiemlst)
                //{
                //    foreach (var i in item.NguoiChamDiem)
                //    {
                //        var chamdiem = new TblBuInputChamDiem()
                //        {
                //            Id = Guid.NewGuid().ToString(),
                //            StoreId = item.storeId,
                //            KiKhaoSatId = data.Code,
                //            UserName = i,
                //            IsActive = true,
                //            IsDeleted=false,
                //        };
                //        lstChamDiem.Add(chamdiem);
                //    }

                //}
                //_dbContext.TblBuInputChamDiem.AddRange(lstChamDiem);

                //_dbContext.TblBuKiKhaoSat.Add(khaosatdata);

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task<KiKhaoSatModel> GetInput(string idKi)
        {
            try
            {
                var ki = _dbContext.TblBuKiKhaoSat.Where(x => x.IsDeleted != true && x.Id == idKi).FirstOrDefault();
                var lstInStore = _dbContext.TblBuInputStore.Where(x => x.IsDeleted != true && x.SurveyMgmtId == ki.SurveyMgmtId && x.IsActive == true).ToList();
                var lstInputStore = new List<InputStore>();
                foreach (var item in lstInStore)
                {
                    var store = _dbContext.tblMdStore.Where(x => x.Id == item.StoreId).FirstOrDefault();
                    
                    var inStore = new InputStore()
                    {
                        Id = store.Id,
                        PhoneNumber = store.PhoneNumber,
                        Name = store.Name,
                        CuaHangTruong = store.CuaHangTruong,
                        NguoiPhuTrach = store.NguoiPhuTrach,
                        ViDo = store.ViDo,
                        KinhDo = store.KinhDo,
                        TrangThaiCuaHang = store.TrangThaiCuaHang,
                        StoreId = store.Id,
                        SurveyMgmtId = ki.SurveyMgmtId,
                        LstInChamDiem = _dbContext.TblBuInputChamDiem.Where(x => x.IsDeleted != true && x.InStoreId == store.Id && x.KiKhaoSatId == idKi).ToList(),
                        LstChamDiem = _dbContext.TblBuInputChamDiem.Where(x => x.IsDeleted != true && x.InStoreId == store.Id && x.KiKhaoSatId == idKi).Select(x => x.UserName).ToList()
                    };
                    lstInputStore.Add(inStore);
                }

                return new KiKhaoSatModel()
                {
                    KiKhaoSat = ki,
                    lstInputStore = lstInputStore
                };
            }
            catch(Exception ex)
            {
                Status = false;
                return null;
            }
        }
        public async Task UpdateDataInput(KiKhaoSatModel data)
        {
            try
            {
                _dbContext.TblBuKiKhaoSat.Update(data.KiKhaoSat);

                foreach (var item in data.lstInputStore)
                {
                    // Mark all old records as deleted
                    foreach (var old in item.LstInChamDiem)
                    {
                        old.IsDeleted = true;
                    }

                    foreach (var d in item.LstChamDiem)
                    {
                        var check = item.LstInChamDiem.FirstOrDefault(x => x.UserName == d);
                        if (check != null)
                        {
                            check.IsDeleted = false;
                        }
                        else
                        {
                            _dbContext.TblBuInputChamDiem.Add(new TblBuInputChamDiem
                            {
                                Id = Guid.NewGuid().ToString(),
                                InStoreId = item.Id,
                                IsDeleted = false,
                                IsActive = true,
                                KiKhaoSatId = data.KiKhaoSat.Id,
                                UserName = d
                            });
                        }
                    }

                    _dbContext.TblBuInputChamDiem.UpdateRange(item.LstInChamDiem);
                }

                await _dbContext.SaveChangesAsync();
                //var lstdel = _dbContext.TblBuInputChamDiem.Where(x => x.KiKhaoSatId == data.Code).ToList();
                //foreach (var item in lstdel)
                //{
                //    item.IsDeleted = true;
                //}
                //var lsttc = new List<TblBuTieuChi>();
                //var lstDtc = new List<TblBuTinhDiemTieuChi>();
                //if (!data.kicopy.IsNullOrEmpty())
                //{
                //    //var lstchamdiem

                //    var tcks = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == data.kicopy).ToList();
                //    foreach (var item in tcks)
                //    {
                //        var code = Guid.NewGuid().ToString();
                //        var itemtc = new TblBuTieuChi()
                //        {
                //            Id = code,
                //            Name = item.Name,
                //            IsGroup = item.IsGroup,
                //            PId = item.PId,
                //            KiKhaoSatId = data.Code,
                //            IsImg = item.IsImg,
                //            OrderNumber = item.OrderNumber,
                //            Report = item.Report,
                //            IsDeleted = false,
                //            ChiChtAtvsv = item.ChiChtAtvsv,
                //            NumberImg = item.NumberImg,
                //        };
                //        lsttc.Add(itemtc);
                //        //if (!item.DiemTieuChi.IsNullOrEmpty())
                //        //{
                //        //    foreach (var dtc in item.DiemTieuChi)
                //        //    {
                //        //        var diemtc = new TblBuTinhDiemTieuChi()
                //        //        {
                //        //            Id = Guid.NewGuid().ToString(),
                //        //            MoTa = dtc.MoTa,
                //        //            TieuChiId = code,
                //        //            IsActive = true,
                //        //            Diem = dtc.Diem
                //        //        };
                //        //        lstDtc.Add(diemtc);
                //        //    }

                //        //}

                //        _dbContext.TblBuTinhDiemTieuChi.AddRange(lstDtc);

                //    }
                //    _dbContext.TblBuTieuChi.AddRange(lsttc);

                //}
                //_dbContext.TblBuInputChamDiem.UpdateRange(lstdel);
                //var lstChamDiem = new List<TblBuInputChamDiem>();

                //foreach (var item in data.Chamdiemlst)
                //{
                //    //foreach (var i in item.NguoiChamDiem)
                //    //{
                //    //    var chamdiem = new TblBuInputChamDiem()
                //    //    {
                //    //        Id = Guid.NewGuid().ToString(),
                //    //        StoreId = item.storeId,
                //    //        KiKhaoSatId = data.Code,
                //    //        UserName = i,
                //    //        IsActive = true,
                //    //        IsDeleted=false
                //    //    };
                //    //    lstChamDiem.Add(chamdiem);

                //    //}

                //}


                //_dbContext.TblBuKiKhaoSat.Update(khaosatdata);
                //_dbContext.TblBuInputChamDiem.AddRange(lstChamDiem);

                //_dbContext.SaveChanges();
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