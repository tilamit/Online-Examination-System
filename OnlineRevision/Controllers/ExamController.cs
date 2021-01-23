using Microsoft.Ajax.Utilities;
using OnlineRevision.DbContext;
using OnlineRevision.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineRevision.Controllers
{
    public class ExamController : Controller
    {
        // GET: Exam
        [HttpGet]
        public ActionResult Index(int? id)
        {
            if (Session["userId"] != null)
            {
                if (id == 0 || id.ToString() == null || id.ToString() == "")
                {
                    return RedirectToAction("", "Dashboard");
                }
                else
                {
                    TempData["quesSetId"] = id.ToString();
                    Session.Remove("myList");

                    TempData["bestOfThree"] = id.ToString();
                    TempData.Remove("Percentage");

                    int setId = Convert.ToInt32(id.ToString());
                    RestoreAttemptedExam(setId);
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpGet]
        public ActionResult IndexDragAndDrop(int? id)
        {
            if (Session["userId"] != null)
            {
                if (id == 0 || id.ToString() == null || id.ToString() == "")
                {
                    return RedirectToAction("", "Dashboard");
                }
                else
                {
                    TempData["quesSetId"] = id.ToString();
                    Session.Remove("myList");

                    TempData["bestOfThree"] = id.ToString();
                    TempData.Remove("Percentage");

                    int setId = Convert.ToInt32(id.ToString());
                    RestoreAttemptedExam(setId);
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpGet]
        public ActionResult MockExam(int? id)
        {
            if (Session["userId"] != null)
            {
                if (id == 0 || id.ToString() == null || id.ToString() == "")
                {
                    return RedirectToAction("", "Home");
                }
                else
                {
                    TempData["quesSetId"] = id.ToString();
                    Session.Remove("myList");

                    TempData["bestOfThree"] = id.ToString();
                    TempData.Remove("Percentage");
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpGet]
        public ActionResult FreeTrial(int? id)
        {
            if (id == 0 || id.ToString() == null || id.ToString() == "")
            {
                return RedirectToAction("", "Home");
            }
            else
            {
                TempData["quesSetId"] = id.ToString();
                Session.Remove("myList");

                TempData["bestOfThree"] = id.ToString();
                TempData.Remove("Percentage");
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllQuesSet()
        {
            List<FoldersViewModel> aLst = null;
            List<QuestionSet> aQuestionLst = GetQuestionSets();

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                try
                {
                    aLst = (from c in aQuestionLst
                            group c by new { c.TabName, c.Details }into grp
                            select new FoldersViewModel
                            {
                                TabName = grp.Key.TabName,
                                FolderName = grp.Key.Details,
                                QuestionSetList = GetQuestionSets().Where(d => d.TabName == grp.Key.TabName && d.Status == 1).OrderByDescending(d => d.QuestionSetId).ToList()
                            }).ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            return await Task.Run(() => Json(aLst, JsonRequestBehavior.AllowGet));
        }

        public List<QuestionSet> GetQuestionSets()
        {
            List<QuestionSet> aLst = null;
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = (from c in db.QuestionSet
                        where c.Status == 1
                        select c).ToList();
            }

            return aLst;
        }

        //Get questions for best of three - Practice
        public async Task<JsonResult> GetQuestions(int id)
        {
            List<QuestionSetViewModel> aLst = null;
            List<QuestionSetViewModel> aLst2 = null;
            List<QuestionSetViewModel> aLst3 = null;

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = db.Questions.Where(c => c.QuestionSetId == id && c.QuestionType == 1).GroupBy(q => new
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

                aLst2 = db.Answers.Where(c => c.QuestionSetId == id).GroupBy(q => new
                {
                    q.QuestionId,
                }, p => p.QuestionAnswers)
                       .Select(
                           grp => new QuestionSetViewModel
                           {
                               QuestionId = grp.Key.QuestionId,
                               AllAnswers = grp.ToList()
                           }
                       ).ToList();

                aLst3 = (from c in aLst
                         join d in aLst2 on c.QuestionId equals d.QuestionId
                         join f in db.Explanation on c.QuestionId equals f.QuestionId
                         where f.QuestionSetId == id
                         //group d by d.QuestionAnswers into g
                         select new QuestionSetViewModel
                         {
                             QuestionId = c.QuestionId,
                             QuestionName = c.QuestionName,
                             AllOptions = c.AllOptions,
                             AllAnswers = d.AllAnswers,
                             Explanation = f.Details
                         }).Distinct().ToList();
            }

            return await Task.Run(() => Json(aLst3, JsonRequestBehavior.AllowGet));
        }

        //Get questions for drag and drop - Practice
        public async Task<JsonResult> GetDragAndDropQuestions(int id)
        {
            List<QuestionSetViewModel> aLst = null;
            List<QuestionSetViewModel> aLst2 = null;
            List<QuestionSetViewModel> aLst3 = null;
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = db.Questions.Where(c => c.QuestionSetId == id && c.QuestionType == 2).GroupBy(q => new
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

                aLst2 = db.Answers.Where(c => c.QuestionSetId == id).GroupBy(q => new
                {
                    q.QuestionId,
                }, p => p.QuestionAnswers)
                       .Select(
                           grp => new QuestionSetViewModel
                           {
                               QuestionId = grp.Key.QuestionId,
                               AllAnswers = grp.ToList()
                           }
                       ).ToList();

                aLst3 = (from c in aLst
                         join d in aLst2 on c.QuestionId equals d.QuestionId
                         join f in db.Explanation on c.QuestionId equals f.QuestionId
                         where f.QuestionSetId == id
                         //group d by d.QuestionAnswers into g
                         select new QuestionSetViewModel
                         {
                             QuestionId = c.QuestionId,
                             QuestionName = c.QuestionName,
                             AllOptions = c.AllOptions,
                             AllAnswers = d.AllAnswers,
                             Explanation = f.Details
                         }).Distinct().ToList();

                //aLst = db.Questions.Where(c => c.QuestionSetId == id && c.QuestionType == 2).GroupBy(q => new
                //{
                //    q.QuestionId,
                //    q.QuestionName
                //}, p => p.Options)
                //        .Select(
                //            grp => new QuestionSetViewModel
                //            {
                //                QuestionId = grp.Key.QuestionId,
                //                QuestionName = grp.Key.QuestionName,
                //                AllOptions = grp.ToList()
                //            }
                //        ).ToList();
            }

            return await Task.Run(() => Json(aLst3, JsonRequestBehavior.AllowGet));
        }

        //Get questions for mock exam
        public async Task<JsonResult> GetMockExamQuestion(int id)
        {
            List<QuestionSetViewModel> aLst = null;
            List<QuestionSetViewModel> aLst2 = null;
            List<QuestionSetViewModel> aLst3 = null;

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = db.Questions.Where(c => c.QuestionSetId == id).GroupBy(q => new
                {
                    q.QuestionId,
                    q.QuestionName,
                    q.QuestionType
                }, p => p.Options)
                        .Select(
                            grp => new QuestionSetViewModel
                            {
                                QuestionId = grp.Key.QuestionId,
                                QuestionName = grp.Key.QuestionName,
                                QuestionType = grp.Key.QuestionType,
                                AllOptions = grp.ToList()
                            }
                        ).ToList();

                aLst2 = db.Answers.Where(c => c.QuestionSetId == id).GroupBy(q => new
                {
                    q.QuestionId,
                }, p => p.QuestionAnswers)
                       .Select(
                           grp => new QuestionSetViewModel
                           {
                               QuestionId = grp.Key.QuestionId,
                               AllAnswers = grp.ToList()
                           }
                       ).ToList();

                aLst3 = (from c in aLst
                         join d in aLst2 on c.QuestionId equals d.QuestionId
                         join f in db.Explanation on c.QuestionId equals f.QuestionId
                         where f.QuestionSetId == id
                         select new QuestionSetViewModel
                         {
                             QuestionId = c.QuestionId,
                             QuestionName = c.QuestionName,
                             AllOptions = c.AllOptions,
                             AllAnswers = d.AllAnswers,
                             Explanation = f.Details,
                             QuestionType = c.QuestionType
                         }).Distinct().ToList();
            }

            return await Task.Run(() => Json(aLst3, JsonRequestBehavior.AllowGet));
        }


        //[HttpPost]
        public async Task<JsonResult> AddAnswers(int quesSetId, int quesId, int quesType, string quesAnswers, int flag)
        {
            List<QuestionSetViewModel> aQuestionSetViewModelLst = null;

            QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();

            if (Session["myList"] == null)
            {
                aQuestionSetViewModelLst = new List<QuestionSetViewModel>();
            }
            else
            {
                aQuestionSetViewModelLst = (List<QuestionSetViewModel>)Session["myList"];
            }


            string userId = "";
            if (Session["userId"].ToString() != null || Session["userId"].ToString() != "")
            {
                userId = Session["userId"].ToString();
            }
            else
            {
                userId = "Anonymous";
            }

            int val = Convert.ToInt32(userId);

            //Check if already exists
            bool ifExist = aQuestionSetViewModelLst.Any(c => c.QuestionId == quesId && c.QuestionSetId == quesSetId && c.UserId == val);
            int index = aQuestionSetViewModelLst.FindIndex(item => item.QuestionId == quesId && item.QuestionSetId == quesSetId && item.UserId == val);

            if (ifExist == true)
            {
                aQuestionSetViewModel.QuestionSetId = quesSetId;
                aQuestionSetViewModel.QuestionId = quesId;
                aQuestionSetViewModel.QuestionAnswers = quesAnswers;
                aQuestionSetViewModelLst[index] = aQuestionSetViewModel;
                aQuestionSetViewModel.UserId = val;
                aQuestionSetViewModel.QuestionType = quesType;

                if (flag != 0)
                {
                    aQuestionSetViewModel.Flag = 1;
                }
            }
            else if (quesId > 0 && quesSetId > 0 && val > 0)
            {
                aQuestionSetViewModel.QuestionSetId = quesSetId;
                aQuestionSetViewModel.QuestionId = quesId;
                aQuestionSetViewModel.QuestionAnswers = quesAnswers;
                aQuestionSetViewModel.UserId = val;
                aQuestionSetViewModel.QuestionType = quesType;

                if (flag != 0)
                {
                    aQuestionSetViewModel.Flag = 1;
                }

                aQuestionSetViewModelLst.Add(aQuestionSetViewModel);
            }

            Session["myList"] = aQuestionSetViewModelLst;

            return await Task.Run(() => Json(aQuestionSetViewModelLst, JsonRequestBehavior.AllowGet));
        }

        public async Task<JsonResult> AddAnswersFreeTrial(int quesSetId, int quesId, int quesType, string quesAnswers, int flag)
        {
            List<QuestionSetViewModel> aQuestionSetViewModelLst = null;

            QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();

            if (Session["myList"] == null)
            {
                aQuestionSetViewModelLst = new List<QuestionSetViewModel>();
            }
            else
            {
                aQuestionSetViewModelLst = (List<QuestionSetViewModel>)Session["myList"];
            }

            int val = 420;

            //Check if already exists
            bool ifExist = aQuestionSetViewModelLst.Any(c => c.QuestionId == quesId && c.QuestionSetId == quesSetId && c.UserId == val);
            int index = aQuestionSetViewModelLst.FindIndex(item => item.QuestionId == quesId && item.QuestionSetId == quesSetId && item.UserId == val);

            if (ifExist == true)
            {
                aQuestionSetViewModel.QuestionSetId = quesSetId;
                aQuestionSetViewModel.QuestionId = quesId;
                aQuestionSetViewModel.QuestionAnswers = quesAnswers;
                aQuestionSetViewModelLst[index] = aQuestionSetViewModel;
                aQuestionSetViewModel.UserId = val;
                aQuestionSetViewModel.QuestionType = quesType;

                if (flag != 0)
                {
                    aQuestionSetViewModel.Flag = 1;
                }
            }
            else if (quesId > 0 && quesSetId > 0 && val > 0)
            {
                aQuestionSetViewModel.QuestionSetId = quesSetId;
                aQuestionSetViewModel.QuestionId = quesId;
                aQuestionSetViewModel.QuestionAnswers = quesAnswers;
                aQuestionSetViewModel.UserId = val;
                aQuestionSetViewModel.QuestionType = quesType;

                if (flag != 0)
                {
                    aQuestionSetViewModel.Flag = 1;
                }

                aQuestionSetViewModelLst.Add(aQuestionSetViewModel);
            }

            Session["myList"] = aQuestionSetViewModelLst;
            Session["userId"] = 420;

            return await Task.Run(() => Json(aQuestionSetViewModelLst, JsonRequestBehavior.AllowGet));
        }

        public ActionResult AttemptedExam(int id)
        {
            if (Session["userId"] != null)
            {
                TempData["quesSetId"] = id.ToString();
                Session.Remove("myList");

                TempData["bestOfThree"] = id.ToString();
                TempData.Remove("Percentage");
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAttemptedExamList(int userId, int quesType)
        {
            int type = 0;
            List<QuestionSetViewModel> aLstAttempted = null;

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                if (quesType == 1)
                {
                    type = 1;
                }
                else
                {
                    type = 2;
                }

                try
                {
                    aLstAttempted = (from c in db.Questions
                                     join f in db.QuestionSet on c.QuestionSetId equals f.QuestionSetId
                                     join d in db.AttemptedExam on c.QuestionSetId equals d.QuestionSetId
                                     into grp
                                     from d in grp.DefaultIfEmpty()
                                     group c by new { c.QuestionSetId, d.UserId, f.QuestionSetName, c.QuestionType, d.Status } into g
                                     where g.Key.UserId == userId && g.Key.QuestionType == type && g.Key.Status == 1
                                     select new QuestionSetViewModel
                                     {
                                         QuestionSetId = g.Key.QuestionSetId,
                                         QuestionSetName = g.Key.QuestionSetName,
                                         TotalQuestions = g.Where(c => c != null).Select(c => c.QuestionId).Distinct().Count(),
                                         TotalAttempted = db.AttemptedExam.Where(c => c.QuestionSetId == g.Key.QuestionSetId && c.UserId == userId && c.Status == 1).Distinct().Count()
                                     }).ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            return await Task.Run(() => Json(aLstAttempted, JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public async Task<JsonResult> GetAttemptedExam(int questionSetId, int userId)
        {
            List<QuestionSetViewModel> aLstQuestions = null;
            List<QuestionSetViewModel> aLstAttempted = null;
            List<QuestionSetViewModel> aLstResult = null;

            //QuestionSetViewModel aQuestionSetViewModel = new QuestionSetViewModel();

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLstQuestions = db.Questions.Where(c => c.QuestionSetId == questionSetId).GroupBy(q => new
                {
                    q.QuestionSetId,
                    q.QuestionId,
                    q.QuestionName,
                    q.QuestionType
                }, p => p.Options)
                       .Select(
                           grp => new QuestionSetViewModel
                           {
                               QuestionSetId = grp.Key.QuestionSetId,
                               QuestionId = grp.Key.QuestionId,
                               QuestionName = grp.Key.QuestionName,
                               QuestionType = grp.Key.QuestionType,
                               AllOptions = grp.ToList()
                           }
                       ).ToList();

                aLstAttempted = db.AttemptedExam.Where(c => c.QuestionSetId == questionSetId && c.Status == 1 && c.UserId == userId).GroupBy(q => new
                {
                    q.QuestionSetId,
                    q.QuestionId,
                    q.QuestionAnswers,
                    q.Status
                })
                       .Select(
                           grp => new QuestionSetViewModel
                           {
                               QuestionSetId = grp.Key.QuestionSetId,
                               QuestionId = grp.Key.QuestionId,
                               QuestionAnswers = grp.Key.QuestionAnswers,
                               Status = grp.Key.Status
                           }
                       ).ToList();

                try
                {
                    aLstResult = (from hg in aLstQuestions
                                  join cg in aLstAttempted on hg.QuestionId equals cg.QuestionId into cgj
                                  from cg in cgj.DefaultIfEmpty()
                                  where hg.QuestionSetId == questionSetId
                                  select new QuestionSetViewModel
                                  {
                                      QuestionSetId = hg.QuestionSetId,
                                      QuestionId = hg.QuestionId,
                                      QuestionName = hg.QuestionName,
                                      QuestionType = hg.QuestionType,
                                      AllOptions = hg.AllOptions,
                                      QuestionAnswers = cg == null || cg.QuestionAnswers == null ? "NULL" : cg.QuestionAnswers
                                  }).ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            return await Task.Run(() => Json(aLstResult, JsonRequestBehavior.AllowGet));
        }

        public async Task<JsonResult> GetBestOfThreeType()
        {
            List<QuestionSetViewModel> aLst = null;
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = (from c in db.QuestionSet
                        join d in db.Questions on c.QuestionSetId equals d.QuestionSetId
                        where d.QuestionType == 1 && !c.QuestionSetName.Contains("Mock")
                        select new QuestionSetViewModel
                        {
                            QuestionSetId = c.QuestionSetId,
                            QuestionSetName = c.QuestionSetName

                        }).Distinct().ToList();
            }

            return await Task.Run(() => Json(aLst, JsonRequestBehavior.AllowGet));
        }

        public async Task<JsonResult> GetRankingType()
        {
            List<QuestionSetViewModel> aLst = null;
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = (from c in db.QuestionSet
                        join d in db.Questions on c.QuestionSetId equals d.QuestionSetId
                        where d.QuestionType == 2 && !c.QuestionSetName.Contains("Mock")
                        select new QuestionSetViewModel
                        {
                            QuestionSetId = c.QuestionSetId,
                            QuestionSetName = c.QuestionSetName

                        }).Distinct().ToList();
            }

            return await Task.Run(() => Json(aLst, JsonRequestBehavior.AllowGet));
        }

        //Save exam details here if user cancels an exam
        public async Task<JsonResult> SaveExamDetails(int quesType, int setId)
        {
            List<QuestionSetViewModel> aLst = null;
            int userId = Convert.ToInt32(Session["userId"].ToString());

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                if (Session["myList"] != null)
                {
                    aLst = (List<QuestionSetViewModel>)Session["myList"];
                }

                try
                {
                    var result = db.AttemptedExam.Where(c => c.UserId == userId && c.QuestionSetId == setId);

                    if (result.Count() > 0)
                    {
                        foreach (var item in result)
                        {
                            item.Status = 0;
                        }

                        db.SaveChanges();
                    }

                    foreach (var record in aLst.Where(c => c.UserId == userId && c.QuestionSetId == setId))
                    {
                        AttemptedExam aAttemptedExam = new AttemptedExam();

                        aAttemptedExam.QuestionSetId = record.QuestionSetId;
                        aAttemptedExam.QuestionId = record.QuestionId;
                        aAttemptedExam.QuesType = quesType;
                        aAttemptedExam.QuestionAnswers = record.QuestionAnswers;
                        aAttemptedExam.AddedOn = DateTime.Now;
                        aAttemptedExam.Status = 1;
                        aAttemptedExam.UserId = record.UserId;

                        db.AttemptedExam.Add(aAttemptedExam);
                        db.SaveChanges();
                    }
                }

                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

            return await Task.Run(() => Json(aLst, JsonRequestBehavior.AllowGet));
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
    }
}