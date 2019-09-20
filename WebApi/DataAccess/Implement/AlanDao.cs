using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DataAccess.Interface;
using WebApi.DataAccess.Base;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.DataAccess.Implement
{
    public class AlanDao : IAlanDao
    {
        public AlanContext Context;
        private readonly IUserRepository _iUserRepository;

        public AlanDao(AlanContext context)
        {
            Context = context;
            //_iUserRepository = iUserRepository;

        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _iUserRepository.GetUserByEmail(email);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_iUserRepository.Any(x => x.Email == user.Email))
                throw new AppException("Email \"" + user.Email + "\" is already taken");

            if (_iUserRepository.Any(x => x.UserName == user.UserName))
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.userId = Guid.NewGuid().ToString();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _iUserRepository.Add(user);

            return user;
        }

        //插入数据
        public bool CreateUser(User student)
        {
            Context.User.Add(student);
            return Context.SaveChanges() > 0;
        }

        //取全部记录
        public IEnumerable<User> GetUsers()
        {
            return Context.User.ToList();
        }

        public User GetById(string id)
        {
            return _iUserRepository.GetById(id);
        }

        //取某id记录
        public User GetUserByID(int ID)
        {
            return Context.User.SingleOrDefault(s => s.ID == ID);
        }

        //根据id更新整条记录
        public bool UpdateStudent(User student)
        {
            Context.User.Update(student);
            return Context.SaveChanges() > 0;
        }

        //根据id更新名称
        public bool UpdateNameByID(int id, string name)
        {
            var state = false;
            var student = Context.User.SingleOrDefault(s => s.ID == id);

            if (student != null)
            {
                student.FirstName = name;
                state = Context.SaveChanges() > 0;
            }

            return state;
        }

        //根据id删掉记录
        public bool DeleteStudentByID(int id)
        {
            var student = Context.User.SingleOrDefault(s => s.ID == id);
            Context.User.Remove(student);
            return Context.SaveChanges() > 0;
        }

        
        

        //public User GetUserByID(int id)
        //{
        //    throw new NotImplementedException();
        //}

        public bool UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUserByID(int id)
        {
            throw new NotImplementedException();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
