using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InfobarAPI.Models
{
    public class Colaborador

    {
        [Key]
        public int IdCol { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        [Required]
        public string Credencial { get; set; } 
        [Required]
        public string Senha { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        
        public ICollection<Pedido> Pedidos { get; set; } 
    }
}
