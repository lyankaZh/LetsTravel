using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Domain.Abstract;
using Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Concrete
{
    public class ExcursionRepository : IExcursionRepository
    {
        private TravelDbContext context;
        private bool disposed = false;

        public ExcursionRepository(TravelDbContext context)
        {
            this.context = context;
        }

        public void Dispose(bool disposing)
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

        public IEnumerable<Excursion> GetExcursions()
        {
            return context.Excursions.ToList();
        }

        public Excursion GetExcursionById(int excursionId)
        {
            return context.Excursions.Find(excursionId);
        }

        public List<Excursion> GetExcursionsByGuideId(string guideId)
        {
            var query = from excursion in context.Excursions
                        where excursion.Guide == guideId
                        select excursion;
            return query.ToList();
        }

        public void InsertExcursion(Excursion excursion)
        {
             context.Excursions.Add(excursion);
        }

        public void DeleteExcursion(int excursionId)
        {
            Excursion excursionToDelete = context.Excursions.Find(excursionId);
            if (excursionToDelete != null)
            {
                context.Excursions.Remove(excursionToDelete);
            }          
        }

        public void UpdateExcursion(Excursion excursion)
        {
           context.Entry(excursion).State = EntityState.Modified;
        }

        public IEnumerable<IdentityUser> GetUsers()
        {
            return context.Users;
        }

        public IdentityUser GetUserById(string userId)
        {
            var query = from user in context.Users
                        where user.Id == userId
                        select user;
            return query.FirstOrDefault();
        } 

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
