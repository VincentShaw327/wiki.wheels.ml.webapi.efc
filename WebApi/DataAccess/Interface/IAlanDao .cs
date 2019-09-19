using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;


namespace WebApi.DataAccess.Interface
{
    public interface IAlanDao
    {
        User Authenticate(string email, string password);

        User Create(User user, string password);

        //插入数据
        bool CreateUser(User user) ;

        //取全部记录
        IEnumerable<User> GetUsers();

        //取某id记录
        User GetUserByID(int ID);

        //根据id更新整条记录
        bool UpdateUser(User user);

        //根据id更新名称
        bool UpdateNameByID(int id, string name);

        //根据id删掉记录
        bool DeleteUserByID(int id);

        //bool GetStudentByID(int ID);
    }
}
