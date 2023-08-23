using hotelsimorgh.Data;
using hotelsimorgh.Data.DbModels;
using MD.PersianDateTime;
using Microsoft.AspNetCore.Mvc;
using hotelsimorgh.Utilities;
using hotelsimorgh.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.Xml;
using NuGet.Protocol;

namespace hotelsimorgh.Controllers
{
    [Authorize]
    public class ReserveController : Controller
    {
        private IConfiguration _configuration;
        public ReserveController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult AddorEdit(int id, DateTime dateValue, string returnUrl, int roomNumber = 0)
        {

            var defaultwebsite = _configuration.GetSection("SiteSetting").GetSection("DefaultWebsite").Value;
            using (UnitOfWork db = new UnitOfWork())
            {
                if (db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Bloked) return RedirectToAction("Logout", "User");
                ViewBag.websites = db.WebsiteRepository.Get().OrderBy(c => c.Sequence).ToList();
                ViewBag.folios = db.FolioRepository.Get(x=>x.Reserves.Count()==0)
                    .ToList().OrderByDescending(x => x.Id).ToList();
                ViewBag.rooms = db.RoomRepository.Get().OrderBy(c => c.Sequence).ToList();
                if (!string.IsNullOrEmpty(returnUrl))
                    ViewBag.returnUrl = returnUrl.Replace("$", "&");
                if (id == 0)
                {
                    if (dateValue.Year != 1)
                    {
                        ViewBag.date = dateValue.ToShamsi().ToStringPersianDateTime();
                    }
                    ViewBag.roomNumber = roomNumber;
                    return View();
                }
                else
                {
                    var reserve = db.ReserveRepository.GetByID(id);
                    var date = reserve.Date.ToShamsi().ToStringPersianDateTime();
                    ViewBag.date = date;
                    return View(reserve);
                }
            }
        }
        [HttpPost]
        public IActionResult AddOrEdit(Reserve reserve, string reserveDate, int numberOfDays, string reservePrice)
        {
            int _price = Convert.ToInt32(reservePrice.Replace(",", ""));
            if (_price < 100000 || _price > 4000000)
                return Json(new { result = false, message = "priceError" });
            reserve.Price = _price;
            if (numberOfDays > 10) return Json(new { result = false, message = "NumberOfDaysError" });

            using (UnitOfWork db = new UnitOfWork())
            {
                reserve.Date = reserveDate.toPersianDateTime().ToMiladi();
                var _reservesToInsert = new List<Reserve>();
                var user = db.UserRepository.Get(x => x.UserName == User.Identity.Name).First();
                if (reserve.Id == 0)
                {
                    for (int i = 0; i <= numberOfDays - 1; i++)
                    {
                        reserve.Date = reserveDate.toPersianDateTime().ToMiladi().AddDays(i);
                        reserve.Id = 0;
                        var IntractReserves = db.ReserveRepository.Get(c => c.Date.Date == reserve.Date && c.RoomId == reserve.RoomId, includeProperties: "Folio");
                        if (IntractReserves.Count() == 0)
                        {
                            _reservesToInsert.Add(new Reserve
                            {
                                Date = reserve.Date,
                                RoomId = reserve.RoomId,
                                FolioId = reserve.FolioId,
                                WebsiteId = reserve.WebsiteId,
                                Price = reserve.Price,
                                UserId = user.Id
                            });
                        }
                        else
                        {
                            var folioFullName = IntractReserves.First().Folio.Name+" "+ IntractReserves.First().Folio.Family + " ،کد:" + IntractReserves.First().FolioId;
                            if (i == 0 && IntractReserves.Count() == 1 && IntractReserves.First().HafDay)
                            {
                                _reservesToInsert.Add(new Reserve
                                {
                                    Date = reserve.Date,
                                    RoomId = reserve.RoomId,
                                    FolioId = reserve.FolioId,
                                    WebsiteId = reserve.WebsiteId,
                                    Price = reserve.Price,
                                    UserId = user.Id
                                });
                            }
                            else
                            {
                                return Json(new { result = false, message = folioFullName });
                            }
                        }
                    }
                    foreach (var item in _reservesToInsert)
                    {
                        item.SubmitDate = DateTime.Now;
                        db.ReserveRepository.Insert(item);
                        db.Save();
                    }
                    return Json(new { result = true, message = _reservesToInsert.First().FolioId.ToString() });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        public IActionResult HafReserve(int id, DateTime date, int roomNumber, string returnUrl)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                if (db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Bloked) return RedirectToAction("Logout", "User");

                ViewBag.folios = db.FolioRepository.Get(x => x.Reserves.Count() == 0);
                ViewBag.returnUrl = returnUrl.Replace("$", "&");
                ViewBag.date = date;
                ViewBag.room = roomNumber;
                var yesterdayReserve = db.ReserveRepository.Get(x => x.Date == date.AddDays(-1) && x.Room.Number == roomNumber, includeProperties: "Room,Folio").FirstOrDefault();
                ViewBag.yesterdayFolio = (yesterdayReserve == null) ?null:yesterdayReserve.Folio;
                return View();
            }
        }

        [HttpPost]
        public IActionResult HafReserve(Reserve reserve, string reservePrice, string DateString, int RoomNumber, string returnUrl)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var user = db.UserRepository.Get(x => x.UserName == User.Identity.Name).First();
                var date = DateString.toPersianDateTime().ToMiladi();
                int _price = Convert.ToInt32(reservePrice.Replace(",", ""));
                var reservesIntersection = db.ReserveRepository.Get(x => x.Date.Date == date.Date && x.Room.Number == RoomNumber, includeProperties: "Room");
                if (reservesIntersection.Any()) return Json($"intersection with folio: {reservesIntersection.First().FolioId}");
                reserve.HafDay = true;
                reserve.Price = _price;
                reserve.Date = date;
                reserve.SubmitDate = DateTime.Now;
                reserve.RoomId = db.RoomRepository.Get(x => x.Number == RoomNumber).First().Id;
                reserve.WebsiteId = 0;
                reserve.UserId = user.Id;
                db.ReserveRepository.Insert(reserve);
                db.Save();
            }
            return RedirectToAction("Index", "Home", new { folioId = reserve.FolioId });
        }

        public IActionResult ReservesTable(string dateRange)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                if (db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Bloked) return RedirectToAction("Logout", "User");
                int defaultDays = Convert.ToInt32(db.SiteSettingRepository.Get(x => x.key == "NumberOfDaysToShowInReserveTable").First().Value);

                DateTime fromDate = dateRange.getFromDate(-6);
                DateTime toDate = dateRange.getUntilDate(defaultDays);

                int numberOfDays = (int)(toDate - fromDate).TotalDays;
                var reserves = db.ReserveRepository.Get(x => x.Date.Date >= fromDate && x.Date.Date <= toDate, includeProperties: "Room,Folio");
                List<ReserveRow> model = new List<ReserveRow>();
                DateTime now = "1402/05/17".toPersianDateTime().ToMiladi();
                var rooms = db.RoomRepository.Get();
                for (int i = 0; i <= numberOfDays; i++)
                {
                    List<RoomModel> fullRooms = new List<RoomModel>();
                    var date = fromDate.AddDays(i);
                    foreach (var item in rooms)
                    {
                        var res = reserves.FirstOrDefault(x => x.Date.Date == date.Date && x.Room.Number == item.Number && !x.HafDay);
                        var hafRes = reserves.FirstOrDefault(x => x.Date.Date == date.Date && x.Room.Number == item.Number && x.HafDay);
                        ReserveHelper helper = new ReserveHelper();
                        RoomModel roomModel = new RoomModel();
                        if (res is not null)
                        {
                            roomModel.hafDay = false;

                            roomModel.status = helper.statusReserve(res);
                            roomModel.number = item.Number;
                            roomModel.folioId = res.FolioId;
                            fullRooms.Add(roomModel);
                        }
                        else if (hafRes is not null)
                        {
                            roomModel.hafDay = true;

                            roomModel.status = helper.statusReserve(hafRes);

                            roomModel.number = item.Number;
                            roomModel.folioId = hafRes.FolioId;
                            fullRooms.Add(roomModel);
                        }
                    }
                    model.Add(new ReserveRow()
                    {
                        rooms = fullRooms,
                        Date = date,
                    });
                }



                List<Reserve> leaveToday = new List<Reserve>();
                List<Reserve> leaveTomorrow = new List<Reserve>();
                var ReservesToleave = db.ReserveRepository.Get(x => x.Date >= now.AddDays(-1) && x.Date <= now.Date.AddDays(1), includeProperties: "Room,Folio");
                foreach (var item in rooms)
                {
                    var yesterdayRes = ReservesToleave.Where(x => x.Date.Date == now.AddDays(-1) && !x.HafDay && x.Room.Number == item.Number).FirstOrDefault();
                    int FyesterdayRes = yesterdayRes == null ? 0 : yesterdayRes.FolioId;

                    var HtodayRes = ReservesToleave.Where(x => x.Date.Date == now && x.HafDay && x.Room.Number == item.Number).FirstOrDefault();
                    int FHtodayRes = HtodayRes == null ? 0 : HtodayRes.FolioId;

                    var todayRes = ReservesToleave.Where(x => x.Date.Date == now && !x.HafDay && x.Room.Number == item.Number).FirstOrDefault();
                    int FtodayRes = todayRes == null ? 0 : todayRes.FolioId;

                    var tomorowwRes = ReservesToleave.Where(x => x.Date.Date == now.AddDays(1) && x.Room.Number == item.Number).FirstOrDefault();

                    if (FyesterdayRes != 0 || FHtodayRes != 0)
                    {
                        if (FyesterdayRes != FtodayRes && FHtodayRes != FyesterdayRes)
                        {
                            if (yesterdayRes != null)
                            {
                                leaveToday.Add(yesterdayRes);
                            }
                        }

                        if (FHtodayRes != 0)
                        {
                            leaveToday.Add(HtodayRes);
                        }

                    }

                    if (todayRes != null)
                    {
                        if (tomorowwRes != null)
                        {
                            if (todayRes.FolioId != tomorowwRes.FolioId)
                            {
                                leaveTomorrow.Add(todayRes);
                            }
                        }
                        else
                        {
                            leaveTomorrow.Add(todayRes);
                        }
                    }

                    if (tomorowwRes != null)
                    {
                        if (tomorowwRes.HafDay)
                        {
                            leaveTomorrow.Add(tomorowwRes);
                        }
                    }
                }




                ViewBag.leaveToday = leaveToday;
                //ViewBag.checkAlert = db.AlertCheckRepository.Get(x => x.date >= now.AddDays(-1) && x.date <= now.AddDays(1));

                ViewBag.leaveTomorrow = leaveTomorrow;
                ViewBag.Date = dateRange ?? fromDate.ToShamsi().ToStringPersianDateTime() + " - " + toDate.ToShamsi().ToStringPersianDateTime();
                ViewBag.rooms = rooms.OrderBy(x => x.bed).ThenBy(x => x.Capacity).ToList();
                ViewBag.returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString.ToString().Replace("&", "$");
                ViewBag.alert = db.AlertCheckRepository.Get(x => x.date.Date >= now.AddDays(-1) && x.date.Date <= now.AddDays(1)).ToList();
                ViewBag.HafDayLeaveHour = db.SiteSettingRepository.Get(x => x.key == "hafdayLeaveHour").First().Value;

                return View(model);
            }
        }

        [HttpPost]
        public IActionResult DeleteAllReservesByFolioId(int folioId)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var reserves = db.ReserveRepository.Get(x => x.FolioId == folioId);
                foreach (var item in reserves)
                {
                    db.ReserveRepository.Delete(item.Id);
                    db.Save();
                }
                foreach (var item in reserves)
                {
                    var folio = db.FolioRepository.GetByID(item.FolioId);
                    db.DeletedReserveRepository.Insert(new DeletedReserve()
                    {
                        datetime = DateTime.Now,
                        folioId = item.FolioId,
                        HafDay = item.HafDay,
                        Price = item.Price,
                        ReserveDate = item.Date,
                        RoomNumber = db.RoomRepository.GetByID(item.RoomId).Number,
                        UserId = db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Id,
                        websiteId = item.WebsiteId,
                        folioName = $"{folio.Name} {folio.Family}",
                        Mobile = folio.Mobile
                    });
                    db.Save();
                }
            }
            return Json("");
        }

        [HttpPost]
        public IActionResult DeleteReserve(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var item = db.ReserveRepository.GetByID(id);

                db.ReserveRepository.Delete(id);
                db.Save();
                var folio = db.FolioRepository.GetByID(item.FolioId);
                db.DeletedReserveRepository.Insert(new DeletedReserve()
                {
                    datetime = DateTime.Now,
                    folioId = item.FolioId,
                    HafDay = item.HafDay,
                    Price = item.Price,
                    ReserveDate = item.Date,
                    RoomNumber = db.RoomRepository.GetByID(item.RoomId).Number,
                    UserId = db.UserRepository.Get(x => x.UserName == User.Identity.Name).First().Id,
                    websiteId = item.WebsiteId,
                    folioName = $"{folio.Name} {folio.Family}",
                    Mobile = folio.Mobile
                });
                db.Save();
                return Json(true);
            }
        }

        [HttpPost]
        public IActionResult CalculateIncomeInDateRange(string dateRange)
        {
            if (dateRange != null)
            {
                var from = dateRange.Split('-')[0].Trim().toPersianDateTime().ToMiladi();
                var to = dateRange.Split('-')[1].Trim().toPersianDateTime().ToMiladi();
                using (UnitOfWork db = new UnitOfWork())
                {
                    var reserves = db.ReserveRepository.Get(x => x.Date >= from && x.Date <= to && !x.HafDay);
                    var hafDay = db.ReserveRepository.Get(x => x.Date >= from && x.Date <= to && x.HafDay).OrderBy(x => x.Date);
                    int totalIncomeReserve = 0;
                    int totalIncomeHafDay = 0;
                    foreach (var item in reserves)
                    {
                        totalIncomeReserve += item.Price;
                    }
                    foreach (var item in hafDay)
                    {
                        totalIncomeHafDay += item.Price;
                    }
                    return Json("کل‌درآمد‌:" + (totalIncomeHafDay + totalIncomeReserve).ToString() + "         تعداد‌رزرو:" + reserves.Count() + "         ‌تعداد‌رزرو‌نیمروز:" + hafDay.Count());
                }
            }
            else
            {
                return Json(false);
            }

        }

        [HttpPost]
        public IActionResult CheckIfReservesHaveSameRoom(int id)
        {
            ReserveHelper helper = new ReserveHelper();
            var resutl = helper.CheckIfReservesHaveSameRoom(id);
            return Json(resutl);
        }
        [HttpPost]
        public IActionResult alertCheck(int roomNumber, DateTime date)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var result = db.AlertCheckRepository.Get(x => x.date == date && x.roomNumber == roomNumber).FirstOrDefault();
                if (result == null)
                {
                    db.AlertCheckRepository.Insert(new AlertCheck() { roomNumber = roomNumber, date = date });
                }
                else
                {
                    db.AlertCheckRepository.Delete(result);
                }
                db.Save();
            }
            return Json("");
        }

    }
}

//Stopwatch stopwatch = new Stopwatch();
//stopwatch.Start();
//stopwatch.Stop();
//TimeSpan executionTime = stopwatch.Elapsed;
//Console.WriteLine($"Execution time: {executionTime.TotalSeconds} seconds");