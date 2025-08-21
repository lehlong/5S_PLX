using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.AD;
using PLX5S.CORE.Entities.AD;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;
using PLX5S.BUSINESS.Dtos.BU;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface ITieuChiService : IGenericService<TblBuTieuChi, TieuChiDto>
    {
        Task<TieuChiDto> BuildDataForTree(string kiKhaoSatId);
        Task InsertTreeGroup(TblBuTieuChi data); 
        Task<List<TieuChiDto>> getLeaves(string pId, string kiKhaoSatId);
        Task InsertTreeLeaves(TieuChiDto data);
        Task updateLeaves(TieuChiDto item);
        Task updateTreeGroup(TieuChiDto item);
        Task UpdateOrderTree(TieuChiDto moduleDto);
        Task UpdateOrderLeaves(List<TieuChiDto> lsrModule);
        Task<bool> CheckLeaves(string pId, string kiKhaoSatId);

    }

    public class TieuChiService(AppDbContext dbContext, IMapper mapper) : GenericService<TblBuTieuChi, TieuChiDto>(dbContext, mapper), ITieuChiService
    {
        public async Task InsertTreeGroup(TblBuTieuChi data)
        {
            try
            {
                data.Id = Guid.NewGuid().ToString();
                data.Code = Guid.NewGuid().ToString();
                _dbContext.TblBuTieuChi.Add(data);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
            }
        }

        public async Task InsertTreeLeaves(TieuChiDto data)
        {
            try
            {
                var tieuChi = new TblBuTieuChi()
                {
                    Code = Guid.NewGuid().ToString(),
                    Id = data.Id,
                    Name = data.Name,
                    IsActive = true,
                    IsGroup = false,
                    IsDeleted = false,
                    KiKhaoSatId = data.KiKhaoSatId,
                    PId = data.PId,
                    Report = data.Report,
                    IsImg = data.IsImg ?? false,
                    ChiChtAtvsv = data.ChiChtAtvsv,
                    NumberImg = data.NumberImg ?? 0
                };
                _dbContext.TblBuTieuChi.Add(tieuChi);
                foreach (var item in data.DiemTieuChi)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.TieuChiCode = tieuChi.Code;
                    _dbContext.TblBuTinhDiemTieuChi.Add(item);
                }
                foreach (var item in data.LstCriteriaExcludedOject)
                {
                    item.Code = Guid.NewGuid().ToString();
                    item.TieuChiCode = tieuChi.Code;
                    _dbContext.TblBuCriteriaExcludedObject.Add(item);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
            }
        }

        public async Task<TieuChiDto> BuildDataForTree(string kiKhaoSatId)
        {
            var lstNode = new List<TieuChiDto>();
            var node = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId == "-1" && x.IsDeleted != true).FirstOrDefault();
            var rootNode = new TieuChiDto()
            {
                Code = node.Code,
                Id = node.Id,
                Key= node.Id,
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

            var lstAllTieuChi = await _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.IsGroup == true && x.PId != "-1" && x.IsDeleted != true).OrderBy(x => x.OrderNumber).ToListAsync();
            foreach (var menu in lstAllTieuChi)
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
                    Expanded = true,
                    IsLeaf = false

                };
                lstNode.Add(node1);
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


        public async Task<List<TieuChiDto>> getLeaves(string pId, string kiKhaoSatId)
        {
            try
            {
                var tieuChi = _dbContext.TblBuTieuChi.Where(x => x.IsDeleted != true && x.PId == pId && x.KiKhaoSatId == kiKhaoSatId).OrderBy(x => x.OrderNumber).ToList();
                var lstTieuChiLeaves = new List<TieuChiDto>();
                foreach (var item in tieuChi)
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
                        ChiChtAtvsv = item.ChiChtAtvsv,
                        OrderNumber = item.OrderNumber,
                        DiemTieuChi = _dbContext.TblBuTinhDiemTieuChi.Where(x => x.TieuChiCode == item.Code && x.IsDeleted != true).OrderByDescending(x => x.Diem).ToList(),
                        LstCriteriaExcludedOject = _dbContext.TblBuCriteriaExcludedObject.Where(x => x.TieuChiCode == item.Code && x.IsDeleted != true).ToList() ?? null

                    };
                    lstTieuChiLeaves.Add(leaves);
                }

                return lstTieuChiLeaves;
            }
            catch (Exception ex)
            {
                Status = false;
                return null;
            }
        }
        public async Task<bool> CheckLeaves(string pId, string kiKhaoSatId)
        {
            try
            {
                var tieuChic = _dbContext.TblBuTieuChi.FirstOrDefault(x => x.IsDeleted != true && x.Code == pId);
                var child =  await _dbContext.TblBuTieuChi.Where(x => x.PId == tieuChic.Id && x.IsGroup == false && x.IsDeleted==false).ToListAsync();
                bool result = child.Count() > 0;


                return  result;
            }
            catch (Exception ex)
            {
                Status = false;
                return false;
            }
        }

        public async Task updateLeaves(TieuChiDto item)
        {
            try
            {
                var lstDiem = new List<TblBuTinhDiemTieuChi>();
                var lstTieuChikhongApDungCuaHang = new List<TblBuTinhDiemTieuChi>();
                _dbContext.TblBuTieuChi.Update(new TblBuTieuChi()
                {
                    Code= item.Code,
                    Id = item.Id,
                    PId = item.PId,
                    Name = item.Name,
                    IsImg = item.IsImg,
                    Report = item.Report,
                    IsGroup = item.IsGroup,
                    NumberImg = item.NumberImg,
                    KiKhaoSatId = item.KiKhaoSatId,
                    ChiChtAtvsv = item.ChiChtAtvsv,
                    OrderNumber = item.OrderNumber,
                    IsDeleted = item.IsDeleted ?? false
                }); 
                foreach (var diem in item.DiemTieuChi)
                {
                    if (diem.Id != "-1")
                    {
                        _dbContext.TblBuTinhDiemTieuChi.Update(diem);
                    }
                    else
                    {
                        diem.Id = Guid.NewGuid().ToString();
                        _dbContext.TblBuTinhDiemTieuChi.Add(diem);
                    }
                }
                foreach (var store in item.LstCriteriaExcludedOject)
                {
                    if (store.Code != "-1")
                    {
                        _dbContext.TblBuCriteriaExcludedObject.Update(store);
                    }
                    else
                    {
                        store.Code = Guid.NewGuid().ToString();
                        _dbContext.TblBuCriteriaExcludedObject.Add(store);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
            }
        }
        public async Task updateTreeGroup(TieuChiDto item)
        {
            try
            {
               
                await this.DeleteTree(item.Id, item.KiKhaoSatId);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
            }
        }
        public async Task DeleteTree(string Id, string Ki)
        {
            try
            {
                var nodetree = await _dbContext.TblBuTieuChi.FirstOrDefaultAsync(x => x.Id == Id && x.KiKhaoSatId == Ki && x.IsDeleted != true);

                var child = await _dbContext.TblBuTieuChi.Where(x => x.PId == nodetree.Id && x.IsDeleted == false && x.KiKhaoSatId == Ki).ToListAsync();
                foreach (var cl in child)
                {
                    await DeleteTree(cl.Id, Ki);
                }

                nodetree.IsDeleted = true;
                _dbContext.TblBuTieuChi.Update(nodetree);
            }
            catch (Exception ex)
            {
              
                Exception = ex;
            }


        }


        public async Task UpdateOrderTree(TieuChiDto moduleDto)
        {
            try
            {
                if (string.IsNullOrEmpty(moduleDto.PId))
                {
                    Status = false;
                    MessageObject.Code = "1012";
                    return;
                }

                var lstModuleDto = new List<TieuChiDto>();
                var lstModuleUpdate = new List<TblBuTieuChi>();

                ConvertNestedToList(moduleDto, ref lstModuleDto);
                if (moduleDto.Children == null || moduleDto.Children.Count == 0)
                {
                    return;
                }
                var numberOrder = 1;
                foreach (var item in lstModuleDto)
                {
                    var module = _mapper.Map<TblBuTieuChi>(item);
                    module.OrderNumber = numberOrder++;
                    lstModuleUpdate.Add(module);
                }
                _dbContext.TblBuTieuChi.UpdateRange(lstModuleUpdate);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        public async Task UpdateOrderLeaves(List<TieuChiDto> lsrModule)
        {
            try
            {
                var numberOrder = 1;
                var lstModuleUpdate = new List<TblBuTieuChi>();
                foreach (var item in lsrModule)
                {
                    var module = _mapper.Map<TblBuTieuChi>(item);
                    module.OrderNumber = numberOrder++;
                    lstModuleUpdate.Add(module);
                }
                _dbContext.TblBuTieuChi.UpdateRange(lstModuleUpdate);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }

        private static void ConvertNestedToList(TieuChiDto node, ref List<TieuChiDto> lstNodeFlat)
        {
            if (node.PId != "-1")
            {
                lstNodeFlat.Add(node);
            }
            if (node.Children != null && node.Children.Count > 0)
            {
                foreach (var item in node.Children)
                {
                    ConvertNestedToList(item, ref lstNodeFlat);
                }
            }
        }

        public async Task<List<TblBuCriteriaExcludedObject>> getStoresExcluded(string tieuChiCode)
        {
            try
            {
                return _dbContext.TblBuCriteriaExcludedObject.Where(x => x.TieuChiCode == tieuChiCode).ToList();
            }
            catch(Exception ex)
            {
                Status = false;
                Exception = ex;
                return null;
            }
        }

        public async Task addStoresExcluded(List<TblBuCriteriaExcludedObject> data)
        {
            try
            {
                _dbContext.TblBuCriteriaExcludedObject.AddRange(data);
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
            }
        }


    }
}
