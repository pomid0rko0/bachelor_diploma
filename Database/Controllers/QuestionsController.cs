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
        public ActionResult<IEnumerable<Question>> GetAll()
        {
            return _context.Questions.ToList();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Question>> GetN([FromQuery] int size)
        {
            if (size < 1)
            {
                return BadRequest("size can't be < 1");
            }
            return _context.Questions.Take(Math.Min(size, _context.Questions.Count())).ToList();
        }

        [HttpGet("{intent}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Question>> GetWithIntents([FromQuery] string[] intents)
        {
            if (intents.Count() < 1)
            {
                return BadRequest("At least 1 intent required");
            }
            return _context.Questions
                .Where(q => q.Intents.Any(intent => intents.Contains(intent.Name)))
                .ToList();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Question> Post(string[] intents, string text)
        {
            bool alreadyExists = _context.Questions.Any(q => q.Text == text);
            if (alreadyExists) return BadRequest("Such question already exists");
            var realIntents = _context.Intents.Where(intent => intents.Contains(intent.Name));
            var difference = intents.Where(intent => realIntents.All(i => i.Name != intent));
            if (difference.Count() > 0) return NotFound("Theres is not existing intents");
            var question = new Question
            {
                Intents = realIntents.ToList(),
                Text = text
            };
            var q = _context.Questions.Add(question).Entity;
            foreach (var intent in realIntents)
            {
                intent.Questions.Add(q);
            }
            _context.SaveChanges();
            return q;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Question> Delete(string text)
        {
            Question? question = null;
            try
            {
                question = _context.Questions.Single(q => q.Text == text);
            }
            catch (Exception)
            {
                return BadRequest("Question not found");
            }
            _context.Questions.Remove(question);
            foreach (var intent in _context.Intents)
            {
                intent.Questions.Remove(question);
            }
            _context.SaveChanges();
            return question;
        }
    }
}
