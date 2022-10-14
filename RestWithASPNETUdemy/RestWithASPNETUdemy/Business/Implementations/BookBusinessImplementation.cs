using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Repository.Generic;
using System.Collections.Generic;

namespace RestWithASPNETUdemy.Business.Implementations
{
    public class BookBusinessImplementation : IBookBusiness
    {
        //private readonly IBookRepository _repository;
        private readonly IRepository<Books> _repository;

        public BookBusinessImplementation(IRepository<Books> repository)
        {
            _repository = repository;
        }

        public Books Create(Books book)
        {
            return _repository.Create(book);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public List<Books> FindAll()
        {
            return _repository.FindAll();
        }

        public Books FindById(long id)
        {
            return _repository.FindById(id);
        }

        public Books Update(Books book)
        {
            return _repository.Update(book);
        }
    }
}
