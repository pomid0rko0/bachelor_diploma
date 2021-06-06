using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models;
using Database.Models.Entities;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicsController : EntitiesController<TopicsController, Topic, EntityTopic>
    {

        public TopicsController(QAContext context, ILogger<TopicsController> logger)
            : base(context, logger, Topic.RemoveReferences)
        {
        }

        [HttpGet("get/{topicId}/subtopics")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<EntitySubtopic>> GetSubtopics(int topicId)
        {
            try
            {
                return Select()
                    .Include(t => t.Subtopic)
                    .First(t => t.Id == topicId)
                    .Subtopic
                    .Select(Subtopic.RemoveReferences)
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<EntityTopic> Post([FromBody, Required] string topicText)
        {
            bool alreadyExists = Select().Any(t => topicText == t.Value);
            if (alreadyExists)
            {
                return BadRequest("Already exists");
            }
            var topic = new Topic
            {
                Value = topicText,
                Subtopic = new List<Subtopic>()
            };
            _context.Topics.Add(topic);
            _context.SaveChanges();
            return topic.RemoveReferences();
        }

        [HttpDelete("delete/{topicId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<EntityTopic> Delete(int topicId)
        {
            Topic topic = null;
            try
            {
                topic = Select()
                    .Include(t => t.Subtopic.Take(1))
                    .First(t => t.Id == topicId);
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
            if (topic.Subtopic.Count() > 0)
            {
                return BadRequest("Topic depends on some subtopics. Delete subtopics first");
            }
            _context.Topics.Remove(topic);
            _context.SaveChanges();
            return topic.RemoveReferences();
        }
    }
}
