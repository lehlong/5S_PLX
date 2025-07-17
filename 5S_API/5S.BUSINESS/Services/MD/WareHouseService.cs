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
using PLX5S.CORE.Entities.AD;

namespace PLX5S.BUSINESS.Services.MD
{
    public interface IWarehouseService : IGenericService<TblMdWareHouse, WarehouseDto>
    {
        Task<IList<WarehouseDto>> GetAll(BaseMdFilter filter);
        Task Insert(WarehouseDto data);
        Task Update(WarehouseDto data);
        Task<List<string>> GetATVSV(string headerId);
    }
    public class WarehouseService(AppDbContext dbContext, IMapper mapper) : GenericService<TblMdWareHouse, WarehouseDto>(dbContext, mapper), IWarehouseService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblMdWareHouse.AsQueryable();
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

        public async Task Insert(WarehouseDto data)
        {
            try
            {
                var lstUpdateAccount = new List<TblAdAccount>();

                var lstAccount = _dbContext.TblAdAccount.OrderBy(x => x.UserName).ToList();

                var CHT = lstAccount.FirstOrDefault(x => x.UserName == data.TruongKho);
                CHT.ChucVuId = "TK";
                lstUpdateAccount.Add(CHT);

                var nguoiPhuTrach = lstAccount.FirstOrDefault(x => x.UserName == data.NguoiPhuTrach);
                nguoiPhuTrach.ChucVuId = "717cae63-83e6-40af-a324-6fc41eb0f121";
                lstAccount.Add(nguoiPhuTrach);

                var store = new TblMdWareHouse()
                {
                    Id = data.Id,
                    Name = data.Name,
                    TruongKho = data.TruongKho,
                    NguoiPhuTrach = data.NguoiPhuTrach,
                    TrangThaiKho = data.TrangThaiKho,
                    IsActive = data.IsActive,
                    KinhDo = data.KinhDo,
                    ViDo = data.ViDo
                };
                _dbContext.TblMdWareHouse.Add(store);

                var lstAtvs = new List<TblBuInputAtvsv>();
                foreach (var item in data.ATVSV)
                {
                    var atvsv = new TblBuInputAtvsv
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = item,
                        InputDoiTuongId = data.Id,
                        IsActive = true,
                        Type = "DT2",
                    };
                    lstAtvs.Add(atvsv);
                    var vsv = lstAccount.FirstOrDefault(x => x.UserName == item);
                    vsv.ChucVuId = "ATVSV";
                    lstUpdateAccount.Add(vsv);
                }
                _dbContext.TblAdAccount.UpdateRange(lstUpdateAccount.Distinct());
                _dbContext.TblBuInputAtvsv.AddRange(lstAtvs);

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
        public async Task Update(WarehouseDto data)
        {
            try
            {
                var lstUpdateAccount = new List<TblAdAccount>();

                var lstAccount = _dbContext.TblAdAccount.OrderBy(x => x.UserName).ToList();

                var CHT = lstAccount.FirstOrDefault(x => x.UserName == data.TruongKho);
                CHT.ChucVuId = "TK";
                lstUpdateAccount.Add(CHT);

                var nguoiPhuTrach = lstAccount.FirstOrDefault(x => x.UserName == data.NguoiPhuTrach);
                nguoiPhuTrach.ChucVuId = "717cae63-83e6-40af-a324-6fc41eb0f121";
                lstAccount.Add(nguoiPhuTrach);

                var store = new TblMdWareHouse()
                {
                    Id = data.Id,
                    Name = data.Name,
                    TruongKho = data.TruongKho,
                    NguoiPhuTrach = data.NguoiPhuTrach,
                    TrangThaiKho = data.TrangThaiKho,
                    IsActive = data.IsActive,
                    KinhDo = data.KinhDo,
                    ViDo = data.ViDo

                };
                _dbContext.TblMdWareHouse.Update(store);
                var lstdel = _dbContext.TblBuInputAtvsv.Where(x => x.InputDoiTuongId == data.Id);
                _dbContext.TblBuInputAtvsv.RemoveRange(lstdel);

                var lstAtvs = new List<TblBuInputAtvsv>();
                foreach (var item in data.ATVSV)
                {
                    var atvsv = new TblBuInputAtvsv
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = item,
                        InputDoiTuongId = data.Id,
                        IsActive = true,
                        Type = "DT2",
                    };
                    lstAtvs.Add(atvsv);
                    var vsv = lstAccount.FirstOrDefault(x => x.UserName == item);
                    vsv.ChucVuId = "ATVSV";
                    lstUpdateAccount.Add(vsv);
                }
                _dbContext.TblAdAccount.UpdateRange(lstUpdateAccount.Distinct());
                _dbContext.TblBuInputAtvsv.AddRange(lstAtvs);

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
                var lst = _dbContext.TblBuInputAtvsv.Where(x => x.InputDoiTuongId == headerId).Select(x => x.Name).ToList();

                return lst;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }


        public async Task<IList<WarehouseDto>> GetAll(BaseMdFilter filter)
        {
            try
            {
                var query = _dbContext.TblMdWareHouse.AsQueryable();
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
