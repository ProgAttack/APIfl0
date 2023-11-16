using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InfobarAPI.Models
{
    public class Produto
    {
        [Key]
        public int IdProd { get; set; }
        public double Preco { get; set; }
        public string NomeProd { get; set; }
        public string CodBarras { get; set; }
        public int Quantidade { get; set; }
        [JsonIgnore]
        public ICollection<Pedido> Pedidos { get; set; }
    }
}
