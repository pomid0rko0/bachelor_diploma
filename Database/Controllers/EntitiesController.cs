using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Database.Data;
using Database.Models.Entities;

namespace Database.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EntitiesController<C, M, R> : ControllerBase
    where M : Entity
    where C : ControllerBase
    {
        protected readonly ILogger<C> _logger;
        protected readonly QAContext _context;
        protected readonly Func<M, R> _unlinker;

        protected IQueryable<M> Select()
        {
            return _context.Set<M>().OrderBy(e => e.Id);
        }

        public EntitiesController(QAContext context, ILogger<C> logger, Func<M, R> unlinker)
        {
            _context = context;
            _logger = logger;
            _unlinker = unlinker;
        }

        [HttpGet("get/all")]
        public ActionResult<IEnumerable<R>> GetAll(
            [Range(0, Int32.MaxValue)] int? offset = 0,
            [Range(0, 1000)] int? size = 1000
        )
        {
            return Select()
                .Skip(offset ?? 0)
                .Take(size ?? 1000)
                .Select(_unlinker)
                .ToList();
        }


        [HttpGet("get/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<R> GetById([Required] int id)
        {
            try
            {
                var e = Select().First(e => e.Id == id);
                return _unlinker(e);
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpGet("find")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<R> Find(
            [Required] string value,
            [RegularExpression("part|full|regex")] string match_type = "part",
            bool? case_sensitivity = false
        )
        {
            var cs = case_sensitivity ?? false;
            match_type ??= "part";
            var string_comparsion = cs ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            try
            {
                M e;
                switch (match_type.ToLower())
                {
                    case "part":
                        e = Select().First(e => e.Value.Contains(value, string_comparsion));
                        break;
                    case "full":
                        e = Select().First(e => e.Value.Equals(value, string_comparsion));
                        break;
                    case "regex":
                        e = Select().First(e => Regex.IsMatch(e.Value, value, cs ? RegexOptions.IgnoreCase : RegexOptions.None));
                        break;
                    default:
                        return BadRequest("match_type must be 'part' | 'full' | 'regex'");
                }
                return _unlinker(e);
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<R> Update([FromBody, Required] string new_value, [Required] int id)
        {
            if (Select().Any(e => e.Value == new_value))
            {
                return BadRequest("Already exists");
            }
            try
            {
                var e = Select().First(e => e.Id == id);
                e.Value = new_value;
                _context.SaveChanges();
                return _unlinker(e);
            }
            catch (Exception)
            {
                return NotFound("Not found");
            }
        }
    }
}
