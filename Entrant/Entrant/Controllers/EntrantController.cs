namespace Entrant.Controllers
{
    using System;
    using Dals;
    using log4net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    /// <summary>
    /// A control that enables CRUD operations over a collection of <see cref="Entrant.Models.Entrant"/> objects.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class EntrantController : ControllerBase
    {
        /// <summary>
        /// The entrant store object
        /// </summary>
        private readonly IEntrantDal _entrantDal;


        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructor for the controller
        /// </summary>
        /// <param name="entrantDal">The Entrants storage DAL</param>
        public EntrantController(IEntrantDal entrantDal)
        {
            _entrantDal = entrantDal ?? new EntrantDal();
            Logger.Info("starting Entrant Controller");
        }


        /// <summary>
        /// A rest operation the returns all the entrants in the store
        /// </summary>
        /// <returns>collection of <see cref="Entrant.Models.Entrant"/> objects</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            Logger.Info("start  GetAll()");

            try
            {
                var entrants = _entrantDal.GetAll();
                return Ok(entrants);
            }
            catch (Exception e)
            {
                Logger.Error("Error in  GetAll()", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.Info("finish  GetAll()");
            }
        }

        /// <summary>
        /// Returns the entrant with specified unique Id
        /// </summary>
        /// <param name="id">The Id of the entrant</param>
        /// <returns>
        /// The <see cref="Entrant.Models.Entrant"/> with the Id.
        /// An NotFound (404) errors is returned if no entrant with the specified Id is found.
        /// </returns>
        [HttpGet("{id}", Name = "Get")]
        public IActionResult GetById(int id)
        {
            Logger.Info("start  GetById(id)");

            try
            {
                var entrant = _entrantDal.GetById(id);

                return Ok(entrant);
            }
            catch (EntrantNotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception e)
            {
                Logger.Error("Error in  GetById(id)", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.Info("finish  GetById(id)");
            }
        }

        /// <summary>
        /// A post request that adds a new <see cref="Entrant.Models.Entrant"/> object to the store 
        /// </summary>
        /// <param name="entrant">A new <see cref="Entrant.Models.Entrant"/> to store, </param>
        /// <returns>returns the unique Id of the new entrant.</returns>
        /// <remarks>If first and last names are not supplied supplied, then an BadRequest (400) error code is returned.</remarks>
        [HttpPost]
        public IActionResult Create([FromBody] Entrant entrant)
        {
            Logger.Info("start  Create()");

            try
            {
                if (string.IsNullOrWhiteSpace(entrant.FirstName))
                {
                    return BadRequest(entrant.FirstName);
                }

                if (string.IsNullOrWhiteSpace(entrant.LastName))
                {
                    return BadRequest(entrant.LastName);
                }

                var newEntrant = _entrantDal.Create(entrant);

                return CreatedAtAction(nameof(GetById), new {id = newEntrant.Id}, newEntrant);
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(argumentException.ParamName);
            }
            catch (Exception e)
            {
                Logger.Error("Error in  Create()", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.Info("finish  Create()");
            }
        }

        /// <summary>
        /// Deletes the entrant with the matching Id
        /// </summary>
        /// <param name="id">The unique Id of the entrant to remove.</param>
        /// <remarks>If an entrant with a matching id is not found then a NotFound (404) error is returned.</remarks>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Logger.Info("start  Delete(id)");

            try
            {
                _entrantDal.Delete(id);

                return Ok();
            }
            catch (EntrantNotFoundException)
            {
                return NotFound(id);
            }
            catch (Exception e)
            {
                Logger.Error("Error in  Delete(id)", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                Logger.Info("finish  Delete(id)");
            }
        }
    }
}
