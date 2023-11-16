using System.Text.Json.Serialization;

namespace InfobarAPI.Models
{
    public class PedidoInputModel
    {
        [JsonIgnore]
        public int IdPedido { get; set; }
        public DateTime DataPedido { get; set; }
        public int IdColaborador { get; set; }
        public int IdProduto { get; set; }

    }
}
