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
    
    public partial class UserDetails
    {
        public int UserId { get; set; }
        public int NewPass { get; set; }
        public string UserPassPre { get; set; }
        public string PaymentId { get; set; }
        public Nullable<System.DateTime> ValidTill { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Institution { get; set; }
        public string Email { get; set; }
        public Nullable<int> UserType { get; set; }
        public Nullable<int> Sex { get; set; }
        public string ImgPath { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<double> YearContinuing { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string TabType { get; set; }
    }
}
