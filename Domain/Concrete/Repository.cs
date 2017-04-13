using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Entities;

namespace Domain.Concrete
{
    public class ExcursionRepository : IExcursionRepository
    {

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Excursion> GetExcursions()
        {
            throw new NotImplementedException();
        }

        public Excursion GetExcursionById(int excursionId)
        {
            throw new NotImplementedException();
        }

        public void InsertExcursion(Excursion excursion)
        {
            throw new NotImplementedException();
        }

        public void DeleteExcursion(int excursionId)
        {
            throw new NotImplementedException();
        }

        public void UpdateExcursion(Excursion excursion)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
