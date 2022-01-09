using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACMAN_V2.Model
{
    class Giocatore
    {
        public string Nome { get; set; }
        public int Record { get; set; }
        public override bool Equals(object? obj)
        {
            return obj != null && (obj as Giocatore).Nome.Equals(this.Nome);
        }
    }
}
