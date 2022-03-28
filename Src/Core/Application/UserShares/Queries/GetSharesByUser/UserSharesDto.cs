using AutoMapper;
using Domain.Common.Helper.Enum;
using Domain.Entities;
using Domain.Enums.Image;

namespace Application.UserShares.Queries.GetSharesByUser
{
    public class UserSharesDto
    {
        public string Filename { get; set; }
        public string Permissions { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserShare, UserSharesDto>()
                .ForMember(g => g.Filename, opt => opt.MapFrom(gs => gs.Filename))
                .ForMember(g => g.Permissions, opt => opt.MapFrom(gs => EnumHelper.GetDescriptionFromEnumValue((Permissions)gs.PermissionId)));
        }
    }
}
