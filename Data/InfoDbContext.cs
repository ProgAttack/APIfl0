using InfobarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InfobarAPI.Data
{
    public class InfoDbContext : DbContext
    {
        public InfoDbContext(DbContextOptions<InfoDbContext> options) : base(options) { 
       
        }   
        public DbSet<Colaborador> Colaboradores { get; set;}
        public DbSet<Pedido> Pedidos { get; set;}
        public DbSet<Produto> Produtos { get; set;}



    }
}
