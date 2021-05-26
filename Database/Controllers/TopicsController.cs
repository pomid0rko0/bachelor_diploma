using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicsController : Selector<Topic>
    {

        public TopicsController(QAContext context, ILogger<IntentsController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<int> GetTopicId([Required, FromQuery] string topicText)
        {
            try
            {
                return Select().First(t => t.TopicText == topicText).TopicId;
            }
            catch (Exception)
            {
                return NotFound("Topic not found");
            }
        }

        [HttpGet("get/{topicId}/text")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<string> GetTopicText(int topicId)
        {
            try
            {
                return Select().First(t => t.TopicId == topicId).TopicText;
            }
            catch (Exception)
            {
                return NotFound("Topic not found");
            }
        }

        [HttpGet("get/{topicId}/subtopics")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Subtopic>> GetSubtopics(int topicId)
        {
            try
            {
                return Select()
                    .Include(t => t.Subtopic)
                    .First(t => t.TopicId == topicId)
                    .Subtopic
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound("Topic not found");
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<Topic> Post([Required] string topicText)
        {
            bool alreadyExists = Select().Any(t => topicText == t.TopicText);
            if (alreadyExists)
            {
                return BadRequest("Topic already exists");
            }
            var topic = new Topic
            {
                TopicText = topicText,
                Subtopic = new List<Subtopic>()
            };
            _context.Topics.Add(topic);
            _context.SaveChanges();
            return topic;
        }

        [HttpDelete("delete/{topicId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Topic> DeleteByIds(int topicId)
        {
            Topic topic = null;
            try
            {
                topic = Select()
                    .Include(t => t.Subtopic.Take(1))
                    .First(t => t.TopicId == topicId);
            }
            catch (Exception)
            {
                return NotFound("Topic not found");
            }            
            if (topic.Subtopic.Count() > 0)
            {
                return BadRequest("Topic depends on some subtopics. Delete subtopics first");
            }
            _context.Topics.Remove(topic);
            _context.SaveChanges();
            return topic;
        }
    }
}
