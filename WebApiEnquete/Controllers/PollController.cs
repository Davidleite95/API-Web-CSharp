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
using System.Web.Http;
using System.Web.Mvc;
using WebApiEnquete.Models;

namespace WebApiEnquete.Controllers
{
    public class PollController : Controller
    {
        private ApiEnqueteEntities db = new ApiEnqueteEntities();

        //List Poll :id /OK
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
                CreateView(id);
                return Json(getPoll, JsonRequestBehavior.AllowGet);
            }
            else
            {
                throw new HttpException(404, "Not Found");
            }
        }

        //Create Views /OK
        public void CreateView(int id)
        {
            try
            {
                stats stats = new stats();
                stats.poll_id = id;
                db.stats.Add(stats);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //List Stats :id Poll /OK
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
                                               from v in db.vote
                                               where v.option_id == o.option_id
                                               select new { v.vote_id }
                                   ).Count()
                                   })
                           }).ToList();
            return Json(getPoll, JsonRequestBehavior.AllowGet);
        }

        //Insert Vote Options /OK
        public void InsertVoteOptions(vote vote)
        {
            try
            {
                vote vote1 = new vote();
                vote1.option_id = vote.option_id;
                db.vote.Add(vote1);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new HttpException(404, "Not Found"); throw;
            }
        }

        //Insert Vote Poll - PENDENTE
        public List<options> InsertPoll(options options)
        {
            List<options> options1 = new List<options>();
            options1.Add(options);

            return options1;
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
