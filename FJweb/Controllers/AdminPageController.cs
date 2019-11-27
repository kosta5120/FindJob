using FJweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FJweb.Controllers
{
    public class AdminPageController : Controller
    {
        FindJobEntities1 db = new FindJobEntities1();

        #region AdminHomePage
        public ActionResult AdminHomePage()
        {

            
            return View();

        }
        #endregion

        #region BadPost
        public ActionResult BadPost(string idPostToDelete, string idPostToRemoveFromBadReports)
        {
            var Br = new List<FJ_JobsPosts>();
            
            
             if(idPostToDelete != null)
             {
                
                var deletePostById = db.FJ_JobsPosts.Where(x => x.ID.ToString() == idPostToDelete).First();
                db.FJ_JobsPosts.Remove(deletePostById);

                var idBadPost = db.FJ_BadReports.Where(x => x.IDPost.ToString() == idPostToDelete).First();
                db.FJ_BadReports.Remove(idBadPost);

                db.SaveChanges();

                
            }
            else if (idPostToRemoveFromBadReports != null)
            {
                var idBadPost = db.FJ_BadReports.Where(x => x.IDPost.ToString() == idPostToRemoveFromBadReports).First();
                db.FJ_BadReports.Remove(idBadPost);

                db.SaveChanges();
            }

            var post = db.FJ_BadReports.ToList();

            foreach (var i in post)
            {
                Br.Add(db.FJ_JobsPosts.Where(x => x.ID == i.IDPost).Single());
            }
           
            ViewBag.br = Br;
            ViewBag.NumberOfReports = post;
            
            

            return View();
        }
        #endregion

        #region ChartPage
        public ActionResult ChartPage()
        {
            var countPerMonth = db.Database.SqlQuery<CountPoistPerMonth_Result>("exec CountPoistPerMonth").ToList();
            decimal Jan = 0;
            decimal Feb = 0;
            decimal Mer = 0;
            decimal Apr = 0;
            decimal May = 0;
            decimal Jun = 0;
            decimal Jul = 0;
            decimal Aug = 0;
            decimal Sep = 0;
            decimal Oct = 0;
            decimal Nov = 0;
            decimal Dec = 0;

            foreach (var month in countPerMonth)
            {
                switch (month.Month)
                {
                    case 1:
                        Jan = int.Parse(month.Count.ToString());
                        break;
                    case 2:
                        Feb = int.Parse(month.Count.ToString());
                        break;
                    case 3:
                        Mer = int.Parse(month.Count.ToString());
                        break;
                    case 4:
                        Apr = int.Parse(month.Count.ToString());
                        break;
                    case 5:
                        May = int.Parse(month.Count.ToString());
                        break;
                    case 6:
                        Jun = int.Parse(month.Count.ToString());
                        break;
                    case 7:
                        Jul = int.Parse(month.Count.ToString());
                        break;
                    case 8:
                        Aug = int.Parse(month.Count.ToString());
                        break;
                    case 9:
                        Sep = int.Parse(month.Count.ToString());
                        break;
                    case 10:
                        Oct = int.Parse(month.Count.ToString());
                        break;
                    case 11:
                        Nov = int.Parse(month.Count.ToString());
                        break;
                    case 12:
                        Dec = int.Parse(month.Count.ToString());
                        break;
                }
                new Chart(width: 1000, height: 400, theme: ChartTheme.Blue)
                .AddSeries(
                    chartType: "column",
                    xValue: new[] { Jan != 0 ? "ינואר" : 
                                    Feb != 0 ? "פבראור" : 
                                    Mer != 0 ? "מרץ" : 
                                    Apr != 0 ? "אפריל" : 
                                    May != 0 ? "מאי" : 
                                    Jun != 0 ? "יוני" : 
                                    Jul != 0 ? "יולי" : 
                                    Aug != 0 ? "אוגוסט" : 
                                    Sep != 0 ? "ספטמבר" : 
                                    Oct != 0 ?  "אוקטובר" : 
                                    Nov != 0 ? "נובמבר" : "דצמבר" },
                    yValues: new[] { Jan != 0 ? Jan : 
                                     Feb != 0 ? Feb : 
                                     Mer !=0 ? Mer : 
                                     Apr != 0 ? Apr : 
                                     May != 0 ? May : 
                                     Jun != 0 ? Jun : 
                                     Jul != 0 ? Jul : 
                                     Aug != 0 ? Aug : 
                                     Sep != 0 ? Sep : 
                                     Oct != 0 ? Oct : 
                                     Nov != 0 ? Nov : Dec })
                .Write();
                
            }
            return null;
        }
        #endregion

        #region QuantityOfJobsInEachCity
        public ActionResult QuantityOfJobsInEachCity()
        {
            var jobsEchCity = db.Database.SqlQuery<QuantityOfJobsInEachCity_Result>("exec QuantityOfJobsInEachCity").ToList();

            new Chart(width: 600, height: 600,theme: ChartTheme.Blue)
                 .AddSeries("Defoult", chartType: "bar",
                 xValue: jobsEchCity, xField: "City",
                 yValues: jobsEchCity, yFields: "Count")
                 .Write();

            return null;
        }
        #endregion

        #region ViewPostPopup
        public ActionResult ViewPostPopup(string idPostToView)
        {                
            ViewBag.ViewPost = db.FJ_JobsPosts.Where(x => x.ID.ToString() == idPostToView).ToList();

            return PartialView();
        }
        #endregion
    }
}