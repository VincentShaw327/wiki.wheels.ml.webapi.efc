using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DataAccess.Interface;
using WebApi.DataAccess.Base;
using WebApi.Entities;


namespace WebApi.DataAccess.Implement
{
    public class AlanDao : IAlanDao
    {
        public AlanContext Context;

        public AlanDao(AlanContext context)
        {
            Context = context;
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
    }
}
