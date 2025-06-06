﻿using Checkpoint.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Checkpoint.API.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Entities.Action> Action { get; set; }
        public DbSet<Entities.Controller> Controller { get; set; }
        public DbSet<Entities.BaseUrl> BaseUrl { get; set; }
        public DbSet<Entities.Project> Project { get; set; }
        public DbSet<RequestedEndpointId> RequestedEndpointId { get; set; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
