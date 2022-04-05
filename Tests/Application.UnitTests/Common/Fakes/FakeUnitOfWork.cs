using Application.Common.Interfaces.Database;
using Application.Common.Virtuals;
using Domain.Entities;
using Moq;
using MockQueryable.Moq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Application.UnitTests.Common.Fakes
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        private IJAAADbContext _context;

        public FakeUnitOfWork()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<JAAADbContext>().UseInMemoryDatabase("InMem").Options;
            _context = new JAAADbContext(dbContextOptionsBuilder);
            _context.Users.AddRange(DbSets.GetUsers());
            _context.Files.AddRange(DbSets.GetFiles());
            _context.Groups.AddRange(DbSets.GetGroups());
            _context.Permissions.AddRange(DbSets.GetPermissions());
            _context.GroupShares.AddRange(DbSets.GetGroupShares());
            _context.UserShares.AddRange(DbSets.GetUserShares());
            _context.GroupUsers.AddRange(DbSets.GetGroupUser());
            _context.SaveChangesAsync();
        }
        public Repository<File> FileRepository => new Repository<File>(_context);

        public Repository<Group> GroupRepository => new Repository<Group>(_context);

        public Repository<Permission> PermissionRepository => new Repository<Permission>(_context);

        public Repository<User> UserRepository => new Repository<User>(_context);

        public Repository<GroupShare> GroupShareRepository => new Repository<GroupShare>(_context);

        public Repository<UserShare> UserShareRepository => new Repository<UserShare>(_context);

        public Repository<GroupUser> GroupUserRepository => new Repository<GroupUser>(_context);

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task Save(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }


    }
}
