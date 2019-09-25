using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.HttpProcess
{
    public class Response
    {
        public int success;
        public string message;
        public object token;
        public object dataList;

        public Response()
        {
            success = 0;
            message = "";
            token = "";
        }
    }
}
