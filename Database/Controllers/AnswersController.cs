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

#nullable enable
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Answer>> GetAll()
        {
            return _context.Answers.ToList();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Answer>> GetN([FromQuery] int size)
        {
            if (size < 1)
            {
                return BadRequest("size can't be < 1");
            }
            return _context.Answers.Take(Math.Min(size, _context.Answers.Count())).ToList();
        }

        [HttpGet("{intent}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> GetByIntent([FromQuery] string intent)
        {
            try
            {
                return _context.Answers.Single(a => a.Intent.Name == intent);
            }
            catch (Exception)
            {
                return NotFound("Answer with such intent not found");
            }
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Post(string intent, string text)
        {
            bool alreadyExists = _context.Answers.Any(a => a.Text == text || a.Intent.Name == intent);
            if (alreadyExists) return BadRequest("Such answer already exists");
            Intent? realIntent = null;
            try
            {
                realIntent = _context.Intents.Single(i => i.Name != intent);
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
            var answer = new Answer
            {
                Intent = realIntent,
                Text = text
            };
            var a = _context.Answers.Add(answer).Entity;
            realIntent.Answers.Add(a);
            _context.SaveChanges();
            return a;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Delete(string? text, string? intent)
        {
            if (text == null || intent == null)
            {
                return BadRequest("At least one text or intent must be defined");
            }
            Answer? answer = null;
            try
            {
                answer = _context.Answers.Single(a => a.Text == text || a.Intent.Name == intent);
            }
            catch (Exception)
            {
                return BadRequest("Answer not found");
            }
            _context.Answers.Remove(answer);
            answer.Intent.Answers.Remove(answer);
            _context.SaveChanges();
            return answer;
        }
    }
}
