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

namespace PLX5S.BUSINESS.Services.MD
{
    public interface IKhoXangDauService : IGenericService<TblMdKhoXangDau, KhoXangDauDto>
{
    Task<IList<KhoXangDauDto>> GetAll(BaseMdFilter filter);
    Task Insert(KhoXangDauDto data);


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
            data.Id = Guid.NewGuid().ToString();
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
                var atvsv = new TblMdAtvsv();
                atvsv.Id = Guid.NewGuid().ToString();
                atvsv.Name = item;
                atvsv.StoreId = data.Id;
                atvsv.IsActive = true;

                _dbContext.tblMdAtvsv.AddRange(atvsv);
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

        }

    //public async Task<byte[]> Export(BaseMdFilter filter)
    //{
    //    try
    //    {
    //        var query = _dbContext.tblMdStore.AsQueryable();
    //        if (!string.IsNullOrWhiteSpace(filter.KeyWord))
    //        {
    //            query = query.Where(x => x.Name.Contains(filter.KeyWord));
    //        }
    //        if (filter.IsActive.HasValue)
    //        {
    //            query = query.Where(x => x.IsActive == filter.IsActive);
    //        }
    //        var data = await base.GetAllMd(query, filter);
    //        int i = 1;
    //        data.ForEach(x =>
    //        {
    //            x.OrdinalNumber = i++;
    //        });
    //        return await ExportExtension.ExportToExcel(data);
    //    }
    //    catch (Exception ex)
    //    {
    //        Status = false;
    //        Exception = ex;
    //        return null;
    //    }
    //}

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
