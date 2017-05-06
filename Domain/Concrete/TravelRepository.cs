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

        //public List<Excursion> GetExcursionsByGuideId(string guideId)
        //{
        //    var query = from excursion in context.Excursions
        //                where excursion.Guide == guideId
        //                select excursion;
        //    return query.ToList();
        //}

        public bool IsInRole(string roleName, User user)
        {
            var role = context.Roles.First(x => x.Name == roleName);
            if (role != null)
            {
                var roleId = role.Id;
                var amountOfUserRolesWithSuchId = (from r in user.Roles
                                                   where r.RoleId == roleId
                                                   select role).Count();
                return amountOfUserRolesWithSuchId != 0;
            }
            return false;

        }

    public void InsertExcursion(Excursion excursion)
    {
        context.Excursions.Add(excursion);
    }

    public void InsertBlockedUser(BlockedUser blockedUser)
    {
        context.BlockedUsers.Add(blockedUser);
    }

    public IEnumerable<BlockedUser> GetBlockedUsers()
    {
        return context.BlockedUsers.ToList();
    }
    public void DeleteExcursion(int excursionId)
    {
        Excursion excursionToDelete = context.Excursions.Find(excursionId);
        if (excursionToDelete != null)
        {
            context.Excursions.Remove(excursionToDelete);
        }
    }

    public void DeleteBlockedUser(int blockedUserId)
    {
        BlockedUser userToDelete = context.BlockedUsers.Find(blockedUserId);
        if (userToDelete != null)
        {
            context.BlockedUsers.Remove(userToDelete);
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

    public IEnumerable<User> GetUsers()
    {
        var users = from user in context.Users select user;
        return users.ToList();
    }

    public User GetUserById(string userId)
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
