namespace Entrant.Dals
{
    using System.Collections.Generic;
    using Models;

    public interface IEntrantDal
    {
        Entrant GetById(int id);
        IEnumerable<Entrant> GetAll();
        Entrant Create(Entrant value);
        void Delete(int id);
    }
}
