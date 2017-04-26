using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using Domain.Abstract;
using Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Concrete
{
    public class TravelRepository : ITravelRepository
    {
        private TravelDbContext context;
        private bool disposed = false;

        public TravelRepository(TravelDbContext context)
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
        
        public List<User> GetSubscribersByExcursionId(int excursionId, string guideId)
        {
            var res = new List<User> ();
            var query = from user in context.Users
                        where user.Id != guideId
                        select user;            
            List<IdentityUser> notGuidUsers = query.ToList();
            foreach (IdentityUser user in notGuidUsers)
            {
              if (((User)user).Excursions.Contains(GetExcursionById(excursionId)))
                {
                    res.Add((User)user);
                }                 
            }
            return res;
        }
        //public List<Excursion> GetExcursionsByTravellerId(string travellerId)
        //{
        //    var query = from user in context.Users
        //                where user.Id == guideId
        //                select excursion;
        //    return query.ToList();
        //}


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

        public void DeleteUser(string userId)
        {
            var userToDelete = context.Users.Find(userId);
            if (userToDelete != null)
            {
                context.Users.Remove(userToDelete);
            }
        }

        public void UpdateExcursion(Excursion excursion)
        {
           context.Entry(excursion).State = EntityState.Modified;
        }

        public void UpdateUser(User user)
        {
            context.Entry(user).State = EntityState.Modified;
        }

        public IEnumerable<IdentityUser> GetUsers()
        {
            var users = from user in context.Users select  user;
            return users.ToList();
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
