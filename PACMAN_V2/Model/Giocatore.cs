namespace PACMAN_V2.Model
{
    class Giocatore
    {
        public string Nome { get; set; }
        public int Record { get; set; }
        public int Livello { get; set; }
        public override string ToString()
        {
            return $"{Nome} {Record}";
        }
        public override bool Equals(object? obj)
        {
            if(obj as Giocatore == null)
                return false;
            return obj != null && (obj as Giocatore).Nome.Equals(this.Nome);
        }
    }
}
