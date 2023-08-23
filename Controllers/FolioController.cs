using hotelsimorgh.Data;
using hotelsimorgh.Data.DbModels;
using hotelsimorgh.Models;
using hotelsimorgh.Utilities;
using MD.PersianDateTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Diagnostics;

namespace hotelsimorgh.Controllers
{
    [Authorize]
    public class FolioController : Controller
    {
        public IActionResult Index(string dateRange, int folioId, bool folioBedehkar, string Family, string mobile, bool noReserve, bool preReserve, int id)
        {

            using (UnitOfWork db = new UnitOfWork())
            {
                if (db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Bloked) return RedirectToAction("Logout", "User");
                int defaultDays = Convert.ToInt32(db.SiteSettingRepository.Get(x => x.key == "NumberOfDaysToShowInFolioList").First().Value);
                DateTime fromDate = dateRange.getFromDate();
                DateTime toDate = dateRange.getUntilDate(defaultDays);
                IEnumerable<Folio> folios = new List<Folio>();
                if (noReserve)
                {
                    folios = db.FolioRepository.Get(x => x.Reserves.Count() == 0, includeProperties: "Reserves,passengers");
                }
                else if (folioId != 0)
                {
                    folios = db.FolioRepository.Get(x => x.Id == folioId, includeProperties: "Reserves,passengers");
                }
                else if (folioBedehkar)
                {
                    folios = db.FolioRepository.Get(x => x.Bedehkar, includeProperties: "Reserves,passengers");
                }
                else if (Family != null)
                {
                    Family = Family.Trim();
                    folios = db.FolioRepository.Get(x => x.Family.Contains(Family) || x.Name.Contains(Family), includeProperties: "Reserves,passengers");
                }
                else if (mobile != null)
                {
                    folios = db.FolioRepository.Get(x => x.Mobile == mobile, includeProperties: "Reserves,passengers");
                }
                else if (preReserve)
                {
                    folios = db.FolioRepository.Get(x =>
                   ((x.Reserves.OrderBy(x => x.Date).First().Date <= fromDate && x.Reserves.OrderBy(x => x.Date).Last().Date.AddDays(1) >= fromDate) ||
                   (x.Reserves.OrderBy(x => x.Date).First().Date <= toDate && x.Reserves.OrderBy(x => x.Date).Last().Date >= toDate) ||
                   (x.Reserves.OrderBy(x => x.Date).First().Date >= fromDate && x.Reserves.OrderBy(x => x.Date).Last().Date <= toDate))
                   && x.PreReserve, includeProperties: "Reserves,passengers");
                    ViewBag.Date = dateRange ?? fromDate.ToShamsi().ToStringPersianDateTime() + " - " + toDate.ToShamsi().ToStringPersianDateTime();
                }
                else
                {
                    folios = db.FolioRepository.Get(x =>
                    ((x.Reserves.OrderBy(x => x.Date).First().Date <= fromDate && x.Reserves.OrderBy(x => x.Date).Last().Date.AddDays(1) >= fromDate) ||
                    (x.Reserves.OrderBy(x => x.Date).First().Date <= toDate && x.Reserves.OrderBy(x => x.Date).Last().Date >= toDate) ||
                    (x.Reserves.OrderBy(x => x.Date).First().Date >= fromDate && x.Reserves.OrderBy(x => x.Date).Last().Date <= toDate))
                    && !x.PreReserve, includeProperties: "Reserves,passengers");
                    ViewBag.Date = dateRange ?? fromDate.ToShamsi().ToStringPersianDateTime() + " - " + toDate.ToShamsi().ToStringPersianDateTime();
                }

                ViewBag.returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString.ToString().Replace("&", "$");
                ViewBag.receptionists = db.UserRepository.Get();
                ViewBag.folioId = folioId == 0 ? null : folioId.ToString();
                ViewBag.foliotitle = folioId == 0 ? null :
              "فولیو: " + folioId.ToString() + "، " + db.FolioRepository.GetByID(folioId)?.Name + " " + db.FolioRepository.GetByID(folioId)?.Family;
                ViewBag.preReserve = preReserve;
                ViewBag.mobile = mobile;
                ViewBag.Family = Family;
                ViewBag.folioBedehkar = folioBedehkar;
                ViewBag.noReserve = noReserve;
                ViewBag.rooms = db.RoomRepository.Get().ToList();
                ViewBag.websites=db.WebsiteRepository.Get().ToList();
                bool order = false;
                foreach (var item in folios)
                {
                    order = (item.Reserves.Count() == 0);
                }
                var model = (!order) ? folios.OrderBy(x => x.Reserves.OrderBy(x => x.Date).First().Date).ToList() : folios;
                if (id!=0)
                {
                   var allfolios = db.FolioRepository.Get(includeProperties: "Reserves,passengers",orderBy:x=>x.OrderByDescending(x=>x.Id));
                    int firstId=allfolios.First().Id;
                    model = allfolios.Where(x => x.Id > firstId - id);
                }
                return View(model);
            }
        }
        public IActionResult AddOrEdit(int id, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.returnUrl = returnUrl.Replace("$", "&");
            using (UnitOfWork db = new UnitOfWork())
            {
                ViewBag.receptionists = db.UserRepository.Get();
                ViewBag.UserId = db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Id;
                if (id == 0)
                {
                    ViewBag.edit = false;
                    return View();
                }
                else
                {
                    ViewBag.edit = true;
                    var folio = db.FolioRepository.GetByID(id);
                    var passengers = db.PassengerRepository.Get(c => c.FolioId == id);
                    FolioModel model = new FolioModel();
                    model.Folio = folio;
                    model.passengers = passengers;
                    return View(model);
                }
            }
        }
        [HttpPost]
        public IActionResult AddOrEdit(Folio folio, List<passenger> passengers, string returnUrl)
        {

            folio = folio.TrimFolioValues();
            passengers = passengers.TrimPassengersValues();
            using (UnitOfWork db = new UnitOfWork())
            {
                if (folio.Id == 0)
                {

                    db.FolioRepository.Insert(folio);
                    db.Save();
                    var Folio = db.FolioRepository.Get(c => c.Mobile == folio.Mobile).Last();
                    foreach (var item in passengers)
                    {
                        item.FolioId = Folio.Id;
                        db.PassengerRepository.Insert(item);
                    }
                    db.Save();
                }
                else
                {
                    folio.PreReserve = false;
                    foreach (var item in db.PassengerRepository.Get(c => c.FolioId == folio.Id))
                    {
                        db.PassengerRepository.Delete(item);
                    }
                    db.Save();
                    db.FolioRepository.Update(folio);
                    foreach (var item in passengers)
                    {
                        item.FolioId = folio.Id;
                        db.PassengerRepository.Insert(item);
                    }
                    db.Save();
                }


                return RedirectToAction("Index", new { folioId = folio.Id });
            }
        }
        [HttpPost]
        public IActionResult AddOrEditPreReserve(Folio folio, string returnUrl)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                if (db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Bloked) return RedirectToAction("Logout", "User");
                folio = folio.TrimFolioValues();
                folio.PreReserve = true;
                if (folio.Id == 0)
                {
                    db.FolioRepository.Insert(folio);
                }
                else
                {
                    db.FolioRepository.Update(folio);
                }
                db.Save();

                return RedirectToAction("Index", new { folioId = folio.Id });
            }
        }
        [HttpPost]
        public IActionResult GetFolioInReserve(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var folio = db.FolioRepository.GetByID(id);
                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(folio));
            }
        }

        [HttpPost]
        public IActionResult DeleteFolio(int folioId)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var reserves = db.ReserveRepository.Get(x => x.FolioId == folioId);
                if (reserves.Count() == 0)
                {
                    db.FolioRepository.Delete(folioId);
                    db.Save();
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
        }

        public IActionResult ChangeRoom(int folioId, int roomId, string ReserveIntersection)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                ViewBag.rooms = db.RoomRepository.Get();
                ViewBag.folioId = folioId;
                ViewBag.ReserveIntersection = ReserveIntersection;
                ViewBag.family = db.FolioRepository.GetByID(folioId).Name+" "+ db.FolioRepository.GetByID(folioId).Family;
                var reserves = db.ReserveRepository.Get(x => x.FolioId == folioId && x.RoomId == roomId);
                return View(reserves.Count() == 0 ? null : reserves.First());
            }
        }
        [HttpPost]
        public IActionResult ChangeRoom(int folioId, int NewRoomId, int oldRoomId)
        {
            ReserveHelper helper = new ReserveHelper();
            if (NewRoomId != 0)
            {
                using (UnitOfWork db = new UnitOfWork())
                {

                    var reserves = db.ReserveRepository.Get(x => x.FolioId == folioId && x.RoomId == oldRoomId, includeProperties: "Room").OrderBy(x => x.Date);
                    List<Reserve> IntractReserves = new List<Reserve>();

                    foreach (var item in reserves)
                    {
                        var Ires = db.ReserveRepository.Get(x => x.Date.Date == item.Date.Date && x.RoomId == NewRoomId, includeProperties: "Folio").FirstOrDefault();
                        if (Ires is not null)
                        {
                            IntractReserves.Add(Ires);
                        }
                    }

                    if (IntractReserves.Count > 0)
                    {
                        return RedirectToAction("ChangeRoom", new { folioId = folioId, roomId = oldRoomId, ReserveIntersection = IntractReserves.First().Folio.Name + " " + IntractReserves.First().Folio.Family + " ،کد:" + IntractReserves.First().FolioId });
                    }

                    foreach (var item in reserves)
                    {
                        item.RoomId = NewRoomId;

                    }

                    foreach (var item in reserves)
                    {
                        db.ReserveRepository.Update(item);
                        db.Save();
                    }
                }
            }
            return RedirectToAction("Index", new { folioId = folioId });
        }
        public IActionResult setfolioprereserve(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                if (id != 0)
                {
                    var folio = db.FolioRepository.GetByID(id);
                    if (!folio.PreReserve)
                    {
                        folio.PreReserve = true;
                    }
                    db.FolioRepository.Update(folio);
                    db.Save();
                }
                return Json("");
            }
        }
    }
}
