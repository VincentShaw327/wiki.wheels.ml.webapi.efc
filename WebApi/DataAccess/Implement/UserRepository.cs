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
        public readonly SqlContext _context;

        public UserRepository(SqlContext context)
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



        //插入数据
        public bool CreateUser(User student)
        {
            _context.User.Add(student);
            return _context.SaveChanges() > 0;
        }

        //取全部记录
        public IEnumerable<User> GetUsers()
        {
            return _context.User.ToList();
        }

        //取某id记录
        public User GetUserByID(int ID)
        {
            return _context.User.SingleOrDefault(s => s.ID == ID);
        }

        //根据id更新整条记录
        public bool UpdateStudent(User student)
        {
            _context.User.Update(student);
            return _context.SaveChanges() > 0;
        }

        //根据id更新名称
        public bool UpdateNameByID(int id, string name)
        {
            var state = false;
            var student = _context.User.SingleOrDefault(s => s.ID == id);

            if (student != null)
            {
                student.FirstName = name;
                state = _context.SaveChanges() > 0;
            }

            return state;
        }

        //根据id删掉记录
        public bool DeleteStudentByID(int id)
        {
            var student = _context.User.SingleOrDefault(s => s.ID == id);
            _context.User.Remove(student);
            return _context.SaveChanges() > 0;
        }


    }
}
