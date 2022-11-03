using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Model.Context;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RestWithASPNETUdemy.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly MySQLContext _context;

        public UserRepository(MySQLContext context)
        {
            _context = context;
        }

        public User ValidateCredentials(UserVO user)
        {
            //descontinuado pela Microsoft.
            //var pass = ComputeHash(user.Password, new SHA256CryptoServiceProvider());

            var password = ComputeHash(user.Password, SHA256.Create());

            return _context.Users.FirstOrDefault(u => 
                (u.UserName == user.UserName) &&
                (u.Password == password));
        }

        public User ValidateCredentials(string username)
        {
            return _context.Users.SingleOrDefault(u => (u.UserName == username));
        }


            //public User RefreshUserInfo(User user)
            //{

            //}

            //Método responsável por encriptar a senha.
            private string ComputeHash(string password, HashAlgorithm hashAlgorithm)
        {
            Byte[] hashedBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password));

            ////Trecho comentado, retirado de uma resposta do forum, porém o ajuste não resolveu o problema.

            //var sBuilder = new StringBuilder();

            //foreach (var item in hashedBytes)
            //{
            //    sBuilder.Append(item.ToString("x2"));
            //}

            //return sBuilder.ToString();

            return BitConverter.ToString(hashedBytes);
        }

        public User RefreshUserInfor(User user)
        {
            if (!_context.Users.Any(u => u.Id.Equals(user.Id))) return null;

            var result = _context.Users.SingleOrDefault(p => p.Id.Equals(user.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        public bool RevokeToken(string username)
        {
            var user = _context.Users.SingleOrDefault(u => (u.UserName == username));

            if (user is null) return false;

            user.RefreshToken = null;

            _context.SaveChanges();

            return true;
        }
    }
}
