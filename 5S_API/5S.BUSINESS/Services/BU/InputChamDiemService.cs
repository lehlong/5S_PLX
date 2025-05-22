using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;


namespace Services.BU
{
    public interface IInputChamDiemService : IGenericService<TblBuInputChamDiem, InputChamDiemDto>
    {
        Task Insert(InputChamDiemDto data);
    }

    public class InputChamDiemService(AppDbContext dbContext, IMapper mapper)
        : GenericService<TblBuInputChamDiem, InputChamDiemDto>(dbContext, mapper), IInputChamDiemService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.Set<TblBuInputChamDiem>().AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.InStoreId.Contains(filter.KeyWord) || x.KiKhaoSatId.Contains(filter.KeyWord) || x.UserName.Contains(filter.KeyWord));
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

        public async Task Insert(InputChamDiemDto data)
        {
            try
            {
                var entity = _mapper.Map<TblBuInputChamDiem>(data);
                _dbContext.Set<TblBuInputChamDiem>().Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }
    }
}
