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

    public interface IKikhaosatService : IGenericService<TblBuKiKhaoSat, KiKhaoSatDto>
    {
        //Task<IList<SurveyMgmtDto>> GetAll(BaseMdFilter filter);
        //Task<byte[]> Export(BaseMdFilter filter);
        Task Insert(TblBuKiKhaoSat data);
        //Task<List<InputStoreDto>> GetallData();
    }
    public class KikhaosatService(AppDbContext dbContext, IMapper mapper) : GenericService<TblBuKiKhaoSat, KiKhaoSatDto>(dbContext, mapper), IKikhaosatService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblBuKiKhaoSat.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.SurveyMgmtId.ToString().Contains(filter.KeyWord) || x.Name.Contains(filter.KeyWord));
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
        public async Task Insert(TblBuKiKhaoSat data)
        {
            try
            {
                var datakks = new TblBuKiKhaoSat()
                {
                    Code = data.Code,
                    SurveyMgmtId = data.SurveyMgmtId,
                    Des = data.Des,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    IsActive=data.IsActive,
                    Name=data.Name
                };
                
                _dbContext.TblBuKiKhaoSat.Add(datakks);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public class ListdataKikhaosat
        {
          public List<InputStoreDto> InputStore { get; set; }
         public List<NameUser> NameUsers { get; set; }

        }
        public class NameUser
        {
            public string UserName { get; set; }
            public string Fullname { get; set; }
         }

        //public async Task<List<ListdataKikhaosat> GetallData()
        //{

        //    try
        //    {
        //        var headerId = "12345";
        //        var data = _dbContext.TblBuInputStore.AsQueryable().ToList();
        //        var dataDto = _mapper.Map<List<InputStoreDto>>(data);
        //        var NguoiChamDiem = _dbContext.TblBuInputChamDiem.Where(x => x.KiKhaoSatId == headerId).ToList();
        //        var NguoiChamDiemDto = _mapper.Map<List<NameUser>>(NguoiChamDiem);

        //        return dataDto;

        //    }
        //    catch (Exception ex)
        //    {
        //        Status = false;
        //        Exception = ex;
        //        return null;
        //    }
        //}


    }
}