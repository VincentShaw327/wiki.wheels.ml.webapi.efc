using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.HttpProcess
{
    public class Response
    {
        public int success;
        public string resTxt;
        public object token;
        public object dataList;
        public object obj;


        public Response()
        {
            success = 0;
            resTxt = "";
            token = "";
            //obj =null;
        }
    }
}
