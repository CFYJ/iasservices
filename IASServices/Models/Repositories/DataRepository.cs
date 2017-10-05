using SyncWebIAS.Model;
using System.Collections.Generic;
using System.Linq;

namespace IASServices.Repositories
{
    public class DelegacjaRepository : IData<Delegacja, long>
    {
        private readonly DelegacjaContext _dbContext;
        public DelegacjaRepository(DelegacjaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int Add(Delegacja entity)
        {
            _dbContext.Add(entity);
            return _dbContext.SaveChanges();
        }

        public int Delete(long id)
        {
            int res = 0;
            var delegacja = _dbContext.Delegacja.FirstOrDefault(d => d.Id == id);
            if (delegacja != null)
            {
                _dbContext.Delegacja.Remove(delegacja);
                res = _dbContext.SaveChanges();
            }
            return res;
        }

        public Delegacja Get(long id)
        {
            return _dbContext.Delegacja.FirstOrDefault(d => d.Id == id);
        }

        public IQueryable<Delegacja> GetAll()
        {
            return _dbContext.Delegacja.Take(2500).OrderByDescending(i=>i.Id);//.ToList();
        }

        public int Update(long id, Delegacja entity)
        {
            var res = 0;
            var delegacja = _dbContext.Delegacja.Find(id);
            if (delegacja!=null)
            {
                delegacja.Cel = entity.Cel;
                delegacja.CzasDo = entity.CzasDo;
                delegacja.CzasOd = entity.CzasOd;
                delegacja.DataWystawienia = entity.DataWystawienia;
                delegacja.Delegowany = entity.Delegowany;
                delegacja.IdDelegowanego = entity.IdDelegowanego;
                delegacja.IdWystawcy = entity.IdWystawcy;
                delegacja.Miejscowosc = entity.Miejscowosc;
                delegacja.Nr = entity.Nr;
                delegacja.Srodek = entity.Srodek;
                delegacja.Wydzial = entity.Wydzial;
                delegacja.Wystawil = entity.Wystawil;

                res = _dbContext.SaveChanges();
            }
            return res;
        }
    }
}
