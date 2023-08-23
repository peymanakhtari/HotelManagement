using hotelsimorgh.Data.DbModels;

namespace hotelsimorgh.Data
{
    public class UnitOfWork : IDisposable
    {
        private AppDbContext context=new AppDbContext();
        private GenericRepository<Folio> folioRepository;
        private GenericRepository<Reserve> reserveRepository;
        private GenericRepository<passenger> passengerRepository;
        private GenericRepository<Website> websiteRepository;
        //private GenericRepository<receptionist> receptionistRepository;
        private GenericRepository<Room> roomRepository;
        private GenericRepository<SiteSetting> siteSettingRepository;
        private GenericRepository<User> userRepository;
        private GenericRepository<DeletedReserve> deletedReserveRepository;
        private GenericRepository<UserLogin> userLoginRepository;
        private GenericRepository<AlertCheck> alertCheckRepository;
        public GenericRepository<AlertCheck> AlertCheckRepository
        {
            get
            {

                if (this.alertCheckRepository == null)
                {
                    this.alertCheckRepository = new GenericRepository<AlertCheck>(context);
                }
                return alertCheckRepository;
            }
        }

        public GenericRepository<UserLogin> UserLoginRepository
        {
            get
            {

                if (this.userLoginRepository == null)
                {
                    this.userLoginRepository = new GenericRepository<UserLogin>(context);
                }
                return userLoginRepository;
            }
        }
        public GenericRepository<DeletedReserve> DeletedReserveRepository
        {
            get
            {

                if (this.deletedReserveRepository == null)
                {
                    this.deletedReserveRepository = new GenericRepository<DeletedReserve>(context);
                }
                return deletedReserveRepository;
            }
        }
        public GenericRepository<User> UserRepository
        {
            get
            {

                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(context);
                }
                return userRepository;
            }
        }
        public GenericRepository<SiteSetting> SiteSettingRepository
        {
            get
            {

                if (this.siteSettingRepository == null)
                {
                    this.siteSettingRepository = new GenericRepository<SiteSetting>(context);
                }
                return siteSettingRepository;
            }
        }
        public GenericRepository<Room> RoomRepository
        {
            get
            {

                if (this.roomRepository == null)
                {
                    this.roomRepository = new GenericRepository<Room>(context);
                }
                return roomRepository;
            }
        }
        public GenericRepository<Folio> FolioRepository
        {
            get
            {

                if (this.folioRepository == null)
                {
                    this.folioRepository = new GenericRepository<Folio>(context);
                }
                return folioRepository;
            }
        }
        public GenericRepository<Reserve> ReserveRepository
        {
            get
            {

                if (this.reserveRepository == null)
                {
                    this.reserveRepository  = new GenericRepository<Reserve>(context);
                }
                return reserveRepository;
            }
        }
        public GenericRepository<passenger> PassengerRepository
        {
            get
            {

                if (this.passengerRepository == null)
                {
                    this.passengerRepository = new GenericRepository<passenger>(context);
                }
                return passengerRepository;
            }
        }
        public GenericRepository<Website> WebsiteRepository
        {
            get
            {

                if (this.websiteRepository == null)
                {
                    this.websiteRepository = new GenericRepository<Website>(context);
                }
                return websiteRepository;
            }
        }
        //public GenericRepository<receptionist> ReceptionistRepository
        //{
        //    get
        //    {

        //        if (this.receptionistRepository == null)
        //        {
        //            this.receptionistRepository = new GenericRepository<receptionist>(context);
        //        }
        //        return receptionistRepository;
        //    }
        //}

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
