using AutoMapper;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.AD;
using PLX5S.CORE;
using PLX5S.CORE.Entities.AD;
using Microsoft.EntityFrameworkCore;

namespace PLX5S.BUSINESS.Services.AD
{
    public interface IAppVersionService : IGenericService<TblAdAppVersion, AppVersionDto>
    {
        Task<AppVersionDto> GetCurrentVersion();
    }
    public class AppVersionService(AppDbContext dbContext, IMapper mapper) : GenericService<TblAdAppVersion, AppVersionDto>(dbContext, mapper), IAppVersionService
    {
        public async Task<AppVersionDto> GetCurrentVersion()
        {
            var data = await _dbContext.TblAdAppVersion.OrderByDescending(x => x.VersionCode).FirstOrDefaultAsync();

            return _mapper.Map<AppVersionDto>(data);
        }
    }
}
