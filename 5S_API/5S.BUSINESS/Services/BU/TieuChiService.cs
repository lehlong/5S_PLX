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
        Task InsertTreeLeaves(TblBuTieuChi data);
    }

    public class TieuChiService(AppDbContext dbContext, IMapper mapper) : GenericService<TblBuTieuChi, TieuChiDto>(dbContext, mapper), ITieuChiService
    {
        public async Task InsertTreeGroup(TblBuTieuChi data)
        {
            try
            {
                _dbContext.TblBuTieuChi.Add(data);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        public async Task InsertTreeLeaves(TblBuTieuChi data)
        {
            try
            {
                _dbContext.TblBuTieuChi.Add(data);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<TieuChiDto> BuildDataForTree(string kiKhaoSatId)
        {
            var lstNode = new List<TieuChiDto>();
            var node = _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.PId == "-1" && x.IsDeleted != true).FirstOrDefault();
            var rootNode = new TieuChiDto()
            {
                Id = node.Id,
                Name = node.Name,
                PId = node.PId,
                IsGroup = node.IsGroup,
                KiKhaoSatId = node.KiKhaoSatId,
                OrderNumber = 1,
                IsImg = node.IsImg,
                Report = node.Report,
            };
            lstNode.Add(rootNode);

            var lstAllTieuChi = await _dbContext.TblBuTieuChi.Where(x => x.KiKhaoSatId == kiKhaoSatId && x.IsGroup == true && x.PId != "-1" && x.IsDeleted != true).OrderBy(x => x.OrderNumber).ToListAsync();
            foreach (var menu in lstAllTieuChi)
            {
                var node1 = new TieuChiDto()
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    PId = menu.PId,
                    IsGroup = menu.IsGroup,
                    KiKhaoSatId = menu.KiKhaoSatId,
                    OrderNumber = menu.OrderNumber,
                    IsImg = menu.IsImg,
                    Report = menu.Report,
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

        public async Task<List<TblBuTieuChi>> getLeaves(string id)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public async Task UpdateOrderTree(TieuChiDto moduleDto)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(moduleDto.PId))
        //        {
        //            Status = false;
        //            MessageObject.Code = "1012";
        //            return;
        //        }

        //        var lstModuleDto = new List<TieuChiDto>();
        //        var lstModuleUpdate = new List<TblBuTieuChi>();

        //        ConvertNestedToList(moduleDto, ref lstModuleDto);
        //        if (moduleDto.Children == null || moduleDto.Children.Count == 0)
        //        {
        //            return;
        //        }
        //        var numberOrder = 1;
        //        foreach (var item in lstModuleDto)
        //        {
        //            var module = _mapper.Map<TblBuTieuChi>(item);
        //            module.OrderNumber = numberOrder++;
        //            lstModuleUpdate.Add(module);
        //        }
        //        _dbContext.UpdateRange(lstModuleUpdate);
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Status = false;
        //        Exception = ex;
        //    }
        //}

        //public async Task<TieuChiDto> GetMenuOfUser(string userName)
        //{
        //    var lstNode = new List<MenuDto>();
        //    var rootNode = new TieuChiDto() { Id = "MNU", PId = "-MNU", Name = "Danh sách menu", Title = "MNU - Danh sách menu", Expanded = true, Key = "MNU" };
        //    lstNode.Add(rootNode);

        //    var lstRightOfUser = await GetRightOfUser(userName);
        //    var lstAllMenu = await _dbContext.TblAdMenu.Include(x => x.RightReferences)
        //        .Where(x => x.RightReferences.Any(y => lstRightOfUser.Contains(y.RightId)) || x.IsActive == true).OrderBy(x => x.OrderNumber).ToListAsync();
        //    foreach (var menu in lstAllMenu)
        //    {
        //        var node = new TieuChiDto()
        //        {
        //            Id = menu.Id,
        //            Name = menu.Name,
        //            PId = menu.PId,
        //            OrderNumber = menu.OrderNumber,
        //            Icon = menu.Icon,
        //            Url = menu.Url,
        //            Title = $"{menu.Id} - {menu.Name}",
        //            Key = menu.Id,
        //            IsActive = menu.IsActive,
        //            Expanded = true,
        //        };
        //        lstNode.Add(node);
        //    }
        //    var nodeDict = lstNode.ToDictionary(n => n.Id);
        //    foreach (var item in lstNode)
        //    {
        //        if (item.ParentId == "-MNU" || !nodeDict.TryGetValue(item.PId, out TieuChiDto parentNode))
        //        {
        //            continue;
        //        }

        //        parentNode.Children ??= [];
        //        parentNode.Children.Add(item);
        //    }
        //    return rootNode;
        //}


        //private static void ConvertNestedToList(TieuChiDto node, ref List<TieuChiDto> lstNodeFlat)
        //{
        //    if (node.Id != "MNU")
        //    {
        //        lstNodeFlat.Add(node);
        //    }
        //    if (node.Children != null && node.Children.Count > 0)
        //    {
        //        foreach (var item in node.Children)
        //        {
        //            ConvertNestedToList(item, ref lstNodeFlat);
        //        }
        //    }
        //}

        //    public async Task<TieuChiDto> Delete(string code)
        //    {
        //        try
        //        {

        //            var codeString = code.ToString();
        //            var query = _dbContext.Set<TblBuTieuChi>().AsQueryable();

        //            query = query.Where(x => x.ParentId == codeString);
        //            var recordsWithSamePid = await query.ToListAsync();

        //            if (recordsWithSamePid.Count == 0)
        //            {
        //                var recordToDelete = await _dbContext.Set<TblBuTieuChi>().FirstOrDefaultAsync(x => x.Id == codeString);

        //                if (recordToDelete != null)
        //                {
        //                    _dbContext.Remove(recordToDelete);
        //                    await _dbContext.SaveChangesAsync();
        //                }
        //                return _mapper.Map<TieuChiDto>(recordToDelete);
        //            }
        //            return null;
        //        }
        //        catch (Exception ex)
        //        {
        //            Status = false;
        //            Exception = ex;
        //            return null;
        //        }
        //    }

        //    public override async Task Update(IDto dto)
        //    {
        //        if (dto is MenuUpdateDto model)
        //        {
        //            var currentObj = await _dbContext.TblAdMenu.Include(x => x.RightReferences).FirstOrDefaultAsync(x => model.Id == x.Id);
        //            await base.UpdateWithListInside(dto, currentObj);
        //        }
        //    }

        //    public async Task<MenuDetailDto> GetMenuWithTreeRight(object id)
        //    {
        //        var data = await _dbContext.TblAdMenu.Include(x => x.RightReferences).FirstOrDefaultAsync(x => x.Id == id as string);

        //        if (data == null) return null;

        //        var lstNode = new List<RightDto>();
        //        var rootNode = new RightDto() { Id = "R", PId = "-R", Name = "Danh sách quyền trong hệ thống" };
        //        lstNode.Add(rootNode);

        //        var lstAllRight = await _dbContext.TblAdRight.Where(x => x.Id != "R").OrderBy(x => x.OrderNumber).ToListAsync();

        //        var lstRightInMenu = data.RightReferences.Select(x => x.RightId).ToList();

        //        if (data.RightReferences.Count > 0)
        //        {
        //            rootNode.IsChecked = true;
        //        }
        //        foreach (var right in lstAllRight)
        //        {
        //            var node = new RightDto()
        //            {
        //                Id = right.Id,
        //                Name = right.Name,
        //                PId = right.PId,
        //                OrderNumber = right.OrderNumber,
        //                Title = $"{right.Id} - {right.Name}",
        //                Key = right.Id,
        //                IsActive = right.IsActive,
        //                Expanded = true,
        //            };
        //            if (lstRightInMenu.Contains(right.Id))
        //            {
        //                node.IsChecked = true;
        //            }
        //            lstNode.Add(node);
        //        }

        //        var nodeDict = lstNode.ToDictionary(n => n.Id);
        //        foreach (var item in lstNode)
        //        {
        //            if (item.PId == "-R" || !nodeDict.TryGetValue(item.PId, out RightDto parentNode))
        //            {
        //                continue;
        //            }

        //            parentNode.Children ??= [];
        //            parentNode.Children.Add(item);
        //        }

        //        var result = _mapper.Map<MenuDetailDto>(data);
        //        result.TreeRight = rootNode;

        //        return result;
        //    }

        //    private async Task<List<string>> GetRightOfUser(string userName)
        //    {
        //        var user = await _dbContext.TblAdAccount.Include(x => x.Account_AccountGroups)
        //            .ThenInclude(x => x.AccountGroup).ThenInclude(x => x.ListAccountGroupRight)
        //            .Include(x => x.AccountRights)
        //            .FirstOrDefaultAsync(x => x.UserName == userName);

        //        if (user == null) return [];

        //        var listRightOfUser = new List<string>();

        //        var lstRightInGroup = user.Account_AccountGroups
        //            .Select(x => x.AccountGroup)
        //            .SelectMany(x => x.ListAccountGroupRight)
        //            .Select(x => x.RightId).ToList();

        //        var listRightOutGroup = user.AccountRights.Where(x => x.IsAdded == true).Select(x => x.RightId).ToList();

        //        var listRightOutGroupRemoved = user.AccountRights.Where(x => x.IsRemoved == true).Select(x => x.RightId).ToList();


        //        var result = listRightOfUser.Concat(lstRightInGroup).Concat(listRightOutGroup).Distinct().ToList();

        //        result.RemoveAll(x => listRightOutGroupRemoved.Contains(x));

        //        return result;
        //    }

        //}
    }
}
