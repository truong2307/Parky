using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext _db;
        public NationalParkRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateNationalPark(NationalPark nationalPark)
        {
            _db.NationalParks.Add(nationalPark);
            return Save();
        }

        public bool DeleteNationalPark(NationalPark nationalPark)
        {
            _db.NationalParks.Remove(nationalPark);
            return Save();
        }

        public NationalPark GetNationalPark(int nationalParkId)
        {
            return _db.NationalParks.FirstOrDefault(c => c.Id == nationalParkId);
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return _db.NationalParks.OrderBy(c => c.Name).ToList();
        }

        public bool NationalParkExists(string name)
        {
            bool value = _db.NationalParks
                .Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool NationalParkExists(int id)
        {
            bool value = _db.NationalParks.Any(c => c.Id == id);
            return value;
        }

        public bool UpdateNationalPark(NationalPark nationalPark)
        {
            var nationalParkInDb = _db.NationalParks.FirstOrDefault(c => c.Id == nationalPark.Id);
            if (nationalParkInDb == null)
            {
                return false;
            }

            nationalParkInDb.Name = nationalPark.Name;
            nationalParkInDb.State = nationalPark.State;
            nationalParkInDb.Created = nationalPark.Created;
            nationalParkInDb.Established = nationalPark.Established;
            nationalParkInDb.Picture = nationalPark.Picture;

            return Save();
        }
        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
