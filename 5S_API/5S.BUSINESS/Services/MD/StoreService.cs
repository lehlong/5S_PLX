﻿using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.MD;
using PLX5S.CORE;
using PLX5S.CORE.Entities.AD;
using PLX5S.CORE.Entities.BU;
using PLX5S.CORE.Entities.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Services.MD
{
    public interface IStoreService : IGenericService<TblMdStore, StoreDto>
    {
        Task<IList<StoreDto>> GetAll(BaseMdFilter filter);
        Task Insert(StoreDto data);
        Task UpdateStore(StoreDto data);
        Task<List<string>> GetATVSV(string headerId);



    }
    public class StoreService(AppDbContext dbContext, IMapper mapper) : GenericService<TblMdStore, StoreDto>(dbContext, mapper), IStoreService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.tblMdStore.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.Id.ToString().Contains(filter.KeyWord) || x.Name.Contains(filter.KeyWord) ||x.CuaHangTruong.Contains(filter.KeyWord)||x.NguoiPhuTrach.Contains(filter.KeyWord));
                }
                if (filter.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == filter.IsActive);
                }
                query = query.Where(x => x.IsActive == true);
                return await Paging(query, filter);

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }
        public new async Task Delete(string id)
        {
            try
            {
                var store = _dbContext.tblMdStore.FirstOrDefault(x => x.Id == id);

                store.IsActive = false;
                _dbContext.tblMdStore.Update(store);

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                Console.WriteLine($"Lỗi khi xóa mềm cửa hàng và dữ liệu liên quan: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }
        public async Task Insert(StoreDto data)
        {
            try
            {
                var lstUpdateAccount = new List<TblAdAccount>();

                var lstAccount = _dbContext.TblAdAccount.OrderBy(x => x.UserName).ToList();

                var CHT = lstAccount.FirstOrDefault(x => x.UserName == data.CuaHangTruong);
                CHT.ChucVuId = "CHT";
                lstUpdateAccount.Add(CHT);

                var nguoiPhuTrach = lstAccount.FirstOrDefault(x => x.UserName == data.NguoiPhuTrach);
                nguoiPhuTrach.ChucVuId = "717cae63-83e6-40af-a324-6fc41eb0f121";
                lstAccount.Add(nguoiPhuTrach);

                var store = new TblMdStore()
                {
                    Id = data.Id,
                    Name = data.Name,
                    PhoneNumber = data.PhoneNumber,
                    CuaHangTruong = data.CuaHangTruong,
                    NguoiPhuTrach = data.NguoiPhuTrach,
                    KinhDo = data.KinhDo,
                    ViDo = data.ViDo,
                    TrangThaiCuaHang = data.TrangThaiCuaHang,
                    IsActive = data.IsActive

                };
                _dbContext.tblMdStore.Add(store);

                foreach (var item in data.ATVSV)
                {
                    var atvsv = new TblBuInputAtvsv();
                    atvsv.Id = Guid.NewGuid().ToString();
                    atvsv.Name = item;
                    atvsv.InputDoiTuongId = data.Id;
                    atvsv.IsActive = true;
                    atvsv.Type = "DT1";

                    _dbContext.TblAdAccount.UpdateRange(lstUpdateAccount.Distinct());
                    _dbContext.TblBuInputAtvsv.AddRange(atvsv);

                    var vsv = lstAccount.FirstOrDefault(x => x.UserName == item);
                    vsv.ChucVuId = "ATVSV";
                    lstUpdateAccount.Add(vsv);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Status = false;
                Exception = ex;
                Console.WriteLine($"Lỗi khi lưu thay đổi: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task UpdateStore(StoreDto data)
        {
            try
            {
                var lstUpdateAccount = new List<TblAdAccount>();

                var lstAccount = _dbContext.TblAdAccount.OrderBy(x => x.UserName).ToList();
                
                var CHT = lstAccount.FirstOrDefault(x => x.UserName == data.CuaHangTruong);
                CHT.ChucVuId = "CHT";
                lstUpdateAccount.Add(CHT);
                
                var nguoiPhuTrach = lstAccount.FirstOrDefault(x => x.UserName == data.NguoiPhuTrach);
                nguoiPhuTrach.ChucVuId = "717cae63-83e6-40af-a324-6fc41eb0f121";
                lstAccount.Add(nguoiPhuTrach);

                var store = new TblMdStore()
                {
                    Id = data.Id,
                    Name = data.Name,
                    PhoneNumber = data.PhoneNumber,
                    CuaHangTruong = data.CuaHangTruong,
                    NguoiPhuTrach = data.NguoiPhuTrach,
                    KinhDo = data.KinhDo,
                    ViDo = data.ViDo,
                    TrangThaiCuaHang = data.TrangThaiCuaHang,
                    IsActive = data.IsActive
                };
                _dbContext.tblMdStore.Update(store);

                var lstdel = _dbContext.tblMdAtvsv.Where(x => x.StoreId == data.Id);

                _dbContext.tblMdAtvsv.RemoveRange(lstdel);
                
                var lst = new List<TblMdAtvsv>();
                
                foreach (var item in data.ATVSV)
                {
                    var atvsv = new TblMdAtvsv
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = item,
                        StoreId = data.Id,
                        IsActive = true,
                    };
                    lst.Add(atvsv);

                    var vsv = lstAccount.FirstOrDefault(x => x.UserName == item);
                    vsv.ChucVuId = "ATVSV";
                    lstUpdateAccount.Add(vsv);
                }

                _dbContext.TblAdAccount.UpdateRange(lstUpdateAccount.Distinct());
                _dbContext.tblMdAtvsv.AddRange(lst);

                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Status = false;
                Exception = ex;
                Console.WriteLine($"Lỗi khi lưu thay đổi: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<IList<StoreDto>> GetAll(BaseMdFilter filter)
        {
            try
            {
                var query = _dbContext.tblMdStore.AsQueryable();
                if (filter.IsActive.HasValue)
                {
                    query = query.Where(x => x.IsActive == filter.IsActive);
                }
                return await base.GetAllMd(query, filter);
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }
        public async Task<List<string>> GetATVSV(string headerId)
        {
            try
            {
                var lst = _dbContext.tblMdAtvsv.Where(x => x.StoreId == headerId && x.IsDeleted != true).Select(x => x.Name).ToList();

                return lst;
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
