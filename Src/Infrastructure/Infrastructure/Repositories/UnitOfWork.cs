using Application.Common.Interfaces.Database;
using Application.Common.Virtuals;
using Domain.Entities;
using File = Domain.Entities.File;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    /// <summary>
    /// JAAADbContext
    /// </summary>
    private IJAAADbContext _dbContext;
    /// <summary>
    /// File repository
    /// </summary>
    private Repository<File?> _fileRepository;
    /// <summary>
    /// Group repository
    /// </summary>
    private Repository<Group?> _groupRepository;
    /// <summary>
    /// Permissions repository
    /// </summary>
    private Repository<Permission> _permissionRepository;
    /// <summary>
    /// User repository
    /// </summary>
    private Repository<User> _userRepository;
    /// <summary>
    /// Group share repository
    /// </summary>
    private Repository<GroupShare?> _groupShareRepository;
    /// <summary>
    /// User share repository
    /// </summary>
    private Repository<UserShare?> _userShareRepository;
    /// <summary>
    /// Group user junction table repository
    /// </summary>
    private Repository<GroupUser> _groupUserRepository;
    /// <summary>
    /// Is disposed
    /// </summary>
    private bool disposedValue = false;

    public UnitOfWork(IJAAADbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// File repository getter
    /// </summary>
    public Repository<File?> FileRepository
    {
        get
        {
            if (_fileRepository == null)
            {
                _fileRepository = new Repository<File?>(_dbContext);
            }
                
            return _fileRepository;
        }
    }
        
    /// <summary>
    /// Group repository getter
    /// </summary>
    public Repository<Group?> GroupRepository
    {
        get
        {
            if(_groupRepository == null)
            {
                _groupRepository = new Repository<Group?>(_dbContext);
            }

            return _groupRepository;
        }
    }
        
    /// <summary>
    /// Permissions repository getter
    /// </summary>
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
        
    /// <summary>
    /// User repository getter
    /// </summary>
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
        
    /// <summary>
    /// Group Share repository getter
    /// </summary>
    public Repository<GroupShare?> GroupShareRepository
    {
        get
        {
            if (_groupShareRepository == null)
            {
                _groupShareRepository = new Repository<GroupShare?>(_dbContext);
            }

            return _groupShareRepository;
        }
    }
        
    /// <summary>
    /// User share repository getter
    /// </summary>
    public Repository<UserShare?> UserShareRepository
    {
        get
        {
            if (_userShareRepository == null)
            {
                _userShareRepository = new Repository<UserShare?>(_dbContext);
            }

            return _userShareRepository;
        }
    }
        
    /// <summary>
    /// Group user repository getter
    /// </summary>
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

    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    public virtual async Task Save(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Dbcontext dispose
    /// </summary>
    /// <param name="disposing">dispose dbcontext</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _dbContext.DisposeAsync();
            }
        }
        disposedValue = true;
    }

    /// <summary>
    /// Dispose method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}