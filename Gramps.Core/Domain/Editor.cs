﻿using System;
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

        }

        public void SetDefaults()
        {
            IsOwner = false;
            ReviewerId = Guid.NewGuid();
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
        public virtual vUser User { get; set; }

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
            Map(x => x.IsOwner);
            Map(x => x.ReviewerEmail);
            Map(x => x.ReviewerName);
            Map(x => x.ReviewerId);

            References(x => x.Template);
            References(x => x.CallForProposal);
            References(x => x.User);
        }
    }
}
