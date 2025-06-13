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
                List<InputWareHouseModel> lstInWareHouse = new List<InputWareHouseModel>();
                List<InputStoreModel> lstInStore = new List<InputStoreModel>();

                var id = Guid.NewGuid().ToString();
                var atvst = await _dbContext.tblMdAtvsv.Where(x => x.IsActive == true).ToListAsync();

                if (doiTuongId == "DT1") 
                {
                    var lstStore = _dbContext.tblMdStore.Where(x => x.IsActive == true).OrderBy(x => x.Id).ToList();

                    foreach (var s in lstStore)
                    {
                        var idSt = Guid.NewGuid().ToString();

                        var inStore = new InputStoreModel()
                        {
                            InputStore = new TblBuInputStore()
                            {
                                Id = idSt,
                                StoreId = s.Id,
                                SurveyMgmtId = id,
                                IsActive = false
                            },
                            Atvsvs = atvst.Where(x => x.StoreId == s.Id).Select(x => new TblBuInputAtvsv()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = x.Name,
                                InputStoreId = idSt
                            }).ToList()
                        };

                        lstInStore.Add(inStore);
                    }
                }else if (doiTuongId == "DT2")
                {
                    var lstWareHouse = _dbContext.TblMdWareHouse.Where(x => x.IsActive == true).ToList();


                    foreach (var s in lstWareHouse)
                    {
                        var idSt = Guid.NewGuid().ToString();

                        var inWareHouse = new InputWareHouseModel()
                        {
                            InputWareHouse = new TblBuInputWareHouse()
                            {
                                Id = idSt,
                                WareHouseId = s.Id,
                                SurveyMgmtId = id,
                                IsActive = false
                            },
                            Atvsvs = atvst.Where(x => x.StoreId == s.Id).Select(x => new TblBuInputAtvsv()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = x.Name,
                                InputStoreId = idSt
                            }).ToList()
                        };

                        lstInWareHouse.Add(inWareHouse);
                    }
                }

                return new SurveyMgmtModel
                {
                    SurveyMgmt =  new TblBuSurveyMgmt
                    {
                        Id = id,
                        Name = "",
                        MoTa = "",
                        DoiTuongId = doiTuongId,
                        Image = "",
                        IsActive = true
                    },
                    InputStores = lstInStore,
                    InputWareHouse = lstInWareHouse
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
                if (dataInput.SurveyMgmt.DoiTuongId == "DT1")
                {
                    foreach (var item in dataInput.InputStores)
                    {
                        _dbContext.TblBuInputStore.Add(item.InputStore);
                        if (item.Atvsvs.Count() != 0)
                        {
                            _dbContext.TblBuInputAtvsv.AddRange(item.Atvsvs);
                        }
                    }

                }else if (dataInput.SurveyMgmt.DoiTuongId == "DT2")
                {
                    foreach (var item in dataInput.InputWareHouse)
                    {
                        _dbContext.TblBuInputWareHouse.Add(item.InputWareHouse);
                        if (item.Atvsvs.Count() != 0)
                        {
                            _dbContext.TblBuInputAtvsv.AddRange(item.Atvsvs);
                        }
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
                var lstInStore = _dbContext.TblBuInputStore.Where(x => x.SurveyMgmtId == id).ToList() ?? [];
                var lstInWareHouse = _dbContext.TblBuInputWareHouse.Where(x => x.SurveyMgmtId == id).ToList() ?? [];
                var lstInAtvsv = _dbContext.TblBuInputAtvsv.Where(x => x.IsDeleted != true).ToList();
                var InputStores = new List<InputStoreModel>();
                var lstWareHouse = new List<InputWareHouseModel>();

                foreach (var item in lstInStore)
                {
                    InputStores.Add(new InputStoreModel
                    {
                        InputStore = item,
                        Atvsvs = lstInAtvsv.Where(x => x.InputStoreId == item.Id).ToList()
                    });
                }
                foreach (var item in lstInWareHouse)
                {
                    lstWareHouse.Add(new InputWareHouseModel
                    {
                        InputWareHouse = item,
                        Atvsvs = lstInAtvsv.Where(x => x.InputStoreId == item.Id).ToList()
                    });
                }
                return new SurveyMgmtModel
                {
                    SurveyMgmt = await _dbContext.TblBuSurveyMgmt.Where(x => x.Id == id).FirstOrDefaultAsync(),
                    InputStores = InputStores,
                    InputWareHouse = lstWareHouse
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