using AutoMapper;
using Domain.Entities;
using Domain.Enums.Image;

namespace Application.Images.Queries.GetUserFiles
{
    public class FileDto
    {
        public string Filename { get; set; }
        public bool IsOwned { get; set; }
        public Permissions? Permission { get; set; }
        public string OriginalName { get; set; }
    }
}
