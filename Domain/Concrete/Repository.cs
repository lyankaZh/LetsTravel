using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstract;

namespace Domain.Concrete
{
    public class Repository<TEntity> : IRepository where TEntity : class
    {

    }
}
