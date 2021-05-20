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
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Intent>> Get(int? intentId = null, string? intentName = null, int? size = null)
        {
            if ((intentId != null || intentName != null) && size != null && size != 1)
            {
                return BadRequest("Only one parameter avaiable at time: intent or number of answers");
            }            
            if ((size == null || size == 1) && (intentId != null || intentName != null))
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
                return new List<Intent> { intent };
            }
            int s = size ?? _context.Intents.Count();
            return _context.Intents.Take(s).ToList();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<Intent> Post(string intentName)
        {
            bool alreadyExists = _context.Intents.Any(i => intentName == i.Name);
            if (alreadyExists)
            {
                return BadRequest("Such intent already exists");
            }
            Intent intent = new Intent
            {
                Name = intentName,
                Questions = new List<Question>(),
                Answers = new List<Answer>(),
            };
            var i = _context.Intents.Add(intent).Entity;
            _context.SaveChanges();
            return i;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Intent> Delete(int? intentId = null, string? intentName = null)
        {
            if (intentId == null && intentName == null)
            {
                return BadRequest("Either intentId or intentName must be defined");
            }
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
            _context.Intents.Remove(intent);
            _context.SaveChanges();
            return intent;
        }
    }
}
