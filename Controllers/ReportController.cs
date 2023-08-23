using hotelsimorgh.Data;
using hotelsimorgh.Data.DbModels;
using hotelsimorgh.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace hotelsimorgh.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        public IActionResult Reserve(string dateRange, string SubmitDate, int minPrice, int maxPrice)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                ViewBag.receptionists = db.UserRepository.Get();
                ViewBag.websites = db.WebsiteRepository.Get();
                if (dateRange != null)
                {
                    DateTime from = dateRange.getFromDate();
                    DateTime to = dateRange.getUntilDate(10);
                    var res = db.ReserveRepository.Get(x => x.Date.Date >= from && x.Date.Date <= to, includeProperties: "Folio,Room,User").OrderBy(x => x.Date).ThenBy(x => x.Room.Number) as IEnumerable<Reserve>;
                    if (minPrice != 0)
                    {
                        res = res.Where(x => x.Price >= minPrice);
                    }
                    if (maxPrice != 0)
                    {
                        res = res.Where(x => x.Price <= maxPrice);
                    }
                    ViewBag.Date = from.ToShamsi().ToStringPersianDateTime() + " - " + to.ToShamsi().ToStringPersianDateTime();
                    ViewBag.minPrice = minPrice;
                    ViewBag.maxPrice = maxPrice;
                    return View(res);
                }
                else if (SubmitDate != null)
                {
                    DateTime from = SubmitDate.getFromDate(-2);
                    DateTime to = SubmitDate.getUntilDate(0);
                    var res = db.ReserveRepository.Get(x => x.SubmitDate.Date >= from && x.SubmitDate.Date <= to, orderBy: x => x.OrderByDescending(x => x.SubmitDate), includeProperties: "Folio,Room,User");
                    ViewBag.SubmitDate =SubmitDate??  from.ToShamsi().ToStringPersianDateTime() + " - " + to.ToShamsi().ToStringPersianDateTime();
                    return View(res);
                }
                else
                {
                    return View(new List<Reserve>());
                }
            }
        }
        public IActionResult Passengers(string dateRange, string nationalCode, string text, int folioId)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                List<Folio> folios = new List<Folio>();
                if (folioId != 0)
                {
                    var _folios = db.FolioRepository.Get(x => x.Id == folioId, includeProperties: "Reserves,passengers");
                    var folio = (_folios.Count() == 0) ? new Folio() : _folios.First();
                    folios.Add(folio);
                    ViewBag.folioId = folioId;
                }
                else
                {
                    if (dateRange is not null)
                    {
                        ViewBag.Date = dateRange;
                        DateTime from = dateRange.getFromDate();
                        DateTime to = dateRange.getUntilDate(10);
                        folios = db.FolioRepository.Get(x => x.Reserves.OrderBy(x => x.Date).First().Date >= from && x.Reserves.OrderBy(x => x.Date).First().Date <= to && !x.PreReserve, includeProperties: "Reserves,passengers").OrderBy(x => x.Reserves.OrderBy(x => x.Date).First().Date).ToList();

                    }
                    if (nationalCode is not null)
                    {
                        if (folios.Count() > 0)
                        {

                            folios = folios.Where(x => x.NationalCode == nationalCode || x.passengers.Where(x => x.NationalCode == nationalCode).Count() > 0).ToList();
                        }
                        else
                        {
                            folios = db.FolioRepository.Get(x => x.NationalCode == nationalCode || x.passengers.Where(x => x.NationalCode == nationalCode).Count() > 0, includeProperties: "Reserves,passengers").ToList();
                        }
                        ViewBag.NationalCode = nationalCode;
                    }
                    if (text is not null)
                    {
                        text = text.Trim();
                        if (folios.Count() > 0)
                        {
                            folios = folios.Where(x => x.Name.Contains(text) || x.Family.Contains(text) ||
                            x.passengers.Where(x => x.Name.Contains(text) || x.Family.Contains(text)).Count() > 0).ToList();
                        }
                        else
                        {
                            folios = db.FolioRepository.Get(x => x.Name.Contains(text) || x.Family.Contains(text) ||
                              x.passengers.Where(x => x.Name.Contains(text) || x.Family.Contains(text)).Count() > 0, includeProperties: "Reserves,passengers").ToList();
                        }
                        ViewBag.Text = text;
                    }
                }


                return View(folios);
            }
        }
        public IActionResult DeletedReserves(string dateRange)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
            DateTime fromDate = dateRange.getFromDate().AddDays(-Convert.ToInt32(db.SiteSettingRepository.Get(x=>x.key== "NumberOfDeletedReservesToShow").FirstOrDefault().Value));
            DateTime toDate = dateRange.getUntilDate(1);
                var rows = db.DeletedReserveRepository.Get(x => x.datetime >= fromDate && x.datetime <= toDate, includeProperties: "User").OrderByDescending(x => x.datetime);
                ViewBag.websites = db.WebsiteRepository.Get();
                return View(rows);
            }
        }
    }
}
