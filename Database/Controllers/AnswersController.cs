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
    public class AnswersController : EntitiesController<AnswersController, Answer>
    {

        public AnswersController(QAContext context, ILogger<AnswersController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/{id}/intent")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> GetAnswerIntent(int id)
        {
            try
            {
                var i = Select()
                    .Include(a => a.Intent)
                    .First(answer => answer.Id == id)
                    .Intent;
                return new Entity { Id = i.Id, Value = i.Value };
            }
            catch (Exception)
            {
                return NotFound("Answer not found");
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> Post([Required] string answerText, [Required] int intentId)
        {
            bool alreadyExists = Select().Any(a => answerText == a.Value || intentId == a.IntentId);
            if (alreadyExists)
            {
                return BadRequest("Answer already exists");
            }
            Intent intent = null;
            try
            {
                intent = _context
                    .Intents
                    .Include(i => i.Answer)
                    .First(i => intentId == i.Id);
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
            var answer = new Answer
            {
                Value = answerText,
                IntentId = intentId,
                Intent = intent
            };
            _context.Answers.Add(answer);
            _context.SaveChanges();
            intent.AnswerId = answer.Id;
            intent.Answer = answer;
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
                    .Include(a => a.Intent)
                    .First(a => answerId == a.Id);
            }
            catch (Exception)
            {
                return NotFound("Answer not found");
            }
            _context.Answers.Remove(answer);
            _context.SaveChanges();
            answer.Intent.AnswerId = null;
            answer.Intent.Answer = null;
            _context.SaveChanges();
            return new Entity { Id = answer.Id, Value = answer.Value };
        }
    }
}
