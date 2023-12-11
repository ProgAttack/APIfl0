using System.ComponentModel.DataAnnotations;

namespace InfobarAPI.Models
{
    public class ColaboradorLogin

      
    {
        
        public int idCol { get; set; }

        [Key]
        [Required]
        public string Credencial { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}