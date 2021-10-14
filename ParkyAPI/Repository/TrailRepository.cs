using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;
        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateTrail(Trail trailNew)
        {
            _db.Trails.Add(trailNew);
            return Save();
        }

        public bool DeleteTrail(int trailId)
        {
            var trailInDb = _db.Trails.FirstOrDefault(c => c.Id == trailId);
            _db.Trails.Remove(trailInDb);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            return _db.Trails
                .Include(c => c.NationalPark).FirstOrDefault(c => c.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails
                .Include(c => c.NationalPark).OrderBy(c => c.Name).ToList();
        }

        public bool TrailExists(int trailId)
        {
            bool value = _db.Trails
                .Any(c => c.Id == trailId);
            return value;
        }

        public bool TrailExists(string trailName)
        {
            bool value = _db.Trails
                .Any(c => c.Name.ToLower().Trim() == trailName.ToLower().Trim());
            return value;
        }

        public bool UpdateTrail(Trail trailUpdate)
        {
            _db.Trails.Update(trailUpdate);
            return Save();
        }

        public ICollection<Trail> GetTrailsIsInNationalPark(int nationalParkId)
        {
            return _db.Trails
                .Include(c => c.NationalPark)
                .Where(c => c.NationalParkId == nationalParkId)
                .ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
