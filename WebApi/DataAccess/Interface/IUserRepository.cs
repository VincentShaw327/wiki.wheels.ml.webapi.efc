using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using WebApi.Entities;


namespace WebApi.DataAccess.Interface
{
    public interface IUserRepository
    {
        User GetUserByEmail(string email);
        IEnumerable<User> GetAll();
        User GetById(string id);
        bool Any(Expression<Func<User, bool>> predicate);
        int Add(User user);
        int Update(User user);
        int Remove(User user);
    }
}
