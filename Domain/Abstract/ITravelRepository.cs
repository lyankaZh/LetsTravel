using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Abstract
{
    public interface ITravelRepository:IDisposable
    {
        IEnumerable<Excursion> GetExcursions();
        Excursion GetExcursionById(int excursionId);
        void InsertExcursion(Excursion excursion);
        void DeleteExcursion(int excursionId);
        void DeleteBlockedUser(int blockedUserId);
        void UpdateExcursion(Excursion excursion);
        
        void InsertBlockedUser(BlockedUser blockedUser);
        
        IEnumerable<User> GetUsers();
        IEnumerable<BlockedUser> GetBlockedUsers();
        User GetUserById(string userId);
        void UpdateUser(User excursion);
        void DeleteUser(string userId);
        bool IsInRole(string roleName, User user);
        void Save();
    }
}
