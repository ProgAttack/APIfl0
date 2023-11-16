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
    public class ProdutosController : ControllerBase
    {
        private readonly InfoDbContext _context;

        public ProdutosController(InfoDbContext context)
        {
            _context = context;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
          if (_context.Produtos == null)
          {
              return NotFound();
          }
            return await _context.Produtos.ToListAsync();
        }

        // GET: api/Produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            if (_context.Produtos == null)
            {
                return NotFound();
            }
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        [HttpGet("{codigo}")] //Conversar com a Prof. Juliana
        public async Task<ActionResult<IEnumerable<ScannerProduto>>> GetProdutosCodigo(string codigo)
        {
            var produtos = await _context.Produtos
                .Where(p => p.CodBarras == codigo)
                .ToListAsync();

            if (produtos == null || !produtos.Any())
            {
                return NotFound();
            }

            return Ok(produtos);//status 500
        }

        // PUT: api/Produtos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.IdProd)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
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

        [HttpPut("EditProdutoGeral{id}")]
        public async Task<IActionResult> PutProduto(int id, ProdutoInputModel model)
        {

            var itemExistente = _context.Produtos.Find(id);

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
            itemExistente.NomeProd = model.NomeProd;
            itemExistente.Preco = model.Preco;

            // Salve as mudanças no banco de dados
            _context.SaveChanges();

            return Ok("Item atualizado com sucesso");

        }

        [HttpPut("EditProdutoQuantidade{id}")]
        public async Task<IActionResult> PutEstoque(int id, Estoque model)
        {

            var itemExistente = _context.Produtos.Find(id);

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
            itemExistente.Quantidade = model.Quantidade;

            // Salve as mudanças no banco de dados
            _context.SaveChanges();

            return Ok("Item atualizado com sucesso");

        }

        // POST: api/Produtos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
          if (_context.Produtos == null)
          {
              return Problem("Entity set 'InfoDbContext.Produtos'  is null.");
          }
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduto", new { id = produto.IdProd }, produto);
        }

        [HttpPost("AddProduto")]
        public async Task<ActionResult<Produto>> PostProduto(ProdutoInputModel model)
        {
            var produto = new Produto
            {
                NomeProd = model.NomeProd,
                Preco = model.Preco,
                CodBarras = model.CodBarras,
                Quantidade = model.Quantidade

            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduto", new { id = produto.IdProd }, produto);
        }


        // DELETE: api/Produtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            if (_context.Produtos == null)
            {
                return NotFound();
            }
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteProd{id}")]
        public async Task<IActionResult> DeleteProd(int id)
        {
            if (_context.Produtos == null)
            {
                return NotFound();
            }
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            var deletedProduto = new
            {
                IdProd = produto.IdProd,
                NomeProd = produto.NomeProd,
                Preco = produto.Preco,
                CodBarras = produto.CodBarras,
                Quantidade = produto.Quantidade,
            };

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return Ok(deletedProduto);
        }
        private bool ProdutoExists(int id)
        {
            return (_context.Produtos?.Any(e => e.IdProd == id)).GetValueOrDefault();
        }
    }
}
