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
    public class IntentsController : ControllerBase
    {
        private readonly ILogger<IntentsController> _logger;
        private readonly QAContext _context;

        public IntentsController(QAContext context, ILogger<IntentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

#nullable enable
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Intent>> GetAll()
        {
            return _context.Intents.ToList();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Intent>> GetN([FromQuery] int size)
        {
            if (size < 1)
            {
                return BadRequest("size can't be < 1");
            }
            return _context.Intents.Take(Math.Min(size, _context.Intents.Count())).ToList();
        }

        [HttpGet("{intent}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> GetByName([FromQuery] string intent)
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
        public ActionResult<Intent> Post(string intent)
        {
            bool alreadyExists = _context.Intents.Any(i => intent == i.Name);
            if (alreadyExists)
            {
                return BadRequest("Such intent already exists");
            }
            Intent i = new Intent
            {
                Name = intent,
                Questions = new List<Question>(),
                Answers = new List<Answer>(),
            };
            var e = _context.Intents.Add(i).Entity;
            _context.SaveChanges();
            return e;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Intent> Delete(string intent)
        {
            Intent? i = null;
            try
            {
                i = _context.Intents.Single(i => intent == i.Name);
            }
            catch (Exception)
            {
                return NotFound("No such intent");
            }
            if (i.Questions.Count() > 0)
            {
                return BadRequest("Some questions depends on this intent");
            }
            if (i.Answers.Count() > 0)
            {
                return BadRequest("Some answers depends on this intent");
            }
            _context.Intents.Remove(i);
            _context.SaveChanges();
            return i;
        }
    }
}
