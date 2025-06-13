using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.MD;
using PLX5S.CORE;
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
                    query = query.Where(x => x.Id.ToString().Contains(filter.KeyWord) || x.Name.Contains(filter.KeyWord));
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
                    atvsv.InputStoreId = data.Id;
                    atvsv.IsActive = true;
                    atvsv.Type = "DT1";
                    _dbContext.TblBuInputAtvsv.AddRange(atvsv);
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
                    var atvsv = new TblMdAtvsv();
                    atvsv.Id = Guid.NewGuid().ToString();
                    atvsv.Name = item;
                    atvsv.StoreId = data.Id;
                    atvsv.IsActive = true;
                    lst.Add(atvsv);
                }
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
