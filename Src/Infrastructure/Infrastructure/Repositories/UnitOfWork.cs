using Application.Common.Interfaces.Database;
using Application.Common.Virtuals.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Domain.Entities.File;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IJAAADbContext _dbContext;
        private Repository<File> _fileRepository;
        private Repository<Group> _groupRepository;
        private Repository<Permission> _permissionRepository;
        private Repository<User> _userRepository;
        private Repository<GroupShare> _groupShareRepository;
        private Repository<UserShare> _userShareRepository;
        private Repository<GroupUser> _groupUserRepository;

        public UnitOfWork(IJAAADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Repository<File> FileRepository
        {
            get
            {
                if (_fileRepository == null)
                {
                    _fileRepository = new Repository<File>(_dbContext);
                }
                
                return _fileRepository;
            }
        }
        public Repository<Group> GroupRepository
        {
            get
            {
                if(_groupRepository == null)
                {
                    _groupRepository = new Repository<Group>(_dbContext);
                }

                return _groupRepository;
            }
        }
        public Repository<Permission> PermissionRepository
        {
            get
            {
                if (_permissionRepository == null)
                {
                    _permissionRepository = new Repository<Permission>(_dbContext);
                }

                return _permissionRepository;
            }
        }
        public Repository<User> UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new Repository<User>(_dbContext);
                }

                return _userRepository;
            }
        }
        public Repository<GroupShare> GroupShareRepository
        {
            get
            {
                if (_groupShareRepository == null)
                {
                    _groupShareRepository = new Repository<GroupShare>(_dbContext);
                }

                return _groupShareRepository;
            }
        }
        public Repository<UserShare> UserShareRepository
        {
            get
            {
                if (_userShareRepository == null)
                {
                    _userShareRepository = new Repository<UserShare>(_dbContext);
                }

                return _userShareRepository;
            }
        }
        public Repository<GroupUser> GroupUserRepository
        {
            get
            {
                if (_groupUserRepository == null)
                {
                    _groupUserRepository = new Repository<GroupUser>(_dbContext);
                }

                return _groupUserRepository;
            }
        }
    }
}
