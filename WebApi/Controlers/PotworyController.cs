using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controlers
{
    
    [ApiController]
    [Route("[controller]")]
    public class PotworyController : ControllerBase
    {
        private DatabaseContext context { get; set; }

        public PotworyController(DatabaseContext con)
        {
            context = con;
        }

        /// <summary>
        /// Zwraca wszystkie potwory z bazy danych
        /// </summary>
        
        [HttpGet, Authorize]
        public IEnumerable<Potwory> Get()
        {
            return context.Potwory.ToList();
        }

        /// <summary>
        /// Zwraca jednego potwora, o podanym id z bazy danych
        /// </summary>
        
        [HttpGet("{id}"), Authorize]
        public Potwory GetOne(int id)
        {
            return context.Potwory.SingleOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Usuwa wybranego potwora z baczy dnaych
        /// </summary>
        
        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public void Delete(int id)
        {
            var pot = context.Potwory.FirstOrDefault(t => t.Id == id);
            if (pot != null)
            {
                context.Potwory.Remove(pot);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Aktualizuje dane wybranego potwora
        /// </summary>
        
        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public bool Update(int id, [FromBody] Potwory pot)
        {
            context.Potwory.Update(pot);
            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Dodaje do bazy danych nowego potwora
        /// </summary>
        
        [HttpPost, Authorize(Roles = "admin")]
        public bool Create([FromBody] Potwory pot)
        {
            context.Potwory.Add(pot);
            context.SaveChanges();
            return true;

        }
    }

}