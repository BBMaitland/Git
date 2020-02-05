namespace Entrant.Dals
{
    using log4net;
    using Models;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// A data access layer for persisting <see cref="Entrant.Models.Entrant"/> objects
    /// </summary>
    public class EntrantDal : IEntrantDal
    {
        /// <summary>
        /// A looger object
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// A dictionary used to store <see cref="Entrant.Models.Entrant"/> objects
        /// </summary>
        private readonly ConcurrentDictionary<int, Entrant> _entrantMap;

        /// <summary>
        /// THe id of the last entrant created
        /// </summary>
        private int _lastId;
        /// <summary>
        /// A constuctor for the DAL
        /// </summary>
        /// <param name="entrantMap">A store of <see cref="Entrant.Models.Entrant"/> objects</param>
        public EntrantDal(ConcurrentDictionary<int, Entrant> entrantMap = null)
        {
            _entrantMap = entrantMap ?? new ConcurrentDictionary<int, Entrant>();

            if (_entrantMap.Count() > 0)
            {
                _lastId = _entrantMap.Keys.Max();
            }
            else
            {
                _lastId = 0;
            }
        }

        /// <summary>
        /// Adds a new  <see cref="Entrant.Models.Entrant"/> object to the store 
        /// </summary>
        /// <param name="entrant">A new entrant to store</param>
        /// <returns>returns the new stored entrant object with a unique Id.</returns>
        /// <remarks>The entrants first and last names must be supplied.</remarks>
        public Entrant Create(Entrant entrant)
        {
            if (entrant == null)
            {
                throw new ArgumentNullException(nameof(entrant));
            }

            if (string.IsNullOrWhiteSpace(entrant.FirstName))
            {
                throw new ArgumentException($"{nameof(entrant.FirstName)} can not be blank", nameof(entrant.FirstName));
            }

            if (string.IsNullOrWhiteSpace(entrant.LastName))
            {
                throw new ArgumentException($"{nameof(entrant.LastName)} can not be blank", nameof(entrant.LastName));
            }

            entrant.Id = Interlocked.Increment(ref _lastId);

            bool added = _entrantMap.TryAdd(entrant.Id, entrant);

            if(! added)
            {
                var msg = "Create new Entrant failed";
                Logger.Error(msg);
                throw new Exception(msg);
            }

            Logger.Info($"Created new Entrant id = {entrant.Id}");

            return entrant;
        }

        /// <summary>
        /// Removes the entrant with the matching Id from the store
        /// </summary>
        /// <param name="id">The unique Id of the entrant to remove.</param>
        /// <exception cref="EntrantNotFoundException">If an entrant with a matching id is not found then a EntrantNotFoundException is thrown.</exception>
        public void Delete(int id)
        {
            bool deleted = false;

            if (_entrantMap.ContainsKey(id))
            {
                if (_entrantMap.TryGetValue(id, out var entrant))
                {
                    deleted = _entrantMap.TryRemove(id, out var _);
                }
            }

            if (deleted)
            {
                Logger.Info($"Deleted Entrant id = {id}");
            }
            else
            {
                Logger.Info($"Delete failed. Entrant id = {id} not found.");
                throw new EntrantNotFoundException($"Entrant with Id = {id} not found.");
            }
        }

        /// <summary>
        /// Returns a collection of all entrants in the store, otherwise an empty collection is returned.
        /// </summary>
        /// <returns>collection of <see cref="Entrant.Models.Entrant"/> objects</returns>
        public IEnumerable<Entrant> GetAll()
        {
            return _entrantMap.Values.ToList(); 
        }

        /// <summary>
        /// Returns the entrant with specified unique Id
        /// </summary>
        /// <param name="id">The Id of the entrant</param>
        /// <returns>The <see cref="Entrant.Models.Entrant"/> with the Id.</returns>
        /// <exception cref="EntrantNotFoundException">If an entrant with a matching id is not found then a EntrantNotFoundException is thrown.</exception>
        public Entrant GetById(int id)
        {
            if (_entrantMap.ContainsKey(id))
            {
                if (_entrantMap.TryGetValue(id, out var entrant))
                {
                    return entrant;
                }
            }

            Logger.Info($"GetById failed. Entrant id = {id} not found.");
            throw new EntrantNotFoundException($"Entrant with Id = {id} not found.");
        }
    }
}