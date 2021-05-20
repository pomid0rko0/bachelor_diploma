using System;
using System.Text.Encodings;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Database.Data;
using Database.Models;

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

#nullable enable
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Answer>> Get(int? intentId = null, string? intentName = null, int? size = null)
        {
            if ((intentId != null || intentName != null) && size != null && size != 1)
            {
                return BadRequest("Only one parameter avaiable at time: intent or number of answers");
            }
            if (size != null && size < 0)
            {
                return BadRequest("size can't be < 0");
            }
            if (intentId != null || intentName != null)
            {
                var intent = _context.Intents
                    .Where(
                        i => (intentId == null || intentId == i.Id) &&
                            (intentName == null || intentName == i.Name) &&
                            (intentId != null || intentName != null)
                    )
                    .First();
                if (intent == null)
                {
                    return NotFound("No such intent");
                }
                return _context.Answers.Where(a => a.Intent.Id == intent.Id).Take(1).ToList();
            }
            else 
            {
                int s = size ?? _context.Answers.Count();
                return _context.Answers.Take(s).ToList();
            }
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Post(string text, int? intentId = null, string? intentName = null)
        {
            if (intentId == null && intentName == null)
            {
                return BadRequest("Either intentId or intentName must be defined");
            }
            Answer answer = _context.Answers.Where(a => a.Text == text).First();
            if (answer != null)
            {
                return BadRequest("Such answer already exists");
            }
            var intent = _context.Intents
                .Where(
                    i => (intentId == null || intentId == i.Id) &&
                         (intentName == null || intentName == i.Name)
                )
                .First();
            if (intent == null)
            {
                return NotFound("Such intent not found");
            }
            answer = _context.Answers
                .Where(                    
                    a => (intentId == null || intentId == a.Intent.Id) &&
                         (intentName == null || intentName == a.Intent.Name)
                )
                .First();
            if (answer != null)
            {
                return BadRequest("Answer with such intent already exists");
            }
            answer = new Answer
            {
                Intent = intent,
                Text = text
            };
            var a = _context.Answers.Add(answer).Entity;
            intent.Answers.Add(a);
            _context.SaveChanges();
            return a;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Delete(int? intentId = null, string? intentName = null, string? text = null)
        {
            if (intentId == null && intentName == null && text == null)
            {
                return BadRequest("Either intent or text must be defined");
            }
            var answer = _context.Answers
                .Where(
                    a => (intentId == null || intentId == a.Intent.Id) &&
                         (intentName == null || intentName == a.Intent.Name) &&
                         (String.IsNullOrEmpty(text) || a.Text == text)
                )
                .First();
            if (answer == null)
            {
                return NotFound("No such answer with such intent");
            }
            _context.Answers.Remove(answer);
            answer.Intent.Answers.Remove(answer);
            _context.SaveChanges();
            return answer;
        }
    }
}
