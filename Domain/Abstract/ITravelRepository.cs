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
        void UpdateExcursion(Excursion excursion);

        List<Excursion> GetExcursionsByGuideId(string guideId);
        
        IEnumerable<User> GetUsers();
        User GetUserById(string userId);
        void UpdateUser(User excursion);
        void DeleteUser(string userId);
        void Save();
    }
}
