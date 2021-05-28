using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Database.Data;
using Database.Models.QA;

namespace Database.Controllers
{
    public class EntitiesController<C, M> : ControllerBase
    where M : Entity
    where C : ControllerBase
    {
        protected readonly ILogger<C> _logger;
        protected readonly QAContext _context;

        protected IQueryable<M> Select()
        {
            return _context.Set<M>().OrderBy(e => e.Id);
        }
        
        public EntitiesController(QAContext context, ILogger<C> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("get/all")]
        public ActionResult<IEnumerable<Entity>> GetAll(
            [Required, Range(0, Int32.MaxValue)] int offset = 0,
            [Required, Range(0, 1000)] int size = 1000
        )
        {
            return Select().Skip(offset).Take(size).Select(e => new Entity { Id = e.Id, Value = e.Value }).ToList();
        }
        

        [HttpGet("get/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]        
        public ActionResult<Entity> GetById([Required] int id)
        {
            try
            {
                var e = Select().Single(e => e.Id == id);
                return new Entity { Id = e.Id, Value = e.Value };
            }
            catch
            {
                return NotFound();
            }
        }
        
        [HttpGet("find")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]        
        public ActionResult<Entity> Find([Required] string value)
        {
            try
            {
                var e = Select().Single(e => e.Value == value);
                return new Entity { Id = e.Id, Value = e.Value };
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
