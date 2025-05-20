using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.MD;
using PLX5S.CORE.Entities.MD;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Services.MD
{
    public interface IKhoXangDauService : IGenericService<TblMdKhoXangDau, KhoXangDauDto>
{
    Task<IList<KhoXangDauDto>> GetAll(BaseMdFilter filter);
    Task Insert(KhoXangDauDto data);
        Task Update(KhoXangDauDto data);
        Task<List<string>> GetATVSV(string headerId);
}
public class KhoXangDauService(AppDbContext dbContext, IMapper mapper) : GenericService<TblMdKhoXangDau, KhoXangDauDto>(dbContext, mapper), IKhoXangDauService
    {
    public override async Task<PagedResponseDto> Search(BaseFilter filter)
    {
        try
        {
            var query = _dbContext.tblMdKhoXangDau.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter.KeyWord))
            {
                query = query.Where(x => x.Id.ToString().Contains(filter.KeyWord) || x.Name.Contains(filter.KeyWord));
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

    public async Task Insert(KhoXangDauDto data)
    {
        try
        {
          
            var store = new TblMdKhoXangDau()
            {
                Id = data.Id,
                Name = data.Name,
                TruongKho = data.TruongKho,
                NguoiPhuTrach = data.NguoiPhuTrach,
                IsActive = data.IsActive
            };
                _dbContext.tblMdKhoXangDau.Add(store);

            foreach (var item in data.ATVSV)
            {
                var atvsv = new TblBuInputAtvsv();
                atvsv.Id = Guid.NewGuid().ToString();
                atvsv.Name = item;
                atvsv.InputStoreId = data.Id;
                atvsv.IsActive = true;
                    atvsv.Type = "DT2";
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
        public async Task Update(KhoXangDauDto data)
        {
            try
            {

                var store = new TblMdKhoXangDau()
                {
                    Id = data.Id,
                    Name = data.Name,
                    TruongKho = data.TruongKho,
                    NguoiPhuTrach = data.NguoiPhuTrach,
                    IsActive = data.IsActive
                };
                _dbContext.tblMdKhoXangDau.Update(store);
                var lstdel = _dbContext.TblBuInputAtvsv.Where(x => x.InputStoreId == data.Id);
                _dbContext.TblBuInputAtvsv.RemoveRange(lstdel);
                foreach (var item in data.ATVSV)
                {
                    var atvsv = new TblBuInputAtvsv();
                    atvsv.Id = Guid.NewGuid().ToString();
                    atvsv.Name = item;
                    atvsv.InputStoreId = data.Id;
                    atvsv.IsActive = true;
                    atvsv.Type = "DT2";
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
        public async Task<List<string>> GetATVSV(string headerId)
        {
            try
            {
                var lst = _dbContext.TblBuInputAtvsv.Where(x => x.InputStoreId == headerId).Select(x => x.Name).ToList();

                return lst;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }


        public async Task<IList<KhoXangDauDto>> GetAll(BaseMdFilter filter)
    {
        try
        {
            var query = _dbContext.tblMdKhoXangDau.AsQueryable();
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
}
}
