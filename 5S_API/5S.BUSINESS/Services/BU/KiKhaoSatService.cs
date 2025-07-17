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
using Microsoft.AspNetCore.Hosting;
using Services.AD;
using Dtos.AD;


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
        Task UpdateKhaoSatTrangThai(TblBuKiKhaoSat kiKhaoSat);
        Task<PagedResponseDto> SearchKiKhaoSat(FilterKiKhaoSat filter);
    }
    public class KikhaosatService : GenericService<TblBuKiKhaoSat, KiKhaoSatDto>, IKikhaosatService
    {
        private readonly IFirebaseNotificationService _firebaseNotificationService;

        public KikhaosatService(AppDbContext dbContext, IMapper mapper, IFirebaseNotificationService firebaseNotificationService) : base(dbContext, mapper)
        {
            _firebaseNotificationService = firebaseNotificationService;
        }

        public class FilterKiKhaoSat : BaseFilter
        {
            public string headerId { get; set; }
        }
        public async Task<PagedResponseDto> SearchKiKhaoSat(FilterKiKhaoSat filter)
        {
            try
            {
                var query = _dbContext.TblBuKiKhaoSat.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => (x.SurveyMgmtId.ToString().Contains(filter.headerId) || x.Name.Contains(filter.KeyWord)) && x.IsDeleted == false);

                }
                else
                {
                    query = query.Where(x => (x.SurveyMgmtId.ToString().Contains(filter.headerId)) && x.IsDeleted == false);
                }
                if (filter.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == filter.IsActive && x.IsDeleted == false);
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
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblBuKiKhaoSat.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.SurveyMgmtId.ToString().Contains(filter.KeyWord) || 
                    x.Name.Contains(filter.KeyWord) || 
                    x.Code.Contains(filter.KeyWord) || 
                    x.StartDate.ToString().Contains(filter.KeyWord) || 
                    x.EndDate.ToString().Contains(filter.KeyWord));
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
            var lstInDoiTuong = await _dbContext.TblBuInputDoiTuong
                .Where(x => x.IsActive == true && x.SurveyMgmtId == surveyMgmtId)
                .ToListAsync();
            var DoiTuongIds = lstInDoiTuong.Select(x => x.DoiTuongId).ToHashSet();

            var lstMdStore = await _dbContext.tblMdStore
                .Where(x => x.IsActive == true && DoiTuongIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .ToListAsync();

            var lstMdWareHouse = await _dbContext.TblMdWareHouse
                .Where(x => DoiTuongIds.Contains(x.Id))
                .ToListAsync();

            var kiKhaoSat = new TblBuKiKhaoSat
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
            };

            var lstInputStore = lstMdStore.Select(x => new InputStore
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
            }).ToList();

            var lstInputWareHouse = lstMdWareHouse.Select(x => new InputWarehouse
            {
                Id = x.Id,
                Name = x.Name,
                TruongKho = x.TruongKho,
                NguoiPhuTrach = x.NguoiPhuTrach,
                WareHouseId = x.Id,
                SurveyMgmtId = surveyMgmtId,
            }).ToList();

            return new KiKhaoSatModel
            {
                KiKhaoSat = kiKhaoSat,
                lstInputStore = lstInputStore,
                lstInputWareHouse = lstInputWareHouse
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
                    await InsertKyCopy(data);
                }
                data.KiKhaoSat.TrangThaiKi = "1";
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
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task InsertKyCopy(KiKhaoSatModel data)
        {
            var lstTieuTri = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == data.KyCopyId && x.IsDeleted != true).ToList();

            var newTieuChiList = new List<TblBuTieuChi>();
            var newDiemTieuChiList = new List<TblBuTinhDiemTieuChi>();
            var newExcludedStoresList = new List<TblBuCriteriaExcludedObject>();

            foreach (var item in lstTieuTri)
            {
                var newCode = Guid.NewGuid().ToString();

                var lstDiemTieuChi = _dbContext.TblBuTinhDiemTieuChi.Where(x => x.TieuChiCode == item.Code).ToList();

                var lstBack = _dbContext.TblBuCriteriaExcludedObject.Where(x => x.TieuChiCode == item.Code).ToList();

                var newItem = new TblBuTieuChi
                {
                    Id = item.Id,
                    Code = newCode,
                    Name = item.PId == "-1" ? data.KiKhaoSat.Name : item.Name,
                    PId = item.PId,
                    KiKhaoSatId = data.KiKhaoSat.Id,
                    IsGroup = item.IsGroup,
                    IsImg = item.IsImg,
                    OrderNumber = item.OrderNumber,
                    Report = item.Report,
                    IsDeleted = item.IsDeleted
                };
                newTieuChiList.Add(newItem);

                if (lstDiemTieuChi.Any())
                {
                    newDiemTieuChiList.AddRange(lstDiemTieuChi.Select(x => new TblBuTinhDiemTieuChi
                    {
                        Id = Guid.NewGuid().ToString(),
                        MoTa = x.MoTa,
                        Diem = x.Diem,
                        TieuChiCode = newCode,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    }));
                }

                if (lstBack.Any())
                {
                    newExcludedStoresList.AddRange(lstBack.Select(x => new TblBuCriteriaExcludedObject
                    {
                        Code = Guid.NewGuid().ToString(),
                        TieuChiCode = newCode,
                        DoiTuongId = x.DoiTuongId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    }));
                }
            }

            _dbContext.TblBuTieuChi.AddRange(newTieuChiList);
            _dbContext.TblBuTinhDiemTieuChi.AddRange(newDiemTieuChiList);
            _dbContext.TblBuCriteriaExcludedObject.AddRange(newExcludedStoresList);
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
                var lstPointstore = _dbContext.TblBuPoint.Where(x => x.KiKhaoSatId == idKi).ToList();
                var lstChamDiem = _dbContext.TblBuInputChamDiem.Where(x => x.IsDeleted != true && x.KiKhaoSatId == idKi).ToList();
                var lstInDoiTuong = _dbContext.TblBuInputDoiTuong.Where(x => x.IsDeleted != true && x.SurveyMgmtId == ki.SurveyMgmtId && x.IsActive == true).ToList();

                foreach (var item in lstInDoiTuong)
                {
                    var store = lstMdStore.Where(x => x.Id == item.DoiTuongId).FirstOrDefault();
                    var WareHouse = lstMdWareHouse.Where(x => x.Id == item.DoiTuongId).FirstOrDefault();
                    if (store != null)
                    {
                        var lstChamDiem2 = lstChamDiem.Where(x => x.InStoreId == item.Id).ToList();
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
                            Point = lstPointstore.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                            LstChamDiem = lstChamDiem2.Select(x => x.UserName).ToList()
                        };
                        lstInputStore.Add(inStore);
                    } 
                    else if (WareHouse != null)
                    {
                        var lstChamDiem3 = lstChamDiem.Where(x => x.InStoreId == item.Id).ToList();
                        var inWareHousee = new InputWarehouse()
                        {
                            Id = item.Id,
                            Name = WareHouse.Name,
                            TruongKho = WareHouse.TruongKho,
                            NguoiPhuTrach = WareHouse.NguoiPhuTrach,
                            WareHouseId = WareHouse.Id,
                            SurveyMgmtId = ki.SurveyMgmtId,
                            LstInChamDiem = lstChamDiem3,
                            Point = lstPointstore.FirstOrDefault(x => x.DoiTuongId == item.Id)?.Point ?? 0,
                            LstChamDiem = lstChamDiem3.Select(x => x.UserName).ToList()
                        };
                        lstInputWareHouse.Add(inWareHousee);
                    }

                }

                foreach (var item in lstInDoiTuong)
                {
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
  

        public async Task UpdateKhaoSatTrangThai(TblBuKiKhaoSat kiKhaoSat)
        {
            try
            {
                _dbContext.TblBuKiKhaoSat.Update(kiKhaoSat);
                if (kiKhaoSat.TrangThaiKi == "2")
                {
                    _dbContext.TblBuNotification.Add(new TblBuNotification
                    {
                        Code = Guid.NewGuid().ToString(),
                        Title = "Kỳ khảo sát Mở",
                        Body = $"Kỳ khảo sát {kiKhaoSat.Name} đã được mở",
                        SurveyId = kiKhaoSat.SurveyMgmtId,
                        KiKhaoSatId = kiKhaoSat.Id,
                        Link = "",
                    });
                    await _firebaseNotificationService.SendToTopicAsync("PLX5S_NOTI", "Có kỳ khảo sát mới", $"Kỳ khảo sát {kiKhaoSat.Name} đã được Mở", new DataFireBase());
                }
                else if (kiKhaoSat.TrangThaiKi == "0")
                {
                    _dbContext.TblBuNotification.Add(new TblBuNotification
                    {
                        Code = Guid.NewGuid().ToString(),
                        Title = "Kỳ khảo sát Đóng",
                        Body = $"Kỳ khảo sát {kiKhaoSat.Name} đã Đóng",
                        SurveyId = kiKhaoSat.SurveyMgmtId,
                        KiKhaoSatId = kiKhaoSat.Id,
                        Link = "",
                    });
                    await _firebaseNotificationService.SendToTopicAsync("PLX5S_NOTI", "Đóng kỳ khảo sát", $"Kỳ khảo sát {kiKhaoSat.Name} đã được Đóng", new DataFireBase());
                }



                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this.Status = false;
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
                kiKhaoSat.KiKhaoSat.StartDate = DateTime.Now;
                kiKhaoSat.KiKhaoSat.EndDate = DateTime.Now;
                kiKhaoSat.KiKhaoSat.TrangThaiKi = "1";

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