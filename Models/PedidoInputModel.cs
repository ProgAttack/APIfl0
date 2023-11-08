using System.Text.Json.Serialization;

namespace InfobarAPI.Models
{
    public class PedidoInputModel
    {
        public DateTime DataPedido { get; set; }
        public int IdColaborador { get; set; }
        public int IdProduto { get; set; }

    }
}
