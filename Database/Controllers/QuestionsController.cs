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
                    return _context.Questions.Where(q => q.Intent.Id == intent.Id).Take(1).ToList();
                }
                catch (Exception)
                {
                    return new List<Question>();
                }
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
            bool alreadyExists = _context.Questions.Any(q => q.Text == text);
            if (alreadyExists) return BadRequest("Such question already exists");
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
            alreadyExists = _context.Questions.Any(                    
                q => (intentId == null || intentId == q.Intent.Id) &&
                    (intentName == null || intentName == q.Intent.Name)
            );
            if (alreadyExists) return BadRequest("Question with such intent already exists");
            var question = new Question
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
            Question? question = null;
            try 
            {
                question = _context.Questions.Single(
                    q => (intentId == null || intentId == q.Intent.Id) &&
                        (intentName == null || intentName == q.Intent.Name) &&
                        (String.IsNullOrEmpty(text) || q.Text == text)
                );
            }
            catch (Exception)
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
