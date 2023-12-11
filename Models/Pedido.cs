using System.ComponentModel.DataAnnotations;

namespace InfobarAPI.Models
{
    public class Pedido
    {
        [Key]
        public int IdPed { get; set; }

        private DateTime _dataPedido;

        public DateTime DataPedido
        {
            get { return _dataPedido; }
            set { _dataPedido = value.ToUniversalTime(); } // Converta para UTC
        }

        public int ColaboradorId { get; set; }
        public Colaborador Colaborador { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public string Situacao { get; set; } = "Pendente"; // Defina a situação padrão como "Pendente"
    }
}
