using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRevision.DbContext
{
    public class QuestionSetViewModel : Questions
    {
        public int? UserId { get; set; }
        public string QuestionSetName { get; set; }
        public string QuestionAnswers { get; set; }
        public string Details { get; set; }
        public int Flag { get; set; }
        public string Explanation { get; set; }
        public List<string> AllOptions { get; set; }
        public List<string> AllAnswers { get; set; }
        public double? Percentage { get; set; }
        public int? Correct { get; set; }
        public int? Incorrect { get; set; }
        public int? Incomplete { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalAttempted { get; set; }
        public string ExamDate { get; set; }
    }
}