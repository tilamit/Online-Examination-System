using OnlineRevision.DbContext;
using OnlineRevision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRevision.Models
{
    public class FoldersViewModel
    {
        public int SetId { get; set; }
        public IEnumerable<string> Folders { get; set; }
        public string FolderName { get; set; }
        public string TabName { get; set; }
        public IEnumerable<QuestionSet> Tabs { get; set; }
        public int TotalUsers { get; set; }
        public List<Users> UsersList { get; set; }
        public List<QuestionSet> QuestionSetList { get; set; }
    }
}