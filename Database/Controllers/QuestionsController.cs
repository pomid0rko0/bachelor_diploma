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
    public class QuestionsController : ControllerBase
    {
        private readonly ILogger<QuestionsController> _logger;
        private readonly QAContext _context;

        public QuestionsController(QAContext context, ILogger<QuestionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

#nullable enable
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Question>> Get(int? intentId = null, string? intentName = null, int? size = null)
        {
            if ((intentId != null || intentName != null) && size != null && size != 1)
            {
                return BadRequest("Only one parameter avaiable at time: intent or number of questions");
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
                return _context.Questions.Where(q => q.Intent.Id == intent.Id).Take(1).ToList();
            }
            else 
            {
                int s = size ?? _context.Questions.Count();
                return _context.Questions.Take(s).ToList();
            }
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Question> Post(string text, int? intentId = null, string? intentName = null)
        {
            if (intentId == null && intentName == null)
            {
                return BadRequest("Either intentId or intentName must be defined");
            }
            Question question = _context.Questions.Where(q => q.Text == text).First();
            if (question != null)
            {
                return BadRequest("Such question already exists");
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
            question = _context.Questions
                .Where(                    
                    q => (intentId == null || intentId == q.Intent.Id) &&
                         (intentName == null || intentName == q.Intent.Name)
                )
                .First();
            if (question != null)
            {
                return BadRequest("Question with such intent already exists");
            }
            question = new Question
            {
                Intent = intent,
                Text = text
            };
            var q = _context.Questions.Add(question).Entity;
            intent.Questions.Add(q);
            _context.SaveChanges();
            return q;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Question> Delete(int? intentId = null, string? intentName = null, string? text = null)
        {
            if (intentId == null && intentName == null && text == null)
            {
                return BadRequest("Either intent or text must be defined");
            }
            var question = _context.Questions
                .Where(
                    q => (intentId == null || intentId == q.Intent.Id) &&
                         (intentName == null || intentName == q.Intent.Name) &&
                         (String.IsNullOrEmpty(text) || q.Text == text)
                )
                .First();
            if (question == null)
            {
                return NotFound("No such question with such intent");
            }
            _context.Questions.Remove(question);
            question.Intent.Questions.Remove(question);
            _context.SaveChanges();
            return question;
        }
    }
}
