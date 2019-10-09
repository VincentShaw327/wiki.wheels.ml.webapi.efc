using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.HttpProcess.Request
{
    public class WikiDrage
    {
        public bool dropToGap;
        public int  dropPosition;
        public int pid;
        public int dragPid;
        public int prePos;
        public int id;
        public WikiDrage()
        {
            dropToGap = false;
            dropPosition = 0;
            pid = 0;
            dragPid = 0;
            prePos = 0;
            id = 0;
        }
    }
}
