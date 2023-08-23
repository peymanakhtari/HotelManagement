using hotelsimorgh.Data;
using hotelsimorgh.Data.DbModels;

namespace hotelsimorgh.Utilities
{
    public class ReserveHelper
    {
        public bool CheckIfReservesHaveSameRoom(int folioId)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var reserves = db.ReserveRepository.Get(x => x.FolioId == folioId, includeProperties: "Room");
                if (reserves is not null)
                {
                    var roomNumber = reserves.First().Room.Number;
                    foreach (var item in reserves)
                    {
                        if (item.Room.Number != roomNumber)
                        {
                            return false;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    return true;

                }
                else
                {
                    return true;
                }
            }
        }

        public int statusReserve(Reserve reserve)
        {
            var folioLastReserve = reserve.Folio.Reserves.OrderBy(x => x.Date).Last();

            var now = "1402/05/17".toPersianDateTime().ToMiladi();

            if (reserve.Folio.PreReserve)
            {
                return 3;
            }
            else if (folioLastReserve.Date < now)
            {
                if (!folioLastReserve.HafDay)
                {
                    if (folioLastReserve.Date.Date < now.AddDays(-1))
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 2;
            }
        }

        public List<int> capacities(List<Room> rooms)
        {
            List<int> capacities = new List<int>();
            int capacity = 0;
            foreach (var item in rooms.OrderBy(x => x.Capacity))
            {
                if (item.Capacity != capacity)
                {
                    capacities.Add(item.Capacity);
                    capacity = item.Capacity;
                }
            }
            return capacities;
        }
        public List<int> bedtypes(List<Room> rooms)
        {
            List<int> bedtypes = new List<int>();
            int bedtype = 0;
            foreach (var item in rooms.OrderBy(x => x.bed))
            {
                if (item.bed != bedtype)
                {
                    bedtypes.Add(item.bed);
                    bedtype = item.bed;
                }
            }
            return bedtypes;
        }

        public string ChangeFolioBedNumberLinks(IEnumerable<Reserve> reserves, List<Room> rooms)
        {
            string beds = "";
            int bedNumber = 0;

            foreach (var item in reserves.OrderBy(x => x.Room.Number))
            {
                int roomNumber = rooms.FirstOrDefault(x => x.Id == item.RoomId).Number;
                if (bedNumber != roomNumber)
                {
                    bedNumber = roomNumber;
                    if (beds == "")
                    {
                        beds = $"<a class='text-dark' href='/Folio/ChangeRoom?folioId={item.FolioId}&roomId={item.RoomId}'>{roomNumber.ToString()}</a>";
                    }
                    else
                    {
                        beds += " , " + $"<a class='text-dark' href='/Folio/ChangeRoom?folioId={item.FolioId}&roomId={item.RoomId}'>{roomNumber.ToString()}</a>";
                    }
                }
            }
            return beds;
        }
    }
}
