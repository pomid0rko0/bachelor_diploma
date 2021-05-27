using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntentsController : Selector<IntentsController, Intent>
    {
        public IntentsController(QAContext context, ILogger<IntentsController> logger)
            : base(context, logger)
        {
        }

        [HttpGet("get/{intentName}/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<int> GetIntentId([Required, FromQuery] string intentName)
        {
            try
            {
                return Select().First(i => i.IntentName == intentName).IntentId;
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
        }

        [HttpGet("get/{intentId}/name")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<string> GetIntentName(int intentId)
        {
            try
            {
                return Select().First(i => i.IntentId == intentId).IntentName;
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
        }

        [HttpGet("get/{intentId}/answer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Answer> GetAnswer(int intentId)
        {
            try
            {
                return Select()
                    .Include(i => i.Answer)
                    .First(i => i.IntentId == intentId)
                    .Answer;
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
        }

        [HttpGet("get/{intentId}/questions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Question>> GetQuestions(int intentId)
        {
            try
            {
                return Select()
                    .Include(i => i.Question)
                    .First(i => i.IntentId == intentId)
                    .Question
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<Intent> Post([Required] string intentName)
        {
            const string regexString = "[a-zA-Z_0-9]+";
            if (!new Regex(regexString).IsMatch(intentName))
            {
                return BadRequest("intentName must satisfy regular expression " + regexString);
            }
            bool alreadyExists = Select().Any(i => intentName == i.IntentName);
            if (alreadyExists)
            {
                return BadRequest("Intent already exists");
            }
            var intent = new Intent
            {
                IntentName = intentName,
                Question = new List<Question>()
            };
            _context.Intents.Add(intent);
            _context.SaveChanges();
            return intent;
        }

        [HttpDelete("delete/{intentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Intent> DeleteById(int intentId)
        {
            Intent intent = null;
            try 
            {
                intent = Select()
                    .Include(i => i.Question.Take(1))
                    .First(i => intentId == i.IntentId);
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
            if (intent.Question.Count() > 0)
            {
                return BadRequest("Intent depends on some questions");
            }
            if (intent.AnswerId != null)
            {
                return BadRequest("Intent depends on some answer");
            }
            _context.Intents.Remove(intent);
            _context.SaveChanges();
            return intent;
        }
    }
}
