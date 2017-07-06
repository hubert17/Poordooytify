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
       
        public ActionResult Index(string q = "", string r = "", string m = "", int pageSize = 20, int page = 1 )
        {

            var dbSongs = db.Songs.Where(x => x.CloudToken.Inactive == false);
            var pagedSongs = dbSongs.OrderByDescending(o => o.Id).Skip((page - 1) * pageSize).Take(pageSize);
            var songs = new List<Song>();
            ViewBag.Moods = db.Moods.OrderByDescending(x => x.ModifiedDate).ThenBy(o => o.CreateDate).ToList();
            var Loadbutton = false;
            var ImHome = false;

            if (page>1)
            {
                var SongIds = Newtonsoft.Json.JsonConvert.SerializeObject(pagedSongs.Select(s => s.Id));
                @ViewBag.SongIds = SongIds;
                return PartialView("_SongCard", pagedSongs);
            }

            if (string.IsNullOrEmpty(m))
            {
                if (string.IsNullOrEmpty(q))
                {
                    if (string.IsNullOrEmpty(r))
                    {
                        songs = pagedSongs.ToList();
                        ImHome = true;
                    }
                    else
                        songs = dbSongs.OrderByDescending(d => d.PlayCount).Take(20).ToList();
                    Loadbutton = true;
                }
                else
                {
                    songs = dbSongs.Where(x => x.Title.Contains(q) || x.Artist.Contains(q) || x.Genre.Contains(q)).Take(20).OrderBy(o => o.PlayCount).ToList();
                }
            }
            else
            {
                songs = (from s in dbSongs
                              join sm in db.SongMoods on s.Key equals sm.SongKey
                              where sm.MoodKey == m && s.CloudToken.Inactive == false
                              select new 
                              {
                                  Id = s.Id,
                                  Title = s.Title,
                                  Artist = s.Artist,
                                  Link = s.Link
                              }).ToList().Select(s => new Song
                                 {
                                     Id = s.Id,
                                     Title = s.Title,
                                     Artist = s.Artist,
                                     Link = s.Link
                                 }).ToList();
                TempData["InstantPlaySongId"] = songs.FirstOrDefault().Id;
            }

            ViewBag.Home = ImHome;
            ViewBag.Loadbutton = Loadbutton;
            return View(songs);
        }

        public ActionResult Create()
        {
            var storage = db.CloudTokens.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.AccountName }).ToList();
            ViewBag.storage = storage;
            ViewBag.genres = Newtonsoft.Json.JsonConvert.SerializeObject(db.Genres.Select(g => g.Name).ToList());
            return View();
        }

        [HttpPost]
        public ActionResult GetLink(int songId)
        {
            var song = new Song();
            string songLink = string.Empty;
            try
            {
                song = db.Songs.Find(songId);
                songLink = song.Link;
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch { }
            return Json(new { songLink = songLink }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public EmptyResult AddPlayCount(int songId)
        {
            var song = new Song();
            try
            {
                song = db.Songs.Find(songId);
                song.PlayCount = song.PlayCount + 1;
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch { }
            //return Json( song.PlayCount, JsonRequestBehavior.AllowGet);
            return null;
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
        [ValidateAntiForgeryToken]
        public ActionResult UploadAudioFile(HttpPostedFileBase audioFile, Song song, string BitRate)
        {
            TempData["InstantPlaySongId"] = 0;
            var songId = 0;
            try
            {
                if (audioFile.ContentLength > 0)
                {
                    //song.Key = PoordooytifyKey.Generate();
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    song.Title = textInfo.ToTitleCase(song.Title);
                    song.Artist = textInfo.ToTitleCase(song.Artist);
                    song.PlayCount = 0;
                    db.Songs.Add(song);
                    song.OrigFilename = Path.GetFileName(audioFile.FileName);
                    song.DateAdded = DateTime.Now;
                    song.Key = PoordooytifyKey.Generate();
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
                    TempData["InstantPlaySongId"] = song.Id;
                    TempData.Keep("InstantPlaySongId");
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