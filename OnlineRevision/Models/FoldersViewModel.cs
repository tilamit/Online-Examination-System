using OnlineRevision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRevision.Models
{
    public class FoldersViewModel
    {
        public string FolderName { get; set; }
        public string TabName { get; set; }
        public int TotalUsers { get; set; }
        public List<Users> UsersList { get; set; }
    }
}