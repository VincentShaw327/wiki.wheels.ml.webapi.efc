using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using WebApi.Entities;
using WebApi.DataAccess.Interface;
using WebApi.DataAccess.Base;

namespace WebApi.DataAccess.Implement
{
    public class UserRepository : IUserRepository
    {
        //private readonly VueContext _context;
        public readonly AlanContext _context;

        public UserRepository(AlanContext context)
        {
            _context = context;
        }

        public User GetUserByEmail(string email)
        {
            return _context.User.SingleOrDefault(x => x.Email == email);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        public User GetById(string id)
        {
            return _context.User.Find(id);
        }

        public bool Any(Expression<Func<User, bool>> predicate)
        {
            return _context.User.Any(predicate);
        }

        public int Add(User user)
        {
            _context.User.Add(user);
            return _context.SaveChanges();
        }

        public int Update(User user)
        {
            _context.User.Update(user);
            return _context.SaveChanges();
        }

        public int Remove(User user)
        {
            _context.User.Remove(user);
            return _context.SaveChanges();
        }
    }
}
