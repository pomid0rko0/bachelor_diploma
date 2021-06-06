using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models;
using Database.Domain;

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
        public ActionResult<IEnumerable<EntityQuestion>> GetQuestion(int id)
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
        public ActionResult<EntityAnswer> Post([FromBody, Required] AddAnswerRequest Answer)
        {
            bool alreadyExists = Select().Any(a => Answer.Text.ToLower() == a.Value.ToLower());
            if (alreadyExists)
            {
                return BadRequest("Already exists");
            }
            var answer = new Answer
            {
                Value = Answer.Text,
                FullAnswerUrl = Answer.Url,
                Question = new List<Question>()
            };
            _context.Answers.Add(answer);
            _context.SaveChanges();
            return new EntityAnswer { Id = answer.Id, Value = answer.Value, FullAnswerUrl = answer.FullAnswerUrl };
        }

        [HttpPut("update/{id}/full_answer_url")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<EntityAnswer> UpdateFullAnswerUrl([Required] int id, [FromBody, Required] string fullAnswerUrl)
        {
            try
            {
                var a = Select().First(a => a.Id == id);
                a.FullAnswerUrl = fullAnswerUrl;
                _context.SaveChanges();
                return new EntityAnswer { Id = a.Id, Value = a.Value, FullAnswerUrl = a.FullAnswerUrl };
            }
            catch
            {
                return NotFound("Not found");
            }
        }
        
        [HttpDelete("delete/{answerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<EntityAnswer> Delete(int answerId)
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
            return new EntityAnswer { Id = answer.Id, Value = answer.Value };
        }
    }
}
