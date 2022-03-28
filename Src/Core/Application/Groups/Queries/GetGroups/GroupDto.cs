using AutoMapper;
using Domain.Entities;

namespace Application.Groups.Queries.GetGroups
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Group, GroupDto>()
                .ForMember(g => g.Id, opt => opt.MapFrom(gd => gd.Id.ToString()));
        }
    }
}
