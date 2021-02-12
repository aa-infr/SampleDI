using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    [ExcludeFromCodeCoverage]
    public abstract class DbContextBase : DbContext
    {
        private const int MaxLoopBaseType = 8;
        private readonly IConfigurationsFactory _configurationsFactory;
        protected readonly IDateService DateService;
        private readonly LoggerFactory _loggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });
        protected readonly IUserContext UserContext;
        //internal LanguageEntityConfiguration LanguageConfiguration;

        protected DbContextBase(IDateService dateService, IUserContext userContext, DbContextOptions options,
            IConfigurationsFactory configurationsFactory) : base(options)
        {
            ContextName = options.ContextType.Name;
            DateService = dateService;
            UserContext = userContext;
            _configurationsFactory = configurationsFactory;
        }

        internal string ContextName { get; }

        public bool OnDebugMode { get; set; } = false;

        protected virtual DateTime Now => DateService.UtcNow;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ApplyAllConfigurations(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(OnDebugMode)
            {
                optionsBuilder.UseLoggerFactory(_loggerFactory);
                optionsBuilder.EnableSensitiveDataLogging(true);
            }
        }

        private void RefineEntries()
        {
            var identity = UserContext.Identity;
            ChangeTracker.DetectChanges();

            ChangeTracker.Entries()
                         .LazyConvertAndExecute<EntityEntry, IAuditableEntity>(a => RefineUpdateAudit(a, identity),
                             e => e.State == EntityState.Modified,
                             e => e.Entity)
                         .LazyConvertAndExecute<EntityEntry, IAuditableEntity>(a =>
                                                                               {
                                                                                   RefineCreateAudit(a, identity);
                                                                                   RefineUpdateAudit(a, identity);
                                                                               }, e => e.State == EntityState.Added, e => e.Entity)
                         .ConvertAndExecute<EntityEntry, IExpirable>(RefineExpirable, e => e.State == EntityState.Added,
                             e => e.Entity);
        }

        private void RefineExpirable(IExpirable expirable)
        {
            expirable.ValidFrom = Now;
        }

        public bool Contains(Type type)
        {
            if(type == null)
                return false;
            foreach(var prop in GetType()
                                .GetProperties())
                if(prop.PropertyType.GenericTypeArguments != null && prop.PropertyType.GenericTypeArguments.Length > 0)
                {
                    var targetType = prop.PropertyType.GenericTypeArguments[0];
                    if(string.Equals(targetType.FullName, type.FullName))
                        return true;
                }

            return false;
        }

        private void ApplyAllConfigurations(ModelBuilder modelBuilder)
        {
            var configurations = _configurationsFactory.GetConfigurationTypes(ContextName, OnDebugMode);
            foreach(var configuration in configurations)
            {
                modelBuilder.ApplyConfiguration(configuration);
                var configurationType = configuration.GetType();
                var genType = GetGenericType(configurationType);
                if(genType != null)
                    WriteDebug($"Load Configuration {genType.FullName}");

                #region add lexicon configuration

                //if(genType != null && typeof(ILanguage).IsAssignableFrom(genType))
                //    // duplicate configuration for Language entity
                //    LanguageConfiguration = new LanguageEntityConfiguration(
                //        configuration.GetDataBaseProvider(),
                //        configuration.GetTableAlias(),
                //        configuration.GetSchemaName(),
                //        configuration.GetTableName());
                #endregion add lexicon configuration
            }
        }

        protected virtual void RefineUpdateAudit(IAuditableEntity auditable, IUserIdentity identity)
        {
            auditable.UpdateDate = Now;
            var userName = identity.Name;
            if(!string.IsNullOrWhiteSpace(userName))
                auditable.UpdatedBy = userName;
        }

        protected virtual void RefineCreateAudit(IAuditableEntity auditable, IUserIdentity identity)
        {
            auditable.CreationDate = Now;
            var userName = identity.Name;
            if(!string.IsNullOrWhiteSpace(userName))
                auditable.CreatedBy = userName;
        }

        private static Type GetGenericType(Type type)
        {
            var currentType = type;
            for(var i = 0; i < MaxLoopBaseType; ++i)
            {
                if(currentType.GetGenericArguments()?.Length > 0)
                    return currentType.GetGenericArguments()[0];
                currentType = currentType.BaseType;
            }

            return null;
        }

        internal void WriteDebug(string message)
        {
            if(OnDebugMode)
                Trace.WriteLine(message);
        }

        #region SaveChanges hook

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            RefineEntries();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            RefineEntries();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        #endregion SaveChanges hook
    }
}