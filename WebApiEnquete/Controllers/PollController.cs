using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebApiEnquete.Models;

namespace WebApiEnquete.Controllers
{
    public class PollController : Controller
    {
        private ApiEnqueteEntities db = new ApiEnqueteEntities();

        [HttpGet]
        [Route("poll")]
        public JsonResult poll(int id)
        {
            var getPoll = (from p in db.poll
                           where p.poll_id == id
                           select new
                           {
                               p.poll_id,
                               p.poll_description,
                               options = (
                               from o in db.options
                               where p.poll_id == o.poll_id
                               select new
                               {
                                   o.option_id,
                                   o.option_description,
                               })
                           }).GroupBy(p => p.poll_id).ToList();
            if (getPoll.Count() != 0)
            {
                CreateView();
                return Json(getPoll, JsonRequestBehavior.AllowGet);
            }
            else
            {
                throw new HttpException(404, "Not Found");
            }
        }

        [HttpGet]
        //Criar Views
        public void CreateView()
        {
            try
            {
                vote vote = new vote();
                vote.option_description = "Registrado";
                db.vote.Add(vote);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("stats")]
        public JsonResult stats(int id)
        {
            var getPoll = (from p in db.poll
                           where p.poll_id == id
                           //join o in db.options on p.poll_id equals o.poll_id
                           //join v in db.vote on o.option_id equals v.option_id into juncao
                           //from j in juncao.DefaultIfEmpty()
                           select new
                           {
                               views = (from v in db.vote
                                        select new
                                        {
                                            v.vote_id
                                        }).Count(),
                               votes = (
                                   from o in db.options
                                   where p.poll_id == o.poll_id
                                   select new
                                   {
                                       o.option_id,
                                       qty = (
                                                   from os in db.options
                                                   select new { os.option_id }
                                   ).Count()
                                   })
                           }).ToList();
            return Json(getPoll, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult poll(HttpResponse data)
        {
            dynamic json = data;
            options options = new options()
            {
                option_description = json.options
            };
            db.options.Add(options);
            db.SaveChanges();

            return Json(options, JsonRequestBehavior.AllowGet);

            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string SubmitOrder()
        {
            return "ok";
        }
    }
}
