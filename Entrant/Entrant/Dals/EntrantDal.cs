namespace Entrant.Dals
{
    using log4net;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        private readonly IDictionary<int, Entrant> _entrantMap;

        /// <summary>
        /// An object used to lock the _entrantMap
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// A constuctor for the DAL
        /// </summary>
        /// <param name="entrantMap">A store of <see cref="Entrant.Models.Entrant"/> objects</param>
        public EntrantDal(IDictionary<int, Entrant> entrantMap = null)
        {
            _entrantMap = entrantMap ?? new Dictionary<int, Entrant>();
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

            lock (_lockObject)
            {
                entrant.Id = _entrantMap.Count + 1;
                _entrantMap.TryAdd(entrant.Id, entrant);
                Logger.Info($"Created new Entrant id = {entrant.Id}");
            }

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
                lock (_lockObject)
                {
                    if (_entrantMap.TryGetValue(id, out var entrant))
                    {
                        deleted = _entrantMap.Remove(id);
                    }
                }
            }

            if (deleted)
            {
                Logger.Info($"Deleted Entrant id{id}");
            }
            else
            {
                Logger.Info($"Delete failed. Entrant id = {id} not found.");
                throw new EntrantNotFoundException($"Entrant with Id={id} not found.");
            }
        }

        /// <summary>
        /// Returns a collection of all entrants in the store, otherwise an empty collection is returned.
        /// </summary>
        /// <returns>collection of <see cref="Entrant.Models.Entrant"/> objects</returns>
        public IEnumerable<Entrant> GetAll()
        {
            lock (_lockObject)
            {
                return _entrantMap.Values.ToList(); 
            }
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
                lock (_lockObject)
                {
                    if (_entrantMap.TryGetValue(id, out var entrant))
                    {
                        return entrant;
                    }
                }
            }

            Logger.Info($"GetById failed. Entrant id = {id} not found.");
            throw new EntrantNotFoundException($"Entrant with Id={id} not found.");
        }
    }
}