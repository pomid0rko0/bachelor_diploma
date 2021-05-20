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
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Question>> Get(int? intentId = null, string? intentName = null, int? size = null)
        {
            var intent = _context.Intents.Where(i => i.IsSame(intentId, intentName)).First();
            if (intent == null)
            {
                return NotFound("No such intent");
            }
            int s = size ?? _context.Questions.Count();
            return _context.Questions.Where(q => q.Intent.Id == intent.Id).Take(s).ToArray();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Question> Post(string text, int? intentId = null, string? intentName = null)
        {
            bool alreadyExist = _context.Questions.Any(q => q.Text == text && q.Intent.IsSame(intentId, intentName));
            if (alreadyExist)
            {
                return BadRequest("Such question already exists");
            }
            var intent = _context.Intents.Where(i => i.IsSame(intentId, intentName)).First();
            if (intent == null)
            {
                return NotFound("Such intent not found");
            }
            Question question = new Question
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
            var question = _context.Questions.Where(q => q.Intent.IsSame(intentId, intentName) && (q.Text == text || String.IsNullOrEmpty(text))).First();
            if (question == null)
            {
                return NotFound("No such question");
            }
            _context.Questions.Remove(question);
            question.Intent.Questions.Remove(question);
            _context.SaveChanges();
            return question;
        }
    }
}
