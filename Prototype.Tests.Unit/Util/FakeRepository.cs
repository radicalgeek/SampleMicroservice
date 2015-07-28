using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoRepository;
using Prototype.Logic.DataEntities;

namespace Prototype.Tests.Unit
{
    public class FakeRepository : IRepository<SampleEntity>
    {
        
        public List<SampleEntity> Entities = new List<SampleEntity>();
        public IEnumerator<SampleEntity> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
        public IQueryProvider Provider { get; private set; }
        public SampleEntity GetById(string id)
        {
            return Entities.First(e => e.Id == id);
        }

        public SampleEntity Add(SampleEntity entity)
        {
            Entities.Add(entity);
            
            return entity;
        }

        public void Add(IEnumerable<SampleEntity> entities)
        {
           
            Entities.AddRange(entities);
        }

        public SampleEntity Update(SampleEntity entity)
        {
            var existingEntity = Entities.Single(e => e.Id == entity.Id);
            existingEntity = entity;
            return existingEntity;

        }

        public void Update(IEnumerable<SampleEntity> entities)
        {
            
            foreach (var sampleEntity in entities)
            {
                var existingEntity = Entities.Single(e => e.Id == sampleEntity.Id);
                Entities.Remove(existingEntity);
                Entities.Add(sampleEntity);
                
            }
        }

        public void Delete(string id)
        {
            var entityToRemove = Entities.Single(e => e.Id == id);
            Entities.Remove(entityToRemove);
        }

        public void Delete(SampleEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<SampleEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<SampleEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IDisposable RequestStart()
        {
            throw new NotImplementedException();
        }

        public void RequestDone()
        {
            throw new NotImplementedException();
        }

        public MongoCollection<SampleEntity> Collection { get;  set; }
    }
}