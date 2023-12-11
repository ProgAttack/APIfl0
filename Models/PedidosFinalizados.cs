using System.ComponentModel.DataAnnotations;

namespace InfobarAPI.Models
{
    public class PedidosFinalizados
    {
        [Key]
        public int Id { get; set; }

        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
    }
}
