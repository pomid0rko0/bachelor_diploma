using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models;
using Database.Models.Entities;

namespace Database.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : EntitiesController<QuestionsController, Question, EntityQuestion>
    {

        public QuestionsController(QAContext context, ILogger<QuestionsController> logger)
            : base(context, logger, Question.WithoutReferences)
        {
        }

        [HttpGet("get/{id}/subtopic")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<EntitySubtopic> GetQuestionSubtopic(int id)
        {
            try
            {
                return Select()
                    .Include(q => q.Subtopic)
                    .First(q => q.Id == id)
                    .Subtopic
                    .WithoutReferences();

            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpGet("get/{id}/answer")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<EntityAnswer> GetQuestionAnswer(int id)
        {
            try
            {
                return Select()
                    .Include(q => q.Answer)
                    .First(q => q.Id == id)
                    .Answer
                    .WithoutReferences();
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpGet("get/ui_questions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<ICollection<EntityQuestion>> GetUiQuestions()
        {
            try
            {
                return Select()
                    .Where(q => q.IsUiQuestion)
                    .Select(Question.WithoutReferences)
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<EntityQuestion> Post(
            [FromBody, Required] string questionText,
            [Required] int answerId,
            [Required] int subtopicId,
            bool? isUiQuestion = false
        )
        {
            bool isUi = isUiQuestion ?? false;
            bool alreadyExists = Select().Any(q => questionText == q.Value);
            if (alreadyExists)
            {
                return BadRequest("Already exists");
            }
            Subtopic subtopic = null;
            try
            {
                subtopic = _context
                    .Subtopics
                    .Include(st => st.Question)
                    .First(subtopic => subtopicId == subtopic.Id);
            }
            catch (Exception)
            {
                return NotFound("Subtopic not found");
            }
            Answer answer = null;
            try
            {
                answer = _context
                    .Answers
                    .Include(a => a.Question)
                    .First(a => a.Id == answerId);
            }
            catch (Exception)
            {
                return NotFound("Answer not found");
            }
            var question = new Question
            {
                IsUiQuestion = isUi,
                Value = questionText,
                SubtopicId = subtopicId,
                Subtopic = subtopic,
                AnswerId = answerId,
                Answer = answer
            };
            _context.Questions.Add(question);
            _context.SaveChanges();
            answer.Question.Add(question);
            subtopic.Question.Add(question);
            _context.SaveChanges();
            return question.WithoutReferences();
        }

        [HttpPut("update/{id}/is_ui_question")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<EntityQuestion> UpdateIsUiQuestion([Required] int id, [FromQuery, Required] bool isUiQuestion)
        {
            try
            {
                var q = Select().First(q => q.Id == id);
                q.IsUiQuestion = isUiQuestion;
                _context.SaveChanges();
                return q.WithoutReferences();
            }
            catch
            {
                return NotFound("Not found");
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<EntityQuestion> Delete(int id)
        {
            Question question = null;
            try
            {
                question = Select()
                    .Include(q => q.Answer)
                    .First(q => id == q.Id);
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
            _context.Questions.Remove(question);
            _context.SaveChanges();
            question.Answer.Question.Remove(question);
            _context.SaveChanges();
            return question.WithoutReferences();
        }
    }
}
