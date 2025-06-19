
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
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Services.BU
{
    public interface IAppReportService : IGenericService<TblBuEvaluateHeader, EvaluateHeaderDto>
    {
        
    }

    public class AppReportService : GenericService<TblBuEvaluateHeader, EvaluateHeaderDto>, IAppReportService
    {
        private readonly IWebHostEnvironment _environment;

        public AppReportService(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment environment) : base(dbContext, mapper)
        {
            _environment = environment;
        }



    }
}