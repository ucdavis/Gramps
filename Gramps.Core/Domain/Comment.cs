﻿using System;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace Gramps.Core.Domain
{
    public class Comment : DomainObject
    {
        #region Constructor
        public Comment()
        {
            SetDefaults();
        }

        public void SetDefaults()
        {
            CreatedDate = DateTime.Now;
        }

        #endregion Constructor

        #region Mapped Fields
        public virtual string Text { get; set; }
        public virtual DateTime CreatedDate { get; set; }

        [NotNull]
        public virtual Proposal Proposal { get; set; }
        [NotNull]
        public virtual Editor Editor { get; set; }

        #endregion Mapped Fields
    }

    public class CommentMap : ClassMap<Comment>
    {
        public CommentMap()
        {
            Map(x => x.Text);
            Map(x => x.CreatedDate);
            References(x => x.Proposal);
            References(x => x.Editor);
        }
    }
}
