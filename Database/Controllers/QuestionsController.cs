using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models.QA;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : EntitiesController<QuestionsController, Question>
    {

        public QuestionsController(QAContext context, ILogger<QuestionsController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/{questionId}/subtopic")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> GetQuestionSubtopic(int questionId)
        {
            try
            {
                return Select()
                    .Include(q => q.Subtopic)
                    .First(q => q.Id == questionId)
                    .Subtopic;
            }
            catch (Exception)
            {
                return NotFound("Question not found");
            }
        }

        [HttpGet("get/{questionId}/intents")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Entity>> GetQuestionIntents(int questionId)
        {
            try
            {
                return Select()
                    .Include(q => q.Intent)
                    .First(q => q.Id == questionId)
                    .Intent
                    .Select(i => new Entity { Id = i.Id, Value = i.Value })
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound("Question not found");
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> Post(
            [Required] string questionText, 
            [Required] int[] intentIds,
            int? subtopicId
        )
        {
            bool alreadyExists = Select().Any(q => questionText == q.Value);
            if (alreadyExists)
            {
                return BadRequest("Question already exists");
            }
            Subtopic subtopic = null;
            if (subtopicId != null)
            {
                try
                {
                    subtopic = _context
                        .Subtopics
                        .Include(st => st.Question)
                        .First(subtopic => subtopicId == subtopic.Id);
                }
                catch (Exception)
                {
                    return NotFound("Subtopic not found");
                }
            }
            var intents = _context
                .Intents
                .Include(i => i.Question)
                .Where(i => intentIds.Contains(i.Id));
            if (intentIds.Any(intentId => !intents.Select(Intent => Intent.Id).Contains(intentId)))
            {
                return NotFound("Some intents not found");
            }
            var question = new Question
            {
                Value = questionText,
                SubtopicId = subtopicId,
                Subtopic = subtopic,
                Intent = intents.ToList()
            };
            _context.Questions.Add(question);
            _context.SaveChanges();
            foreach (var intent in intents)
            {
                intent.Question.Add(question);
            }
            if (subtopic != null)
            {
                subtopic.Question.Add(question);
                _context.SaveChanges();
            }
            return new Entity { Id = question.Id, Value = question.Value };
        }

        [HttpDelete("delete/{questionId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> Delete(int questionId)
        {
            Question question = null;
            try 
            {
                question = Select()
                    .Include(q => q.Intent)
                    .First(q => questionId == q.Id);
            }
            catch (Exception)
            {
                return NotFound("Question not found");
            }
            _context.Questions.Remove(question);
            _context.SaveChanges();
            foreach (var intent in question.Intent)
            {
                intent.Question.Remove(question);
            }
            _context.SaveChanges();
            return new Entity { Id = question.Id, Value = question.Value };
        }
    }
}
