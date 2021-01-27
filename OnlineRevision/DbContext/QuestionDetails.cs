using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRevision.DbContext
{
    public class QuestionDetails
    {
        public int? aQuestionSetId { get; set; }
        public int? aQuestionId { get; set; }
        public string aQuestion { get; set; }
        public List<Questions> aOption { get; set; }
        public List<Answers> aAnswer { get; set; }
        public IEnumerable<string> aExplanation { get; set; }
    }
}