using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Poordooytify.Models;

namespace Poordooytify.Controllers
{
    public class MoodController : Controller
    {
        private PoordooytifyContext db = new PoordooytifyContext();

        // GET: Mood
        public ActionResult Index()
        {            
            return View(db.Moods.OrderByDescending(x=>x.ModifiedDate).ThenBy(o=>o.CreateDate).ToList());
        }

        public ActionResult GetTitles(string moodKey)
        {
            var songTitles = (from s in db.Songs
                              join sm in db.SongMoods on s.Key equals sm.SongKey
                              where sm.MoodKey == moodKey
                              select s.Title).Take(10).ToList();

            return Content(String.Join(", ", songTitles));
        }

        [HttpPost]
        public ActionResult AddSong(string moodKey, string songKey)
        {
            try
            {
                var sm = new SongMood() { MoodKey = moodKey, SongKey = songKey };
                db.SongMoods.Add(sm);
                var m = db.Moods.Where(x=>x.Key==moodKey).FirstOrDefault();
                m.ModifiedDate = DateTime.Now;
                db.Entry(m).State = EntityState.Modified;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        #region Crud

        // GET: Mood/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mood mood = db.Moods.Find(id);
            if (mood == null)
            {
                return HttpNotFound();
            }
            return View(mood);
        }

        // GET: Mood/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mood/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description,Categories,InActive,CreatedBy,CreateDate")] Mood mood)
        {
            if (ModelState.IsValid)
            {
                mood.CreateDate = DateTime.Now;
                mood.Key = PoordooytifyKey.Generate();
                db.Moods.Add(mood);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mood);
        }

        // GET: Mood/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mood mood = db.Moods.Find(id);
            if (mood == null)
            {
                return HttpNotFound();
            }
            return View(mood);
        }

        // POST: Mood/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Categories,InActive,CreatedBy,CreateDate")] Mood mood)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mood).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mood);
        }

        // GET: Mood/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mood mood = db.Moods.Find(id);
            if (mood == null)
            {
                return HttpNotFound();
            }
            return View(mood);
        }

        // POST: Mood/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Mood mood = db.Moods.Find(id);
            db.Moods.Remove(mood);
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

        #endregion
    }
}
