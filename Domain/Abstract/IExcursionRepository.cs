using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Abstract
{
    public interface IExcursionRepository:IDisposable
    {
        IEnumerable<Excursion> GetExcursions();
        Excursion GetExcursionById(int excursionId);
        void InsertExcursion(Excursion excursion);
        void DeleteExcursion(int excursionId);
        void UpdateExcursion(Excursion excursion);
        void Save();
    }
}
