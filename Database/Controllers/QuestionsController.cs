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
    public class QuestionsController : Selector<Question>
    {

        public QuestionsController(QAContext context, ILogger<IntentsController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<int> GetQuestionId([Required, FromQuery] string questionText)
        {
            try
            {
                return Select().First(q => q.QuestionText == questionText).QuestionId;
            }
            catch (Exception)
            {
                return NotFound("Question not found");
            }
        }

        [HttpGet("get/{questionId}/text")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<string> GetQuestionText(int questionId)
        {
            try
            {
                return Select().First(q => q.QuestionId == questionId).QuestionText;
            }
            catch (Exception)
            {
                return NotFound("Question not found");
            }
        }

        [HttpGet("get/{questionId}/subtopic")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Subtopic> GetQuestionSubtopic(int questionId)
        {
            try
            {
                return Select()
                    .Include(q => q.Subtopic)
                    .First(q => q.SubtopicId == questionId)
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
        public ActionResult<IEnumerable<Intent>> GetQuestionIntents(int questionId)
        {
            try
            {
                return Select()
                    .Include(q => q.Intent)
                    .First(q => q.QuestionId == questionId)
                    .Intent
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
        public ActionResult<Question> Post(
            [Required] string questionText, 
            [Required] int[] intentIds,
            int? subtopicId
        )
        {
            bool alreadyExists = Select().Any(q => questionText == q.QuestionText);
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
                        .First(subtopic => subtopicId == subtopic.SubtopicId);
                }
                catch (Exception)
                {
                    return NotFound("Subtopic not found");
                }
            }
            var intents = _context
                .Intents
                .Include(i => i.Question)
                .Where(i => intentIds.Contains(i.IntentId));
            if (intentIds.Any(intentId => !intents.Select(Intent => Intent.IntentId).Contains(intentId)))
            {
                return NotFound("Some intents not found");
            }
            var question = new Question
            {
                QuestionText = questionText,
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
            return question;
        }

        [HttpDelete("delete/{questionId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Question> DeleteById(int questionId)
        {
            Question question = null;
            try 
            {
                question = Select()
                    .Include(q => q.Intent)
                    .First(q => questionId == q.QuestionId);
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
            return question;
        }
    }
}
