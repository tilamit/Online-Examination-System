using Newtonsoft.Json;
using OnlineRevision.DbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineRevision.Controllers
{
    public class ResultController : Controller
    {
        // GET: Result
        public ActionResult Index(int id)
        {
            List<QuestionSetViewModel> answers = new List<QuestionSetViewModel>();
            List<QuestionSetViewModel> aLst = null;

            double val = 0;
            double valTotal = 0;
            double valResult = 0;

            if (Session["userId"] != null)
            {
                int resultId = 0;
                TempData["id"] = Session["userId"].ToString();

                if (Session["myList"] != null)
                {
                    aLst = (List<QuestionSetViewModel>)Session["myList"];

                    using (OnlineRevisionEntities db = new OnlineRevisionEntities())
                    {
                        if (Session["myList"] != null)
                        {
                            aLst = (List<QuestionSetViewModel>)Session["myList"];
                        }

                        int userId = Convert.ToInt32(Session["userId"].ToString());

                        bool chkResult = aLst.Where(c => c.UserId == userId && c.QuestionSetId == id).Any();

                        if (chkResult)
                        {
                            //Save details in 'Results' table here
                            Results aResults = new Results();
                            aResults.StudentId = userId;
                            aResults.CreatedOn = DateTime.Now;
                            aResults.Status = 1;
                            aResults.QuestionSetId = id;

                            resultId = aResults.Id;
                            TempData["result"] = resultId.ToString();
                            TempData["quesSet"] = id.ToString();

                            try
                            {
                                db.Results.Add(aResults);
                                db.SaveChanges();

                                resultId = aResults.Id;
                                TempData["result"] = resultId.ToString();
                            }
                            catch (Exception ex)
                            {
                                ex.ToString();
                            }
                        }

                        //Save answers here
                        var resultStudent = (from c in db.StudentResults
                                             where c.StudentId == id && c.Status == 1 && c.Id == resultId
                                             select c).ToList();

                        if (resultStudent.Count == 0)
                        {
                            foreach (var record in aLst.Where(c => c.QuestionSetId == id && c.UserId == userId))
                            {
                                try
                                {
                                    StudentResults aStudentResults = new StudentResults();

                                    aStudentResults.QuestionSetId = record.QuestionSetId;
                                    aStudentResults.QuestionId = record.QuestionId;
                                    aStudentResults.Id = resultId;
                                    aStudentResults.QuestionAnswers = record.QuestionAnswers;
                                    aStudentResults.ExamTaken = DateTime.Now;
                                    aStudentResults.Status = 1;
                                    aStudentResults.StudentId = record.UserId;

                                    db.StudentResults.Add(aStudentResults);
                                    db.SaveChanges();

                                    Thread.Sleep(2000);
                                }

                                catch (Exception ex)
                                {
                                    ex.ToString();
                                }
                            }
                        }

                        if (aLst != null)
                        {
                            foreach (var item2 in aLst)
                            {
                                string[] splitOptions2 = null;
                                if (item2.QuestionType == 2)
                                {
                                    splitOptions2 = item2.QuestionAnswers.Split('_');
                                    //splitOptions2 = item2.QuestionAnswers.Substring(item2.QuestionAnswers.IndexOf(".") + 1).Split('_');
                                }
                                else
                                {
                                    splitOptions2 = item2.QuestionAnswers.Split('_');
                                }

                                var result = db.Answers.Where(c => c.QuestionId == item2.QuestionId && c.QuestionSetId == item2.QuestionSetId).GroupBy(q => new
                                {
                                    q.QuestionSetId,
                                    q.QuestionId,
                                }, p => p.QuestionAnswers)
                                .Select(
                                    grp => new QuestionSetViewModel
                                    {
                                        QuestionSetId = grp.Key.QuestionSetId,
                                        QuestionId = grp.Key.QuestionId,
                                        AllAnswers = grp.ToList()
                                    }
                                ).ToList();

                                QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();
                                int cnt = 0;

                                List<string> aLstAnswers = new List<string>();

                                foreach (var item in result)
                                {
                                    cnt = item.AllAnswers.Count();
                                    aLstAnswers = item.AllAnswers;
                                }

                                bool equal = false;

                                if (item2.QuestionAnswers != "")
                                {
                                    if (item2.QuestionType == 1)
                                    {
                                        equal = aLstAnswers.OrderBy(c => c.FirstOrDefault()).SequenceEqual(splitOptions2.OrderBy(c => c.FirstOrDefault()));
                                    }
                                    else
                                    {
                                        equal = aLstAnswers.SequenceEqual(splitOptions2);
                                    }

                                    if (equal == true)
                                    {
                                        aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                        aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                        aQuestionSetViewModel.QuestionAnswers = "Correct";
                                    }
                                    else if (equal == false)
                                    {
                                        aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                        aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                        aQuestionSetViewModel.QuestionAnswers = "Wrong";
                                    }
                                }
                                else
                                {
                                    aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                    aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                    aQuestionSetViewModel.QuestionAnswers = "";
                                }

                                answers.Add(aQuestionSetViewModel);
                            }

                            val = answers.Where(c => c.QuestionAnswers.Contains("Correct")).Count();
                            valTotal = answers.Count();
                            valResult = (val / valTotal) * 100;

                            TempData["Percentage"] = String.Format("{0:0.00}", valResult);
                        }
                    }

                    //return RedirectToAction("Index", "Result", new { id = resultId });
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetQuestions(int id, int questionId)
        {
            List<QuestionSetViewModel> aLst = null;
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = db.Questions.Where(c => c.QuestionSetId == id && c.QuestionId == questionId).GroupBy(q => new
                {
                    q.QuestionId,
                    q.QuestionName
                }, p => p.Options)
                        .Select(
                            grp => new QuestionSetViewModel
                            {
                                QuestionId = grp.Key.QuestionId,
                                QuestionName = grp.Key.QuestionName,
                                AllOptions = grp.ToList()
                            }
                        ).ToList();
            }

            return await Task.Run(() => Json(aLst, JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public async Task<JsonResult> GetAttemptedQuestions(int resultId, int quesSetId)
        {
            List<QuestionSetViewModel> answers = new List<QuestionSetViewModel>();
            List<QuestionSetViewModel> aLst = null;

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                if (Session["myList"] != null)
                {
                    aLst = (List<QuestionSetViewModel>)Session["myList"];
                }

                int id = Convert.ToInt32(Session["userId"].ToString());

                if (aLst != null)
                {
                    foreach (var item2 in aLst)
                    {
                        string[] splitOptions2 = null;
                        if (item2.QuestionType == 2)
                        {
                            splitOptions2 = item2.QuestionAnswers.Split('_');
                            //splitOptions2 = item2.QuestionAnswers.Substring(item2.QuestionAnswers.IndexOf(".") + 1).Split('_');
                        }
                        else
                        {
                            splitOptions2 = item2.QuestionAnswers.Split('_');
                        }

                        var result = db.Answers.Where(c => c.QuestionId == item2.QuestionId && c.QuestionSetId == item2.QuestionSetId).GroupBy(q => new
                        {
                            q.QuestionSetId,
                            q.QuestionId,
                        }, p => p.QuestionAnswers)
                        .Select(
                            grp => new QuestionSetViewModel
                            {
                                QuestionSetId = grp.Key.QuestionSetId,
                                QuestionId = grp.Key.QuestionId,
                                AllAnswers = grp.ToList()
                            }
                        ).ToList();

                        QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();
                        int cnt = 0;

                        List<string> aLstAnswers = new List<string>();

                        foreach (var item in result)
                        {
                            cnt = item.AllAnswers.Count();
                            aLstAnswers = item.AllAnswers;
                        }

                        bool equal = false;

                        if (item2.QuestionAnswers != "")
                        {
                            if (item2.QuestionType == 1)
                            {
                                equal = aLstAnswers.OrderBy(c => c.FirstOrDefault()).SequenceEqual(splitOptions2.OrderBy(c => c.FirstOrDefault()));
                            }
                            else
                            {
                                equal = aLstAnswers.SequenceEqual(splitOptions2);
                            }

                            if (equal == true)
                            {
                                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                aQuestionSetViewModel.QuestionAnswers = "Correct";
                            }
                            else if (equal == false)
                            {
                                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                aQuestionSetViewModel.QuestionAnswers = "Wrong";
                            }
                        }
                        else
                        {
                            aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                            aQuestionSetViewModel.QuestionId = item2.QuestionId;
                            aQuestionSetViewModel.QuestionAnswers = "";
                        }

                        answers.Add(aQuestionSetViewModel);
                    }

                    int correct = (from n in answers
                                   where n.QuestionAnswers == "Correct"
                                   select n).Count();

                    int inCorrect = (from n in answers
                                   where n.QuestionAnswers == "Wrong"
                                   select n).Count();

                    int inComplete = (from n in answers
                                     where n.QuestionAnswers == ""
                                     select n).Count();


                    var results = db.Results.Find(resultId);

                    results.Correct = correct;
                    results.Incorrect = inCorrect;
                    results.Incomplete = inComplete;
                    results.Percentage = 100;
                    results.Status = 1;
                    results.CreatedOn = DateTime.Now;

                    db.Entry(results).State = EntityState.Modified;
                    db.SaveChanges();

                    aLst.RemoveAll(c => c.QuestionSetId == quesSetId && c.UserId == id);
                    Session["myList"] = aLst;

                    //RestoreAttemptedExam(quesSetId);

                    //Session.Remove("myList");
                }
                //else
                //{
                //    foreach (var item2 in resultStudent)
                //    {
                //        string[] splitOptions2 = item2.QuestionAnswers.Split(',');

                //        var result = db.Answers.Where(c => c.QuestionId == item2.QuestionId && c.QuestionSetId == item2.QuestionSetId).GroupBy(q => new
                //        {
                //            q.QuestionSetId,
                //            q.QuestionId,
                //        }, p => p.QuestionAnswers)
                //        .Select(
                //            grp => new QuestionSetViewModel
                //            {
                //                QuestionSetId = grp.Key.QuestionSetId,
                //                QuestionId = grp.Key.QuestionId,
                //                AllAnswers = grp.ToList()
                //            }
                //        ).ToList();

                //        QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();
                //        int cnt = 0;

                //        List<string> aLstAnswers = new List<string>();

                //        foreach (var item in result)
                //        {
                //            cnt = item.AllAnswers.Count();
                //            aLstAnswers = item.AllAnswers;
                //        }

                //        bool equal = false;

                //        if (item2.QuestionAnswers != "")
                //        {
                //            equal = aLstAnswers.SequenceEqual(splitOptions2);

                //            if (equal == true)
                //            {
                //                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                //                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                //                aQuestionSetViewModel.QuestionAnswers = "Correct";
                //            }
                //            else if (equal == false)
                //            {
                //                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                //                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                //                aQuestionSetViewModel.QuestionAnswers = "Wrong";
                //            }
                //        }
                //        else
                //        {
                //            aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                //            aQuestionSetViewModel.QuestionId = item2.QuestionId;
                //            aQuestionSetViewModel.QuestionAnswers = "";
                //        }

                //        answers.Add(aQuestionSetViewModel);
                //    }
                //}

                //val = answers.Where(c => c.QuestionAnswers.Contains("Correct")).Count();
                //valTotal = answers.Count();
                //valResult = (val / valTotal) * 100;

                //TempData["Percentage"] = String.Format("{0:0.00}", valResult);
            }

            return await Task.Run(() => Json(answers, JsonRequestBehavior.AllowGet));
        }

        public async Task<JsonResult> GetAnswers(int id, int questionId)
        {
            string val = "";

            List<string> str = new List<string>();
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                var result = (from c in db.Answers
                              where c.QuestionId == questionId && c.QuestionSetId == id
                              select c).ToList();

                if (result.Count == 1)
                {
                    foreach (var item in result.OrderByDescending(c => c.QuestionAnswers))
                    {
                        val = item.QuestionAnswers;
                    }
                }
                else
                {
                    foreach (var item in result.OrderByDescending(c => c.QuestionAnswers))
                    {
                        val = item.QuestionAnswers;
                        str.Add(val);
                    }

                    val = string.Join(", ", str);
                }
            }

            return await Task.Run(() => Json(val.TrimEnd(','), JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public async Task<JsonResult> GetResult(int userId)
        {
            List<QuestionSetViewModel> aLst = null;
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = (from c in db.Results
                        where c.StudentId == userId
                        select new QuestionSetViewModel
                        {
                            UserId = c.StudentId,
                            QuestionSetId = c.QuestionSetId,
                            Id = c.Id,
                            Percentage = c.Percentage,
                            Correct = c.Correct,
                            Incorrect = c.Incorrect,
                            Incomplete = c.Incomplete,
                            CreatedOn = c.CreatedOn
                            //ExamDate = c.CreatedOn.ToString("dd/mm/yyyy", CultureInfo.InvariantCulture)
                        }).ToList();
            }

            return await Task.Run(() => Json(aLst, JsonRequestBehavior.AllowGet));
        }

        //[HttpPost]
        public ActionResult ShowResult(int id, int setId)
        {
            List<QuestionSetViewModel> aLst = null;
            List<QuestionSetViewModel> answers = new List<QuestionSetViewModel>();

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                int userId = Convert.ToInt32(Session["userId"].ToString());

                aLst = (from c in db.StudentResults
                            //join f in db.Results on c.Id equals f.Id
                            //join d in db.Questions on c.QuestionId equals d.QuestionId
                        where c.StudentId == userId && c.QuestionSetId == setId && c.Id == id
                        select new QuestionSetViewModel
                        {
                            UserId = c.StudentId,
                            QuestionSetId = c.QuestionSetId,
                            //QuestionName = d.QuestionName,
                            QuestionId = c.QuestionId,
                            QuestionAnswers = c.QuestionAnswers
                        }).ToList();

                if (aLst != null)
                {
                    foreach (var item2 in aLst)
                    {
                        string[] splitOptions2 = item2.QuestionAnswers.Split(',');

                        var result = db.Answers.Where(c => c.QuestionId == item2.QuestionId && c.QuestionSetId == item2.QuestionSetId).GroupBy(q => new
                        {
                            q.QuestionSetId,
                            q.QuestionId,
                        }, p => p.QuestionAnswers)
                        .Select(
                            grp => new QuestionSetViewModel
                            {
                                QuestionSetId = grp.Key.QuestionSetId,
                                QuestionId = grp.Key.QuestionId,
                                AllAnswers = grp.ToList()
                            }
                        ).ToList();

                        QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();
                        int cnt = 0;

                        List<string> aLstAnswers = new List<string>();

                        foreach (var item in result)
                        {
                            cnt = item.AllAnswers.Count();
                            aLstAnswers = item.AllAnswers;
                        }

                        bool equal = false;
                        string ques = "";

                        if (item2.QuestionAnswers != "")
                        {
                            if (item2.QuestionType == 1)
                            {
                                equal = aLstAnswers.OrderBy(c => c.FirstOrDefault()).SequenceEqual(splitOptions2.OrderBy(c => c.FirstOrDefault()));
                            }
                            else
                            {
                                equal = aLstAnswers.SequenceEqual(splitOptions2);
                            }

                            if (equal == true)
                            {
                                var item = (from c in db.Questions
                                            where c.QuestionId == item2.QuestionId
                                            select c.QuestionName).FirstOrDefault();

                                ques = item;

                                aQuestionSetViewModel.QuestionName = ques;
                                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                aQuestionSetViewModel.QuestionAnswers = "Correct" + item2.QuestionAnswers;
                            }
                            else if (equal == false)
                            {
                                var item = (from c in db.Questions
                                            where c.QuestionId == item2.QuestionId
                                            select c.QuestionName).FirstOrDefault();

                                ques = item;

                                aQuestionSetViewModel.QuestionName = ques;
                                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                aQuestionSetViewModel.QuestionAnswers = "Wrong" + item2.QuestionAnswers;
                            }
                        }
                        else
                        {
                            aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                            aQuestionSetViewModel.QuestionId = item2.QuestionId;
                            aQuestionSetViewModel.QuestionAnswers = "";
                        }

                        answers.Add(aQuestionSetViewModel);
                    }
                }
            }

            return View(answers);
        }

        public void RestoreAttemptedExam(int setId)
        {
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                try
                {
                    int userId = Convert.ToInt32(Session["userId"].ToString());
                    var result = db.AttemptedExam.Where(c => c.UserId == userId && c.QuestionSetId == setId);

                    if (result.Count() > 0)
                    {
                        foreach (var item in result)
                        {
                            item.Status = 0;
                        }

                        db.SaveChanges();
                    }
                }

                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }


        public ActionResult FreeTrial(int id)
        {
            List<QuestionSetViewModel> answers = new List<QuestionSetViewModel>();
            List<QuestionSetViewModel> aLst = null;

            double val = 0;
            double valTotal = 0;
            double valResult = 0;

            if (Session["userId"] != null)
            {
                TempData["id"] = Session["userId"].ToString();

                if (Session["myList"] != null)
                {
                    aLst = (List<QuestionSetViewModel>)Session["myList"];

                    using (OnlineRevisionEntities db = new OnlineRevisionEntities())
                    {
                        if (Session["myList"] != null)
                        {
                            aLst = (List<QuestionSetViewModel>)Session["myList"];
                        }

                        int userId = Convert.ToInt32(Session["userId"].ToString());

                        if (aLst != null)
                        {
                            foreach (var item2 in aLst)
                            {
                                string[] splitOptions2 = null;
                                if (item2.QuestionType == 2)
                                {
                                    splitOptions2 = item2.QuestionAnswers.Split('_');
                                    //splitOptions2 = item2.QuestionAnswers.Substring(item2.QuestionAnswers.IndexOf(".") + 1).Split('_');
                                }
                                else
                                {
                                    splitOptions2 = item2.QuestionAnswers.Split('_');
                                }

                                var result = db.Answers.Where(c => c.QuestionId == item2.QuestionId && c.QuestionSetId == item2.QuestionSetId).GroupBy(q => new
                                {
                                    q.QuestionSetId,
                                    q.QuestionId,
                                }, p => p.QuestionAnswers)
                                .Select(
                                    grp => new QuestionSetViewModel
                                    {
                                        QuestionSetId = grp.Key.QuestionSetId,
                                        QuestionId = grp.Key.QuestionId,
                                        AllAnswers = grp.ToList()
                                    }
                                ).ToList();

                                QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();
                                int cnt = 0;

                                List<string> aLstAnswers = new List<string>();

                                foreach (var item in result)
                                {
                                    cnt = item.AllAnswers.Count();
                                    aLstAnswers = item.AllAnswers;
                                }

                                bool equal = false;

                                if (item2.QuestionAnswers != "")
                                {
                                    if (item2.QuestionType == 1)
                                    {
                                        equal = aLstAnswers.OrderBy(c => c.FirstOrDefault()).SequenceEqual(splitOptions2.OrderBy(c => c.FirstOrDefault()));
                                    }
                                    else
                                    {
                                        equal = aLstAnswers.SequenceEqual(splitOptions2);
                                    }

                                    if (equal == true)
                                    {
                                        aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                        aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                        aQuestionSetViewModel.QuestionAnswers = "Correct";
                                    }
                                    else if (equal == false)
                                    {
                                        aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                        aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                        aQuestionSetViewModel.QuestionAnswers = "Wrong";
                                    }
                                }
                                else
                                {
                                    aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                    aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                    aQuestionSetViewModel.QuestionAnswers = "";
                                }

                                answers.Add(aQuestionSetViewModel);
                            }

                            val = answers.Where(c => c.QuestionAnswers.Contains("Correct")).Count();
                            valTotal = answers.Count();
                            valResult = (val / valTotal) * 100;

                            TempData["Percentage"] = String.Format("{0:0.00}", valResult);
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("FreeTrial", "Exam");
            }

            return View();
        }

        public ActionResult MockExam(int id)
        {
            List<QuestionSetViewModel> answers = new List<QuestionSetViewModel>();
            List<QuestionSetViewModel> aLst = null;

            double val = 0;
            double valTotal = 0;
            double valResult = 0;

            if (Session["userId"] != null)
            {
                TempData["id"] = Session["userId"].ToString();

                if (Session["myList"] != null)
                {
                    aLst = (List<QuestionSetViewModel>)Session["myList"];

                    using (OnlineRevisionEntities db = new OnlineRevisionEntities())
                    {
                        if (Session["myList"] != null)
                        {
                            aLst = (List<QuestionSetViewModel>)Session["myList"];
                        }

                        int userId = Convert.ToInt32(Session["userId"].ToString());

                        if (aLst != null)
                        {
                            foreach (var item2 in aLst)
                            {
                                string[] splitOptions2 = null;
                                if (item2.QuestionType == 2)
                                {
                                    splitOptions2 = item2.QuestionAnswers.Split('_');
                                    //splitOptions2 = item2.QuestionAnswers.Substring(item2.QuestionAnswers.IndexOf(".") + 1).Split('_');
                                }
                                else
                                {
                                    splitOptions2 = item2.QuestionAnswers.Split('_');
                                }

                                var result = db.Answers.Where(c => c.QuestionId == item2.QuestionId && c.QuestionSetId == item2.QuestionSetId).GroupBy(q => new
                                {
                                    q.QuestionSetId,
                                    q.QuestionId,
                                }, p => p.QuestionAnswers)
                                .Select(
                                    grp => new QuestionSetViewModel
                                    {
                                        QuestionSetId = grp.Key.QuestionSetId,
                                        QuestionId = grp.Key.QuestionId,
                                        AllAnswers = grp.ToList()
                                    }
                                ).ToList();

                                QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();
                                int cnt = 0;

                                List<string> aLstAnswers = new List<string>();

                                foreach (var item in result)
                                {
                                    cnt = item.AllAnswers.Count();
                                    aLstAnswers = item.AllAnswers;
                                }

                                bool equal = false;

                                if (item2.QuestionAnswers != "")
                                {
                                    if (item2.QuestionType == 1)
                                    {
                                        equal = aLstAnswers.OrderBy(c => c.FirstOrDefault()).SequenceEqual(splitOptions2.OrderBy(c => c.FirstOrDefault()));
                                    }
                                    else
                                    {
                                        equal = aLstAnswers.SequenceEqual(splitOptions2);
                                    }

                                    if (equal == true)
                                    {
                                        aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                        aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                        aQuestionSetViewModel.QuestionAnswers = "Correct";
                                    }
                                    else if (equal == false)
                                    {
                                        aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                        aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                        aQuestionSetViewModel.QuestionAnswers = "Wrong";
                                    }
                                }
                                else
                                {
                                    aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                    aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                    aQuestionSetViewModel.QuestionAnswers = "";
                                }

                                answers.Add(aQuestionSetViewModel);
                            }

                            val = answers.Where(c => c.QuestionAnswers.Contains("Correct")).Count();
                            valTotal = answers.Count();
                            valResult = (val / valTotal) * 100;

                            TempData["Percentage"] = String.Format("{0:0.00}", valResult);
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("MockExam", "Exam");
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAttemptedFreeTrial(int quesSetId)
        {
            List<QuestionSetViewModel> answers = new List<QuestionSetViewModel>();
            List<QuestionSetViewModel> aLst = null;

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                if (Session["myList"] != null)
                {
                    aLst = (List<QuestionSetViewModel>)Session["myList"];
                }

                int id = Convert.ToInt32(Session["userId"].ToString());

                if (aLst != null)
                {
                    foreach (var item2 in aLst)
                    {
                        string[] splitOptions2 = null;
                        if (item2.QuestionType == 2)
                        {
                            splitOptions2 = item2.QuestionAnswers.Split('_');
                        }
                        else
                        {
                            splitOptions2 = item2.QuestionAnswers.Split('_');
                        }

                        var result = db.Answers.Where(c => c.QuestionId == item2.QuestionId && c.QuestionSetId == item2.QuestionSetId).GroupBy(q => new
                        {
                            q.QuestionSetId,
                            q.QuestionId,
                        }, p => p.QuestionAnswers)
                        .Select(
                            grp => new QuestionSetViewModel
                            {
                                QuestionSetId = grp.Key.QuestionSetId,
                                QuestionId = grp.Key.QuestionId,
                                AllAnswers = grp.ToList()
                            }
                        ).ToList();

                        QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();
                        int cnt = 0;

                        List<string> aLstAnswers = new List<string>();

                        foreach (var item in result)
                        {
                            cnt = item.AllAnswers.Count();
                            aLstAnswers = item.AllAnswers;
                        }

                        bool equal = false;

                        if (item2.QuestionAnswers != "")
                        {
                            if (item2.QuestionType == 1)
                            {
                                equal = aLstAnswers.OrderBy(c => c.FirstOrDefault()).SequenceEqual(splitOptions2.OrderBy(c => c.FirstOrDefault()));
                            }
                            else
                            {
                                equal = aLstAnswers.SequenceEqual(splitOptions2);
                            }

                            if (equal == true)
                            {
                                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                aQuestionSetViewModel.QuestionAnswers = "Correct";
                            }
                            else if (equal == false)
                            {
                                aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                                aQuestionSetViewModel.QuestionId = item2.QuestionId;
                                aQuestionSetViewModel.QuestionAnswers = "Wrong";
                            }
                        }
                        else
                        {
                            aQuestionSetViewModel.QuestionSetId = item2.QuestionSetId;
                            aQuestionSetViewModel.QuestionId = item2.QuestionId;
                            aQuestionSetViewModel.QuestionAnswers = "";
                        }

                        answers.Add(aQuestionSetViewModel);
                    }

                    int correct = (from n in answers
                                   where n.QuestionAnswers == "Correct"
                                   select n).Count();

                    int inCorrect = (from n in answers
                                     where n.QuestionAnswers == "Wrong"
                                     select n).Count();

                    int inComplete = (from n in answers
                                      where n.QuestionAnswers == ""
                                      select n).Count();


                    aLst.RemoveAll(c => c.QuestionSetId == quesSetId && c.UserId == id);
                    Session["myList"] = aLst;
                }
            }

            return await Task.Run(() => Json(answers, JsonRequestBehavior.AllowGet));
        }
    }
}