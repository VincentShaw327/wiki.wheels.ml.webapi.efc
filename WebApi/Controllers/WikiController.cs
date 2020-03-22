using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using WebApi.DataAccess.Base;
using WebApi.Entities;
using WebApi.HttpProcess;
using WebApi.HttpProcess.Request;

namespace WebApi.Controllers
{
    //[Authorize("Permission")]
    [Route("api/[controller]")]
    //[ApiController]
    //[Authorize(Roles = "admin,system")]
    public class WikiController : ControllerBase
    {
        private readonly SqlContext _context;

        public WikiController(SqlContext context)
        {
            _context = context;
        }

        // GET: api/Wiki
        [HttpGet]
        public IEnumerable<Wiki> GetWiki_item()
        {
            return _context.Wiki_item.ToList();
        }

        // GET: api/Wikis/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWiki([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wiki = await _context.Wiki_item.FindAsync(id);

            if (wiki == null)
            {
                return NotFound();
            }

            //res.dataList = topic;
            //res.success = 1;
            //return res;

            return Ok(wiki);
        }

        [HttpPost]
        //[Authorize]
        ////[Authorize("Permission")]
        [Route("item")]
        public async Task<Response> GetItem([FromBody] Wiki wiki)
        {
            Response res = new Response();
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }
            List<Wiki> _list = new List<Wiki>();

             _list = _context.Wiki_item.Where(
                b => b.uTopicUUID==wiki.uTopicUUID&& b.uObjectParentUUID == wiki.uObjectParentUUID
                &&b.nDelFlag==1
                ).ToList();
            var obj = await _context.Wiki_topic.FindAsync(wiki.uTopicUUID);

            try
            {
                _list.Sort();

            }
            catch (System.Exception ex)
            {
                //eError = TError.TError_DataBase_Exception;
            }

            //await _context.SaveChangesAsync();
            //return CreatedAtAction("GetWiki", new { id = wiki.ID }, wiki);
            _list.Sort(delegate (Wiki x, Wiki y)
            {
                if (x.orderID > y.orderID) return 1;
                else return -1;
                //if (x.PartName == null && y.PartName == null) return 0;
                //else if (x.PartName == null) return -1;
                //else if (y.PartName == null) return 1;
                //else return x.PartName.CompareTo(y.PartName);
            });
            //_list.Sort();


            res.dataList = _list; 
            res.success = 1;
            res.obj = obj;
            return res;
        }

        // PUT: api/Wikis/5
        [HttpPut("{id}")]
        public async Task<Response> PutWiki([FromRoute] int id, [FromBody] Wiki wiki)
        {
            Response res = new Response();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }

            if (id != wiki.ID)
            {
                //return BadRequest();
            }

            _context.Entry(wiki).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                res.obj = wiki;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WikiExists(id))
                {
                    //return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            res.success = 1;
            return res;
        }

        // POST: api/Wikis
        [HttpPost]
        public async Task<Response> PostWiki([FromBody] Wiki wiki)
        {
            Response res = new Response();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }

            _context.Wiki_item.Add(wiki);
            await _context.SaveChangesAsync();

            res.obj= CreatedAtAction("GetWiki", new { id = wiki.ID }, wiki).Value;
            res.success = 1;
            return res;
        }

        [HttpPost("updateChildren")]
        public async Task<Response> UpdateChildren([FromBody] Wiki wiki)
        {
            Response res = new Response();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }
            var list = _context.Wiki_item.Where(b => b.uObjectParentUUID == wiki.ID && b.nDelFlag == 1).ToList();
            var wikiTemp = await _context.Wiki_item.FindAsync(wiki.ID);
            if (list.Count >= 1)
            {
                wikiTemp.nHasChildren = 1;
            }
            else
            {
                wikiTemp.nHasChildren = 0;
            }
            await _context.SaveChangesAsync();
            //_context.Wiki_item.Add(wiki);

            //res.obj = CreatedAtAction("GetWiki", new { id = wiki.ID }, wiki).Value;
            res.success = 1;
            return res;
        }

        [HttpPost("drag")]
        public async Task<Response> DrageItem([FromBody] WikiDrage wikiDrage)
        {
            Response res = new Response();
            List<Wiki> dropList = new List<Wiki>();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }
            var item = await _context.Wiki_item.FindAsync(wikiDrage.id);
            var dropParentItem = await _context.Wiki_item.FindAsync(wikiDrage.pid);
            var dragParentItem = await _context.Wiki_item.FindAsync(wikiDrage.dragPid);
            var dragList = _context.Wiki_item.Where(b => b.uObjectParentUUID == wikiDrage.dragPid && b.nDelFlag == 1&&b.uTopicUUID==item.uTopicUUID).ToList();
             dropList = _context.Wiki_item.Where(b => b.uObjectParentUUID == wikiDrage.pid && b.nDelFlag == 1&& b.uTopicUUID == item.uTopicUUID).ToList();
            //dropList.Sort();
            dropList.Sort(delegate (Wiki x, Wiki y)
            {
                if (x.orderID > y.orderID) return 1;
                else return -1;

                //if (x.PartName == null && y.PartName == null) return 0;
                //else if (x.PartName == null) return -1;
                //else if (y.PartName == null) return 1;
                //else return x.PartName.CompareTo(y.PartName);
            });
            if (wikiDrage.dropToGap == false)
            {
                item.orderID = dropList.Count;
                item.uObjectParentUUID = wikiDrage.pid;
                this.UpdateChildren(dropParentItem);
            }
            else
            {
                var insertPos=0;
                var pos = wikiDrage.dropPosition;
                if (pos == -1) insertPos = 0;
                else if (pos == 0 ) insertPos = 1;
                else insertPos = pos;

                item.uObjectParentUUID = wikiDrage.pid;
                if (wikiDrage.pid == wikiDrage.dragPid)
                {
                    dropList.RemoveAt(wikiDrage.prePos);
                }
                dropList.Insert(insertPos, item);

                //list.Remove()
                //list.ForEach((ele,index)=>ele.orderID=index)
                for (int i = 0; i < dropList.Count; i++)
                {
                    dropList[i].orderID = i;
                }
                this.UpdateChildren(dragParentItem);

            }



            //var wikiTemp = await _context.Wiki_item.FindAsync(wikiDrage.ID);
            //if (wikiDrage.Count >= 1)
            //{
            //    wikiTemp.nHasChildren = 1;
            //}
            //else
            //{
            //    wikiTemp.nHasChildren = 0;
            //}
            await _context.SaveChangesAsync();
            res.dataList = _context.Wiki_item.Where(b => b.uObjectParentUUID == wikiDrage.pid && b.nDelFlag == 1).ToList();

            //_context.Wiki_item.Add(wiki);

            //res.obj = CreatedAtAction("GetWiki", new { id = wiki.ID }, wiki).Value;
            res.success = 1;
            return res;
        }

        // DELETE: api/Wikis/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWiki([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var wiki = await _context.Wiki_item.FindAsync(id);
            if (wiki == null)
            {
                return NotFound();
            }

            _context.Wiki_item.Remove(wiki);
            await _context.SaveChangesAsync();

            return Ok(wiki);
        }

        private bool WikiExists(int id)
        {
            return _context.Wiki_item.Any(e => e.ID == id);
        }
    }
}