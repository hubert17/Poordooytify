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
using E.Deezer;

namespace Poordooytify.Controllers
{
    public class SonglibController : Controller
    {
        // GET: Songlib
        PoordooytifyContext db = new PoordooytifyContext();

        // GET: Home
        public ActionResult Index(string q = "", string r = "")
        {
            var songs = new List<Song>();
            if(string.IsNullOrEmpty(q))
            {
                if(string.IsNullOrEmpty(r))
                    songs = db.Songs.Where(x => x.CloudToken.Inactive == false).OrderByDescending(o => o.Id).ToList();
                else
                    songs = db.Songs.Where(x => x.CloudToken.Inactive == false).OrderByDescending(d=>d.PlayCount).Take(20).ToList();
            }
            else
            {
                songs = db.Songs.Where(x => x.CloudToken.Inactive == false).Where(x => x.Title.Contains(q) || x.Artist.Contains(q) || x.Genre.Contains(q)).Take(10).OrderBy(o=>o.PlayCount).ToList();                
            }
            return View(songs);
        }

        public ActionResult Create()
        {
            var storage = db.CloudTokens.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.AccountName }).ToList();
            ViewBag.storage = storage;
            return View();
        }

        public ActionResult GetLink(int songId)
        {
            var song = new Song();
            string songLink = string.Empty;
            try
            {
                song = db.Songs.Find(songId);
                songLink = song.Link;
                song.PlayCount = song.PlayCount + 1;
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch { }
            return Json(new { songLink = songLink }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Artists(string q)
        {
            //Create a new DeezerSession for your application
            //You'll retrieve a 'Deezer' object which you can browse the API from.
            var Deezer = DeezerSession.CreateNew();

            //This performs an async search on Deezer for albums matching "Abba"
            //Mapping to API: search/album?q=Abba&index=0&limit=25
            //var search = await Deezer.Search.Albums("Abba");

            //You can vary the size and starting position of querys...

            //Will only return UP-TO a maximum of 10 artists matching "Queen"
            //Mapping to API: search/artist?q=Queen&index=0&limit=10
            var small_search = await Deezer.Search.Artists(q, 10);

            //This will return for UP-TO a maximum of 15 tracks by Elvis. 
            //These will be offset by 20 places in the results. This is useful for pagination.
            //Mapping to API: search/track/?q=Elivs&index=20&limt=15
            //var offset_search = await Deezer.Search.Tracks("Elivs", 20, 15);

            var artists = small_search.Select(x => x.Name).ToList();
            return Json(artists, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UploadAudioFile(HttpPostedFileBase audioFile, Song song, string BitRate)
        {
            TempData["RecentSongId"] = 0;
            var songId = 0;
            try
            {
                if (audioFile.ContentLength > 0)
                {
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    song.Title = textInfo.ToTitleCase(song.Title);
                    song.Artist = textInfo.ToTitleCase(song.Artist);
                    song.PlayCount = 0;
                    db.Songs.Add(song);
                    song.OrigFilename = Path.GetFileName(audioFile.FileName);
                    song.DateAdded = DateTime.Now;
                    db.SaveChanges();

                    songId = song.Id;
                    string _FileName = song.Title + " - " + song.Artist + " [" + song.Id + "]" + Path.GetExtension(audioFile.FileName);
                    string _path = Path.Combine(Server.MapPath("~/TempUpload"), _FileName);
                    audioFile.SaveAs(_path);
                    var fileToUpload = ConvertToOpus(_path, BitRate);
                    if (System.IO.File.Exists(_path))
                    {
                        System.IO.File.Delete(_path);
                    }
                    var task = Task.Run(() => UploadToDropbox(fileToUpload, song.Id, song.CloudTokenId));
                    task.Wait();
                    TempData["RecentSongId"] = song.Id;
                    TempData.Keep("RecentSongId");
                }
            }
            catch(Exception ex)
            {
                TempData["Message"] = "File upload failed. Err: " + ex.Message;
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            finally { }

            return RedirectToAction("Index");
        }

        private string ConvertToOpus(string inputFile, string BitRate)
        {
            string outputFile = Path.Combine(Server.MapPath("~/TempUpload"), Path.GetFileNameWithoutExtension(inputFile) + ".opus");
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

            ffMpeg.ConvertMedia(inputFile,
                null, // autodetect by input file extension 
                outputFile,
                null, // autodetect by output file extension 
                new NReco.VideoConverter.ConvertSettings()
                {
                    CustomOutputArgs = " -acodec libopus -b:a " + BitRate + " -vbr on -compression_level 10 "
                }
            );

            return outputFile;
        }

        private async Task UploadToDropbox(string localPath, int songId, int CloudTokenId)
        {
            var token = db.CloudTokens.Find(CloudTokenId).Token;
            var song = db.Songs.Find(songId);
            using (var dbx = new DropboxClient(token))
            {
                var remotePath = "/" + Path.GetFileName(localPath);
                using (var fileStream = System.IO.File.Open(localPath, FileMode.Open))
                {
                    var s = await dbx.Files.UploadAsync(remotePath, body: fileStream);
                    var result = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(remotePath);
                    song.Link = result.Url;
                    db.Entry(song).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (System.IO.File.Exists(localPath))
            {
                System.IO.File.Delete(localPath);
            }
        }

    }
}