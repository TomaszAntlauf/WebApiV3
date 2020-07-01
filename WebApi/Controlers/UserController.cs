using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controlers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private DatabaseContext _context { get; set; }

        public UserController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Zwraca wszystkich użytkowników z bazy danych
        /// </summary>
        [HttpGet, Authorize]
        public IEnumerable<User> Get()
        {
            return _context.User.ToList();
        }

        /// <summary>
        /// Zwraca jednego użytkownika, o podanym id z bazy danych
        /// </summary>
        [HttpGet("{id}"), Authorize]
        public User GetOne(int id)
        {
            return _context.User.SingleOrDefault(m => m.Id == id);
        }
    }
}