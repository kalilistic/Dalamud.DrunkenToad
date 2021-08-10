using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

using LiteDB;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Data Service to persist plugin data.
    /// </summary>
    public abstract class Repository
    {
        private readonly BsonMapper bsonMapper;
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository"/> class.
        /// </summary>
        /// <param name="pluginService">plugin base.</param>
        protected Repository(PluginService pluginService)
        {
            this.bsonMapper = BsonMapper();
            var dirPath = $"{pluginService.PluginFolder()}\\data";
            Directory.CreateDirectory(dirPath);
            this.connectionString = $"Filename={dirPath}\\data.db;connection=shared";
        }

        /// <summary>
        /// Insert item in collection.
        /// </summary>
        /// <param name="item">item to insert.</param>
        /// <typeparam name="T">collection.</typeparam>
        public void InsertItem<T>(T item)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            collection.Insert(item);
        }

        /// <summary>
        /// Insert items in collection.
        /// </summary>
        /// <param name="items">items to insert.</param>
        /// <typeparam name="T">collection.</typeparam>
        public void InsertItems<T>(IEnumerable<T> items)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            var enumerable = items as T[] ?? items.ToArray();
            collection.InsertBulk(enumerable, enumerable.Length);
        }

        /// <summary>
        /// Upsert items in collection.
        /// </summary>
        /// <param name="items">items to upsert.</param>
        /// <typeparam name="T">collection.</typeparam>
        public void UpsertItems<T>(IEnumerable<T> items)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            collection.Upsert(items);
        }

        /// <summary>
        /// Update item in collection.
        /// </summary>
        /// <param name="item">item to update.</param>
        /// <typeparam name="T">collection.</typeparam>
        /// <returns>indicator if successful.</returns>
        public bool UpdateItem<T>(T item)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            return collection.Update(item);
        }

        /// <summary>
        /// Delete item in collection.
        /// </summary>
        /// <param name="id">item id to delete.</param>
        /// <typeparam name="T">collection.</typeparam>
        /// <returns>indicator if item was deleted successfully.</returns>
        public bool DeleteItem<T>(int id)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            return collection.Delete(id);
        }

        /// <summary>
        /// Delete all items matching criteria.
        /// </summary>
        /// <param name="predicate">criteria to delete item.</param>
        /// <typeparam name="T">collection.</typeparam>
        /// <returns>number of documents deleted.</returns>
        public int DeleteItems<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            return collection.DeleteMany(predicate);
        }

        /// <summary>
        /// Get item by id.
        /// </summary>
        /// <param name="id">item id.</param>
        /// <typeparam name="T">collection.</typeparam>
        /// <returns>item.</returns>
        public T? GetItem<T>(int id)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            return collection.FindById(id);
        }

        /// <summary>
        /// Get first item found matching criteria.
        /// </summary>
        /// <param name="predicate">criteria to return item.</param>
        /// <typeparam name="T">collection.</typeparam>
        /// <returns>item.</returns>
        public T? GetItem<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return this.InternalGetItems(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Get all items.
        /// </summary>
        /// <typeparam name="T">collection.</typeparam>
        /// <returns>items.</returns>
        public IEnumerable<T> GetItems<T>()
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            var result = collection.Find(Query.All());
            return result.AsEnumerable();
        }

        /// <summary>
        /// Get all items matching criteria.
        /// </summary>
        /// <param name="predicate">criteria to return item.</param>
        /// <typeparam name="T">collection.</typeparam>
        /// <returns>items.</returns>
        public IEnumerable<T> GetItems<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return this.InternalGetItems(predicate);
        }

        /// <summary>
        /// Rebuilds index.
        /// </summary>
        /// <param name="predicate">expression to use.</param>
        /// <param name="unique">if field for index is unique.</param>
        /// <typeparam name="T">collection.</typeparam>
        public void RebuildIndex<T>(Expression<Func<T, object>> predicate, bool unique = false)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();
            collection.EnsureIndex(predicate, unique);
        }

        /// <summary>
        /// Rebuild database.
        /// </summary>
        public void RebuildDatabase()
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            db.Rebuild();
        }

        /// <summary>
        /// Get user version.
        /// </summary>
        /// <returns>user version.</returns>
        public int GetVersion()
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            return db.UserVersion;
        }

        /// <summary>
        /// Set user version.
        /// </summary>
        /// <param name="version">user version.</param>
        public void SetVersion(int version)
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper)
            {
                UserVersion = version,
            };
        }

        private static BsonMapper BsonMapper()
        {
            var bsonMapper = new BsonMapper
            {
                EmptyStringToNull = false,
                SerializeNullValues = true,
                EnumAsInteger = true,
            };
            bsonMapper.RegisterType(
                vector4 =>
                {
                    var doc = new BsonArray
                    {
                        new (vector4.X),
                        new (vector4.Y),
                        new (vector4.Z),
                        new (vector4.W),
                    };
                    return doc;
                },
                (doc) => new Vector4((float)doc[0].AsDouble, (float)doc[1].AsDouble, (float)doc[2].AsDouble, (float)doc[3].AsDouble));
            return bsonMapper;
        }

        private IEnumerable<T> InternalGetItems<T>(
            Expression<Func<T, bool>>? predicate)
            where T : class
        {
            using var db = new LiteDatabase(this.connectionString, this.bsonMapper);
            var collection = db.GetCollection<T>();

            var result = predicate != null
                             ? collection.Find(predicate)
                             : collection.Find(Query.All());

            return result.AsEnumerable();
        }
    }
}
