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
using DocumentFormat.OpenXml.Bibliography;

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
                //List<InputWareHouseModel> lstInWareHouse = new List<InputWareHouseModel>();
                //List<InputStoreModel> lstInStore = new List<InputStoreModel>();
                List<InputDoiTuong> lstInDoiTuong = new List<InputDoiTuong>();

                var id = Guid.NewGuid().ToString();
                var atvst = await _dbContext.tblMdAtvsv.Where(x => x.IsActive == true).ToListAsync();

                if (doiTuongId == "DT1") 
                {
                    var lstStore = _dbContext.tblMdStore.Where(x => x.IsActive == true).OrderBy(x => x.Id).ToList();

                    foreach (var s in lstStore)
                    {
                        var idSt = Guid.NewGuid().ToString();

                        var inDT = new InputDoiTuong()
                        {
                            DoiTuong = new TblBuInputDoiTuong()
                            {
                                Id = idSt,
                                DoiTuongId = s.Id,
                                SurveyMgmtId = id,
                                IsActive = false
                            },
                            Atvsvs = atvst.Where(x => x.StoreId == s.Id).Select(x => new TblBuInputAtvsv()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = x.Name,
                                InputDoiTuongId = idSt
                            }).ToList()
                        };

                        lstInDoiTuong.Add(inDT);
                    }
                }else if (doiTuongId == "DT2")
                {
                    var lstWareHouse = _dbContext.TblMdWareHouse.Where(x => x.IsActive == true).ToList();


                    foreach (var s in lstWareHouse)
                    {
                        var idSt = Guid.NewGuid().ToString();

                        var inDT = new InputDoiTuong()
                        {
                            DoiTuong = new TblBuInputDoiTuong()
                            {
                                Id = idSt,
                                DoiTuongId = s.Id,
                                SurveyMgmtId = id,
                                IsActive = false
                            },
                            Atvsvs = atvst.Where(x => x.StoreId == s.Id).Select(x => new TblBuInputAtvsv()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = x.Name,
                                InputDoiTuongId = idSt
                            }).ToList()
                        };

                        lstInDoiTuong.Add(inDT);
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
                    InputDoiTuong = lstInDoiTuong
                    //InputStores = lstInStore,
                    //InputWareHouse = lstInWareHouse
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
                foreach (var item in dataInput.InputDoiTuong)
                {
                    _dbContext.TblBuInputDoiTuong.Add(item.DoiTuong);
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
                var survey = await _dbContext.TblBuSurveyMgmt.Where(x => x.Id == id).FirstOrDefaultAsync();
                var lstInDoiTuong = _dbContext.TblBuInputDoiTuong.Where(x => x.SurveyMgmtId == id).ToList() ?? [];
                var lstInAtvsv = _dbContext.TblBuInputAtvsv.Where(x => x.IsDeleted != true).ToList();
                var atvst = await _dbContext.tblMdAtvsv.Where(x => x.IsActive == true).ToListAsync();

                var InputDoiTuong = new List<InputDoiTuong>();

                if (survey.DoiTuongId == "DT1")
                {
                    var lstStore = _dbContext.tblMdStore.Where(x => x.IsActive == true).OrderBy(x => x.Id).ToList();

                    foreach (var s in lstStore)
                    {
                        var check = lstInDoiTuong.FirstOrDefault(x => x.DoiTuongId == s.Id);
                        if (check != null)
                        {
                            InputDoiTuong.Add(new InputDoiTuong
                            {
                                DoiTuong = check,
                                Atvsvs = lstInAtvsv.Where(x => x.InputDoiTuongId == check.Id).ToList()
                            });
                        }
                        else
                        {
                            InputDoiTuong.Add(new InputDoiTuong
                            {
                                DoiTuong = new TblBuInputDoiTuong()
                                {
                                    Id = "-",
                                    DoiTuongId = s.Id,
                                    SurveyMgmtId = id,
                                    IsActive = false
                                },
                            });
                        }
                    }
                }
                else if (survey.DoiTuongId == "DT2")
                {
                    var lstWareHouse = _dbContext.TblMdWareHouse.Where(x => x.IsActive == true).ToList();


                    foreach (var s in lstWareHouse)
                    {
                        var check = lstInDoiTuong.FirstOrDefault(x => x.DoiTuongId == s.Id);
                        if (check != null)
                        {
                            InputDoiTuong.Add(new InputDoiTuong
                            {
                                DoiTuong = check,
                                Atvsvs = lstInAtvsv.Where(x => x.InputDoiTuongId == check.Id).ToList()
                            });
                        }
                        else
                        {
                            InputDoiTuong.Add(new InputDoiTuong
                            {
                                DoiTuong = new TblBuInputDoiTuong()
                                {
                                    Id = "-",
                                    DoiTuongId = s.Id,
                                    SurveyMgmtId = id,
                                    IsActive = false
                                },
                            });
                        }
                    }
                }
                return new SurveyMgmtModel
                {
                    SurveyMgmt = survey,
                    InputDoiTuong = InputDoiTuong
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
                foreach (var item in dataInput.InputDoiTuong)
                {
                    if(item.DoiTuong.Id == "-")
                    {
                        item.DoiTuong.Id = Guid.NewGuid().ToString();
                        _dbContext.TblBuInputDoiTuong.Add(item.DoiTuong);
                    }
                    else
                    {
                        _dbContext.TblBuInputDoiTuong.Update(item.DoiTuong);
                        _dbContext.TblBuInputAtvsv.UpdateRange(item.Atvsvs);
                    }
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