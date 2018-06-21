using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wuzzuf.Models;

namespace Wuzzuf.Controllers
{
    [Authorize(Roles = "Admins")]
    public class RoleViewController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: RoleView
        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }

        // GET: RoleView/Details/5
        public ActionResult Details(string id)

        {
            ViewBag.Name = new SelectList(db.Roles, "Name");
            var role = db.Roles.Find(id);
            if (role==null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // GET: RoleView/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleView/Create
        [HttpPost]
        public ActionResult Create(IdentityRole role)
        {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            return View(role);
        }

        // GET: RoleView/Edit/5
        public ActionResult Edit(string id)
        {
            var role = db.Roles.Find(id);
            if (role==null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: RoleView/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include ="Id,Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        // GET: RoleView/Delete/5
        public ActionResult Delete(string id)
        {
            ViewBag.Name = new SelectList(db.Roles, "Name");
            var role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: RoleView/Delete/5
        [HttpPost]
        public ActionResult Delete(IdentityRole role)
        {
            ViewBag.Name = new SelectList(db.RoleviewModels,"Name");
            if (ModelState.IsValid)
            {
                var myrole = db.Roles.Find(role.Id);
                db.Roles.Remove(myrole); 
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }
    }
}
