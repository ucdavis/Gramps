﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gramps.Core.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class StaticValues {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StaticValues() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Gramps.Core.Resources.StaticValues", typeof(StaticValues).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email sent out to list for proposals.
        /// </summary>
        public static string InitialCall {
            get {
                return ResourceManager.GetString("InitialCall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email sent out to applicant if proposal is approved.
        /// </summary>
        public static string ProposalApproved {
            get {
                return ResourceManager.GetString("ProposalApproved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email sent out to applicant to confirm proposal has been submitted.
        /// </summary>
        public static string ProposalConfirmation {
            get {
                return ResourceManager.GetString("ProposalConfirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email sent out to applicant to notify them that their proposal has not been accepted.
        /// </summary>
        public static string ProposalDenied {
            get {
                return ResourceManager.GetString("ProposalDenied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email sent to applicant to inform them their proposal has been set back to edit.
        /// </summary>
        public static string ProposalUnsubmitted {
            get {
                return ResourceManager.GetString("ProposalUnsubmitted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email sent out to list of reviewers to notify them that the proposals are ready to be reviewed.
        /// </summary>
        public static string ReadyForReview {
            get {
                return ResourceManager.GetString("ReadyForReview", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email sent out to list applicants that have not finilized their proposals.
        /// </summary>
        public static string ReminderCallIsAboutToClose {
            get {
                return ResourceManager.GetString("ReminderCallIsAboutToClose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to VersionKey.
        /// </summary>
        public static string VersionKey {
            get {
                return ResourceManager.GetString("VersionKey", resourceCulture);
            }
        }
    }
}
