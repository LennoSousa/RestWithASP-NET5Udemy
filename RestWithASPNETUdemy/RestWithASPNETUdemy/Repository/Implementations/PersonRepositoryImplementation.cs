using RestWithASPNETUdemy.Repository.Generic;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository;
using RestWithASPNETUdemy.Model.Context;
using System.Linq;
using System;

namespace restwithaspnetudemy.repository
{
    public class PersonRepositoryImplementation : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepositoryImplementation(MySQLContext context) : base(context) { }

        public Person Disable(long id)
        {
            if (!_context.Persons.Any(p => p.Id.Equals(id))) return null;
            
            var user = _context.Persons.SingleOrDefault(p => p.Id.Equals(id));

            if (user != null)
            {
                user.Enabled = false;
                try
                {
                    _context.Entry(user).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return user;
        }


        #region old font
        //        private MySQLContext _context;

        //        public PersonRepositoryImplementation(MySQLContext context)
        //        {
        //            _context = context;
        //        }

        //        public Person Create(Person person)
        //        {
        //            try
        //            {
        //                _context.Add(person);
        //                _context.SaveChanges();
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }
        //            return person;
        //        }

        //        public void Delete(long id)
        //        {
        //            var result = _context.Persons.SingleOrDefault(p => p.Id.Equals(id));

        //            if (result != null)
        //            {
        //                try
        //                {
        //                    _context.Persons.Remove(result);
        //                    _context.SaveChanges();
        //                }
        //                catch (Exception)
        //                {
        //                    throw;
        //                }
        //            }
        //        }

        //        public List<Person> FindAll()
        //        {
        //            return _context.Persons.ToList();
        //        }

        //        public Person FindById(long id)
        //        {
        //            return _context.Persons.SingleOrDefault(p => p.Id.Equals(id));
        //        }

        //        public Person Update(Person person)
        //        {
        //            if (!Exists(person.Id)) return null;

        //            var result = _context.Persons.SingleOrDefault(p => p.Id.Equals(person.Id));

        //            if (result != null)
        //            {
        //                try
        //                {
        //                    _context.Entry(result).CurrentValues.SetValues(person);
        //                    _context.SaveChanges();
        //                }
        //                catch (Exception)
        //                {
        //                    throw;
        //                }
        //            }

        //            return person;
        //        }

        //        public bool Exists(long id)
        //        {
        //            return _context.Persons.Any(p => p.Id.Equals(id));
        //        }

        #endregion


    }
}
