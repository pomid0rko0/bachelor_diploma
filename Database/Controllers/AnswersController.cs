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
                Intent? intent = null;
                try
                {
                    intent = _context.Intents.Single(
                        i => (intentId == null || intentId == i.Id) &&
                             (intentName == null || intentName == i.Name)
                    );
                }
                catch (Exception)
                {
                    return NotFound("No such intent");
                }
                try
                {
                    return _context.Answers.Where(a => a.Intent.Id == intent.Id).Take(1).ToList();
                }
                catch (Exception)
                {
                    return new List<Answer>();
                }
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
            bool alreadyExists = _context.Answers.Any(a => a.Text == text);
            if (alreadyExists) return BadRequest("Such answer already exists");
            Intent? intent = null;
            try
            {
                intent = _context.Intents.Single(
                    i => (intentId == null || intentId == i.Id) &&
                         (intentName == null || intentName == i.Name)
                );
            }
            catch (Exception)
            {
                return NotFound("No such intent");
            }
            alreadyExists = _context.Answers.Any(                    
                a => (intentId == null || intentId == a.Intent.Id) &&
                    (intentName == null || intentName == a.Intent.Name)
            );
            if (alreadyExists) return BadRequest("Answer with such intent already exists");
            var answer = new Answer
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
            Answer? answer = null;
            try 
            {
                answer = _context.Answers.Single(
                    a => (intentId == null || intentId == a.Intent.Id) &&
                        (intentName == null || intentName == a.Intent.Name) &&
                        (String.IsNullOrEmpty(text) || a.Text == text)
                );
            }
            catch (Exception)
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
