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
    
    public partial class Folders
    {
        public int Id { get; set; }
        public string FolderName { get; set; }
        public string TabName { get; set; }
        public string Details { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
    }
}
