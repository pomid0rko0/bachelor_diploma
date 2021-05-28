using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models.QA;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntentsController : EntitiesController<IntentsController, Intent>
    {
        public IntentsController(QAContext context, ILogger<IntentsController> logger)
            : base(context, logger)
        {
        }
 
        [HttpGet("get/{intentId}/answer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> GetAnswer(int intentId)
        {
            try
            {
                var a = Select()
                    .Include(i => i.Answer)
                    .First(i => i.Id == intentId)
                    .Answer;
                return new Entity { Id = a.Id, Value = a.Value };
            }
            catch (Exception)
            {
                return NotFound("Intent not found");
            }
        }

        [HttpGet("get/{intentId}/questions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Entity>> GetQuestions(int intentId)
        {
            try
            {
                return Select()
                    .Include(i => i.Question)
                    .First(i => i.Id == intentId)
                    .Question
                    .Select(q => new Entity { Id = q.Id, Value = q.Value })
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
        public ActionResult<Entity> Post([Required] string intentName)
        {
            const string regexString = "[a-zA-Z_0-9]+";
            if (!new Regex(regexString).IsMatch(intentName))
            {
                return BadRequest("intentName must satisfy regular expression " + regexString);
            }
            bool alreadyExists = Select().Any(i => intentName == i.Value);
            if (alreadyExists)
            {
                return BadRequest("Intent already exists");
            }
            var intent = new Intent
            {
                Value = intentName,
                Question = new List<Question>()
            };
            _context.Intents.Add(intent);
            _context.SaveChanges();
            return new Entity { Id = intent.Id, Value = intent.Value };
        }

        [HttpDelete("delete/{intentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Entity> Delete(int intentId)
        {
            Intent intent = null;
            try 
            {
                intent = Select()
                    .Include(i => i.Question.Take(1))
                    .First(i => intentId == i.Id);
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
            return new Entity { Id = intent.Id, Value = intent.Value };
        }
    }
}
