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

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Question>> Get(
            [FromQuery] int? size, 
            [FromQuery] int? questionId, 
            [FromQuery] string questionText, 
            [FromQuery] int?[] intentIds, 
            [FromQuery] string[] intentNames
        )
        {
            var N = size ?? _context.Questions.Count();
            if (N < 0)
            {
                return BadRequest("size can't be < 0");
            }
            return _context
                .Questions
                .Where(q => (questionId == null && 
                             questionText == null && 
                             intentNames == null && 
                             intentIds == null
                            ) || 
                            q.QuestionText == questionText || 
                            q.Intent.Where(
                                Intent => intentIds == null || intentIds.Contains(Intent.IntentId) ||
                                          intentNames == null || intentNames.Contains(Intent.IntentName)
                            ).Take(1).Count() > 0
                      )
                .Take(N)
                .ToList();
        }
        
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<Question> Post(string questionText, string[] intentNames)
        {
            if (intentNames.Take(1).Count() == 0)
            {
                return BadRequest("At least one intent required");
            }
            bool alreadyExists = _context.Questions.Any(a => a.QuestionText == questionText);
            if (alreadyExists) return BadRequest("Question already exists");
            var Intents = _context.Intents.Where(Intent => intentNames.Contains(Intent.IntentName));
            if (intentNames.Any(intentName => !Intents.Select(Intent => Intent.IntentName).Contains(intentName)))
            {
                return NotFound("Some intents does not exists");
            }
            var Question = new Question
            {
                Intent = Intents.ToList(),
                QuestionText = questionText
            };
            _context.Questions.Add(Question);
            _context.SaveChanges();

            foreach (var Intent in Intents)
            {
                Intent.Question ??= new List<Question>();
                Intent.Question.Add(Question);
            }
            _context.SaveChanges();
            
            return Question;
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Question> Delete(int questionId)
        {
            Question Question = null;
            try
            {
                Question = _context.Questions.Single(q => q.QuestionId == questionId);
            }
            catch (Exception)
            {
                return BadRequest("Question not found");
            }

            foreach (var Intent in Question.Intent)
            {
                Intent.Question.Remove(Question);
            }
            _context.SaveChanges();

            _context.Questions.Remove(Question);
            _context.SaveChanges();
            return Question;
        }
    }
}
