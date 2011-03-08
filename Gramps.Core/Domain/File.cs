using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace Gramps.Core.Domain
{
    public class File : DomainObject
    {
        #region Constructor
        public File()
        {
           SetDefaults(); 
        }

        protected void SetDefaults()
        {
            DateAdded = DateTime.Now;
        }
        #endregion Constructor
        #region Mapped Fields

        public virtual string ContentType { get; set; }
        [Required]
        [Length(512)]
        public virtual string Name { get; set; }
        public virtual DateTime DateAdded { get; set; }
        public virtual byte[] Contents { get; set; }
        #endregion Mapped Fields
       
    }

    public class FileMap : ClassMap<File>
    {
        public FileMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.ContentType);
            Map(x => x.DateAdded);
            Map(x => x.Contents);
        }
    }
}
