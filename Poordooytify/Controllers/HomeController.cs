using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Poordooytify.Models;
using Dropbox.Api;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Globalization;

namespace Poordooytify.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var dbsql = new PoordooytifyContext();
            //var dbmdb = new AccessPoordooytifyContext();

            //var token = dbmdb.CloudTokens.ToList();
            //dbsql.CloudTokens.AddRange(token);

            //var songs = dbmdb.Songs.AsNoTracking().ToList();
            //dbsql.Songs.AddRange(songs);
            //dbsql.SaveChanges();

            return RedirectToAction("Index", "Songlib");
        }

    }
}