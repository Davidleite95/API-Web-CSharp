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

        [HttpGet]
        [Route("Get")]
        public JsonResult Get(int id)
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

            //return getPoll;
            return Json(getPoll, JsonRequestBehavior.AllowGet);
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
