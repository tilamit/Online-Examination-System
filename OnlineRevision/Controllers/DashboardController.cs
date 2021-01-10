using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using OnlineRevision.DbContext;
using System.Data.Entity;
using System.Threading.Tasks;
using System.IO;
using System.Web.Helpers;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;
using OnlineRevision.Models;

namespace OnlineRevision.Controllers
{
    public class DashboardController : Controller
    {
        OnlineRevisionEntities db = new OnlineRevisionEntities();

        // GET: Dashboard
        public ActionResult Index()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                Session.Remove("examStatus");
            }

            //GetTotalUsers();
            return View();
        }

        public ActionResult LoadPartialView(string passVal)
        {
            Thread.Sleep(1000);

            switch (passVal)
            {
                case "dashboard":
                    return PartialView("frontPartialView");

                case "dashboardAdmin":
                    ViewBag.TotalStudents = GetTotalStudents().ToString();
                    ViewBag.TotalPaidStudents = GetTotalPaidStudents().ToString();
                    ViewBag.TotalAdmins = GetTotalAdmins().ToString();
                    return PartialView("adminPartialView");

                case "study":
                    List<StudySection> lstStudy = GetStudySection();
                    return PartialView("studyPartialView", lstStudy);

                case "studySection":
                    List<StudySection> lstStudySection = GetStudySection();
                    return PartialView("studyPartialView", lstStudySection);

                case "addContent":
                    return PartialView("addContentPartialView");

                case "updateContent":
                    List<StudySectionViewModel> lstEditStudySection = GetStudyDetails();
                    return PartialView("ContentsPartialView", lstEditStudySection);

                case "studentDetails":
                    return PartialView("studentPartialView");

                case "questionBank":
                    List<SampleTable> lstQuestionSetViewModel = GetQuestions();
                    return PartialView("questionBankPartialView");

                case "simulate":
                    return PartialView("simulatePartialView");

                case "ranking":
                    return PartialView("rankingPartialView");

                case "results":
                    return PartialView("resultPartialView");

                case "0":
                    return PartialView("frontPartialView");

                case "1":
                    ViewBag.TotalStudents = GetTotalStudents().ToString();
                    ViewBag.TotalPaidStudents = GetTotalPaidStudents().ToString();
                    ViewBag.TotalAdmins = GetTotalAdmins().ToString();
                    return PartialView("adminPartialView");

                default:
                    return PartialView("frontPartialView");
            }
        }

        public ActionResult LoadPartialViewAgain(string passVal)
        {
            Thread.Sleep(1000);

            switch (passVal)
            {
                case "study2":
                    List<StudySection> lstStudySection = GetStudySection();
                    return PartialView("studyPartialView", lstStudySection);

                case "bestOfThree":
                    return PartialView("simulatePartialView");

                case "ranking2":
                    return PartialView("rankingPartialView");

                default:
                    return PartialView("patientsPartialView");
            }
        }

        public ActionResult LoadStudentPartialView(string passVal, string id)
        {
            Thread.Sleep(1000);

            switch (passVal)
            {
                case "editDetails":
                    UserDetails aUser = GetUserDetails(id);
                    return PartialView("EditUserDetails", aUser);

                case "editContents":
                    int studyId = Convert.ToInt32(id);
                    StudySection aStudySection = GetStudySection(studyId);
                    return PartialView("EditContents", aStudySection);

                default:
                    return PartialView("");
            }
        }

        //Image upload
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadImg()
        {
            string _imgname = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);

                    _imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/img/") + _imgname + _ext;
                    _imgname = "" + _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    pic.SaveAs(path);

                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(_comPath);

                    if (img.Width > 200)
                        img.Resize(200, 200);
                    img.Save(_comPath);

                    UserDetails user = new UserDetails();

                    int userId = Convert.ToInt32(Session["userId"].ToString());
                    var result = db.UserDetails.Find(userId);

                    result.ImgPath = _imgname;

                    db.Entry(result).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUserImg()
        {
            string img = "";

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                int userId = Convert.ToInt32(Session["userId"].ToString());
                var result = db.UserDetails.Find(userId);

                img = result.ImgPath;
            }

            return Json(Convert.ToString(img), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            string status = "";

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                var result = (from c in db.UserDetails
                              where c.Email == email
                              select c).ToList();

                if (result.Count > 0)
                {
                    status = "Email exists!";
                }
                else
                {
                    status = "";
                }
            }

            return Json(Convert.ToString(status), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllTransactions(string id)
        {
            List<PaymentDetails> aLst = null;

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aLst = (from c in db.PaymentDetails
                        where c.email == id
                        select c).ToList();

            }

            return Json(aLst, JsonRequestBehavior.AllowGet);
        }

        public List<SampleTable> GetQuestions()
        {
            var result = (from c in db.SampleTable
                          select c).ToList();

            return result;
        }


        //Student registration
        [HttpPost]
        public JsonResult AddStudents(string name, string email, string pass, string universities)
        {
            UserDetails aUserDetails = new UserDetails();

            if (name == "" || pass == "" || email == "")
            {
                return Json(new { value = "<div style='color:red;'>Few fields are blank!</div>" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                aUserDetails.UserName = name;
                aUserDetails.Email = email;
                aUserDetails.Password = pass;
                aUserDetails.Sex = null;
                aUserDetails.DateOfBirth = null;
                aUserDetails.YearContinuing = null;
                aUserDetails.Institution = universities;
                aUserDetails.UserType = 2;
                aUserDetails.Status = 1;
                aUserDetails.CreatedOn = DateTime.Now;

                try
                {
                    db.UserDetails.Add(aUserDetails);
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    ex.ToString();
                }

                return Json(new { value = "<div style='color:green;'>Saved successfully!</div>" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult AddStudents(string name, string pass, string email, int gender, DateTime dateOfBirth, double yearContinuing)
        //{
        //    UserDetails aUserDetails = new UserDetails();

        //    if (name == "" || pass == "" || email == "")
        //    {
        //        return Json(new { value = "<div style='color:red;'>Few fields are blank!</div>" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        aUserDetails.UserName = name;
        //        aUserDetails.Password = pass;
        //        aUserDetails.Sex = gender;
        //        aUserDetails.DateOfBirth = dateOfBirth;
        //        aUserDetails.YearContinuing = yearContinuing;
        //        aUserDetails.Status = 1;
        //        aUserDetails.CreatedOn = DateTime.Now;

        //        try
        //        {
        //            db.UserDetails.Add(aUserDetails);
        //            db.SaveChanges();
        //        }

        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //        }

        //        return Json(new { value = "<div style='color:green;'>Saved successfully!</div>" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public UserDetails GetUserDetails(string id)
        {
            UserDetails user = new UserDetails();
            TempData["emailStudent"] = id;

            if (id != "" || id != null)
            {
                user = db.UserDetails.Where(c => c.Email == id).FirstOrDefault<UserDetails>();
            }

            return user;
        }

        public StudySection GetStudySection(int id)
        {
            StudySection study = new StudySection();
            TempData["emailStudent"] = id;

            if (id != 0)
            {
                study = db.StudySection.Where(c => c.Id == id).FirstOrDefault<StudySection>();
            }

            return study;
        }

        [HttpPost]
        public JsonResult UpdateUserDetails(int id, string name, string pass, string email, DateTime dateOfBirth)
        {
            UserDetails user = new UserDetails();

            var result = db.UserDetails.Find(id);

            result.UserName = name;
            result.Password = pass;
            result.Email = email;
            result.DateOfBirth = dateOfBirth;

            db.Entry(result).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { value = "<div style='color:green;'>Data updated successfully!</div>" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateContents(int id, string heading, string content)
        {
            StudySection studySection = new StudySection();

            var result = db.StudySection.Find(id);

            result.Heading = heading;
            result.StudyContent = content;

            db.Entry(result).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { value = "Content updated successfully!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateStudentStatus(int id)
        {
            UserDetails aUserDetails = new UserDetails();

            var result = db.UserDetails.Find(id);

            result.Status = 2;

            db.Entry(result).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { value = "Achived successfully!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddQuestions(string quesSetName, List<SampleTable> QuestionSetViewModel)
        {
            //Check for NULL.
            if (QuestionSetViewModel == null)
            {
                QuestionSetViewModel = new List<SampleTable>();
            }

            int id = 0;
            int insertedRecords = 0;
            int quesSetId = 0;

            //Create Question Set Here 
            QuestionSet aQuestionSet = new QuestionSet();
            aQuestionSet.QuestionSetName = quesSetName;
            aQuestionSet.CreatedOn = DateTime.Now;
            aQuestionSet.Status = 1;

            db.QuestionSet.Add(aQuestionSet);
            db.SaveChanges();

            quesSetId = aQuestionSet.QuestionSetId;

            //Insert Question With Options Here
            //Loop and insert data
            foreach (SampleTable question in QuestionSetViewModel)
            {
                var result = db.Questions.OrderByDescending(c => c.QuestionId).FirstOrDefault();

                if (result == null || result.QuestionId.ToString().Count() == 0)
                {
                    id = 1;
                }
                else
                {
                    id = Convert.ToInt32(result.QuestionId) + 1;
                }

                string options = question.Options;
                string[] splitOptions = options.Split('_');

                string quesAnswers = question.QuestionAnswers;
                string[] splitAnswers = quesAnswers.Split('_');

                foreach (string itemOptions in splitOptions)
                {
                    if (itemOptions != "")
                    {
                        Questions aQuestions = new Questions();
                        aQuestions.QuestionSetId = quesSetId;
                        aQuestions.QuestionId = id;
                        aQuestions.QuestionName = question.QuestionName;
                        aQuestions.Options = itemOptions.Trim();
                        aQuestions.Status = 1;
                        aQuestions.CreatedOn = DateTime.Now;

                        db.Questions.Add(aQuestions);
                        insertedRecords = db.SaveChanges();
                    }
                }

                foreach (string itemAnswers in splitAnswers)
                {
                    if (itemAnswers != "")
                    {
                        Answers aAnswers = new Answers();
                        aAnswers.QuestionId = id;
                        aAnswers.QuestionAnswers = itemAnswers.Trim();
                        aAnswers.Status = 1;
                        aAnswers.CreatedOn = DateTime.Now;

                        db.Answers.Add(aAnswers);
                        db.SaveChanges();
                    }
                }
            }

            return Json(new { value = "<div style='color:green;'>Question added successfully!</div>" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetQuestionDetails()
        {
            List<QuestionSetViewModel> aLst = null;
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                var result = (from d in db.Questions
                              join f in db.Answers on d.QuestionId equals f.QuestionId
                              select new QuestionSetViewModel
                              {
                                  QuestionSetId = d.QuestionSetId,
                                  QuestionId = d.QuestionSetId,
                                  QuestionName = d.QuestionName,
                                  QuestionAnswers = f.QuestionAnswers,
                                  Options = d.Options
                              }).ToList();

                aLst = result.Where(c => c.QuestionSetId == 1).GroupBy(q => new
                {
                    q.QuestionId,
                    q.QuestionName,
                    q.QuestionSetId
                }, p => p.Options)
                        .Select(
                            grp => new QuestionSetViewModel
                            {
                                QuestionSetId = grp.Key.QuestionSetId,
                                QuestionId = grp.Key.QuestionId,
                                QuestionName = grp.Key.QuestionName,
                                AllOptions = grp.Distinct().ToList()
                            }
                        ).ToList();
            }

            return await Task.Run(() => Json(aLst, JsonRequestBehavior.AllowGet));
        }

        public int GetTotalStudents()
        {
            var resultStudents = db.UserDetails.Where(c => c.UserType == 2).ToList();
            return resultStudents.Count();
        }

        public int GetTotalAdmins()
        {
            var resultAdmins = db.UserDetails.Where(c => c.UserType == 1).ToList();
            return resultAdmins.Count();
        }

        public int GetTotalPaidStudents()
        {
            var resultTotalStudents = db.PaymentDetails.ToList();
            return resultTotalStudents.Count();
        }

        public ActionResult PieChart()
        {
            var result = (from c in db.UserDetails
                          join d in db.UserType on c.UserType equals d.TypeId
                          select new Users
                          {
                              UserId = c.UserId,
                              UserName = c.UserName,
                              Email = c.Email,
                              type = d.TypeName,
                              DateOfBirth = c.DateOfBirth
                          }).ToList();

            var groups = result.GroupBy(c => c.type).Select(c => new
            {
                UserType = c.Key,
                UserCount = c.Count()
            }).OrderBy(n => n.UserType);

            return Json(groups.ToList(), JsonRequestBehavior.AllowGet);
        }

        //User Details
        public ActionResult GetAllDetails(int id)
        {
            UserDetails aUserDetails = new UserDetails();
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                aUserDetails = db.UserDetails.Find(id);

                ViewBag.Id = aUserDetails.UserId;
                ViewBag.UserName = aUserDetails.UserName;
                ViewBag.Email = aUserDetails.Email;
                ViewBag.University = aUserDetails.Institution;
                ViewBag.UserPass = aUserDetails.Password;
                if (aUserDetails.DateOfBirth.HasValue)
                {
                    ViewBag.DateOfBirth = Convert.ToDateTime(aUserDetails.DateOfBirth).ToString("MM/dd/yyyy");
                }
                else
                {
                    ViewBag.DateOfBirth = Convert.ToDateTime("01/01/1900").ToString("MM/dd/yyyy");
                }
            }

            return PartialView("detailsUserPartialView", aUserDetails);
        }

        //Upate Users
        [HttpPost]
        public JsonResult UpdateUsers(UserDetails aUserDetails)
        {
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                if (aUserDetails.NewPass == 0)
                {
                    try
                    {
                        var result = db.UserDetails.Find(aUserDetails.UserId);

                        if (result != null)
                        {
                            result.UserName = aUserDetails.UserName;
                            result.Email = aUserDetails.Email;
                            result.DateOfBirth = aUserDetails.DateOfBirth;
                            result.Password = aUserDetails.Password;

                            db.Entry(result).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }

                    return Json(true);
                }
                else
                {
                    var query = (from c in db.UserDetails
                                 where c.UserId == aUserDetails.UserId && c.Password == aUserDetails.UserPassPre
                                 select c).ToList();

                    if (query.Count == 0)
                    {
                        return Json(new { value = "<div style='color:red;'>Incorrect password!</div>" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var result = db.UserDetails.Find(aUserDetails.UserId);

                        if (result != null)
                        {
                            result.UserName = aUserDetails.UserName;
                            result.Email = aUserDetails.Email;
                            result.DateOfBirth = aUserDetails.DateOfBirth;
                            result.Password = aUserDetails.Password;

                            db.Entry(result).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        return Json(new { value = "<div style='color:green;'>Updated!</div>" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        //Get study section details here
        public List<StudySection> GetStudySection()
        {
            List<StudySection> result = (from c in db.StudySection
                                         where c.Status == 1
                                         select c).ToList();

            return result;
        }

        public List<StudySectionViewModel> GetStudyDetails()
        {
            List<StudySectionViewModel> result = (from c in db.StudySection
                                                  where c.Status == 1
                                                  select new StudySectionViewModel
                                                  {
                                                      Id = c.Id,
                                                      Heading = c.Heading,
                                                      StudyContent = c.StudyContent.Substring(0, 100),
                                                      AddedBy = c.AddedBy,
                                                      sectionId = c.sectionId,
                                                      CreatedOn = c.CreatedOn,
                                                      Status = c.Status,
                                                      Preference = c.Preference
                                                  }).ToList();

            return result;
        }

        //Get study section details here
        public int GetStudyId()
        {
            int result = db.StudySection.Max(c => c.Id);

            return result;
        }

        //Get study section preference here
        public int? GetStudyPreference()
        {
            int? result = db.StudySection.Max(c => c.Preference);

            return result;
        }

        //Get last tab id
        public int GetTabId()
        {
            int id = db.Folders.Max(c => c.Id);

            return id;
        }

        //Get only numbers
        public static string GetDigitsOnly(string strRawData)
        {
            return Regex.Replace(strRawData, "[^0-9]", "");
        }

        [HttpPost]
        public JsonResult AddContent(string heading, string content)
        {
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                try
                {
                    StudySection aStudySection = new StudySection();

                    aStudySection.sectionId = "content-" + (GetStudyId() + 1);
                    aStudySection.Preference = GetStudyPreference() + 1;
                    aStudySection.Heading = heading;
                    aStudySection.StudyContent = content;
                    aStudySection.AddedBy = Convert.ToInt32(Session["userId"]);
                    aStudySection.Status = 1;
                    aStudySection.CreatedOn = DateTime.Now;

                    db.StudySection.Add(aStudySection);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return Json(new { value = "Content added successfully!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdatePreference(int[] contentId)
        {
            OnlineRevisionEntities entities = new OnlineRevisionEntities();
            int preference = 1;

            foreach (int id in contentId)
            {
                var obj = entities.StudySection.Find(id);
                obj.Preference = preference;

                entities.SaveChanges();
                preference += 1;
            }

            return Json(new { value = "Updated!" }, JsonRequestBehavior.AllowGet);
        }

        //Add Folders
        public ActionResult AddFolders()
        {
            return PartialView("addFoldersPartialView");
        }

        [HttpPost]
        public async Task<JsonResult> AddFolders(string folderName, string details)
        {
            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                try
                {
                    Folders aFolders = new Folders();

                    aFolders.FolderName = folderName;
                    int tabId = GetTabId() + 1;
                    aFolders.TabName = "tabs-" + tabId.ToString();
                    //aFolders.Details = details;
                    aFolders.Status = 1;
                    aFolders.CreatedOn = DateTime.Now;
                    aFolders.CreatedBy = Convert.ToInt32(Session["userId"]);

                    db.Folders.Add(aFolders);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return await Task.Run(() => Json(new { value = "Folders added successfully!" }, JsonRequestBehavior.AllowGet));
            }
        }

        [HttpPost]
        public JsonResult GetFoldersWithStudent()
        {
            List<FoldersViewModel> aLst = null;
            List <Folders> aFolderLst = GetFolders();
            List<Users> aUsersLst = GetStudentDetails();

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                try
                {
                    aLst = (from c in aFolderLst
                            where c.Status == 1
                            select new FoldersViewModel
                            {
                                FolderName = c.FolderName,
                                TabName = c.TabName,
                                TotalUsers = aUsersLst.Where(d => d.TabType == c.TabName && d.UserType == 2 && d.Status == 1).Count(),
                                UsersList = aUsersLst.Where(d => d.TabType == c.TabName && d.UserType == 2 && d.Status == 1).OrderByDescending(d => d.UserId).ToList()
                            }).ToList();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return Json(aLst, JsonRequestBehavior.AllowGet);
            }
        }

        //Transfer students to different folders
        [HttpPost]
        public JsonResult UpdateToFolder(string userId, string tabName)
        {
            UserDetails aUserDetails = new UserDetails();
            string[] id = userId.Split(',');

            foreach (var item in id)
            {
                int user = Convert.ToInt32(item);
                var result = db.UserDetails.Find(user);

                result.TabType = tabName;

                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { value = "Transferred successfully!" }, JsonRequestBehavior.AllowGet);
        }

        //Transfer students to different folders
        [HttpPost]
        public JsonResult DeleteFolders(string tabName)
        {
            var resultUsers = (from c in db.UserDetails
                               where c.TabType == tabName
                               select c).ToList();

            foreach (var item in resultUsers)
            {
                item.TabType = "";

                db.SaveChanges();
            }

            var resultFolders = (from c in db.Folders
                                 where c.TabName == tabName
                                 select c).ToList();

            foreach (var item in resultFolders)
            {
                item.Status = 2;

                db.SaveChanges();
            }

            return Json(new { value = "Transferred successfully!" }, JsonRequestBehavior.AllowGet);
        }

        //Delete students
        [HttpPost]
        public JsonResult DeleteStudents(string userId)
        {
            UserDetails aUserDetails = new UserDetails();
            string[] id = userId.Split(',');

            foreach (var item in id)
            {
                int user = Convert.ToInt32(item);
                var result = db.UserDetails.Find(user);

                result.Status = 2;

                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { value = "User deleted successfully!" }, JsonRequestBehavior.AllowGet);
        }

        public List<Folders> GetFolders()
        {
            var result = (from c in db.Folders
                          where c.Status == 1
                          select c).ToList();

            return result;
        }

        public List<Users> GetStudentDetails()
        {
            var result = (from c in db.UserDetails
                          join d in db.PaymentDetails on c.Email equals d.email into ps
                          from d in ps.DefaultIfEmpty()
                          select new Users
                          {
                              TabType = c.TabType,
                              CreatedOn = c.CreatedOn,
                              UserId = c.UserId,
                              UserName = c.UserName,
                              Email = c.Email,
                              DateOfBirth = c.DateOfBirth,
                              University = c.Institution,
                              Status = c.Status,
                              UserType = c.UserType,
                              TotalDays = DbFunctions.DiffDays(d.AddedOn, d.ValidTill)
                              //TotalSubscription = db.PaymentDetails.Where(f => f.email == c.Email),
                              //TotalArchived = db.UserDetails.Where(f => f.Status == 2).Count()
                          }).ToList();

            return result;
        }
    }
}