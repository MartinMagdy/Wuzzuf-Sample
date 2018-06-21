using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Wuzzuf.Models;
//using Wuzzuf.Models;

namespace Wuzzuf.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult Details(int JobId)
        {

            var Job = db.Jobs.Find(JobId);
            if (Job==null)
            {
                return HttpNotFound();
            }
            Session["JobId"] = JobId;
            return View(Job);
        }
        [Authorize]
        public ActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Apply(string Message)
        {
            var UserId = User.Identity.GetUserId();
            var JobId = (int)Session["JobId"];
            var check = db.ApplyForJobs.Where(x => x.JobId == JobId && x.UserId == UserId).ToList();
            if (check.Count<1)
            {
                var job = new ApplyForJob
                {
                    UserId = UserId,
                    JobId = JobId,
                    Message = Message,
                    ApplyDate = DateTime.Now
                };
                db.ApplyForJobs.Add(job);
                db.SaveChanges();
                ViewBag.Result = "Added Successfully";
            }
            else
            {
                ViewBag.Result = "Sorry you Are Already Applied";
            }
            return View();
        }
        [Authorize]
        public ActionResult GetJobByUser()
        {

            var UserId = User.Identity.GetUserId();
            var Jobs = db.ApplyForJobs.Where(a => a.UserId == UserId);
            return View(Jobs.ToList());
        }
        [Authorize]
        public ActionResult DetailsOfJob(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }
        [Authorize]
        public ActionResult GetJobsByPublisher()
        {
            var UserId = User.Identity.GetUserId();
            var Jobs = from app in db.ApplyForJobs join job in db.Jobs on app.JobId equals job.Id where job.User.Id == UserId select app;
            var grouped = from j in Jobs group j by j.Job.JobTitle into gr select new JobsViewModel { JobTitle = gr.Key, Items = gr };
            return View(grouped.ToList());
        }
        public ActionResult Edit(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job==null)
            {
                return HttpNotFound();
            }
            return View(job);
        }
        [HttpPost]
        public ActionResult Edit(ApplyForJob job)
        {
            if (ModelState.IsValid)
            {
                job.ApplyDate = DateTime.Now;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetJobByUser");
            }
            return View(job);
        }
        // GET: RoleView/Delete/5
        public ActionResult Delete(int id)
        {
            ViewBag.Name = new SelectList(db.Roles, "Name");
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: RoleView/Delete/5
        [HttpPost]
        public ActionResult Delete(ApplyForJob job)
        {
            ViewBag.Name = new SelectList(db.RoleviewModels, "Name");
            if (ModelState.IsValid)
            {
                var myjob = db.ApplyForJobs.Find(job.Id);
                db.ApplyForJobs.Remove(myjob);
                db.SaveChanges();
                return RedirectToAction("GetJobByUser");
            }
            return View(job);
        }
        [Authorize]
        // GET: RoleView/Delete/5
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Contact(ContactModelcs Contact)
        {
            var Mail = new MailMessage();
            var loginInfo = new NetworkCredential("Mailk a hndsa", "Passwordk");
            Mail.From = new MailAddress(Contact.Mail);
            Mail.To.Add(new MailAddress("nfs el Mail"));
            Mail.Subject = Contact.Subject;
            Mail.IsBodyHtml = true;
            string body = "Sender Name" + Contact.Name + "<br>" + "Sender Mail" + Contact.Mail + "<br>" + "Message Subject" + Contact.Subject + "<br>" + "The Message" + Contact.Message;
            Mail.Body = Contact.Message;
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(Mail);
            return RedirectToAction("Index");
        }
        public ActionResult Search()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Search(string searchname)
        {
            var Result = db.Jobs.Where(a => a.JobTitle.Contains(searchname)
              || a.JobContent.Contains(searchname)
              || a.JobTitle.Contains(searchname)
              || a.Category.CategoryName.Contains(searchname)
              || a.Category.CategoryDescription.Contains(searchname)).ToList();
          
            return View(Result);
        }
    }
}