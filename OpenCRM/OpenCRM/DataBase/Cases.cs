//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenCRM.DataBase
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cases
    {
        public int CaseId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public Nullable<int> AccountId { get; set; }
        public Nullable<int> CaseTypeId { get; set; }
        public Nullable<int> CaseReasonId { get; set; }
        public Nullable<int> CaseOriginId { get; set; }
        public Nullable<int> CaseStatusId { get; set; }
        public Nullable<int> CasePriorityId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> HiddenCase { get; set; }
        public string SolutionTitle { get; set; }
        public string SolutionDescription { get; set; }
        public System.DateTime ViewDate { get; set; }
        public string Subject { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Case_Origin Case_Origin { get; set; }
        public virtual Case_Priority Case_Priority { get; set; }
        public virtual Case_Reason Case_Reason { get; set; }
        public virtual Case_Status Case_Status { get; set; }
        public virtual Case_Type Case_Type { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual Products Products { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
