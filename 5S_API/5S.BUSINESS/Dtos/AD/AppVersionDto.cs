﻿using PLX5S.CORE.Entities.AD;
using AutoMapper;
using Common;

namespace PLX5S.BUSINESS.Dtos.AD
{
    public class AppVersionDto : IMapFrom, IDto
    {
        public int VersionCode { get; set; }

        public string VersionName { get; set; }

        public bool IsRequiredUpdate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblAdAppVersion, AppVersionDto>().ReverseMap();
        }
    }
}
