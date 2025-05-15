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

namespace PLX5S.BUSINESS.Services.BU
{

    public interface ISurveyMgmtService : IGenericService<TblBuSurveyMgmt, SurveyMgmtDto>
    {
        //Task<IList<SurveyMgmtDto>> GetAll(BaseMdFilter filter);
        //Task<byte[]> Export(BaseMdFilter filter);
        Task<SurveyMgmtDto> BuildInput(string doiTuongId);
    }
    public class SurveyMgmtService(AppDbContext dbContext, IMapper mapper) : GenericService<TblBuSurveyMgmt, SurveyMgmtDto>(dbContext, mapper), ISurveyMgmtService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblBuSurveyMgmt.AsQueryable();
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

        public async Task<SurveyMgmtDto> BuildInput(string doiTuongId)
        {
            try
            {
                var lstStore = _dbContext.tblMdStore.Where(x => x.IsActive == true).ToList();
                var id = Guid.NewGuid().ToString();
                List<TblBuInputStore> lstInStore = new List<TblBuInputStore>();
                foreach (var s in lstStore)
                {
                    var st = new TblBuInputStore()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Ma = s.Id,
                        SurveyMgmtId = id,
                        Name = s.Name,
                        PhoneNumber = s.PhoneNumber,
                        CuaHangTruong = s.CuaHangTruong,
                        NguoiPhuTrach = s.NguoiPhuTrach,
                        KinhDo = s.KinhDo,
                        ViDo = s.ViDo,  
                        TrangThaiCuaHang = s.TrangThaiCuaHang,
                        IsActive = true
                    };
                    lstInStore.Add(st);
                }
                return new SurveyMgmtDto()
                {
                    Id = id,
                    Name = "",
                    MoTa = "",
                    DoiTuongId = doiTuongId,
                    Image = "",
                    InputStore = lstInStore,
                    IsActive = true
                };
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public async Task Insert()
        {
            try
            {

            }catch(Exception ex)
            {

            }
        }
    }
}