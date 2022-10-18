using RestWithASPNETUdemy.Data.Converter.Implementation;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Repository.Generic;
using System;
using System.Collections.Generic;

namespace RestWithASPNETUdemy.Business.Implementations
{
    public class BookBusinessImplementation : IBookBusiness
    {
        //private readonly IBookRepository _repository;
        private readonly IRepository<Books> _repository;
        private readonly BookConverter _converter;

        public BookBusinessImplementation(IRepository<Books> repository)
        {
            _repository = repository;
            _converter = new BookConverter();
        }

        public BookVO Create(BookVO book)
        {
            //Para persistir um objeto no banco, temos que realizar os seguintes passos.

            //converter o objetoVO para um objeto que pode ser inserido na base, um VO não pode ser inserido diretamente.
            var bookEntity = _converter.Parse(book);

            //em seguida, é enviado esse objeto que foi convertido com o comando para inserir na base de dados.
            bookEntity = _repository.Create(bookEntity);

            //e para retornar as informações ao nosso client. convertemos o retorno de entidade para um objetoVO.
            return _converter.Parse(bookEntity);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public List<BookVO> FindAll()
        {
            return _converter.Parse(_repository.FindAll());
        }

        public BookVO FindById(long id)
        {
            return _converter.Parse(_repository.FindById(id));
        }

        public BookVO Update(BookVO book)
        {
            //converter o objetoVO para um objeto que pode ser inserido na base, um VO não pode ser inserido diretamente.
            var bookEntity = _converter.Parse(book);

            //em seguida, é enviado esse objeto que foi convertido com o comando para inserir na base de dados.
            bookEntity = _repository.Update(bookEntity);

            //e para retornar as informações ao nosso client. convertemos o retorno de entidade para um objetoVO.
            return _converter.Parse(bookEntity);
        }
    }
}
