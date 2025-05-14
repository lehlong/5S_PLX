using System.ComponentModel.DataAnnotations;
using PLX5S.CORE.Entities.AD;
using AutoMapper;
using Common;

namespace PLX5S.BUSINESS.Dtos.AD
{
    public class MessageDto : IMapFrom, IDto
    {

        [Key]
   
        public string Code { get; set; }

        public string Lang { get; set; }

        public string Value { get; set; }
      
        public void Mapping(Profile profile)
        {
            profile.CreateMap<TblAdMessage, MessageDto>().ReverseMap();
        }
    }
}
