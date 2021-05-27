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
    public class AnswersController : Selector<AnswersController, Answer>
    {

        public AnswersController(QAContext context, ILogger<AnswersController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<int> GetAnswerId([Required, FromQuery] string answerText)
        {
            try
            {
                return Select().First(a => a.AnswerText == answerText).AnswerId;
            }
            catch (Exception)
            {
                return NotFound("Answer not found");
            }
        }

        [HttpGet("get/{answerId}/text")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<string> GetAnswerText(int answerId)
        {
            try
            {
                return Select().First(answer => answer.AnswerId == answerId).AnswerText;
            }
            catch (Exception)
            {
                return NotFound("Answer not found");
            }
        }

        [HttpGet("get/{answerId}/intent")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Intent> GetAnswerIntent(int answerId)
        {
            try
            {
                return Select()
                    .Include(a => a.Intent)
                    .First(answer => answer.AnswerId == answerId)
                    .Intent;
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
        public ActionResult<Answer> Post([Required] string answerText, [Required] int intentId)
        {
            bool alreadyExists = Select().Any(a => answerText == a.AnswerText || intentId == a.IntentId);
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
                    .First(i => intentId == i.IntentId);
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
            var answer = new Answer
            {
                AnswerText = answerText,
                IntentId = intentId,
                Intent = intent
            };
            _context.Answers.Add(answer);
            _context.SaveChanges();
            intent.AnswerId = answer.AnswerId;
            intent.Answer = answer;
            _context.SaveChanges();
            return answer;
        }

        [HttpDelete("delete/{answerId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> DeleteById(int answerId)
        {
            Answer answer = null;
            try 
            {
                answer = Select()
                    .Include(a => a.Intent)
                    .First(a => answerId == a.AnswerId);
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
            return answer;
        }
    }
}
