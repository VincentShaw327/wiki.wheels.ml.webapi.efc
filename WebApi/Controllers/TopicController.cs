using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

using WebApi.DataAccess.Base;
using WebApi.Entities;
using WebApi.HttpProcess;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly SqlContext _context;
        private readonly IMapper _iMapper;

        public TopicController(SqlContext context, IMapper iMapper)
        {
            _context = context;
        }

        // GET: api/Wiki_topic
        //[AllowAnonymous]
        [HttpGet]
        //[Authorize]
        public Response GetWiki_topic()
        {
            Response res = new Response();
            //return _context.Wiki_topic;
            var topic = _context.Wiki_topic.ToList();
            //var userModels = _iMapper.Map<Topic>(topic);
            //var userModels = _iMapper.Map<IList<Topic>>(_context.Wiki_topic);

            //return Ok(userModels);
            res.dataList = topic;
            res.success = 1;
            return res;
        }

        // GET: api/Wiki_topic/5
        [HttpGet("{id}")]
        //[Authorize]
        public async Task<Response> GetWiki_topic([FromRoute] int id)
        {
            Response res = new Response();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }

            //var wiki_topic = await _context.Wiki_topic.FindAsync(ikd);
            var list =  _context.Wiki_topic.Where(b => b.uObjectCategoryUUID == id&&b.nDelFlag==1).ToList();

            if (list == null)
            {
                //return NotFound();
            }

            //return Ok(wiki_topic);
            res.dataList = list;
            res.success = 1;
            return res;
        }

        // PUT: api/Wiki_topic/5
        [HttpPut("{id}")]
        public async Task<Response> PutWiki_topic([FromRoute] int id, [FromBody] Topic wiki_topic)
        {
            Response res = new Response();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }

            if (id != wiki_topic.ID)
            {
                //return BadRequest();
            }

            _context.Entry(wiki_topic).State = EntityState.Modified;

            try
            {
               await _context.SaveChangesAsync();
                res.dataList = wiki_topic;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Wiki_topicExists(id))
                {
                    //return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            //res.dataList = list;
            res.success = 1;
            return res;
        }

        // POST: api/Wiki_topic
        [HttpPost]
        public async Task<Response> PostWiki_topic([FromBody] Topic wiki_topic)
        {
            Response res = new Response();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }

            _context.Wiki_topic.Add(wiki_topic);
            await _context.SaveChangesAsync();

            res.dataList = CreatedAtAction("GetWiki_topic", new { id = wiki_topic.ID }, wiki_topic).Value;
            res.success = 1;
            return res;
        }

        // DELETE: api/Wiki_topic/5
        [HttpDelete("{id}")]
        public async Task<Response> DeleteWiki_topic([FromRoute] int id)
        {
            Response res = new Response();

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
            }

            var wiki_topic = await _context.Wiki_topic.FindAsync(id);
            if (wiki_topic == null)
            {
                //return NotFound();
            }

            _context.Wiki_topic.Remove(wiki_topic);
            await _context.SaveChangesAsync();

            //return Ok(wiki_topic);
            res.dataList = wiki_topic;
            res.success = 1;
            return res;
        }

        private bool Wiki_topicExists(int id)
        {
            return _context.Wiki_topic.Any(e => e.ID == id);
        }
    }
}