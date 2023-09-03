using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Application.Common.Mappings;

/// <summary>
/// Determines IMapFrom interface to provide mapping
/// </summary>
/// <typeparam name="T">Source Entity</typeparam>
public interface IMapFrom<T>
{
    /// <summary>
    /// Makes mapping
    /// </summary>
    /// <param name="profile"><see cref="Profile"/></param>
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}