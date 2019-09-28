using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DataAccess.Base;
using WebApi.Entities;
using WebApi.HttpProcess;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [Route("item")]
        public async Task<Response> GetItem([FromBody] Wiki wiki)
        {
            Response res = new Response();
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }

            var list=_context.Wiki_item.Where(
                b => b.uTopicUUID==wiki.uTopicUUID&& b.uObjectParentUUID == wiki.uObjectParentUUID
                &&b.nDelFlag==1
                ).ToList();
            var obj = await _context.Wiki_topic.FindAsync(wiki.uTopicUUID);

            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetWiki", new { id = wiki.ID }, wiki);
            res.dataList = list;
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