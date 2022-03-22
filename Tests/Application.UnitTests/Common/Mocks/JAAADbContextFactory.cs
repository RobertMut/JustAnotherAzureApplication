using Domain.Constants.Image;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Common.Mocks
{
    public class JAAADbContextFactory
    {
        public static Guid ProfileId = Guid.NewGuid();
        public static JAAADbContext Create()
        {
            var options = new DbContextOptionsBuilder<JAAADbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new JAAADbContext(options);
            context.Database.EnsureCreated();
            context.Files.AddRange(new[]
            {
                new File
                {
                    Filename = $"{Prefixes.OriginalImage}{ProfileId}_file.jpg",
                    UserId = ProfileId
                },
                new File
                {
                    Filename = $"{Prefixes.MiniatureImage}_50x50_{ProfileId}_file.jpg",
                    UserId = ProfileId
                },
            });
            context.Users.Add(new User
            {
                Id = ProfileId,
                Password = "12345",
                Username = "Default"
            });
            context.SaveChanges();
            return context;
        }

        public static void Destroy(JAAADbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
