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
    public class AnswersController : EntitiesController<AnswersController, Answer>
    {

        public AnswersController(QAContext context, ILogger<AnswersController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/{id}/questions")]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Entity>> GetAnswerIntent(int id)
        {
            try
            {
                return Select()
                    .Include(a => a.Question)
                    .First(a => a.Id == id)
                    .Question
                    .Select(q => new EntityQuestion { Id = q.Id, Value = q.Value, IsUiQuestion = q.IsUiQuestion })
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
        public ActionResult<Entity> Post([FromBody, Required] string answerText)
        {
            bool alreadyExists = Select().Any(a => answerText.ToLower() == a.Value.ToLower());
            if (alreadyExists)
            {
                return BadRequest("Answer already exists");
            }
            var answer = new Answer
            {
                Value = answerText,
                Question = new List<Question>()
            };
            _context.Answers.Add(answer);
            _context.SaveChanges();
            return new Entity { Id = answer.Id, Value = answer.Value };
        }

        [HttpDelete("delete/{answerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> Delete(int answerId)
        {
            Answer answer = null;
            try
            {
                answer = Select()
                    .Include(a => a.Question)
                    .First(a => answerId == a.Id);
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
            if (answer.Question.Take(1).Count() > 0)
            {
                return BadRequest("Some questions depends on this answer");
            }
            _context.Answers.Remove(answer);
            _context.SaveChanges();
            return new Entity { Id = answer.Id, Value = answer.Value };
        }
    }
}
