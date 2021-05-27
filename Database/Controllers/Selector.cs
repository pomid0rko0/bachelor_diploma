using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

using Database.Data;

namespace Database.Controllers
{
    public class Selector<C, M> : ControllerBase
    where M : class
    where C : ControllerBase
    {
        protected readonly ILogger<C> _logger;
        protected readonly QAContext _context;

        protected IQueryable<M> Select()
        {
            return _context.Set<M>();
        }

        [HttpGet("get/all")]
        public ActionResult<IEnumerable<M>> GetAll(
            [Required, Range(0, Int32.MaxValue)] int offset = 0,
            [Required, Range(0, 1000)] int size = 1000
        )
        {
            return Select().Skip(offset).Take(size).ToList();
        }
        
        public Selector(QAContext context, ILogger<C> logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}
