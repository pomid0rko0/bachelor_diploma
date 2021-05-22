using System;
using System.Text.Encodings;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Database.Data;
using Database.Models;
using System.ComponentModel.DataAnnotations;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnswersController : ControllerBase
    {
        private readonly ILogger<AnswersController> _logger;
        private readonly QAContext _context;

        public AnswersController(QAContext context, ILogger<AnswersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Answer>> Get(
            [FromQuery] int? size, 
            [FromQuery] int? answerId, 
            [FromQuery] string answerText, 
            [FromQuery] int?[] intentIds, 
            [FromQuery] string[] intentNames
        )
        {
            var N = size ?? _context.Answers.Count();
            if (N < 0)
            {
                return BadRequest("size can't be < 0");
            }
            return _context
                .Answers
                .Where(a => (answerId == null && 
                             answerText == null && 
                             intentNames == null && 
                             intentIds == null
                            ) || 
                            a.AnswerText == answerText || 
                            (intentIds != null && intentIds.Contains(a.Intent.IntentId)) ||
                            (intentNames != null && intentNames.Contains(a.Intent.IntentName))
                      )
                .Take(N)
                .ToList();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Post([Required] string answerText, [Required] string intentName)
        {
            bool alreadyExists = _context.Answers.Any(a => a.AnswerText == answerText ||
                                                           a.Intent.IntentName == intentName
                                                     );
            if (alreadyExists)
            {
                return BadRequest("Answer already exists");
            }
            Intent Intent = null;
            try
            {
                Intent = _context.Intents.Single(i => i.IntentName == intentName);
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
            Answer Answer = new Answer
            {
                AnswerText = answerText,
                IntentId = Intent.IntentId,
                Intent = Intent,
            };

            _context.Answers.Add(Answer);
            _context.SaveChanges();

            Intent.AnswerId = Answer.AnswerId;
            Intent.Answer = Answer;
            _context.SaveChanges();
            return Intent.Answer;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Delete(int answerId)
        {
            Answer Answer = null;
            try
            {
                Answer = _context.Answers.Single(a => a.AnswerId == answerId);
            }
            catch (Exception)
            {
                return BadRequest("Answer not found");
            }
            Answer.Intent.AnswerId = null;
            Answer.Intent.Answer = null;
            _context.SaveChanges();

            _context.Answers.Remove(Answer);
            _context.SaveChanges();
            return Answer;
        }
    }
}
