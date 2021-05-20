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
            if ((size == null || size == 1) && (intentId != null || intentName != null))
            {
                var intent = _context.Intents.Where(i => i.IsSame(intentId, intentName)).First();
                if (intent == null)
                {
                    return NotFound("No such intent");
                }
                return new List<Intent> { intent };
            }
            if (size != null && intentId == null && intentName == null)
            {
                int s = size ?? _context.Intents.Count();
                return _context.Intents.Take(s).ToArray();
            }
            return BadRequest("Either search for one intent or take several intents");
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<Intent> Post(string intentName)
        {
            bool alreadyExist = _context.Intents.Any(i => i.IsSame(null, intentName));
            if (alreadyExist)
            {
                return BadRequest("Such intent already exists");
            }

            Intent intent = new Intent
            {
                Name = intentName
            };
            return _context.Intents.Add(intent).Entity;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Intent> Delete(int? intentId = null, string? intentName = null, string? text = null)
        {
            var intent = _context.Intents.Where(i => i.IsSame(intentId, intentName)).First();
            if (intent == null)
            {
                return NotFound("No such intent");
            }
            _context.Intents.Remove(intent);
            return intent;
        }
    }
}
