using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Prototype.Data.Interface
{
    public interface IRepository<T> where T: IEntity
    {
        void Create(T entity);
        void Delete(string id);
        T GetById(string id);
        IList<T> GettAll();
        void Update(T entity);
    }

}
