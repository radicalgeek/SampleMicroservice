using System.Collections;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Prototype.Data.Interface;


namespace Prototype.Data
{
    /// <summary>
    /// A MongoDB repository. Maps to a _collection with the same name
    /// as type TEntity.
    /// </summary>
    /// <typeparam name="T">Entity type for this repository</typeparam>
    public abstract class MongoRepository<T, TKey> : IRepository<T, TKey>, IEnumerable where T : IEntity<TKey>
    {
        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected internal IMongoCollection<T> _collection;

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public MongoRepository()
            : this(Util<TKey>.GetDefaultConnectionString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public MongoRepository(string connectionString)
        {
            _collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the _collection to use.</param>
        public MongoRepository(string connectionString, string collectionName)
        {
            _collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString, collectionName);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        public MongoRepository(MongoUrl url)
        {
            _collection = Util<TKey>.GetCollectionFromUrl<T>(url);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the _collection to use.</param>
        public MongoRepository(MongoUrl url, string collectionName)
        {
            _collection = Util<TKey>.GetCollectionFromUrl<T>(url, collectionName);
        }

        /// <summary>
        /// Gets the Mongo _collection (to perform advanced operations).
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
        /// for most purposes you can use the MongoRepositoryManager&lt;T&gt;
        /// </remarks>
        /// <value>The Mongo _collection (to perform advanced operations).</value>
        public IMongoCollection<T> Collection
        {
            get { return _collection; }
        }

        /// <summary>
        /// Gets the name of the _collection
        /// </summary>
        public string CollectionName
        {
            get { return _collection.CollectionNamespace.CollectionName; }
        }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public async virtual Task<T> GetById(TKey id)
        {
            if (typeof(T).IsSubclassOf(typeof(Entity)))
            {
                return await GetById(new ObjectId(id as string));
            }

            var filter = Builders<T>.Filter.Eq("_id", BsonValue.Create(id)); 
            return await _collection.Find<T>(filter).FirstOrDefaultAsync(); 

        }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public async virtual Task<T> GetById(ObjectId id)
        {
            return await _collection.Find<T>(entity => id.Equals(entity.Id)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public async virtual Task<T> Add(T entity)
        {
            
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public async virtual Task Add(IEnumerable<T> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public async virtual Task<T> Update(T entity)
        {
             await _collection.ReplaceOneAsync<T>(x => entity.Id.Equals(x.Id), entity, new UpdateOptions { IsUpsert = true }); 
             return entity; 

        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public async virtual Task Update(IEnumerable<T> entities)
        {
            var tasks = entities.Select(async entity => 
             { 
                 await this.Update(entity); 
             }); 
             await Task.WhenAll(tasks); 

        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public async virtual Task Delete(TKey id)
        {
            if (typeof(T).IsSubclassOf(typeof(Entity))) 
             { 
                 await Delete(new ObjectId(id as string)); 
             } 
             else 
             { 
                 var filter = Builders<T>.Filter.Eq("_id", BsonValue.Create(id)); 
                 await _collection.DeleteOneAsync(filter); 
             } 

        }

        /// <summary>
        /// Deletes an entity from the repository by its ObjectId.
        /// </summary>
        /// <param name="id">The ObjectId of the entity.</param>
        public async virtual Task Delete(ObjectId id)
        {
            await _collection.DeleteOneAsync<T>(entity => id.Equals(entity.Id));
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public async virtual Task Delete(T entity)
        {
            await this.Delete(entity.Id);
        }

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public async virtual Task Delete(Expression<Func<T, bool>> predicate)
        {
            await _collection.DeleteManyAsync<T>(predicate);
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public async virtual Task DeleteAll()
        {
            await this.Delete(entity => true);
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the _collection.</returns>
        public async virtual Task<long> Count()
        {
            return await _collection.CountAsync<T>(entity => true);
        }

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        public async virtual Task<bool> Exists(Expression<Func<T, bool>> predicate)
        {
            var entity = await _collection.Find<T>(predicate).FirstOrDefaultAsync(); 
            return entity != null; 

        }

        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        /// <remarks>
        ///     <para>
        ///         Sometimes a series of operations needs to be performed on the same connection in order to guarantee correct
        ///         results. This is rarely the case, and most of the time there is no need to call RequestStart/RequestDone.
        ///         An example of when this might be necessary is when a series of Inserts are called in rapid succession with
        ///         SafeMode off, and you want to query that data in a consistent manner immediately thereafter (with SafeMode
        ///         off the writes can queue up at the server and might not be immediately visible to other connections). Using
        ///         RequestStart you can force a query to be on the same connection as the writes, so the query won't execute
        ///         until the server has caught up with the writes.
        ///     </para>
        ///     <para>
        ///         A thread can temporarily reserve a connection from the connection pool by using RequestStart and
        ///         RequestDone. You are free to use any other databases as well during the request. RequestStart increments a
        ///         counter (for this thread) and RequestDone decrements the counter. The connection that was reserved is not
        ///         actually returned to the connection pool until the count reaches zero again. This means that calls to
        ///         RequestStart/RequestDone can be nested and the right thing will happen.
        ///     </para>
        ///     <para>
        ///         Use the connectionstring to specify the readpreference; add "readPreference=X" where X is one of the following
        ///         values: primary, primaryPreferred, secondary, secondaryPreferred, nearest.
        ///         See http://docs.mongodb.org/manual/applications/replication/#read-preference
        ///     </para>
        /// </remarks>
        public virtual IDisposable RequestStart()
        {
            //return _collection.Database.RequestStart();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
        public virtual void RequestDone()
        {
            //_collection.Database.RequestDone();
            throw new NotImplementedException();
        }

        #region IQueryable<T>
        /// <summary>
        /// Returns an enumerator that iterates through a _collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the _collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            //return _collection.AsQueryable<T>().GetEnumerator();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a _collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the _collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            //return this._collection.AsQueryable<T>().GetEnumerator();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get {  throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

    }


}
