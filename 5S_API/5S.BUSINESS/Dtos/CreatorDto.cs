using AutoMapper;
using Common;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.BUSINESS.Dtos.Common
{
    public class CreatorDto : IMapFrom, IDto
    {
        public string FullName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblAdAccount, CreatorDto>().ReverseMap();
        }
    }
}
