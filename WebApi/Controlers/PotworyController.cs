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
        SqliteConnection connection;
        List<Potwory> pot;

        public PotworyController(DatabaseContext con)
        {
            context = con;
            connection = new SqliteConnection("Data Source=Potwory.db");
            pot = new List<Potwory>();
            fill_list();
        }

        /// <summary>
        /// Zwraca wszystkie potwory z bazy danych
        /// </summary>
        
        [HttpGet, Authorize]
        public ActionResult<IEnumerable<Potwory>> Get([FromQuery]string sort, [FromQuery]string filtr)
        {
            List<Potwory> tmp = pot.Select(x => new Potwory { Id = x.Id, Img = x.Img, Nazwa = x.Nazwa, Opis = x.Opis }).ToList();
            if (!String.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "nazasc":
                        tmp = tmp.OrderBy(x => x.Nazwa).ToList();
                        break;
                    case "nazdecs":
                        tmp = tmp.OrderByDescending(x => x.Nazwa).ToList();
                        break;
                    case "opasc":
                        tmp = tmp.OrderBy(x => x.Opis).ToList();
                        break;
                    case "opdesc":
                        tmp = tmp.OrderByDescending(x => x.Opis).ToList();
                        break;
                }
            }
            if (!String.IsNullOrEmpty(filtr))
            {
                tmp = tmp.Where(x => x.Nazwa == filtr).ToList();
            }
            return tmp;
            //return context.Potwory.ToList();
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

        void fill_list()
        {
            pot.Clear();
            string cmd = "select * from Potwory;";
            SqliteCommand sql_cmd = new SqliteCommand(cmd, connection);
            connection.Open();
            SqliteDataReader data = sql_cmd.ExecuteReader();
            while (data.Read())
            {
                long id = (long)data[0];
                string naz = (string)data[1];
                string img = (string)data[2];
                string opi = (string)data[3];
                pot.Add(new Potwory { Id = (int)id, Nazwa = naz, Img = img, Opis = opi });
            }
            
            connection.Close();
        }
       
    }

}