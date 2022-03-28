using AutoMapper;
using Domain.Common.Helper.Enum;
using Domain.Entities;
using Domain.Enums.Image;

namespace Application.GroupShares.Queries.GetSharesByGroup
{
    public class GroupSharesDto
    {
        public string Filename { get; set; }
        public string Permissions { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<GroupShare, GroupSharesDto>()
                .ForMember(g => g.Filename, opt => opt.MapFrom(gs => gs.Filename))
                .ForMember(g => g.Permissions, opt => opt.MapFrom(gs => EnumHelper.GetDescriptionFromEnumValue((Permissions)gs.PermissionId)));
        }
    }
}
