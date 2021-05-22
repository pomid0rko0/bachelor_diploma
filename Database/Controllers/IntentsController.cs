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

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Intent>> Get(
            [FromQuery] int? size, 
            [FromQuery] int?[] intentIds, 
            [FromQuery] string[] intentNames
        )
        {
            var N = size ?? _context.Intents.Count();
            if (N < 0)
            {
                return BadRequest("size can't be < 0");
            }
            return _context
                .Intents
                .Where(i => (intentIds == null && intentNames == null) ||
                             (intentIds != null && intentIds.Contains(i.IntentId)) ||
                             (intentNames != null && intentNames.Contains(i.IntentName))
                            )
                .Take(N)
                .ToList();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<Intent> Post(string intentName)
        {
            if (String.IsNullOrWhiteSpace(intentName))
            {
                return BadRequest("intentName must be defined");
            }
            bool alreadyExists = _context.Intents.Any(i => intentName == i.IntentName);
            if (alreadyExists)
            {
                return BadRequest("Intent already exists");
            }
            Intent Intent = new Intent
            {
                IntentName = intentName,
                Question = new List<Question>()
            };
            _context.Intents.Add(Intent);
            _context.SaveChanges();
            return Intent;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Intent> Delete(int intentId)
        {
            Intent Intent = null;
            try
            {
                Intent = _context.Intents.Single(i => i.IntentId == intentId);
            }
            catch (Exception)
            {
                return NotFound("intent not found");
            }
            if (Intent.Question != null && Intent.Question.Count() > 0)
            {
                return BadRequest("Some questions depends on this intent");
            }
            if (Intent.AnswerId != null)
            {
                return BadRequest("Some answer depends on this intent");
            }
            _context.Intents.Remove(Intent);
            _context.SaveChanges();
            return Intent;
        }
    }
}
