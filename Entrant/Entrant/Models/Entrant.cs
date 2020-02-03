namespace Entrant.Models
{
    public class Entrant
    {
        /// <summary>
        /// get or sets a unique Id for the entrant.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The first name of the entrant.
        /// </summary>
        public string FirstName { get; set; }


        /// <summary>
        /// The last (or family) name of the entrant.
        /// </summary>
        public string LastName { get; set; }
    }
}
