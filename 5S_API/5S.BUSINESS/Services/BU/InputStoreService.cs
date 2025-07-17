//using AutoMapper;
//using Common;
//using Microsoft.EntityFrameworkCore;
//using PLX5S.BUSINESS.Common;
//using PLX5S.BUSINESS.Dtos.BU;
//using PLX5S.CORE;
//using PLX5S.CORE.Entities.BU;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PLX5S.BUSINESS.Services.BU
//{
//    public interface IInputStoreService : IGenericService<TblBuInputStore, InputStoreDto>
//    {
//        Task<IList<InputStoreDto>> GetAll(BaseMdFilter filter);
//        Task Insert(InputStoreDto data);
//    }

//    public class InputStoreService(AppDbContext dbContext, IMapper mapper)
//        : GenericService<TblBuInputStore, InputStoreDto>(dbContext, mapper), IInputStoreService
//    {
//        public override async Task<PagedResponseDto> Search(BaseFilter filter)
//        {
//            try
//            {
//                var query = _dbContext.TblBuInputStore.AsQueryable();
//                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
//                {
//                    query = query.Where(x => x.Id.Contains(filter.KeyWord) || x.StoreId.Contains(filter.KeyWord));
//                }
//                if (filter.IsActive.HasValue)
//                {
//                    query = query.Where(x => x.IsActive == filter.IsActive);
//                }
//                return await Paging(query, filter);
//            }
//            catch (Exception ex)
//            {
//                Status = false;
//                Exception = ex;
//                return null;
//            }
//        }

//        public async Task Insert(InputStoreDto data)
//        {
//            try
//            {
//                var entity = _mapper.Map<TblBuInputStore>(data);
//                _dbContext.TblBuInputStore.Add(entity);
//                await _dbContext.SaveChangesAsync();
//            }
//            catch (DbUpdateException ex)
//            {
//                Status = false;
//                Exception = ex;
//                throw;
//            }
//        }

//        public async Task<IList<InputStoreDto>> GetAll(BaseMdFilter filter)
//        {
//            try
//            {
//                var query = _dbContext.TblBuInputStore.AsQueryable();
//                if (filter.IsActive.HasValue)
//                {
//                    query = query.Where(x => x.IsActive == filter.IsActive);
//                }
//                return await base.GetAllMd(query, filter);
//            }
//            catch (Exception ex)
//            {
//                Status = false;
//                Exception = ex;
//                return null;
//            }
//        }
//    }
//}
