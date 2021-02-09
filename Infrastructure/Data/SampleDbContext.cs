using System;
using ICT.Template.Core.Entities;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework;
using Infrabel.ICT.Framework.Service;
using Microsoft.EntityFrameworkCore;

namespace ICT.Template.Infrastructure.Data
{
    public class SampleDbContext : DbContextBase
    {
        public SampleDbContext(IDateService dateService, IUserContext userContext, DbContextOptions<SampleDbContext> options, IConfigurationsFactory factory) : base(dateService, userContext, options, factory)
        {
            OnDebugMode = true;
            this.Database
                .EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var sample1 = new Sample
            {
                CreatedBy = "DummyUser",
                CreationDate = DateTime.UtcNow
                                       .AddDays(-1),
                Name = "TestSample",
                UpdateDate = DateTime.UtcNow
                                     .AddDays(-1),
                UpdatedBy = "DummyUser"
            };
            sample1.SetId(1L);
            var sample2 = new Sample
            {
                CreatedBy = "DummyUser",
                CreationDate = DateTime.UtcNow
                                       .AddDays(-1),
                Name = "TestSample2",
                UpdateDate = DateTime.UtcNow
                                     .AddDays(-1),
                UpdatedBy = "DummyUser"
            };
            sample2.SetId(2L);
            var sample3 = new Sample
            {
                CreatedBy = null,
                CreationDate = DateTime.UtcNow
                                       .AddDays(-1),
                Name = "TestSample2",
                UpdateDate = DateTime.UtcNow
                                     .AddDays(-1),
                UpdatedBy = "DummyUser"
            };
            sample3.SetId(3L);
            modelBuilder.Entity<Sample>()
                        .HasData(sample1, sample2, sample3);
        }
    }
}