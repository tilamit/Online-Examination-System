//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineRevision.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuestionSet
    {
        public int QuestionSetId { get; set; }
        public string QuestionSetName { get; set; }
        public Nullable<int> Timer { get; set; }
        public string Details { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string TabName { get; set; }
    }
}
