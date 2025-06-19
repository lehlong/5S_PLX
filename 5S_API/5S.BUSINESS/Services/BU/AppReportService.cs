
using AutoMapper;
using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using PLX5S.BUSINESS.Common;
using PLX5S.BUSINESS.Dtos.BU;
using PLX5S.BUSINESS.Models;
using PLX5S.CORE;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface IAppReportService : IGenericService<TblBuEvaluateHeader, EvaluateHeaderDto>
    {
        Task<List<KetQuaChamDiem>> KetQuaChamDiem(FilterReport filterReport);
    }

    public class AppReportService : GenericService<TblBuEvaluateHeader, EvaluateHeaderDto>, IAppReportService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IKikhaosatService _kiKhaoSatService;

        public AppReportService(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment environment, IKikhaosatService KiKhaoSatService) : base(dbContext, mapper)
        {
            _environment = environment;
            _kiKhaoSatService = KiKhaoSatService;
        }

        public async Task<List<KetQuaChamDiem>> KetQuaChamDiem(FilterReport filterReport)
        {
            try
            {
                var inputKy = _kiKhaoSatService.GetInput(filterReport.KiKhaoSatId);
                var lstInStore = inputKy.Result.lstInputStore.ToList();
                var lstPoint = _dbContext.TblBuPointStore.Where(x => x.KiKhaoSatId == filterReport.KiKhaoSatId).ToList();


                var result = new List<KetQuaChamDiem>();

                foreach (var item in lstInStore)
                {
                    var report = new KetQuaChamDiem()
                    {
                        stt = item.StoreId,
                        StoreName= item.Name,
                        Length = lstPoint.FirstOrDefault(x => x.InStoreId == item.Id)?.Length ?? 0,
                        point = lstPoint.FirstOrDefault(x => x.InStoreId == item.Id)?.Point ?? 0,
                    };

                    result.Add(report);
                }

                return result;
            }
            catch (Exception ex) 
            {
                return null;
            }
        }

    }
}