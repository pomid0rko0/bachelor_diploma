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
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Answer>> Get(int? intentId = null, string? intentName = null, int? size = null)
        {
            var intent = _context.Intents.Where(i => i.IsSame(intentId, intentName)).First();
            if (intent == null)
            {
                return NotFound("No such intent");
            }
            int s = size ?? _context.Answers.Count();
            return _context.Answers.Where(a => a.Intent.Id == intent.Id).Take(s).ToArray();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Post(string text, int? intentId = null, string? intentName = null)
        {
            bool alreadyExist = _context.Answers.Any(a => a.Text == text && a.Intent.IsSame(intentId, intentName));
            if (alreadyExist)
            {
                return BadRequest("Such answer already exists");
            }
            var intent = _context.Intents.Where(i => i.IsSame(intentId, intentName)).First();
            if (intent == null)
            {
                return NotFound("Such intent not found");
            }
            Answer answer = new Answer
            {
                Intent = intent,
                Text = text
            };
            var a = _context.Answers.Add(answer).Entity;
            intent.Answers.Add(a);
            return a;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> Delete(int? intentId = null, string? intentName = null, string? text = null)
        {
            var answer = _context.Answers.Where(a => a.Intent.IsSame(intentId, intentName) && (a.Text == text || String.IsNullOrEmpty(text))).First();
            if (answer == null)
            {
                return NotFound("No such answer");
            }
            _context.Answers.Remove(answer);
            answer.Intent.Answers.Remove(answer);
            return answer;
        }
    }
}
