using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRevision.DbContext
{
    public class FoldersViewModel
    {
        public string FolderName { get; set; }
        public string TabName { get; set; }
        public int TotalUsers { get; set; }
        public List<UserDetails> UsersList { get; set; }
    }
}