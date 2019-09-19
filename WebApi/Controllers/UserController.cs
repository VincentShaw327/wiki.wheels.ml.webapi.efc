using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WebApi.DataAccess.Interface;
using WebApi.DataAccess.Base;
using WebApi.Entities;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IAlanDao AlanDao;

        public UserController(IAlanDao alanDao)
        {
            AlanDao = alanDao;
        }

        //插入数据
        [HttpPost]
        [Route("create")]
        public ActionResult<string> Create([FromBody]User userModel)
        {
            // string FirstName, string LastName, string UserName
            if (string.IsNullOrEmpty(userModel.FirstName.Trim()))
            {
                return "姓不能为空";
            }

            if (string.IsNullOrEmpty(userModel.LastName.Trim()))
            {
                return "名不能为空";
            }

            if (string.IsNullOrEmpty(userModel.UserName.Trim()))
            {
                return "用户名不能为空";
            }

            var student = new User()
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                UserName = userModel.UserName
            };

            var result = AlanDao.CreateUser(student);

            if (result)
            {
                return "学生插入成功";
            }
            else
            {
                return "学生插入失败";
            }

        }

        //查询所有数据
        [HttpGet]
        [Route("getsAll")]
        public ActionResult<string> GetAll()
        {
            var names = "没有数据";
            var students = AlanDao.GetUsers();

            if (students != null)
            {
                names = "";
                foreach (var s in students)
                {
                    names += $"{s.UserName} \r\n";
                }

            }

            return names;
        }

        //取某id记录
        public ActionResult<string> GetOne(int ID)
        {
            var name = "没有数据";
            var student = AlanDao.GetUserByID(ID);

            if (student != null)
            {
                name = student.UserName;
            }

            return name;
        }

        //根据id更新整条记录
        [HttpPost]
        [Route("update")]
        public ActionResult<string> Update(int ID, string FirstName, string LastName, string UserName, string Email, byte PasswordHash, byte PasswordSalt)
        {
            if (ID <= 0)
            {
                return "id 不能小于0";
            }

            if (string.IsNullOrEmpty(UserName.Trim()))
            {
                return "姓名不能为空";
            }

            //if (sex < 0 || sex > 2)
            //{
            //    return "性别数据有误";
            //}

            //if (age <= 0)
            //{
            //    return "年龄数据有误";
            //}

            var user = new User()
            {
                ID = ID,
                FirstName = FirstName,
                LastName = LastName,
                UserName = UserName
            };

            var result = AlanDao.UpdateUser(user);

            if (result)
            {
                return "学生更新成功";
            }
            else
            {
                return "学生更新失败";
            }
        }

        //根据id更新名称
        [HttpPost]
        [Route("updateName")]
        public ActionResult<string> UpdateName(int id, string name)
        {
            if (id <= 0)
            {
                return "id 不能小于0";
            }

            if (string.IsNullOrEmpty(name.Trim()))
            {
                return "姓名不能为空";
            }

            var result = AlanDao.UpdateNameByID(id, name);

            if (result)
            {
                return "学生更新成功";
            }
            else
            {
                return "学生更新失败";
            }
        }

        //根据id删掉记录
        [HttpPost]
        [Route("delete")]
        public ActionResult<string> Delete(int id)
        {
            if (id <= 0)
            {
                return "id 不能小于0！";
            }

            var result = AlanDao.DeleteUserByID(id);

            if (result)
            {
                return "学生删除成功";
            }
            else
            {
                return "学生删除失败";
            }
        }


        // GET: api/User
        [HttpGet]
        [Route("get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "user1", "user2" };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete0(int id)
        {
        }
    }
}
