using AutoMapper;
using Common;
using DocumentFormat.OpenXml.Bibliography;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Services.BU
{

    public interface ISurveyService : IGenericService<TblBuSurveyMgmt, SurveyMgmtDto>
    {
    }
    public class SurveyService(AppDbContext dbContext, IMapper mapper) : GenericService<TblBuSurveyMgmt, SurveyMgmtDto>(dbContext, mapper), ISurveyService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            return null;
        }
        
        public async Task<List<InputDoiTuong2>> GetLstDoiTuong()
        {
            try 
            {
                return null;
            }
            catch (Exception ex)
            {
                Status = false;
                return null;
            }
        }
    }

}
