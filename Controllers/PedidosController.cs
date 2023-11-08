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
    public class PedidosController : ControllerBase
    {
        private readonly InfoDbContext _context;

        public PedidosController(InfoDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
          if (_context.Pedidos == null)
          {
              return NotFound();
          }
            return await _context.Pedidos.ToListAsync();
        }

        // GET: api/Pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
          if (_context.Pedidos == null)
          {
              return NotFound();
          }
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound();
            }

            return pedido;
        }

               [HttpGet("ViewCol{idCol}")]
        public async Task<ActionResult<List<PedidoViewCol>>> GetPedidosCol(int idCol)
        {
            if (_context.Pedidos == null)
            {
                return NotFound();
            }

            var lista = _context.Pedidos.Where(pedido => pedido.ColaboradorId == idCol);

            var pedido = lista.Select(p => new PedidoViewCol 
            { 
                DataPedido = p.DataPedido,
                ProdutoNome = p.Produto.NomeProd,
                Preco = p.Produto.Preco
            }).ToList();

            return pedido;
        }

        // PUT: api/Pedidos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedido)
        {
            if (id != pedido.IdPed)
            {
                return BadRequest();
            }

            _context.Entry(pedido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(id))
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

        // POST: api/Pedidos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
          if (_context.Pedidos == null)
          {
              return Problem("Entity set 'InfoDbContext.Pedidos' is null.");
          }
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedido", new { id = pedido.IdPed }, pedido);
        }
        */
        [HttpPost("AddPedido")]
        public async Task<ActionResult<Pedido>> FazerPedido(PedidoInputModel model)
        {
            if (_context.Pedidos == null)
            {
                return Problem("Entity set 'InfoDbContext.Pedidos' is null.");
            }
            Produto p = await _context.Produtos.FindAsync(model.IdProduto);
            Colaborador c = await  _context.Colaboradores.FindAsync(model.IdColaborador);
            


            if(p == null || c == null)
            {
                return Problem("Produto ou Colaborador não encontrado");
            }
            var pedido = new Pedido
            {
                DataPedido = model.DataPedido,
                ColaboradorId = model.IdColaborador,
                Colaborador = c,
                ProdutoId = model.IdProduto,
                Produto = p
            };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedido", new { id = pedido.IdPed }, pedido);
        } 


        // DELETE: api/Pedidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            if (_context.Pedidos == null)
            {
                return NotFound();
            }
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoExists(int id)
        {
            return (_context.Pedidos?.Any(e => e.IdPed == id)).GetValueOrDefault();
        }
    }
}
