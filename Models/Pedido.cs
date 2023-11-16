using System.ComponentModel.DataAnnotations;

namespace InfobarAPI.Models
{
    public class Pedido
    {
        [Key]
        public int IdPed { get; set; }
        public DateTime DataPedido { get; set; }
        public int ColaboradorId { get; set; }
        public Colaborador Colaborador { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

    }
}
