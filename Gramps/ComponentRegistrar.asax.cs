﻿using Castle.Windsor;
using Gramps.Models;
using Gramps.Services;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace Gramps
{
    internal static class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);

            container.AddComponent("validator",
                                   typeof(IValidator), typeof(Validator));
            container.AddComponent("dbContext", typeof(IDbContext), typeof(DbContext));

            container.AddComponent("accessService", typeof (IAccessService), typeof (AccessService));

            container.AddComponent("emailService", typeof(IEmailService), typeof(EmailService));

        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.AddComponent("repositoryWithTypedId",
                typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));
            container.AddComponent("repositoryType",
                                   typeof(IRepository<>), typeof(Repository<>));
            container.AddComponent("repository",
                                   typeof(IRepository), typeof(Repository));

        }
    }
}