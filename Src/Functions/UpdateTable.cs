using System;
using Application.Common.Interfaces.Database;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using File = Domain.Entities.File;

namespace Functions
{
    public class UpdateTable
    {
        private readonly IJAAADbContext _jaaaDbContext;

        public UpdateTable(IJAAADbContext jaaaDbContext)
        {
            _jaaaDbContext = jaaaDbContext;
        }

        [FunctionName("UpdateTable")]
        public void Run([BlobTrigger("{name}", Connection = "StorageConnectionString")]string name, ILogger log)
        {
            string[] splittedFilename = name.Split("-");
            string userId = splittedFilename[^2];
            var file = _jaaaDbContext.Files.FindAsync(name);
            if (file == null)
            {
                _jaaaDbContext.Files.Add(new File
                {
                    Filename = name,
                    UserId = Guid.Parse(userId)
                });
                _jaaaDbContext.SaveChangesAsync();
                log.LogInformation($"C# Blob trigger function Added blob\n Name:{name} \n UserId: {userId}");
            }
            else
            {
                log.LogInformation($"C# Blob trigger function Existing blob\n Name:{name} \n UserId: {userId} \n No changes were made");
            }
        }
    }
}
