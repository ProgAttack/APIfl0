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
using Newtonsoft.Json;

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
            try
            {
                // Certifique-se de que o contexto e a tabela Pedidos não são nulos
                if (_context == null || _context.Pedidos == null)
                {
                    return NotFound("Contexto ou tabela de Pedidos não encontrados");
                }

                // Busque o pedido incluindo os dados do produto
                var pedido = await _context.Pedidos
                    .Include(p => p.Produto)
                    .FirstOrDefaultAsync(p => p.IdPed == id);

                // Verifique se o pedido foi encontrado
                if (pedido == null || pedido.Produto == null)
                {
                    return NotFound($"Pedido não encontrado para o ID: {id}");
                }

                // Crie o objeto de resposta
                var resumoPedido = new PedidoViewCol
                {
                    DataPedido = pedido.DataPedido,
                    ProdutoNome = pedido.Produto.NomeProd,
                    Preco = pedido.Produto.Preco
                };

                return resumoPedido;
            }
            catch (Exception ex)
            {
                // Log do erro, você pode personalizar conforme necessário
                Console.Error.WriteLine($"Erro durante a obtenção do pedido: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }


        [HttpGet("ViewCol/{idCol}")]
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


     
      [HttpGet("ValorTotal")]
        public async Task<ActionResult<IEnumerable<ResumoColaborador>>> GetValorTotal()
        {
            try
            {
                var colaboradores = await _context.Colaboradores.ToListAsync();
        
                var resumoColaboradores = colaboradores.Select(colaborador =>
                {
                    var pedidosPendentes = _context.Pedidos
                        .Include(p => p.Produto)
                        .Where(p => p.ColaboradorId == colaborador.Id && p.Situacao == "Pendente")
                        .ToList();
        
                    double valorTotal = pedidosPendentes.Sum(p => p.Produto.Preco);
        
                    return new ResumoColaborador
                    {
                        IdCol = colaborador.Id,
                        Nome = colaborador.Nome,
                        ValorTotal = colaborador.valorTotal
                    };
                }).ToList();
        
                return resumoColaboradores;
            }
            catch (Exception ex)
            {
                // Log do erro, você pode personalizar conforme necessário
                Console.Error.WriteLine($"Erro durante a obtenção do valor total: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }


        // Novo método para finalizar os pedidos pendentes
       [HttpPost("FinalizarPedidos/{idCol}")]
        public async Task<IActionResult> FinalizarPedidos(int idCol)
        {
            // Obtém os pedidos pendentes do colaborador
            var pedidosPendentes = await _context.Pedidos
                .Where(p => p.ColaboradorId == idCol && p.Situacao == "Pendente")
                .ToListAsync();
        
            if (pedidosPendentes == null || pedidosPendentes.Count == 0)
            {
                return NotFound("Nenhum pedido pendente encontrado para o colaborador.");
            }
        
            // Atualiza a situação dos pedidos para "Finalizado"
            foreach (var pedido in pedidosPendentes)
            {
                pedido.Situacao = "Finalizado";
            }
        
            await _context.SaveChangesAsync();
        
            return Ok("Pedidos pendentes finalizados com sucesso.");
        }
        [HttpGet("CodBarrasConfirma/{codigo}")]
        public async Task<ActionResult> GetProdutosByCodigo(string codigo)
        {
            try
            {
                // Verifica se o produto existe com base no código de barras
                var produto = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.CodBarras == codigo);

                if (produto == null)
                {
                    return NotFound("Produto não encontrado");
                }

                // Crie um objeto para representar as informações do produto
                var produtoInfo = new
                {
                    IdProd = produto.IdProd,
                    NomeProd = produto.NomeProd,
                    Preco = produto.Preco
                };
                Console.WriteLine($"Produto encontrado: {produtoInfo}");

                return Ok(produtoInfo);
            }
            catch (Exception ex)
            {
                // Log do erro, você pode personalizar conforme necessário
                Console.Error.WriteLine($"Erro ao buscar produto por código de barras: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

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

            foreach (Pedido item in pedidos)
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
        public async Task<ActionResult<PedidoViewCol>> FazerPedido([FromBody] PedidoInputModel model)
        {
            try
            {
                if (model == null || model.IdProduto <= 0 || model.IdColaborador <= 0)
                {
                    Console.Error.WriteLine("Model, Produto ou Colaborador inválidos.");
                    return BadRequest("Model, Produto ou Colaborador inválidos");
                }

                Produto p = await _context.Produtos.FindAsync(model.IdProduto);
                Colaborador c = await _context.Colaboradores.FindAsync(model.IdColaborador);

                if (p == null || c == null)
                {
                    Console.Error.WriteLine(p);
                    Console.Error.WriteLine(c);
                    Console.Error.WriteLine("Produto ou Colaborador não encontrado.");
                    return NotFound("Produto ou Colaborador não encontrado");
                }

                var pedido = new Pedido
                {
                    DataPedido = model.DataPedido,
                    ColaboradorId = c.IdCol, // Use o IdCol do Colaborador
                    Colaborador = c,
                    ProdutoId = model.IdProduto,
                    Produto = p
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                var pedidoViewCol = new PedidoViewCol
                {
                    DataPedido = pedido.DataPedido,
                    ProdutoNome = pedido.Produto.NomeProd,
                    Preco = pedido.Produto.Preco
                };

                Console.WriteLine($"Pedido concluído com sucesso: {JsonConvert.SerializeObject(pedidoViewCol)}");

                return CreatedAtAction(nameof(GetPedidoViewCol), new { id = pedido.IdPed }, pedidoViewCol);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro durante o processamento do pedido: {ex.Message}");
                Console.Error.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
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
