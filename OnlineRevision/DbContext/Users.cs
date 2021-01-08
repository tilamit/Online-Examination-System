using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRevision.DbContext
{
    public class Users : UserDetails
    {
        public string AddedOn { get; set; }
        public string type { get; set; }
        public int TotalSubscription { get; set; }
        public string University { get; set; }
        public int TotalArchived { get; set; }
    }
}