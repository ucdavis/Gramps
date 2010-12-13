﻿using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace Gramps.Core.Domain
{
    public class Editor : DomainObject
    {
        #region Constructor
        public Editor()
        {
            SetDefaults();
        }

        protected void SetDefaults()
        {
            IsOwner = false;
            ReviewerId = Guid.NewGuid();
            Comments = new List<Comment>();
        }

        #endregion Constructor

        #region Mapped Fields
        public virtual bool IsOwner { get; set; }
        [Email]
        [Length(100)]
        public virtual string ReviewerEmail { get; set; }
        [Length(200)]
        public virtual string ReviewerName { get; set; }
        
        public virtual Guid ReviewerId { get; set; }

        public virtual Template Template { get; set; }
        public virtual CallForProposal CallForProposal { get; set; }
        public virtual User User { get; set; }

        public virtual IList<Comment> Comments { get; set; }
 
        #endregion Mapped Fields

        #region Validation Fields

        [AssertTrue(Message = "Must be related to Template or CallForProposal not both.")]
        private bool RelatedTable
        {
            get
            {
                if ((Template == null && CallForProposal == null) || (Template != null && CallForProposal != null))
                {
                    return false;
                }
                return true;
            }
        }

        #endregion Validation Fields
    }

    public class EditorMap : ClassMap<Editor>
    {
        public EditorMap()
        {
            Id(x => x.Id);
            Map(x => x.IsOwner);
            Map(x => x.ReviewerEmail);
            Map(x => x.ReviewerName);
            Map(x => x.ReviewerId);

            References(x => x.Template);
            References(x => x.CallForProposal);
            References(x => x.User);
            HasMany(x => x.Comments);
        }
    }
}
