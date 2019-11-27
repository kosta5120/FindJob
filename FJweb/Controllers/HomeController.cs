using FJweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Mail;
using System.Net;
using PagedList;
using PagedList.Mvc;
using System.Web.Optimization;

namespace FJweb.Controllers
{

    public class HomeController : Controller
    {
        FindJobEntities1 db = new FindJobEntities1();
        public static List<FJ_SubCategory_Work> subcatList = new List<FJ_SubCategory_Work>();
        public static List<FJ_JobType> allTypJob = new List<FJ_JobType>();
        public static List<FJ_Categorys_Of_Works> allCategory = new List<FJ_Categorys_Of_Works>();

        public static int[] Subcat = new int[subcatList.Count];
        public static int[] typJob = new int[allTypJob.Count];
        public static int[] cate = new int[allCategory.Count];
        public static string num = string.Empty;


        #region HomePage
        public ActionResult HomePage()//להציג את כל הנתונים מבסיס נתונים בדף הבית  
        {
            List<FJ_Categorys_Of_Works> CategoryList = db.FJ_Categorys_Of_Works.ToList();
            ViewBag.CL = CategoryList;

            List<FJ_JobType> JobTypeList = db.FJ_JobType.ToList();
            ViewBag.JTL = JobTypeList;

            List<FJ_JobsPosts> jobsPosts = db.FJ_JobsPosts.ToList();

            var category = new int[CategoryList.Count];
            Array.Clear(category, 0, category.Length);

            foreach (var i in CategoryList)
            {
                foreach (var y in jobsPosts)
                {
                    DateTime dataTimeNow = DateTime.Now;

                    if ((int)Math.Round((dataTimeNow - Convert.ToDateTime(y.dateTime)).TotalDays) > 14)
                    {
                        var id = y.ID;
                        var deletePostById = db.FJ_JobsPosts.Where(x => x.ID == id).First();
                        db.FJ_JobsPosts.Remove(deletePostById);

                        var idBadPost = db.FJ_BadReports.Where(x => x.IDPost == id).FirstOrDefault();
                        if (idBadPost != null)
                            db.FJ_BadReports.Remove(idBadPost);

                        db.SaveChanges();
                        jobsPosts = db.FJ_JobsPosts.ToList();
                    }
                    if (i.CategoryWork == y.CategoryPosition)
                    {
                        category[CategoryList.IndexOf(i)] += 1;
                    }

                }
            }
            ViewBag.countC = category;
            ModelState.Clear();


            return View();
        }
        #endregion

        #region AdvertisingPage
        public ActionResult AdvertisingPage()//דף פירסום מודעות ששןמר את הנתונים בבסיס הנתונים 
        {
            var CategoryList = db.FJ_Categorys_Of_Works.OrderBy(x=>x.CategoryWork).ToList();
            ViewBag.CategoryList = new SelectList(CategoryList, "ID", "CategoryWork");

            var region = db.FJ_Region.ToList();
            ViewBag.regionList = new SelectList(region, "idRegion", "Region");

            var TypePositionList = db.FJ_JobType.OrderBy(x=>x.jobType).ToList();
            ViewBag.positionType = new SelectList(TypePositionList, "IDTypePosition", "jobType");

            return View();
        }
        #endregion

        #region Json-GetSubcategory

        public JsonResult GetSubcategory(int ID)//הצגת תת קטגוריה לאחר בחירה קטגוריה בדף פירסום מודעה
        {
            db.Configuration.ProxyCreationEnabled = false;
            var SubcategoryList = db.FJ_SubCategory_Work.Where(x => x.IDCategoryWork == ID).OrderBy(x=>x.Subcategory).ToList();
            return Json(SubcategoryList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Json-GetCitysByRegion
        public JsonResult GetCitysByRegion(int idRegion)//הצגת ערים לפי איזור
        {
            db.Configuration.ProxyCreationEnabled = false;
            var CitysList = db.FJ_Citys.Where(x => x.idRegion == idRegion).OrderBy(x=>x.City).ToList();
            return Json(CitysList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region SubmitPost
        public ActionResult SubmitPost(PostDetails pd)//שמירה של הנתונים שהוכנסו בדף פירסום מודעה
        {

            var jp = new FJ_JobsPosts();

            jp.Post_Title = pd.Post_Title;
            jp.Name_of_Publisher = pd.Name_of_Publisher;
            jp.Address = pd.Address;
            jp.Region = db.FJ_Region.Where(x => x.idRegion.ToString() == pd.Region).SingleOrDefault().Region;
            jp.Salary = pd.Salary;
            jp.Typ_of_job = db.FJ_JobType.Where(x => x.IDTypePosition.ToString() == pd.Typ_of_job).SingleOrDefault().jobType;
            jp.Email = pd.Email;
            jp.Description = pd.Description;
            jp.Experience = pd.Experience;
            jp.CategoryPosition = db.FJ_Categorys_Of_Works.Where(x => x.ID.ToString() == pd.CategoryPosition).SingleOrDefault().CategoryWork;
            jp.Subcategory = pd.Subcategory;
            jp.NumberOfPosition = pd.NumberOfPosition;
            jp.dateTime = pd.dateTime;


            db.FJ_JobsPosts.Add(jp);
            db.SaveChanges();

            return RedirectToAction("HomePage");//ניווט בחזרה לדף הבית

        }
        #endregion

        #region AllJobs
        public ActionResult AllJobs(string searchFromTextBoxWhat, string searchFromTexBoxWhere, string searchForPage, string searchByTypeFromHomePage, string searchhByRegionFromHomePage, string Filter, int page = 1, int pageSize = 2)//הצגת כל המשרות לפי החיפוש מהעמוד הבית והצגת נתונים בשביל חיפוש חוזר מדף מודעות
        {

            string searchByCategory = null;
            string searchBySubCate = null;
            string searchByType = null;
            string searchByRegion = null;
            string searchByCity = null;

            var multiSearech = new string[7];//multiSearch [searchFromTextBoxWhat, searchFromTexBoxWhere, searchByCategory, searchByType, searchBySubCate, searchByRegion, searchByCity]
            var count = 0;
            
            var postDetails = new List<FJ_JobsPosts>();
            PagedList<FJ_JobsPosts> model = new PagedList<FJ_JobsPosts>(postDetails, page, pageSize);

            if (Filter != null)
            {
                if (!searchForPage.Contains(','))
                    return RedirectToAction("HomePage");
                else
                    searchForPage = searchForPage.Replace("," + Filter, string.Empty);
            }

            if (searchForPage != null)
            {

                ViewBag.Search = searchForPage;
                var str = searchForPage.Split(',');


                var searchByCategory1 = new List<FJ_Categorys_Of_Works>();
                var searchBySubCate1 = new List<FJ_SubCategory_Work>();
                var searchByType1 = new List<FJ_JobType>();
                var searchByRegion1 = new List<FJ_Region>();
                var searchByCity1 = new List<FJ_Citys>();
                var searchFromTextBoxWhat1 = new List<FJ_JobsPosts>();
                var searchFromTexBoxWhere1 = new List<FJ_JobsPosts>();

                for (var i = 0; i < str.Length; i++)
                {
                    var word = str[i];
                    word = word.Trim();
                    var flag = false;


                    searchByCategory1 = db.FJ_Categorys_Of_Works.Where(x => x.CategoryWork == word).ToList();
                    searchBySubCate1 = db.FJ_SubCategory_Work.Where(x => x.Subcategory == word).ToList();
                    searchByType1 = db.FJ_JobType.Where(x => x.jobType == word).ToList();
                    searchByRegion1 = db.FJ_Region.Where(x => x.Region == word).ToList();
                    searchByCity1 = db.FJ_Citys.Where(x => x.City == word).ToList();

                    if (searchByCategory1.Count != 0)
                    {
                        searchByCategory = searchByCategory1[0].CategoryWork.ToString();
                        multiSearech[2] = searchByCategory;
                        flag = true;
                    }

                    if (searchBySubCate1.Count != 0 && str.Length > 1)
                    {
                        searchBySubCate = searchBySubCate1[0].Subcategory.ToString();
                        multiSearech[4] = searchBySubCate;
                        flag = true;
                    }

                    if (searchByType1.Count != 0 /*&& str.Length > 1 || searchByTypeFromHomePage != null*/ )
                    {
                        searchByType = searchByType1[0].jobType.ToString();
                        multiSearech[3] = searchByType;
                        flag = true;
                    }

                    if (searchByRegion1.Count != 0 /*&& str.Length > 1 || searchhByRegionFromHomePage != null*/)
                    {
                        searchByRegion = searchByRegion1[0].Region.ToString();
                        multiSearech[5] = searchByRegion;
                        flag = true;
                    }

                    if (searchByCity1.Count != 0 && str.Length > 2)
                    {
                        searchByCity = searchByCity1[0].City.ToString();
                        multiSearech[6] = searchByCity;
                        flag = true;

                    }

                    if (i == 0 && flag == false || i == 1 && flag == false)
                    {


                        searchFromTextBoxWhat1 = db.FJ_JobsPosts
                                     .Where(x => x.Post_Title.Contains(word) ||
                                             x.CategoryPosition.Contains(word) ||
                                             x.Typ_of_job.Contains(word) ||
                                             x.Experience.Contains(word) ||
                                             x.Description.Contains(word) ||
                                             x.Subcategory.Contains(word)
                                            ).ToList();

                        searchFromTexBoxWhere1 = db.FJ_JobsPosts
                                    .Where(x =>
                                           x.Region.Contains(word) ||
                                           x.Address.Contains(word)
                                           ).ToList();

                        if (searchFromTextBoxWhat1.Count != 0)
                        {
                            searchFromTextBoxWhat = word;
                        }
                        if (searchFromTexBoxWhere1.Count != 0)
                        {
                            searchFromTexBoxWhere = word;
                            if (str.Length == 1)
                                searchFromTextBoxWhat = string.Empty;
                        }

                    }

                }

                if (searchByRegion != null && str.Length == 1 || searchByType != null && str.Length == 1 || searchByCategory != null && str.Length == 1)
                {
                    postDetails = db.FJ_JobsPosts
                                            .Where(x => x.Region.Contains(searchByRegion) ||
                                                   x.Typ_of_job.Contains(searchByType) ||
                                                   x.CategoryPosition.Contains(searchByCategory)
                                                   ).OrderByDescending(x => x.dateTime).ToList();

                }
                if (searchByCategory != null && searchByType != null && str.Length == 2 || searchByCategory != null && searchByRegion != null && str.Length == 2 || searchByCategory != null && searchBySubCate != null && str.Length == 2)
                {
                    postDetails = db.FJ_JobsPosts
                                            .Where(x =>
                                                   x.CategoryPosition.Contains(searchByCategory) && x.Subcategory.Contains(searchBySubCate) ||
                                                   x.CategoryPosition.Contains(searchByCategory) && x.Typ_of_job.Contains(searchByType) ||
                                                   x.CategoryPosition.Contains(searchByCategory) && x.Region.Contains(searchByRegion)
                                                   ).OrderByDescending(x => x.dateTime).ToList();
                }

                if (searchByType != null && searchByCategory != null && str.Length == 2 || searchByType != null && searchByRegion != null && str.Length == 2 || searchByType != null && searchBySubCate != null && str.Length == 2)
                {
                    postDetails = db.FJ_JobsPosts
                                            .Where(x =>
                                                   x.Typ_of_job.Contains(searchByType) && x.CategoryPosition.Contains(searchByCategory) ||
                                                   x.Typ_of_job.Contains(searchByType) && x.Region.Contains(searchByRegion)
                                                   ).OrderByDescending(x => x.dateTime).ToList();
                }

                if (searchByCategory != null && searchBySubCate != null && searchByType != null && str.Length == 3 || searchByCategory != null && searchBySubCate != null && searchByRegion != null && str.Length == 3 || searchByCategory != null && searchBySubCate != null && searchByCity != null && str.Length == 3 || searchByCategory != null && searchByType != null && searchByRegion != null && str.Length == 3 || searchByCategory != null && searchByType != null && searchByCity != null && str.Length == 3 || searchByCategory != null && searchByRegion != null && searchByCity != null && str.Length == 3)
                {
                    postDetails = db.FJ_JobsPosts
                                        .Where(x => x.CategoryPosition.Contains(searchByCategory) && x.Subcategory.Contains(searchBySubCate) && x.Typ_of_job.Contains(searchByType) ||
                                                x.CategoryPosition.Contains(searchByCategory) && x.Subcategory.Contains(searchBySubCate) && x.Region.Contains(searchByRegion) ||
                                                x.CategoryPosition.Contains(searchByCategory) && x.Subcategory.Contains(searchBySubCate) && x.Address.Contains(searchByCity) ||
                                                x.CategoryPosition.Contains(searchByCategory) && x.Typ_of_job.Contains(searchByType) && x.Region.Contains(searchByRegion) ||
                                                x.CategoryPosition.Contains(searchByCategory) && x.Typ_of_job.Contains(searchByType) && x.Address.Contains(searchByCity) ||
                                                x.CategoryPosition.Contains(searchByCategory) && x.Region.Contains(searchByRegion) && x.Address.Contains(searchByCity)
                                                ).OrderByDescending(x => x.dateTime).ToList();
                }

                if (searchByCategory != null && searchBySubCate != null && searchByType != null && searchByRegion != null && str.Length == 4 || searchByCategory != null && searchByType != null && searchByRegion != null && searchByCity != null && str.Length == 4 || searchByCategory != null && searchBySubCate != null && searchByRegion != null && searchByCity != null && str.Length == 4 || searchByCategory != null && searchBySubCate != null && searchByType != null && searchByCity != null && str.Length == 4)
                {
                    postDetails = db.FJ_JobsPosts.Where(x => x.CategoryPosition.Contains(searchByCategory) &&
                                                            x.Subcategory.Contains(searchBySubCate) &&
                                                            x.Typ_of_job.Contains(searchByType) &&
                                                            x.Region.Contains(searchByRegion) ||

                                                            x.CategoryPosition.Contains(searchByCategory) &&
                                                            x.Typ_of_job.Contains(searchByType) &&
                                                            x.Region.Contains(searchByRegion) &&
                                                            x.Address.Contains(searchByCity) ||

                                                            x.CategoryPosition.Contains(searchByCategory) &&
                                                            x.Subcategory.Contains(searchBySubCate) &&
                                                            x.Region.Contains(searchByRegion) &&
                                                            x.Address.Contains(searchByCity) ||

                                                            x.CategoryPosition.Contains(searchByCategory) &&
                                                            x.Subcategory.Contains(searchBySubCate) &&
                                                            x.Typ_of_job.Contains(searchByType) &&
                                                            x.Address.Contains(searchByCity)

                                                            ).OrderByDescending(x => x.dateTime).ToList();
                }

               

                if (searchByCategory != null && searchBySubCate != null && searchByType != null && searchByRegion != null && searchByCity != null && str.Length == 5)
                {
                    postDetails = db.FJ_JobsPosts.Where(x => x.CategoryPosition.Contains(searchByCategory) &&
                                                            x.Subcategory.Contains(searchBySubCate) &&
                                                            x.Typ_of_job.Contains(searchByType) &&
                                                            x.Region.Contains(searchByRegion) &&
                                                            x.Address.Contains(searchByCity)
                                                            ).OrderByDescending(x => x.dateTime).ToList();
                }

                model = new PagedList<FJ_JobsPosts>(postDetails, page, pageSize);

            }


            if (searchFromTextBoxWhat != null && searchFromTexBoxWhere != null && searchFromTextBoxWhat != "" && searchFromTexBoxWhere != "")
            {
                postDetails = db.FJ_JobsPosts
                                   .Where(x => x.CategoryPosition.Contains(searchFromTextBoxWhat) && x.Region.Contains(searchFromTexBoxWhere) ||
                                          x.CategoryPosition.Contains(searchFromTextBoxWhat) && x.Address.Contains(searchFromTexBoxWhere) ||
                                          x.Subcategory.Contains(searchFromTextBoxWhat) && x.Address.Contains(searchFromTexBoxWhere) ||
                                          x.Subcategory.Contains(searchFromTextBoxWhat) && x.Region.Contains(searchFromTexBoxWhere) ||
                                          x.Typ_of_job.Contains(searchFromTextBoxWhat) && x.Address.Contains(searchFromTexBoxWhere) ||
                                          x.Typ_of_job.Contains(searchFromTextBoxWhat) && x.Region.Contains(searchFromTexBoxWhere) ||
                                          x.Experience.Contains(searchFromTextBoxWhat) && x.Address.Contains(searchFromTexBoxWhere) ||
                                          x.Experience.Contains(searchFromTextBoxWhat) && x.Region.Contains(searchFromTexBoxWhere) ||
                                          x.Description.Contains(searchFromTextBoxWhat) && x.Address.Contains(searchFromTexBoxWhere) ||
                                          x.Description.Contains(searchFromTextBoxWhat) && x.Region.Contains(searchFromTexBoxWhere)
                                          )
                                   .OrderByDescending(x => x.dateTime).ToList();

                multiSearech[0] = searchFromTextBoxWhat;
                multiSearech[1] = searchFromTexBoxWhere;

                model = new PagedList<FJ_JobsPosts>(postDetails, page, pageSize);
            }

            else if (searchFromTexBoxWhere != null && searchFromTexBoxWhere != "")
            {
                postDetails = db.FJ_JobsPosts
                                    .Where(x =>
                                           x.Region.Contains(searchFromTexBoxWhere) ||
                                           x.Address.Contains(searchFromTexBoxWhere)
                                           )
                                    .OrderByDescending(x => x.dateTime).ToList();

                multiSearech[1] = searchFromTexBoxWhere;

                model = new PagedList<FJ_JobsPosts>(postDetails, page, pageSize);

            }

            else if (searchFromTextBoxWhat != null && searchFromTextBoxWhat != "")
            {
                postDetails = db.FJ_JobsPosts.Where(x => x.CategoryPosition == searchFromTextBoxWhat).ToList();

                if (postDetails.Count == 0)
                {
                    postDetails = db.FJ_JobsPosts
                                     .Where(x => x.Post_Title.Contains(searchFromTextBoxWhat) ||
                                             x.CategoryPosition.Contains(searchFromTextBoxWhat) ||
                                             x.Typ_of_job.Contains(searchFromTextBoxWhat) ||
                                             x.Experience.Contains(searchFromTextBoxWhat) ||
                                             x.Description.Contains(searchFromTextBoxWhat) ||
                                             x.Subcategory.Contains(searchFromTextBoxWhat)
                                            )
                                     .OrderByDescending(x => x.dateTime).ToList();
                    multiSearech[0] = searchFromTextBoxWhat;
                }
                else
                {
                    multiSearech[2] = searchFromTextBoxWhat;
                    searchFromTextBoxWhat = null;

                }


                model = new PagedList<FJ_JobsPosts>(postDetails, page, pageSize);

            }



            if (postDetails.Count != 0)
            {
                ViewBag.AJL = postDetails.ToPagedList(page, pageSize);

                foreach (var i in postDetails)
                {
                    count += 1;
                }

                ViewBag.numberOfJobs = count;

                if (searchFromTextBoxWhat == null && searchFromTexBoxWhere == null)
                {
                    if (searchByCategory == null /*&& searchByTypeFromHomePage != null || searchByCategory == null && searchByRegion != null*/)
                    {
                        allCategory = db.FJ_Categorys_Of_Works.ToList();
                        cate = new int[allCategory.Count];
                        Array.Clear(cate, 0, cate.Length);

                        foreach (var i in postDetails)
                        {
                            foreach (var y in allCategory)
                                if (i.CategoryPosition == y.CategoryWork)
                                {
                                    cate[allCategory.IndexOf(y)] += 1;
                                }
                        }

                        ViewBag.category = allCategory.ToList();
                        ViewBag.countSC = cate;
                    }
                    else if (searchBySubCate == null)
                    {
                        var category = postDetails[0].CategoryPosition;
                        var IDCategory = db.FJ_Categorys_Of_Works.Where(x => x.CategoryWork.Contains(category)).SingleOrDefault();
                        subcatList = db.FJ_SubCategory_Work.Where(x => x.IDCategoryWork == IDCategory.ID).ToList();

                        Subcat = new int[subcatList.Count];
                        Array.Clear(Subcat, 0, Subcat.Length);

                        foreach (var i in subcatList)
                        {
                            foreach (var y in postDetails)
                                if (i.Subcategory == y.Subcategory)
                                {
                                    Subcat[subcatList.IndexOf(i)] += 1;
                                }

                        }
                        ViewBag.subcategory = subcatList;
                        ViewBag.countSC = Subcat;
                    }


                    if (searchByTypeFromHomePage == null && searchByType == null)
                    {
                        allTypJob = db.FJ_JobType.ToList();
                        typJob = new int[allTypJob.Count];
                        Array.Clear(typJob, 0, typJob.Length);
                        foreach (var i in postDetails)
                        {
                            foreach (var y in allTypJob)
                                if (i.Typ_of_job == y.jobType)
                                {
                                    typJob[allTypJob.IndexOf(y)] += 1;
                                }
                        }
                        ViewBag.typJob = db.FJ_JobType.ToList();
                        ViewBag.countOfTypJobs = typJob;
                    }
               

                    if (searchByRegion == null)
                    {
                        var CitysList = db.FJ_Citys.ToList();
                        var RigionList = db.FJ_Region.ToList();
                        var region = new int[RigionList.Count];
                        Array.Clear(region, 0, region.Length);

                        foreach (var i in postDetails)
                        {
                            foreach (var y in CitysList)
                                if (i.Address == y.City)
                                {
                                    var id = y.idRegion;
                                    foreach (var x in RigionList)
                                        if (id == x.idRegion)
                                            region[RigionList.IndexOf(x)] += 1;
                                }
                        }
                        ViewBag.region = RigionList;
                        ViewBag.countRegion = region;
                    }

                    if (searchByRegion != null)
                    {

                        var region = searchByRegion;
                        var IDRegion = db.FJ_Region.Where(r => r.Region.Contains(region)).SingleOrDefault();
                        var cityList = db.FJ_Citys.Where(c => c.idRegion == IDRegion.idRegion).ToList();

                        if (searchByCity == null)
                        {
                            var city = new int[cityList.Count];
                            Array.Clear(city, 0, city.Length);
                            foreach (var i in postDetails)
                            {
                                foreach (var y in cityList)
                                    if (i.Address == y.City)
                                    {
                                        city[cityList.IndexOf(y)] += 1;
                                    }
                            }
                            ViewBag.cityList = cityList;
                            ViewBag.countCitys = city;
                        }

                    }


                }

                ModelState.Clear();
            }
            var cou = 0;


            for (var i = 0; i < multiSearech.Length; i++)
            {
                if (multiSearech[i] != null)
                {
                    cou++;

                    if (cou == 1)
                        ViewBag.TitleSearch = multiSearech[i];

                    else
                        ViewBag.TitleSearch += "," + multiSearech[i];
                        
                    
                        
                }
            }
            ViewBag.popupTitle = ViewBag.TitleSearch;
            var filters = new string[cou];

            for (var i = 0; i < cou; i++)
            {
                for (var y = 0; y < multiSearech.Length; y++)

                    if (multiSearech[y] != null)
                    {
                        filters[i] = multiSearech[y];
                        i++;
                    }
            }
            ViewBag.Filters = filters.ToList();

          
            return View(model);
        }
        #endregion

        #region BadPost
        public ActionResult BadPost(int Id)//דיווחים על מןדעות שלא הולמות ושחמרה בבסיס הנתונים
        {
            FJ_BadReports update = db.FJ_BadReports.ToList().Find(u => u.IDPost == Id);
            if (update == null)
            {
                var report = new FJ_BadReports();

                report.IDPost = Id;
                report.NumberOfReports = 1;
                db.FJ_BadReports.Add(report);
            }
            else
                update.NumberOfReports += 1;
            db.SaveChanges();

            var url = Request.UrlReferrer.PathAndQuery;
          
            return Content(@"<script language='javascript' type='text/javascript'>
                         alert('תודה רבה! המנהלים יבדקו את המודעה ');
                         window.location.href='" + url + "'</script>");
        }
        #endregion

        #region SendCV
        [HttpPost]
        public ActionResult SendCV(string id, HttpPostedFileBase fileUploader, string Body)//שליחת קורות חיים לפי מודעה
        {
            var To = db.FJ_JobsPosts.Where(x => x.ID.ToString() == id).SingleOrDefault();
            if (ModelState.IsValid)
            {
                string from = "find.job.cv.2019@gmail.com";

                using (MailMessage mail = new MailMessage(from, To.Email))
                {
                    var messageToCustomer = "שלום רב,\n קיבלתה קורות חיים למשרה: " + "\n" + To.Post_Title + "\n מספר משרה הוא: " + "\n" + To.NumberOfPosition + "\nמכתב מקדים: ";
                    mail.Subject = "קורות חיים חדשים למס' משרה " + To.NumberOfPosition;
                    mail.Body = messageToCustomer + "\n" + Body;

                    if (fileUploader != null)
                    {
                        string fileName = Path.GetFileName(fileUploader.FileName);

                        mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
                    }

                    mail.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient();

                    smtp.Host = "smtp.gmail.com";

                    smtp.EnableSsl = true;

                    NetworkCredential networkCredential = new NetworkCredential(from, "kosta512090");

                    smtp.UseDefaultCredentials = true;

                    smtp.Credentials = networkCredential;

                    smtp.Port = 587;

                    smtp.Send(mail);

                    var url = Request.UrlReferrer.PathAndQuery;

                    return Content(@"<script language='javascript' type='text/javascript'>
                         alert('קורות חיים נשלחו בהצלחה!');
                         window.location.href='" + url + "'</script>");
                }
            }

            else
            {
                return View();
            }
        }
        #endregion

        #region ConnectAdmin
        [HttpPost]
        public ActionResult ConnectToAdmin(string userName, string password)
        {
            var match = 0;
            if (userName != string.Empty)
            {
                var connection = db.FJ_Administrators.ToList();
                foreach (var unp in connection)
                {
                    if (unp.UserName == userName && unp.Password == password)
                        match = 1;
                }
                ViewBag.match = match;
            }
            if (match == 1)
            {
                ModelState.Clear();
                return RedirectToAction("AdminHomePage", "AdminPage");
            }
            else
            {
                ModelState.Clear();
                return Content(@"<script language='javascript' type='text/javascript'>
                         alert('הסיסמא או שם משתמש לא תקינים');
                         window.location.href='http://localhost:50203/'
                         </script>
                      ");
                
            }

        }
        #endregion

        #region ModalPopup
        public ActionResult ModalPopup(string Val)
        {
            var str = Val.Split(',');
            var num = str[0];
            var title = string.Empty;
            for (var i = 1; i < str.Length; i++)
            {

                title += i == 1 ? str[i] : "," + str[i];
            }


            if (num == "1")
            {

                ViewBag.ShowMore = subcatList;
                ViewBag.count = Subcat;
                ViewBag.val = 1;
                ViewBag.TitleSearch = title;
            }
            else if (num == "2")
            {
                ViewBag.ShowMore = allTypJob;
                ViewBag.count = typJob;
                ViewBag.val = 2;
                ViewBag.TitleSearch = title;
            }
            else if (num == "3")
            {
                ViewBag.ShowMore = allCategory;
                ViewBag.count = cate;
                ViewBag.val = 3;
                ViewBag.TitleSearch = title;
            }

            return PartialView();
        }
        #endregion

    }
}