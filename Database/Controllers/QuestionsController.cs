using System;
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

        [HttpGet("get/{questionId}/answer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> GetQuestionAnswer(int questionId)
        {
            try
            {
                var a = Select()
                    .Include(q => q.Answer)
                    .First(q => q.Id == questionId)
                    .Answer;
                return new Entity { Id = a.Id, Value = a.Value };
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
            [FromBody, Required] string questionText, 
            [Required] int answerId,
            [Required] int subtopicId,
            bool? isExample = true
        )
        {
            bool isEx = isExample ?? true;
            bool alreadyExists = Select().Any(q => questionText == q.Value);
            if (alreadyExists)
            {
                return BadRequest("Question already exists");
            }
            Subtopic subtopic = null;
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
            Answer answer = null;
            try
            {
                answer = _context
                    .Answers
                    .Include(a => a.Question)
                    .First(a => a.Id == answerId);
            }
            catch (Exception)
            {
                return NotFound("Answer not found");
            }
            var question = new Question
            {
                Value = questionText,
                SubtopicId = subtopicId,
                Subtopic = subtopic,
                AnswerId = answerId,
                Answer = answer
            };
            _context.Questions.Add(question);
            _context.SaveChanges();
            answer.Question.Add(question);
            subtopic.Question.Add(question);
            _context.SaveChanges();
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
                    .Include(q => q.Answer)
                    .First(q => questionId == q.Id);
            }
            catch (Exception)
            {
                return NotFound("Question not found");
            }
            _context.Questions.Remove(question);
            _context.SaveChanges();
            question.Answer.Question.Remove(question);
            _context.SaveChanges();
            return new Entity { Id = question.Id, Value = question.Value };
        }
    }
}
