using AutoMapper;
using Domain.Entities;
using Domain.Enums.Image;

namespace Application.Images.Queries.GetUserFiles;

public class FileDto
{
    /// <summary>
    /// Filename
    /// </summary>
    public string Filename { get; set; }
    /// <summary>
    /// Determines if file is owned or comes from share
    /// </summary>
    public bool IsOwned { get; set; }
    /// <summary>
    /// Permissions
    /// </summary>
    public Permissions? Permission { get; set; }
    /// <summary>
    /// Upload name
    /// </summary>
    public string OriginalName { get; set; }
}