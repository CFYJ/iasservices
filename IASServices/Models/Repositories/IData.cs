using System.Collections.Generic;
using System.Linq;

namespace IASServices.Repositories
{
    public interface IData<TEntity, U> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        TEntity Get(U id);
        int Add(TEntity entity);
        int Update(U id, TEntity entity);
        int Delete(U id);
    }
}
