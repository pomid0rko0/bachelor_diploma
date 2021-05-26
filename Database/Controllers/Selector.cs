using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;

using Database.Data;

namespace Database.Controllers
{
    public class Selector<M> : ControllerBase
    where M : class
    {
        protected readonly ILogger<IntentsController> _logger;
        protected readonly QAContext _context;

        protected IQueryable<M> Select()
        {
            return _context.Set<M>();
        }

        public Selector(QAContext context, ILogger<IntentsController> logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}