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
                                   o.option_description
                               })
                           }).GroupBy(p => p.poll_id).ToList();

            return Json(getPoll, JsonRequestBehavior.AllowGet);
        }

        public JsonResult stats(int id)
        {
            var getPoll = (from p in db.poll
                           where p.poll_id == id
                           //join o in db.options on p.poll_id equals o.poll_id
                           //join v in db.vote on o.option_id equals v.option_id into juncao
                           //from j in juncao.DefaultIfEmpty()
                           select new
                           {
                               views = (from ps in db.poll
                                        where ps.poll_id == id 
                                        select new
                                        {
                                            ps.poll_id
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
        public JsonResult Poll1(JObject data)
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

        public JsonResult TesteReturn()
        {
            string json = @"{
                              'channel': {
                                'title': 'James Newton-King',
                                'link': 'http://james.newtonking.com',
                                'description': 'James Newton-King\'s blog.',
                                'item': [
                                  {
                                    'title': 'Json.NET 1.3 + New license + Now on CodePlex',
                                    'description': 'Announcing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
                                    'link': 'http://james.newtonking.com/projects/json-net.aspx',
                                    'category': [
                                      'Json.NET',
                                      'CodePlex'
                                    ]
                                  },
                                  {
                                    'title': 'LINQ to JSON beta',
                                    'description': 'Announcing LINQ to JSON',
                                    'link': 'http://james.newtonking.com/projects/json-net.aspx',
                                    'category': [
                                      'Json.NET',
                                      'LINQ'
                                    ]
                                  }
                                ]
                              }
                            }";

            JObject rss = JObject.Parse(json);

            var postTitles =
                from p in rss["channel"]["item"]
                select (string)p["title"];

            foreach (var item in postTitles)
            {
                Console.WriteLine(item);
            }
            //LINQ to JSON beta
            //Json.NET 1.3 + New license + Now on CodePlex

            var categories =
                from c in rss["channel"]["item"].Children()["category"].Values<string>()
                group c by c
                into g
                orderby g.Count() descending
                select new { Category = g.Key, Count = g.Count() };

            foreach (var c in categories)
            {
                Console.WriteLine(c.Category + " - Count: " + c.Count);
            }

            return Json(categories, JsonRequestBehavior.AllowGet);
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
