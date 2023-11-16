using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfobarAPI.Data;
using InfobarAPI.Models;

namespace InfobarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColaboradoresController : ControllerBase
    {
        private readonly InfoDbContext _context;

        public ColaboradoresController(InfoDbContext context)
        {
            _context = context;
        }

        // GET: api/Colaboradores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Colaborador>>> GetColaboradores()
        {
            if (_context.Colaboradores == null)
            {
                return NotFound();
            }

            var colaboradores =  await _context.Colaboradores.ToListAsync();

            return colaboradores;
        }

        // GET: api/Colaboradores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Colaborador>> GetColaborador(int id)
        {
            if (_context.Colaboradores == null)
            {
                return NotFound();
            }
            var colaborador = await _context.Colaboradores.FindAsync(id);

            if (colaborador == null)
            {
                return NotFound();
            }

            return colaborador;
        }

        // PUT: api/Colaboradores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColaborador(int id, Colaborador colaborador)
        {
            if (id != colaborador.IdCol)
            {
                return BadRequest();
            }

            _context.Entry(colaborador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColaboradorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("EditColaborador{id}")]
        public async Task<IActionResult> PutColaborador(int id, ColaboradorInputModel model)
        {

            var itemExistente = _context.Colaboradores.Find(id);

            if (itemExistente == null)
            {
                return NotFound(); // O item com o ID fornecido não foi encontrado
            }

            // Verifique se o ID é válido (por exemplo, maior que zero)
            if (id <= 0)
            {
                return BadRequest("ID inválido");
            }

            // Atualize as propriedades do item existente com as do novo item
            itemExistente.Nome = model.Nome;
            itemExistente.Cargo = model.Cargo;
            itemExistente.Credencial = model.Credencial;
            itemExistente.Senha = model.Senha;
            itemExistente.Email = model.Email;
            itemExistente.DataNascimento = model.DataNascimento;

            // Salve as mudanças no banco de dados
            _context.SaveChanges();

            return Ok("Item atualizado com sucesso");

        }

        // POST: api/Colaboradores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Colaborador>> PostColaborador(Colaborador colaborador)
        {
          if (_context.Colaboradores == null)
          {
              return Problem("Entity set 'InfoDbContext.Colaboradores'  is null.");
          }
            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetColaborador", new { id = colaborador.IdCol }, colaborador);
        }

        [HttpPost("AddColaborador")]
        public async Task<ActionResult<Colaborador>> PostColaborador(ColaboradorInputModel model)
        {
            var colaborador = new Colaborador
            {
                Nome = model.Nome,
                Cargo = model.Cargo,
                Credencial = model.Credencial,
                Senha = model.Senha,
                Email = model.Email,
                DataNascimento = model.DataNascimento
            };

            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetColaborador", new { id = colaborador.IdCol }, colaborador);
        }


        // DELETE: api/Colaboradores/5
        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColaborador(int id)
        {
            if (_context.Colaboradores == null)
            {
                return NotFound();
            }
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador == null)
            {
                return NotFound();
            }

            _context.Colaboradores.Remove(colaborador);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

        
        [HttpDelete("DeleteCol{id}")]
        public async Task<IActionResult> DeleteColaborador(int id)
        {
            if (_context.Colaboradores == null)
            {
                return NotFound();
            }
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador == null)
            {
                return NotFound();
            }

            var deletedColaborador = new
            {
                IdCol = colaborador.IdCol,
                Nome = colaborador.Nome,
                Cargo = colaborador.Cargo,
                Credencial = colaborador.Credencial,
                Senha = colaborador.Senha,
                Email = colaborador.Email,
                DataNascimento = colaborador.DataNascimento,
            };

            _context.Colaboradores.Remove(colaborador);
            await _context.SaveChangesAsync();

            return Ok(deletedColaborador);
        }
        private bool ColaboradorExists(int id)
        {
            return (_context.Colaboradores?.Any(e => e.IdCol == id)).GetValueOrDefault();
        }
    }
}
