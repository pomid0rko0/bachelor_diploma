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
    public class SubtopicsController : Selector<SubtopicsController, Subtopic>
    {

        public SubtopicsController(QAContext context, ILogger<SubtopicsController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<int> GetSubtopicId([Required, FromQuery] string subtopicText)
        {
            try
            {
                return Select().First(st => st.SubtopicText == subtopicText).SubtopicId;
            }
            catch (Exception)
            {
                return NotFound("Subtopic not found");
            }
        }

        [HttpGet("get/{subtopicId}/text")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<string> GetSubtopicText(int subtopicId)
        {
            try
            {
                return Select().First(st => st.SubtopicId == subtopicId).SubtopicText;
            }
            catch (Exception)
            {
                return NotFound("Subtopic not found");
            }
        }

        [HttpGet("get/{subtopicId}/topic")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Topic> GetSubtopicTopic(int subtopicId)
        {
            try
            {
                return Select()
                    .Include(st => st.Topic)
                    .First(st => st.SubtopicId == subtopicId)
                    .Topic;
            }
            catch (Exception)
            {
                return NotFound("Subtopic not found");
            }
        }

        [HttpGet("get/{subtopicId}/questions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Question>> GetQuestions(int subtopicId)
        {
            try
            {
                return Select()
                    .Include(st => st.Question)
                    .First(st => st.SubtopicId == subtopicId)
                    .Question
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound("Subtopic not found");
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Subtopic> Post([Required] string subtopicText, [Required] int topicId)
        {
            bool alreadyExists = Select().Any(st => subtopicText == st.SubtopicText);
            if (alreadyExists)
            {
                return BadRequest("Subtopic already exists");
            }
            Topic topic = null;
            try
            {
                topic = _context
                    .Topics
                    .Include(t => t.Subtopic)
                    .First(t => topicId == t.TopicId);
            }
            catch (Exception)
            {
                return NotFound("Topic not found");
            }
            var subtopic = new Subtopic
            {
                SubtopicText = subtopicText,
                TopicId = topicId,
                Topic = topic,
                Question = new List<Question>()
            };
            _context.Subtopics.Add(subtopic);
            _context.SaveChanges();
            topic.Subtopic.Add(subtopic);
            _context.SaveChanges();
            return subtopic;
        }

        [HttpDelete("delete/{subtopicId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Subtopic> DeleteByIds(int subtopicId)
        {
            Subtopic subtopic = null;
            try
            {
                subtopic = Select()
                    .Include(st => st.Question.Take(1))
                    .First(st => st.SubtopicId == subtopicId);
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
            return subtopic;
        }
    }
}
