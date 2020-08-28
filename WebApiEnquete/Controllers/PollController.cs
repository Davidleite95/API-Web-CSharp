using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApiEnquete.Models;

namespace WebApiEnquete.Controllers
{
    public class PollController : Controller
    {
        private ApiEnqueteEntities db = new ApiEnqueteEntities();

        // GET: Poll
        public ActionResult Index()
        {
            return View(db.poll.ToList());
        }

        [HttpGet]
        [Route("Get")]
        public Object Get(int id)
        {
            var getPoll = (from p in db.poll
                           where p.poll_id == id
                           from o in db.options
                           where o.poll_id == p.poll_id
                           select new
                           {
                               p.poll_id,
                               p.poll_description,
                               o.option_id,
                               o.option_description
                           }).ToList();

            return getPoll;
        }
       
        
        // GET: Poll/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            poll poll = db.poll.Find(id);
            if (poll == null)
            {
                return HttpNotFound();
            }
            return View(poll);
        }

        // GET: Poll/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Poll/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "poll_id,poll_description")] poll poll)
        {
            if (ModelState.IsValid)
            {
                db.poll.Add(poll);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(poll);
        }

        // GET: Poll/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            poll poll = db.poll.Find(id);
            if (poll == null)
            {
                return HttpNotFound();
            }
            return View(poll);
        }

        // POST: Poll/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "poll_id,poll_description")] poll poll)
        {
            if (ModelState.IsValid)
            {
                db.Entry(poll).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(poll);
        }

        // GET: Poll/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            poll poll = db.poll.Find(id);
            if (poll == null)
            {
                return HttpNotFound();
            }
            return View(poll);
        }

        // POST: Poll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            poll poll = db.poll.Find(id);
            db.poll.Remove(poll);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
