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
using PLX5S.BUSINESS.Models;

namespace PLX5S.BUSINESS.Services.BU
{

    public interface ISurveyMgmtService : IGenericService<TblBuSurveyMgmt, SurveyMgmtDto>
    {
        Task<SurveyMgmtModel> BuildInput(string doiTuongId);
        Task Insert(SurveyMgmtModel dataInput);
        Task<SurveyMgmtModel> GetInput(string id);
        Task UpdateInput(SurveyMgmtModel dataInput);
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

        public async Task<SurveyMgmtModel> BuildInput(string doiTuongId)
        {
            try
            {
                var lstStore = _dbContext.tblMdStore.Where(x => x.IsActive == true).ToList();

                var id = Guid.NewGuid().ToString();

                List<InputStoreModel> lstInStore = new List<InputStoreModel>();

                foreach (var s in lstStore)
                {
                    var idSt = Guid.NewGuid().ToString();

                    var atvst = await _dbContext.tblMdAtvsv.Where(x => x.StoreId == s.Id).ToListAsync();


                    List<TblBuInputAtvsv> inAtvsvs = new List<TblBuInputAtvsv>();

                    foreach (var item in atvst)
                    {
                        var inAtvsv = new TblBuInputAtvsv
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = item.Name,
                            InputStoreId = idSt
                        };
                        inAtvsvs.Add(inAtvsv);
                    }
                    var st = new TblBuInputStore()
                    {
                        Id = idSt,
                        StoreId = s.Id,
                        SurveyMgmtId = id,
                        IsActive = false
                    };
                    var inStore = new InputStoreModel()
                    {
                        InputStore = st,
                        Atvsvs = inAtvsvs
                    };

                    lstInStore.Add(inStore);
                }
                var surveymgmt = new TblBuSurveyMgmt
                {

                    Id = id,
                    Name = "",
                    MoTa = "",
                    DoiTuongId = doiTuongId,
                    Image = "",
                    IsActive = true
                };
                return new SurveyMgmtModel
                {
                    SurveyMgmt = surveymgmt,
                    InputStores = lstInStore
                };
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public async Task Insert(SurveyMgmtModel dataInput)
        {
            try
            {
                foreach (var item in dataInput.InputStores)
                {
                    _dbContext.TblBuInputStore.Add(item.InputStore);
                    if (item.Atvsvs.Count() != 0)
                    {
                        _dbContext.TblBuInputAtvsv.AddRange(item.Atvsvs);
                    }
                }
                _dbContext.TblBuSurveyMgmt.Add(dataInput.SurveyMgmt);

                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task<SurveyMgmtModel> GetInput(string id)
        {
            try
            {
                var lstInStore = _dbContext.TblBuInputStore.Where(x => x.SurveyMgmtId == id).ToList();
                var InputStores = new List<InputStoreModel>();
                foreach (var item in lstInStore)
                {
                    var inputStoreModel = new InputStoreModel
                    {
                        InputStore = item,
                        Atvsvs = _dbContext.TblBuInputAtvsv.Where(x => x.InputStoreId == item.Id).ToList()
                    };

                    InputStores.Add(inputStoreModel);
                }
                return new SurveyMgmtModel
                {
                    SurveyMgmt = await _dbContext.TblBuSurveyMgmt.Where(x => x.Id == id).FirstOrDefaultAsync(),
                    InputStores = InputStores
                };
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public async Task UpdateInput(SurveyMgmtModel dataInput)
        {
            try
            {
                foreach (var item in dataInput.InputStores)
                {
                    _dbContext.TblBuInputStore.Update(item.InputStore);
                    _dbContext.TblBuInputAtvsv.UpdateRange(item.Atvsvs);
                }
                _dbContext.TblBuSurveyMgmt.Update(dataInput.SurveyMgmt);

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