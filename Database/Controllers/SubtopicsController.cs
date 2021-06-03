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
    public class SubtopicsController : EntitiesController<SubtopicsController, Subtopic>
    {

        public SubtopicsController(QAContext context, ILogger<SubtopicsController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/{subtopicId}/topic")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> GetSubtopicTopic(int subtopicId)
        {
            try
            {
                var t = Select()
                    .Include(st => st.Topic)
                    .First(st => st.Id == subtopicId)
                    .Topic;
                return new Entity { Id = t.Id, Value = t.Value };
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpGet("get/{subtopicId}/questions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Entity>> GetQuestions(int subtopicId)
        {
            try
            {
                return Select()
                    .Include(st => st.Question)
                    .First(st => st.Id == subtopicId)
                    .Question
                    .Select(q => new Entity { Id = q.Id, Value = q.Value })
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
        [ProducesResponseType(404)]
        public ActionResult<Entity> Post([FromBody, Required] string subtopicText, [Required] int topicId)
        {
            bool alreadyExists = Select().Any(st => subtopicText == st.Value);
            if (alreadyExists)
            {
                return BadRequest("Already exists");
            }
            Topic topic = null;
            try
            {
                topic = _context
                    .Topics
                    .Include(t => t.Subtopic)
                    .First(t => topicId == t.Id);
            }
            catch (Exception)
            {
                return NotFound("Topic not found");
            }
            var subtopic = new Subtopic
            {
                Value = subtopicText,
                TopicId = topicId,
                Topic = topic,
                Question = new List<Question>()
            };
            _context.Subtopics.Add(subtopic);
            _context.SaveChanges();
            topic.Subtopic.Add(subtopic);
            _context.SaveChanges();
            return new Entity { Id = subtopic.Id, Value = subtopic.Value };
        }

        [HttpDelete("delete/{subtopicId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> Delete(int subtopicId)
        {
            Subtopic subtopic = null;
            try
            {
                subtopic = Select()
                    .Include(st => st.Question.Take(1))
                    .First(st => st.Id == subtopicId);
            }
            catch (Exception)
            {
                return NotFound("Subtopic not found");
            }            
            if (subtopic.Question.Count() > 0)
            {
                return BadRequest("Subtopic depends on some questions. Delete questions first");
            }
            _context.Subtopics.Remove(subtopic);
            _context.SaveChanges();
            return new Entity { Id = subtopic.Id, Value = subtopic.Value };
        }
    }
}
