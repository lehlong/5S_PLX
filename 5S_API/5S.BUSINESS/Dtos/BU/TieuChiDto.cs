﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PLX5S.BUSINESS.Dtos.AD;
using PLX5S.CORE.Entities.AD;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.BUSINESS.Dtos.BU
{
    public class TieuChiDto : IMapFrom, IDto
    {
        [Key]
        public string? Code { get; set; }

        public string Id { get; set; }

        public string? Key { get; set; }

        public string Name { get; set; }
        
        public string? Title { get; set; }

        public bool Expanded { set; get; }

        public bool IsLeaf { set; get; }

        public string? PId { get; set; }

        public string? KiKhaoSatId { get; set; }
        
        public decimal? OrderNumber { get; set; }
        
        public bool IsGroup { get; set; }
        
        public bool? IsImg { get; set; }

        public bool? IsDeleted { get; set; }

        public decimal? NumberImg { get; set; }

        public string? Report { get; set; }

        public bool ChiChtAtvsv { get; set; }

        public List<TieuChiDto>? Children { set; get; }
        
        public List<TblBuTinhDiemTieuChi>? DiemTieuChi { set; get; }

        public List<TblBuCriteriaExcludedObject>? LstCriteriaExcludedOject { set; get; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblBuTieuChi, TieuChiDto>().ReverseMap();
        }
    }
}
