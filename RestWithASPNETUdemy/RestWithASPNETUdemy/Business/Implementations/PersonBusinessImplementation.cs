using RestWithASPNETUdemy.Data.Converter.Implementation;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Repository.Generic;
using System.Collections.Generic;

namespace RestWithASPNETUdemy.Business.Implementations
{
    public class PersonBusinessImplementation : IPersonBusiness
    {
        //private readonly IBookRepository _repository;

        //sendo comentado, pois a nova demanda de incluir o verbo patch, para ativar ou desativar uma pessoa, requer um acesso a base mais unico.
        //private readonly IRepository<Person> _repository;

        private readonly IPersonRepository _repository;
        
        
        private readonly PersonConverter _converter;

        public PersonBusinessImplementation(IPersonRepository repository)
        {
            _repository = repository;
            _converter = new PersonConverter();
        }

        public PersonVO Create(PersonVO person)
        {
            //Para persistir um objeto no banco, temos que realizar os seguintes passos.

            //converter o objetoVO para um objeto que pode ser inserido na base, um VO não pode ser inserido diretamente.
            var personEntity = _converter.Parse(person);

            //em seguida, é enviado esse objeto que foi convertido com o comando para inserir na base de dados.
            personEntity = _repository.Create(personEntity);

            //e para retornar as informações ao nosso client. convertemos o retorno de entidade para um objetoVO.
            return _converter.Parse(personEntity);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public PersonVO Disable(long id)
        {
            var personEntity = _repository.Disable(id);

            return _converter.Parse(personEntity);
        }

        public List<PersonVO> FindAll()
        {
            return _converter.Parse(_repository.FindAll());
        }

        public PersonVO FindById(long id)
        {
            return _converter.Parse(_repository.FindById(id));
        }

        public PersonVO Update(PersonVO person)
        {
            //converter o objetoVO para um objeto que pode ser inserido na base, um VO não pode ser inserido diretamente.
            var personEntity = _converter.Parse(person);

            //em seguida, é enviado esse objeto que foi convertido com o comando para inserir na base de dados.
            personEntity = _repository.Update(personEntity);

            //e para retornar as informações ao nosso client. convertemos o retorno de entidade para um objetoVO.
            return _converter.Parse(personEntity);
        }
    }
}
