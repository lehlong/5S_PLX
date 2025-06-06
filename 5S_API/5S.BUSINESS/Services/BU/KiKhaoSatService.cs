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
using NPOI.SS.Formula.Functions;

namespace PLX5S.BUSINESS.Services.BU
{

    public interface IKikhaosatService : IGenericService<TblBuKiKhaoSat, KiKhaoSatDto>
    {
        Task Insert(KiKhaoSatModel data);
        Task<KiKhaoSatModel> BuilObjCreate(string surveyMgmtId);
        Task<KiKhaoSatModel> getKyCopy(string kiKhaoSatId);
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

        public async Task<KiKhaoSatModel> BuilObjCreate(string surveyMgmtId)
        {
            var lstInStore = _dbContext.TblBuInputStore.Where(x => x.IsActive == true && x.SurveyMgmtId == surveyMgmtId).ToList();
            var lstMdStore = _dbContext.tblMdStore.Where(x => x.IsDeleted != true).ToList();
            var lstMdWareHouse = _dbContext.TblMdWareHouse.Where(x => x.IsDeleted != true).ToList();
            var lstInWareHouse = _dbContext.TblBuInputWareHouse.Where(x => x.IsActive == true && x.SurveyMgmtId == surveyMgmtId).ToList();
            var kiKhaoSatModel = new KiKhaoSatModel();

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
                lstInputStore = lstMdStore.Where(x => lstInStore.Select(x => x.StoreId).Contains(x.Id)).Select(x => new InputStore
                {
                    Id = x.Id,
                    PhoneNumber = x.PhoneNumber,
                    Name = x.Name,
                    CuaHangTruong = x.CuaHangTruong,
                    NguoiPhuTrach = x.NguoiPhuTrach,
                    ViDo = x.ViDo,
                    KinhDo = x.KinhDo,
                    TrangThaiCuaHang = x.TrangThaiCuaHang,
                    StoreId = x.Id,
                    SurveyMgmtId = surveyMgmtId,
                }).ToList(),

                lstInputWareHouse = lstMdWareHouse.Where(x => lstInWareHouse.Select(x => x.WareHouseId).Contains(x.Id)).Select(x => new InputWarehouse
                {
                    Id = x.Id,
                    Name = x.Name,
                    TruongKho = x.TruongKho,
                    NguoiPhuTrach = x.NguoiPhuTrach,
                    WareHouseId = x.Id,
                    SurveyMgmtId = surveyMgmtId,
                }).ToList(),
            };

        }


        public async Task Insert(KiKhaoSatModel data)
        {
            try
            {
                if(data.KyCopyId == null)
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
                }
                else
                {
                    var lstTieuTri = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == data.KyCopyId && x.IsDeleted != true).ToList();

                    foreach (var item in lstTieuTri)
                    {
                        item.Code = Guid.NewGuid().ToString();
                        item.Name = item.PId == "-1" ? data.KiKhaoSat.Name : item.Name;
                        item.KiKhaoSatId = data.KiKhaoSat.Id;
                        var lstDiemTieuChi = _dbContext.TblBuTinhDiemTieuChi.Where(x => x.TieuChiCode == item.Code).ToList();

                        lstDiemTieuChi.Select(x => new TblBuTinhDiemTieuChi
                        {
                            Id = Guid.NewGuid().ToString(),
                            MoTa = x.MoTa,
                            Diem = x.Diem,
                            TieuChiCode = item.Code,
                            IsActive = x.IsActive,
                            IsDeleted = x.IsDeleted,
                        });
                        _dbContext.TblBuTinhDiemTieuChi.AddRange(lstDiemTieuChi);

                    }
                    _dbContext.TblBuTieuChi.AddRange(lstTieuTri);
                }

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
                foreach (var item in data.lstInputWareHouse)
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
                var lstInputStore = new List<InputStore>();
                var lstInputWareHouse = new List<InputWarehouse>();
                var ki = _dbContext.TblBuKiKhaoSat.Where(x => x.IsDeleted != true && x.Id == idKi).FirstOrDefault();
                var lstMdStore = _dbContext.tblMdStore.ToList();
                var lstMdWareHouse = _dbContext.TblMdWareHouse.ToList();
                var lstChamDiem = _dbContext.TblBuInputChamDiem.Where(x => x.IsDeleted != true && x.KiKhaoSatId == idKi).ToList();
                var lstInStore = _dbContext.TblBuInputStore.Where(x => x.IsDeleted != true && x.SurveyMgmtId == ki.SurveyMgmtId && x.IsActive == true).ToList();
                var lstInWareHouse = _dbContext.TblBuInputWareHouse.Where(x => x.IsDeleted != true && x.SurveyMgmtId == ki.SurveyMgmtId && x.IsActive == true).ToList();

                foreach (var item in lstInStore)
                {
                    var store = lstMdStore.Where(x => x.Id == item.StoreId).FirstOrDefault();
                    var lstChamDiem2 = lstChamDiem.Where(x => x.InStoreId == store.Id).ToList();
                    var inStore = new InputStore()
                    {
                        Id = item.Id,
                        PhoneNumber = store.PhoneNumber,
                        Name = store.Name,
                        CuaHangTruong = store.CuaHangTruong,
                        NguoiPhuTrach = store.NguoiPhuTrach,
                        ViDo = store.ViDo,
                        KinhDo = store.KinhDo,
                        TrangThaiCuaHang = store.TrangThaiCuaHang,
                        StoreId = store.Id,
                        SurveyMgmtId = ki.SurveyMgmtId,
                        LstInChamDiem = lstChamDiem2,
                        LstChamDiem = lstChamDiem2.Select(x => x.UserName).ToList()
                    };
                    lstInputStore.Add(inStore);
                }

                foreach (var item in lstInWareHouse)
                {
                    var WareHouse = lstMdWareHouse.Where(x => x.Id == item.WareHouseId).FirstOrDefault();
                    var lstChamDiem2 = lstChamDiem.Where(x => x.InStoreId == WareHouse.Id).ToList();
                    var inWareHousee = new InputWarehouse()
                    {
                        Id = item.Id,
                        Name = WareHouse.Name,
                        TruongKho = WareHouse.TruongKho,
                        NguoiPhuTrach = WareHouse.NguoiPhuTrach,
                        WareHouseId = WareHouse.Id,
                        SurveyMgmtId = ki.SurveyMgmtId,
                        LstInChamDiem = lstChamDiem2,
                        LstChamDiem = lstChamDiem2.Select(x => x.UserName).ToList()
                    };
                    lstInputWareHouse.Add(inWareHousee);
                }
                return new KiKhaoSatModel()
                {
                    KiKhaoSat = ki,
                    lstInputStore = lstInputStore,
                    lstInputWareHouse = lstInputWareHouse
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

                foreach (var item in data.lstInputWareHouse)
                {
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
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task<KiKhaoSatModel> getKyCopy(string kiKhaoSatId)
        {
            try
            {
                var idKi = Guid.NewGuid().ToString();

                var kiKhaoSat = await GetInput(kiKhaoSatId);
                kiKhaoSat.KiKhaoSat.Id = idKi;
                kiKhaoSat.KyCopyId = kiKhaoSatId;

                foreach (var s in kiKhaoSat.lstInputStore) 
                {
                    foreach (var d in s.LstInChamDiem)
                    {
                        d.Id = Guid.NewGuid().ToString();
                        d.KiKhaoSatId = idKi;
                    }
                }

                foreach (var s in kiKhaoSat.lstInputWareHouse)
                {
                    foreach (var d in s.LstInChamDiem)
                    {
                        d.Id = Guid.NewGuid().ToString();
                        d.KiKhaoSatId = idKi;
                    }
                }
                return kiKhaoSat;
            }
            catch (Exception ex)
            {
                this.Status = false;
                return null;
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

        

    }
}