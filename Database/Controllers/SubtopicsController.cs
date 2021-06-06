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
    public class SubtopicsController : EntitiesController<SubtopicsController, Subtopic, EntitySubtopic>
    {

        public SubtopicsController(QAContext context, ILogger<SubtopicsController> logger)
            : base(context, logger, Subtopic.RemoveReferences)
        {
        }

        [HttpGet("get/{subtopicId}/topic")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<EntityTopic> GetSubtopicTopic(int subtopicId)
        {
            try
            {
                return Select()
                    .Include(st => st.Topic)
                    .First(st => st.Id == subtopicId)
                    .Topic
                    .RemoveReferences();
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpGet("get/{subtopicId}/questions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<EntityQuestion>> GetQuestions(int subtopicId)
        {
            try
            {
                return Select()
                    .Include(st => st.Question)
                    .First(st => st.Id == subtopicId)
                    .Question
                    .Select(Question.RemoveReferences)
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
        public ActionResult<EntitySubtopic> Post([FromBody, Required] string subtopicText, [Required] int topicId)
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
            return subtopic.RemoveReferences();
        }

        [HttpDelete("delete/{subtopicId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<EntitySubtopic> Delete(int subtopicId)
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
            return subtopic.RemoveReferences();
        }
    }
}
