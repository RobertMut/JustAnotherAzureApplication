using Domain.Common.Helper.Filename;
using Domain.Entities;
using Domain.Enums.Image;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using File = Domain.Entities.File;

namespace Application.UnitTests.Common.Fakes
{
    [ExcludeFromCodeCoverage]
    public class DbSets
    {
        public static Guid UserId = Guid.NewGuid();
        public static Guid SecondUserId = Guid.NewGuid();
        public static Guid GroupId = Guid.NewGuid();
        public static string MiniatureFilename = NameHelper.GenerateMiniature(UserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("test.Png"));
        public static string OriginalFilename = NameHelper.GenerateOriginal(UserId.ToString(), NameHelper.GenerateHashedFilename("test.png"));

        public static IEnumerable<User> GetUsers()
        {
            var users = new List<User>()
            {
                new User
            {
                Id = Guid.NewGuid(),
                Username = "Default",
                Password = "123456"
            },
                new User
                {
                    Id = SecondUserId,
                    Username = "test",
                    Password = "12345"
                }
            };

            return users;
        }

        public static IEnumerable<File> GetFiles()
        {
            var files = new List<File>()
            {
                new File
                {
                    Filename = OriginalFilename,
                    UserId = UserId,
                    OriginalName = "test.png",
                },
                new File
                {
                    Filename = MiniatureFilename,
                    UserId = UserId,
                    OriginalName = "test.Png",
                },
                new File
                {
                    Filename = NameHelper.GenerateMiniature(SecondUserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("test.Png")),
                    UserId = SecondUserId,
                    OriginalName = "test.Png"
                },
                new File
                {
                    Filename = NameHelper.GenerateMiniature(UserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("notshared.Png")),
                    UserId = UserId,
                    OriginalName = "notshared.Png"
                },
                new File
                {
                    Filename = NameHelper.GenerateMiniature(SecondUserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("notshared.Png")),
                    UserId = SecondUserId,
                    OriginalName = "notshared.Png"
                },
                new File
                {
                    Filename = NameHelper.GenerateOriginal(SecondUserId.ToString(), NameHelper.GenerateHashedFilename("test.Png")),
                    UserId = SecondUserId,
                    OriginalName = "test.png"
                },
            };

            return files;
        }

        public static IEnumerable<Group> GetGroups()
        {
            var groups = new List<Group>()
            {
                new Group
                {
                    Id = GroupId,
                    Name = "Everyone",
                    Description = "Everyone"
                }
            };

            return groups;
        }

        public static IEnumerable<GroupShare> GetGroupShares()
        {
            var groupShares = new List<GroupShare>()
            {
                new GroupShare
                {
                    GroupId = GroupId,
                    Filename = MiniatureFilename,
                    PermissionId = (int)Permissions.readwrite
                },
                new GroupShare
                {
                    GroupId = GroupId,
                    Filename = NameHelper.GenerateMiniature(SecondUserId.ToString(), "300x300", NameHelper.GenerateHashedFilename("test.Png")),
                    PermissionId = (int)Permissions.read
                }
            };

            return groupShares;
        }

        public static IEnumerable<UserShare> GetUserShares()
        {
            var userShares = new List<UserShare>
            {
                new UserShare
                {
                    UserId = SecondUserId,
                    Filename = MiniatureFilename,
                    PermissionId = (int)Permissions.write
                }
            };

            return userShares;
        }

        public static IEnumerable<Permission> GetPermissions()
        {
            var permissions = new List<Permission>
            {
                new Permission
                {
                    Id = 0,
                    Name = "Full",
                    Delete = true,
                    Read = true,
                    Write = true
                },
                new Permission
                {
                    Id = 1,
                    Name = "ReadWrite",
                    Delete = false,
                    Read = true,
                    Write = true
                },
                new Permission
                {
                    Id = 2,
                    Name = "Read",
                    Delete = false,
                    Read = true,
                    Write = false,
                },
                new Permission
                {
                    Id = 3,
                    Name = "Write",
                    Delete = false,
                    Read = false,
                    Write = true
                }
            };

            return permissions;
        }

        public static IEnumerable<GroupUser> GetGroupUser()
        {
            var groupUsers = new List<GroupUser>()
            {
                new GroupUser
                {
                    GroupId = GroupId,
                    UserId = UserId
                }
            };

            return groupUsers;
        }

    }
}
