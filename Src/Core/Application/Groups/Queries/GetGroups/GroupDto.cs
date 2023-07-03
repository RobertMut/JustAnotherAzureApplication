using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Groups.Queries.GetGroups;

public class GroupDto : IMapFrom<Group>
{
    /// <summary>
    /// Group id
    /// </summary>
    public string Id { get; set; }
        
    /// <summary>
    /// Group name
    /// </summary>
    public string Name { get; set; }
        
    /// <summary>
    /// Group Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Mapping entity and dto
    /// </summary>
    /// <param name="profile">
    /// <see cref="Profile"/>
    /// </param>
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Group, GroupDto>()
            .ForMember(g => g.Id, opt => opt.MapFrom(gd => gd.Id.ToString()))
            .ForMember(g => g.Name, opt => opt.MapFrom(gd => gd.Name.ToString()))
            .ForMember(g => g.Description, opt => opt.MapFrom(gd => gd.Description!.ToString()));
    }
}