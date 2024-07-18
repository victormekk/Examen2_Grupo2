using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio2._2_Grupo2.Models
{
    [Table("Firmas")]
    public class Firmas
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ImageFirma { get; set; }
        public DateTime Fecha { get; set; }
        public string nombres { get; set; }
        public string descripcion { get; set; }
    }
}
