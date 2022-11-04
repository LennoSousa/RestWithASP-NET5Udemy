using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository.Generic;

namespace RestWithASPNETUdemy.Repository
{
    public interface IPersonRepository : IRepository<Person>
    {
        //Person Create(Person person);
        //Person FindById(long id);
        //List<Person> FindAll();
        //Person Update(Person person);
        //void Delete(long id);
        //bool Exists(long id);

        Person Disable(long id);

    }
}
