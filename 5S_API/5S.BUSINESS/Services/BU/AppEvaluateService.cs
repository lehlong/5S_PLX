using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface IAppEvaluateService : IGenericService<TblBuEvaluateHeader, EvaluateHeaderDto>
    {
        Task<TieuChiDto> BuildDataTreeForApp(string kiKhaoSatId, string storeId);
        Task<List<TieuChiDto>> GetAllTieuChiLeaves(string kiKhaoSatId, string storeId);
        Task<EvaluateModel> BuildInputEvaluate(string kiKhaoSatId, string storeId);
        Task InsertEvaluate(EvaluateModel data);
    }

    public class AppEvaluateService(AppDbContext dbContext, IMapper mapper) : GenericService<TblBuEvaluateHeader, EvaluateHeaderDto>(dbContext, mapper), IAppEvaluateService
    {
        public override async Task<PagedResponseDto> Search(BaseFilter filter)
        {
            try
            {
                var query = _dbContext.TblBuEvaluateHeader.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.KeyWord))
                {
                    query = query.Where(x => x.StoreId.Contains(filter.KeyWord));
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


        public async Task<TieuChiDto> BuildDataTreeForApp(string kiKhaoSatId, string storeId)
        {
            var lstNode = new List<TieuChiDto>();
            var node = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId == "-1" && x.IsDeleted != true).FirstOrDefault();
            var lstBlack = _dbContext.TblBuCriteriaExcludedStores.Where(x => x.IsDeleted != true).ToList();
            var rootNode = new TieuChiDto()
            {
                Code = node.Code,
                Id = node.Id,
                Key = node.Id,
                Name = node.Name,
                Title = node.Name,
                PId = node.PId,
                IsGroup = node.IsGroup,
                KiKhaoSatId = node.KiKhaoSatId,
                OrderNumber = 1,
                IsImg = node.IsImg,
                Report = node.Report,
                Expanded = true,
                IsLeaf = false
            };
            lstNode.Add(rootNode);

            var lstAllTieuChi = await _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId != "-1" && x.IsDeleted != true).OrderBy(x => x.OrderNumber).ToListAsync();
            foreach (var menu in lstAllTieuChi)
            {
                var checkBack = lstBlack.FirstOrDefault(x => x.TieuChiCode == menu.Code && x.StoreId == storeId);
                if (checkBack == null)
                {
                    var node1 = new TieuChiDto()
                    {
                        Code = menu.Code,
                        Id = menu.Id,
                        Key = menu.Id,
                        Name = menu.Name,
                        Title = menu.Name,
                        PId = menu.PId,
                        IsGroup = menu.IsGroup,
                        KiKhaoSatId = menu.KiKhaoSatId,
                        OrderNumber = menu.OrderNumber,
                        IsImg = menu.IsImg,
                        Report = menu.Report,
                        NumberImg = menu.NumberImg ?? 0,
                        Expanded = true,
                        IsLeaf = false,
                        DiemTieuChi = _dbContext.TblBuTinhDiemTieuChi.Where(x => x.TieuChiCode == menu.Code).ToList(),
                    };
                    lstNode.Add(node1);
                }

            }
            var nodeDict = lstNode.ToDictionary(n => n.Id);
            foreach (var item in lstNode)
            {
                if (item.PId == "-1" || !nodeDict.TryGetValue(item.PId, out TieuChiDto parentNode))
                {
                    continue;
                }

                parentNode.Children ??= [];
                parentNode.Children.Add(item);
            }
            return rootNode;

        }

        public async Task<List<TieuChiDto>> GetAllTieuChiLeaves(string kiKhaoSatId, string storeId)
        {
            try
            {
                var tieuChi = _dbContext.TblBuTieuChi.Where(x => x.IsDeleted != true && x.KiKhaoSatId == kiKhaoSatId && x.IsGroup == false).OrderBy(x => x.Id).ToList();
                var lstBlack = _dbContext.TblBuCriteriaExcludedStores.Where(x => x.IsDeleted != true).ToList();
                var lstTieuChiLeaves = new List<TieuChiDto>();
                foreach (var item in tieuChi)
                {
                    var checkBack = lstBlack.FirstOrDefault(x => x.TieuChiCode == item.Code && x.StoreId == storeId);
                    if (checkBack == null)
                    {
                        var leaves = new TieuChiDto()
                        {
                            Code = item.Code,
                            Id = item.Id,
                            PId = item.PId,
                            Name = item.Name,
                            Title = item.Name,
                            IsImg = item.IsImg,
                            Report = item.Report,
                            IsGroup = item.IsGroup,
                            NumberImg = item.NumberImg,
                            KiKhaoSatId = item.KiKhaoSatId,
                            OrderNumber = item.OrderNumber,

                        };
                        lstTieuChiLeaves.Add(leaves);
                    }
                }

                return lstTieuChiLeaves;
            }
            catch (Exception ex)
            {
                Status = false;
                return new List<TieuChiDto>();
            }

        }


        public async Task<EvaluateModel> BuildInputEvaluate(string kiKhaoSatId, string storeId)
        {
            try
            {
                var idStore = _dbContext.TblBuInputStore.FirstOrDefault(x => x.Id == storeId).StoreId;
                var nameStore = _dbContext.tblMdStore.FirstOrDefault(x => x.Id == idStore).Name;
                var lstTieuChi = await GetAllTieuChiLeaves(kiKhaoSatId, storeId);
                var idHeader = Guid.NewGuid().ToString();
                return new EvaluateModel()
                {
                    Header = new TblBuEvaluateHeader()
                    {
                        Code = idHeader,
                        Name = "Bản nháp" ,
                        Point = 0,
                        Order = 0,
                        StoreId = storeId,
                        KiKhaoSatId = kiKhaoSatId,
                    },
                    LstEvaluate = lstTieuChi.Select(x => new TblBuEvaluateValue
                    {
                        Code = Guid.NewGuid().ToString(),
                        PointId = "",
                        TieuChiCode = x.Code,
                        EvaluateHeaderCode = idHeader,

                    }).ToList(),
                };
            }
            catch (Exception ex) 
            { 

                return null;
            }
        }

        public async Task InsertEvaluate(EvaluateModel data)
        {
            try
            {
                _dbContext.TblBuEvaluateImage.AddRange(data.LstImages);
                _dbContext.TblBuEvaluateValue.AddRange(data.LstEvaluate);
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                this.Status = false;
            }
        }



    } 
}