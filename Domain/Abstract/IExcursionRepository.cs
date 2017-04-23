using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Abstract
{
    public interface IExcursionRepository:IDisposable
    {
        IEnumerable<Excursion> GetExcursions();
        Excursion GetExcursionById(int excursionId);
        void InsertExcursion(Excursion excursion);
        void DeleteExcursion(int excursionId);
        void UpdateExcursion(Excursion excursion);
        void UpdateUser(User excursion);
        List<Excursion> GetExcursionsByGuideId(string guideId);
        IEnumerable<IdentityUser> GetUsers();
        IdentityUser GetUserById(string userId);
        void Save();
    }
}
