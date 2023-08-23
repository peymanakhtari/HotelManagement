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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int folioId, string dateRange, int roomId, int bedCount, bool folioBedehkar, int filter, int websiteId, bool hafDay, int submitter)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                bool seprateRows = true;
                bool thinLineSeprator = false;
                if (db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Bloked) return RedirectToAction("Logout", "User");
                int defaultDays = Convert.ToInt32(db.SiteSettingRepository.Get(x => x.key == "NumberOfDaysToShowInMainPage").First().Value);

                DateTime fromDate = dateRange.getFromDate();
                DateTime toDate = dateRange.getUntilDate(defaultDays);

                int Days = (int)(toDate - fromDate).TotalDays;
                var websites = db.WebsiteRepository.Get().OrderBy(x => x.Sequence).ToList();
                var rooms = (bedCount != 0) ? db.RoomRepository.Get().OrderBy(x => x.bed).ThenBy(x => x.Capacity).ToList() : db.RoomRepository.Get().ToList();

                IEnumerable<Reserve> reserves = new List<Reserve>();
                List<ReserveTemplateRow> reserveTemplates = new List<ReserveTemplateRow>();

                if (folioId != 0)
                {
                    seprateRows = false;
                    filter = 3;
                    roomId = 0;
                    websiteId = 0;
                    reserves = db.ReserveRepository.Get(x => x.FolioId == folioId, includeProperties: "Folio,Room,User").OrderBy(x => x.Room.Number).ThenBy(x => x.Date);
                    foreach (var item in reserves)
                    {
                        if (!reserveTemplates.Any(x => x.date == item.Date && x.room.Number == item.Room.Number))
                        {
                            reserveTemplates.Add(new ReserveTemplateRow()
                            {
                                date = item.Date,
                                room = item.Room,
                            });
                        }
                    }
                }
                else
                {
                    ViewBag.Date = dateRange ?? fromDate.ToShamsi().ToStringPersianDateTime() + " - " + toDate.ToShamsi().ToStringPersianDateTime(); ;
                    reserves = db.ReserveRepository.Get(x => x.Date.Date >= fromDate.Date && x.Date.Date <= toDate.Date, includeProperties: "Folio,Room,User");
                    for (int i = 0; i <= Days; i++)
                    {
                        foreach (var item in rooms)
                        {
                            DateTime date = fromDate.AddDays(i);
                            reserveTemplates.Add(new ReserveTemplateRow()
                            {
                                date = date,
                                room = item
                            });
                        }

                    }
                    switch (filter)
                    {
                        case 1:
                            reserves = reserves.Where(x => !x.Folio.PreReserve); break;
                        case 2:
                            reserves = reserves.Where(x => x.Folio.PreReserve); break;
                    }

                    if (folioBedehkar)
                    {
                        seprateRows = false;
                        roomId = 0;
                        websiteId = 0;
                        reserves = reserves.Where(x => x.Folio.Bedehkar);
                        filter = 3;
                    }
                    else
                    {

                        if (hafDay)
                        {
                            reserves = reserves.Where(x => x.HafDay);
                            filter = 3;
                        }
                        if (bedCount != 0)
                        {
                            reserves = reserves.Where(x => x.Room.bed == bedCount);
                            reserveTemplates = reserveTemplates.Where(x => x.room.bed == bedCount).ToList();
                        }
                        if (roomId != 0)
                        {
                            seprateRows = false;
                            reserves = reserves.Where(x => x.RoomId == roomId);
                            reserveTemplates = reserveTemplates.Where(x => x.room.Id == roomId).ToList();
                        }
                        if (websiteId != 0)
                        {
                            reserves = (websiteId == -1) ? reserves.Where(x => x.WebsiteId == 0) : reserves.Where(x => x.WebsiteId == websiteId);
                            filter = 3;
                        }
                        if (submitter != 0)
                        {
                            reserves = reserves.Where(x => x.UserId == submitter);
                            filter = 3;
                        }

                    }


                }

                ReserveHelper helper = new ReserveHelper();
                foreach (var item in reserveTemplates)
                {
                    var reserve = reserves.FirstOrDefault(x => x.Date.Date == item.date.Date && x.RoomId == item.room.Id && !x.HafDay);
                    var RhafDay = reserves.FirstOrDefault(x => x.Date.Date == item.date.Date && x.RoomId == item.room.Id && x.HafDay);

                    var now = DateTime.Now.Date;
                    if (reserve != null)
                    {
                        if (item.reserve == null)
                        {
                            item.reserve = new ReserveWithWebsite();
                        }
                        item.statusRes = helper.statusReserve(reserve);

                        item.reserve.reserve = reserve;
                        item.reserve.website = websites.FirstOrDefault(x => x.Id == reserve.WebsiteId)?.Name;
                    }

                    if (RhafDay != null)
                    {
                        if (item.HafDay == null)
                        {
                            item.HafDay = new ReserveWithWebsite();
                        }

                        item.statusHres = helper.statusReserve(RhafDay);

                        item.HafDay.reserve = RhafDay;
                        item.HafDay.website = websites.FirstOrDefault(x => x.Id == RhafDay.WebsiteId)?.Name;
                    }
                }

                if (filter == 1 || filter == 2 || filter == 3)
                {
                    thinLineSeprator = true;
                    reserveTemplates = reserveTemplates.Where(x => x.reserve != null || x.HafDay != null).ToList();
                }

                if (filter == 4)
                {
                    reserveTemplates = reserveTemplates.Where(x => x.reserve == null).ToList();
                }

                ViewBag.rooms = rooms.OrderBy(x => x.Number).ToList();
                ViewBag.websites = websites;
                ViewBag.returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString.ToString().Replace("&", "$");
                ViewBag.filter = filter;
                ViewBag.roomNumber = roomId == 0 ? 0 : db.RoomRepository.GetByID(roomId).Number;
                ViewBag.websiteId = websiteId;
                ViewBag.folioBedehkar = folioBedehkar;
                ViewBag.folioId = folioId == 0 ? null : folioId.ToString();
                ViewBag.foliotitle = folioId == 0 ? null :
               "رزرو: " + folioId.ToString() + "، " + db.FolioRepository.GetByID(folioId).Name + " " + db.FolioRepository.GetByID(folioId).Family;
                ViewBag.hafDay = hafDay;
                ViewBag.seprateRows = seprateRows;
                ViewBag.thinLineSeprator = thinLineSeprator;
                ViewBag.bedTypes = helper.bedtypes(rooms);
                ViewBag.bedCount = bedCount;
                ViewBag.Submitters = db.UserRepository.Get();
                ViewBag.submitter = submitter;
                ViewBag.HafDayLeaveHour = db.SiteSettingRepository.Get(x => x.key == "hafdayLeaveHour").First().Value;
                return View(reserveTemplates);
            }
        }
        public IActionResult Test()
        {
            return View();
        }
        public IActionResult testreserves()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var res = db.ReserveRepository.Get(x => !x.HafDay);
                var Hres = db.ReserveRepository.Get(x => x.HafDay);
                List<Reserve> list = new List<Reserve>();
                List<Reserve> Hlist = new List<Reserve>();
                foreach (var item in res)
                {
                    if (res.Any(x => x.Date.Date == item.Date.Date && x.RoomId == item.RoomId && x.Id != item.Id))
                    {
                        list.Add(item);
                    }
                }
                foreach (var item in Hres)
                {
                    if (Hres.Any(x => x.Date.Date == item.Date.Date && x.RoomId == item.RoomId && x.Id != item.Id))
                    {
                        Hlist.Add(item);
                    }
                }
                ViewBag.reserve = list;
                ViewBag.HafDay = Hlist;
            }

            return View("test");
        }
        public IActionResult testIfHafdayhasSameFolioOfPreviousDay()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var Hres = db.ReserveRepository.Get(x => x.HafDay);
                var res = db.ReserveRepository.Get(x => !x.HafDay);
                var folios = new List<int>();
                foreach (var item in Hres)
                {
                    if (!res.Any(x => x.FolioId == item.FolioId && x.Date.Date.AddDays(1) == item.Date.Date))
                    {
                        folios.Add(item.FolioId);
                    }
                    ViewBag.folios = folios;
                }
            }
            return View("test");
        }
        public IActionResult checkGapInFolioReserves()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var folios = db.FolioRepository.Get(includeProperties: "Reserves");
                List<Folio> foliosWithIssue = new List<Folio>();
                foreach (var item in folios)
                {
                    if (item.Reserves.Any())
                    {
                        DateTime enter = item.Reserves.OrderBy(x => x.Date).FirstOrDefault().Date;
                        DateTime exit = item.Reserves.OrderBy(x => x.Date).LastOrDefault().Date;
                        var days = (exit - enter).TotalDays;

                        if (days > 0)
                        {
                            var datelist = new List<DateTime>();
                            for (int i = 0; i < days; i++)
                            {
                                datelist.Add(enter.AddDays(i));
                            }
                            foreach (var day in datelist)
                            {
                                if (!item.Reserves.Any(x => x.Date == day))
                                {
                                    foliosWithIssue.Add(item);
                                }
                            }

                        }
                    }
                }
                ViewBag.foliosWithIssue = foliosWithIssue;
            }
            return View("test");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }
    }
}