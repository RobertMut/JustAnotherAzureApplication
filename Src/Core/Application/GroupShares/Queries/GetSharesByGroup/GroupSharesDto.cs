using Application.Common.Mappings;
using AutoMapper;
using Domain.Common.Helper.Enum;
using Domain.Entities;
using Domain.Enums.Image;

namespace Application.GroupShares.Queries.GetSharesByGroup
{
    /// <summary>
    /// Class GroupSharesDto
    /// </summary>
    public class GroupSharesDto : IMapFrom<GroupShare>
    {
        /// <summary>
        /// Shared file
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Permission
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// Mapping entity and dto
        /// </summary>
        /// <param name="profile">
        /// <see cref="Profile"/>
        /// </param>
        public void Mapping(Profile profile)
        {
            profile.CreateMap<GroupShare, GroupSharesDto>()
                .ForMember(g => g.Filename, opt => opt.MapFrom(gs => gs.Filename))
                .ForMember(g => g.Permissions, opt => opt.MapFrom(gs => EnumHelper.GetDescriptionFromEnumValue((Permissions)gs.PermissionId)));
        }
    }
}
