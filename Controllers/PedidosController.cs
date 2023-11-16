using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfobarAPI.Data;
using InfobarAPI.Models;
using System.Xml.Schema;
using MySqlX.XDevAPI.Common;

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

        [HttpGet("PedidoView/{id}")]
        public async Task<ActionResult<PedidoViewCol>> GetPedidoViewCol(int id)
        {
            if (_context.Pedidos == null)
            {
                return NotFound();
            }
            var pedido = await _context.Pedidos.FindAsync(id);

            var produto = await _context.Produtos.FindAsync(pedido.ProdutoId);

            if (pedido == null || produto == null )
            {
                return NotFound();
            }

            var resumoPedido = new PedidoViewCol
            {
                DataPedido = pedido.DataPedido,
                ProdutoNome = produto.NomeProd,
                Preco = produto.Preco
            };

            return resumoPedido;
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

        /*        [HttpGet("ValorTotal/{idCol}")]
                public async Task<ActionResult<ResumoColaborador>> GetValor(int idCol, DateTime dataInicial, DateTime dataFinal)
                {
                    var colaborador = await _context.Colaboradores.FindAsync(idCol);

                    if (colaborador == null)
                    {
                        return NotFound("Colaborador " + idCol + " não encontrado");
                    }

                    var lista = _context.Pedidos.Where(pedido => pedido.ColaboradorId == idCol);

                    var pedido = lista.Select(p => new PedidoViewCol
                    {
                        DataPedido = p.DataPedido,
                        ProdutoNome = p.Produto.NomeProd,
                        Preco = p.Produto.Preco
                    }).ToList();

                    var resumo = new ResumoColaborador
                    {
                        Nome = colaborador.Nome,
                        ValorTotal = pedido.Sum(p => p.Preco)
                    };

                    if (resumo == null)
                    {
                        return Problem("Resumo do colaborador não encontrado");
                    }
                    else
                    {
                        return resumo;
                    }
                }*/

        [HttpGet("ValorTotal/{idCol}")]
        public async Task<ActionResult<ResumoColaborador>> GetValor(int idCol, DateTime dataInicial, DateTime dataFinal)
        {
            var colaborador = await _context.Colaboradores.FindAsync(idCol);

            if (colaborador == null)
            {
                return NotFound("Colaborador " + idCol + " não encontrado");
            }

            var pedidosCalendario = await _context.Pedidos
                .Include(p => p.Colaborador)
                .Include(p => p.Produto)
                .Where(p => p.DataPedido >= dataInicial && p.DataPedido <= dataFinal && p.ColaboradorId == idCol)
                .ToListAsync();

            if (pedidosCalendario == null || pedidosCalendario.Count == 0)
            {
                return NotFound("Nenhum pedido encontrado no período especificado para o colaborador.");
            }

            var resumo = new ResumoColaborador
            {
                Nome = colaborador.Nome,
                ValorTotal = pedidosCalendario.Sum(p => p.Produto.Preco)
            };

            if (resumo == null)
            {
                return Problem("Resumo do colaborador não encontrado");
            }
            else
            {
                return resumo;
            }
        }

/*
        [HttpGet("ValorTotalTodos")]
        public async Task<ActionResult<ResumoColaborador>> GetValorTodos(DateTime dataInicial, DateTime dataFinal)
        {
            var colaborador = await _context.Colaboradores.FindAsync();

            if (colaborador == null)
            {
                return NotFound("Colaboradores não encontrados");
            }

            var pedidosCalendario = await _context.Pedidos
                .Include(p => p.Colaborador)
                .Include(p => p.Produto)
                .Where(p => p.DataPedido >= dataInicial && p.DataPedido <= dataFinal)
                .ToListAsync();

            if (pedidosCalendario == null || pedidosCalendario.Count == 0)
            {
                return NotFound("Nenhum pedido encontrado no período especificado para o colaborador.");
            }

            var resumo = new ResumoColaborador
            {
                Nome = colaborador.Nome,
                ValorTotal = pedidosCalendario.Sum(p => p.Produto.Preco)
            };

            if (resumo == null)
            {
                return Problem("Resumo do colaborador não encontrado");
            }
            else
            {
                return resumo;
            }
        }*/

        // GET: api/Pedidos/Periodo
        [HttpGet("Periodo")]
        public async Task<ActionResult<IEnumerable<PedidoViewCol>>> ObterPedidosPorPeriodo(DateTime dataInicial, DateTime dataFinal, int idCol)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Colaborador)
                .Include(p => p.Produto)
                .Where(p => p.DataPedido >= dataInicial && p.DataPedido <= dataFinal && p.ColaboradorId == idCol)
                .ToListAsync();

            if (pedidos == null || pedidos.Count == 0)
            {
                return NotFound("Nenhum pedido encontrado no período especificado para o colaborador.");
            }

            List<PedidoViewCol> pedidosColaboradores = new List<PedidoViewCol>();

            double total = 0.0;

            foreach ( Pedido item in pedidos )
            {
                var resumoPedido = new PedidoViewCol
                {
                    DataPedido = item.DataPedido,
                    ProdutoNome = item.Produto.NomeProd,
                    Preco = item.Produto.Preco
                };
                total += item.Produto.Preco;
                pedidosColaboradores.Add(resumoPedido);
            }

            return pedidosColaboradores;
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
        public async Task<ActionResult<PedidoViewCol>> FazerPedido(PedidoInputModel model)
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

            return CreatedAtAction("GetPedidoViewCol", new { id = model.IdPedido }, model);
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

        [HttpDelete("DeletePedido/{id}")]
        public async Task<IActionResult> DeletePedidoa(int id)
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

            var deletePedido = new
            {
                IdPed = pedido.IdPed,
                DataPedido = pedido.DataPedido,
                IdCol = pedido.ColaboradorId,
                Colaborador = pedido.Colaborador,
                IdProd = pedido.ProdutoId,
                Produto = pedido
            };

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return Ok(deletePedido);
        }

        private bool PedidoExists(int id)
        {
            return (_context.Pedidos?.Any(e => e.IdPed == id)).GetValueOrDefault();
        }
    }
}
