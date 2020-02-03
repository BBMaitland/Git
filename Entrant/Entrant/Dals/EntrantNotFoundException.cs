namespace Entrant.Dals
{
    using System;

    public class EntrantNotFoundException : Exception
    {
        public EntrantNotFoundException(string message) :
            base(message)
        {
        }
    }
}
