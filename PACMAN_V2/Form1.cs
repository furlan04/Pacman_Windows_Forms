using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using PACMAN_V2.Model;

namespace PACMAN_V2
{
    public partial class Form1 : Form
    {
        #region VARIABILI
        Giocatore? player = null;
        readonly object _lock = new object();
        int n = 0;
        bool su, giu, sinistra, destra;
        int cont = 0;
        int pacman_mangiati = 0, bonus_mangiati = 0;
        int punti, velocitàPacMan = 50, numeroVite = 3, livello = 1;
        bool rosa_mangibile = false, rosso_mangibile = false;
        int difficoltà = 3;
        Keys pL = Keys.None;
        int redlast = 10;
        int pinkLast = 10;
        const int speed_facile = 50;
        const int speed_difficile = 60;
        Random r = new Random();
        List<Giocatore> giocatores = new List<Giocatore>();
        Coordinate pacman = new Coordinate { X = 49, Y = 86 }, rosa = new Coordinate { X = 813, Y = 522 }, rosso = new Coordinate { X = 902, Y = 522 };
        const string path = "../../../dati.csv";
        #endregion
        #region FILE_UTENTI
        void CaricaUtenti()
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                return;
            }
            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string[] dato = reader.ReadLine().Split(';');
                    giocatores.Add(new Giocatore
                    {
                        Nome = dato[0],
                        Record = int.Parse(dato[1]),
                    });
                }
            }
        }
        void AggiornaFile()
        {
            using (StreamWriter w = new StreamWriter(path))
            {
                if (!giocatores.Contains(player))
                {
                    w.WriteLine($"{player.Nome};{punti}");
                    foreach (var item in giocatores)
                    {
                        w.WriteLine($"{item.Nome};{item.Record}");
                    }
                }
                else if (giocatores.Where(g => g.Nome.Equals(player.Nome)).First().Record < punti)
                {
                    w.WriteLine($"{player.Nome};{punti}");
                    foreach (var item in giocatores)
                    {
                        if (!item.Equals(player))
                            w.WriteLine($"{item.Nome};{item.Record}");
                    }
                }
                else
                {
                    foreach (var item in giocatores)
                    {
                        w.WriteLine($"{item.Nome};{item.Record}");
                    }
                }
            }
            player = null;
            numeroVite = 3;
            n = 0;
        }
        #endregion
        public Form1()
        {
            InitializeComponent();

            CaricaUtenti();

            Reset();
        }
        #region CONTROLLI / EVENTI
        bool èConsumabile(string tag)
        {
            return tag == "Coin" || tag == "Bonus" || tag == "Mangia";
        }
        bool LoginEffettuato()
        {
            return player != null;
        }
        bool vittoria()
        {
            foreach (Control item in this.Controls)
            {
                if (item is PictureBox && item != null)
                {
                    if (èConsumabile((string)item.Tag) && item.Visible)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        void Vinto()
        {
            livello++;
            foreach (Control item in this.Controls)
            {
                if (item is PictureBox)
                {
                    item.Visible = true;
                }
            }
            rosso_mangibile = false;
            rosso_mangibile = false;
            Reset();
        }
        void EffettoPacMan(bool fantasmi)
        {
            if (fantasmi)
            {
                if (Fantasma_Rosa.Left < -10)
                    Fantasma_Rosa.Left = 1140;
                else if (Fantasma_Rosa.Left > 1140)
                    Fantasma_Rosa.Left -= 10;
                if (Fantasma_Rosso.Left < -10)
                    Fantasma_Rosso.Left = -10;
                else if (Fantasma_Rosso.Left > 1140)
                    Fantasma_Rosso.Left = -10;
            }
            else
            {
                if (PacMan.Left < -10)
                    PacMan.Left = 1140;
                else if (PacMan.Left > 1140)
                    PacMan.Left = -10;
            }
        }
        void GestisciMangiabili()
        {
            if (cont < 500 && (rosso_mangibile || rosa_mangibile))
            {
                cont++;
            }
            else
            {
                cont = 0;
                pacman_mangiati = 0;
                lock (_lock)
                {
                    rosso_mangibile = false;
                    rosa_mangibile = false;
                }
            }
        }
        void ImmaginiFantasmi()
        {
            if (rosa_mangibile)
            {
                Fantasma_Rosa.Image = Properties.Resources._9czxbbycE;
            }
            else
            {
                Fantasma_Rosa.Image = Properties.Resources.pink_guy;
            }
            if (rosso_mangibile)
            {
                Fantasma_Rosso.Image = Properties.Resources._9czxbbycE;
            }
            else
            {
                Fantasma_Rosso.Image = Properties.Resources.red_guy;
            }
        }
        void ControllaCollisioni()
        {
            foreach (Control item in this.Controls)
            {
                if (item is PictureBox)
                {
                    if ((string)item.Tag == "Coin" && item.Visible)
                    {
                        if (PacMan.Bounds.IntersectsWith(item.Bounds))
                        {
                            punti += 10;
                            item.Visible = false;
                        }
                    }
                    else if ((string)item.Tag == "Fantasma")
                    {
                        if (PacMan.Bounds.IntersectsWith(item.Bounds))
                        {
                            if (item == Fantasma_Rosso && rosso_mangibile)
                            {
                                Fantasma_Rosso.Left = 902;
                                Fantasma_Rosso.Top = 522;
                                pacman_mangiati++;
                                punti += 200 * pacman_mangiati;
                                rosso_mangibile = false;
                            }
                            else if (item == Fantasma_Rosso && !rosso_mangibile)
                            {
                                numeroVite--;
                                ImmagineAvvio.BackgroundImage = Properties.Resources.pac_man_game_over;
                                Reset();
                            }
                            else if (item == Fantasma_Rosa && rosa_mangibile)
                            {
                                Fantasma_Rosa.Left = 813;
                                Fantasma_Rosa.Top = 522;
                                pacman_mangiati++;
                                punti += 200 * pacman_mangiati;
                                rosa_mangibile = false;
                            }
                            else if (item == Fantasma_Rosa && !rosa_mangibile)
                            {
                                numeroVite--;
                                ImmagineAvvio.BackgroundImage = Properties.Resources.pac_man_game_over;
                                Reset();
                            }
                        }
                    }
                    else if ((string)item.Tag == "Mangia" && !rosso_mangibile && !rosa_mangibile && item.Visible)
                    {
                        if (PacMan.Bounds.IntersectsWith(item.Bounds))
                        {
                            punti += 50;
                            item.Visible = false;
                            rosa_mangibile = true;
                            rosso_mangibile = true;
                            cont++;
                        }
                    }
                    else if ((string)item.Tag == "Muro" && PacMan.Bounds.IntersectsWith(item.Bounds))
                    {
                        switch (pL)
                        {
                            case Keys.Up:
                                PacMan.Top += velocitàPacMan;
                                break;
                            case Keys.Down:
                                PacMan.Top -= velocitàPacMan;
                                break;
                            case Keys.Left:
                                PacMan.Left += velocitàPacMan;
                                break;
                            case Keys.Right:
                                PacMan.Left -= velocitàPacMan;
                                break;
                        }
                    }
                    else if ((string)item.Tag == "Bonus" && PacMan.Bounds.IntersectsWith(item.Bounds) && item.Visible)
                    {
                        bonus_mangiati++;
                        punti += 200 * bonus_mangiati;
                        item.Visible = false;
                    }
                }
            }
        }
        #endregion
        #region TASTO LOGIN
        private void button1_Click(object sender, EventArgs e)
        {
            string nome = Inserimento_nome.Text;
            if (SceltaDifficoltà.SelectedItem == null)
            {
                difficoltà = 1;
            }
            else
            {
                switch(SceltaDifficoltà.SelectedItem)
                {
                    case "facile":
                        difficoltà = 1;
                        break;
                    case "medio":
                        difficoltà = 2;
                        break;
                    case "difficile":
                        difficoltà= 3;
                        break;
                }
            }
            if(nome == null)
            {
                nome = "nome-assente";
            }
            if (nome != null)
            {
                if (giocatores.Contains(new Giocatore { Nome = nome }))
                    player = giocatores.Where(g => g.Nome.Equals(nome)).First();
                else
                {
                    player = new Giocatore
                    {
                        Nome = nome,
                        Record = 0
                    };
                }
            }
            foreach (Control item in this.Controls)
            {
                if (èConsumabile((string)item.Tag))
                {
                    if(!((string)item.Tag == "Bonus" && bonus_mangiati >= 4))
                        item.Visible = true;
                }
            }
            PacMan.Visible = true;
            Fantasma_Rosa.Visible = true;
            Fantasma_Rosso.Visible = true;
            livello = 1;
            punti = 0;
            Login.Hide();
            Inserimento_nome.Enabled = false;
            PacMan.BringToFront();
            Fantasma_Rosa.BringToFront();
            Fantasma_Rosso.BringToFront();
            Reset();
        }
        #endregion
        #region FANTASMI
        void Muovi_Rosso()
        {
            int vel = difficoltà == 1 ? speed_facile : speed_difficile;
            if (difficoltà == 3)
            {
                switch (r.Next(1, 3))
                {
                    case 1:
                        if (PacMan.Left < Fantasma_Rosso.Left)
                        {
                            if (!rosso_mangibile)
                            {
                                Fantasma_Rosso.Left -= vel;
                                redlast = 2;
                            }
                            else
                            {
                                Fantasma_Rosso.Left += vel;
                                redlast = 1;
                            }
                        }
                        else
                        {
                            if (!rosso_mangibile)
                            {
                                Fantasma_Rosso.Left += vel;
                                redlast = 1;
                            }
                            else
                            {
                                Fantasma_Rosso.Left -= vel;
                                redlast = 2;
                            }
                        }
                        break;
                    case 2:
                        if (PacMan.Top < Fantasma_Rosso.Top)
                        {
                            if (!rosso_mangibile)
                            {
                                Fantasma_Rosso.Top -= vel;
                                redlast = 4;
                            }
                            else
                            {
                                Fantasma_Rosso.Top += vel;
                                redlast = 3;
                            }
                        }
                        else
                        {
                            if (!rosso_mangibile)
                            {
                                Fantasma_Rosso.Top += vel;
                                redlast = 3;
                            }
                            else
                            {
                                Fantasma_Rosso.Top -= vel;
                                redlast = 4;
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (r.Next(1, 5))
                {
                    case 1:
                        Fantasma_Rosso.Left += vel;
                        redlast = 1;
                        break;
                    case 2:
                        Fantasma_Rosso.Left -= vel;
                        redlast = 2;
                        break;
                    case 3:
                        Fantasma_Rosso.Top += vel;
                        redlast = 3;
                        break;
                    case 4:
                        Fantasma_Rosso.Top -= vel;
                        redlast = 4;
                        break;
                }
            }
            foreach (Control item in this.Controls)
            {
                if (item is PictureBox && (string)item.Tag == "Muro" && Fantasma_Rosso.Bounds.IntersectsWith(item.Bounds))
                {
                    switch (redlast)
                    {
                        case 1:
                            Fantasma_Rosso.Left -= vel;
                            break;
                        case 2:
                            Fantasma_Rosso.Left += vel;
                            break;
                        case 3:
                            Fantasma_Rosso.Top -= vel;
                            break;
                        case 4:
                            Fantasma_Rosso.Top += vel;
                            break;
                    }
                }
            }
        }
        void Muovi_Rosa()
        {
            int vel = difficoltà == 1 ? speed_facile : speed_difficile;
            if (difficoltà == 3)
            {
                switch (r.Next(1, 3))
                {
                    case 1:
                        if (PacMan.Left < Fantasma_Rosa.Left)
                        {
                            if (!rosa_mangibile)
                            {
                                Fantasma_Rosa.Left -= vel;
                                pinkLast = 2;
                            }
                            else
                            {
                                Fantasma_Rosa.Left += vel;
                                pinkLast = 1;
                            }
                        }
                        else
                        {
                            if (!rosa_mangibile)
                            {
                                Fantasma_Rosa.Left += vel;
                                pinkLast = 1;
                            }
                            else
                            {
                                Fantasma_Rosa.Left -= vel;
                                pinkLast = 2;
                            }
                        }
                        break;
                    case 2:
                        if (PacMan.Top < Fantasma_Rosa.Top)
                        {
                            if (!rosa_mangibile)
                            {
                                Fantasma_Rosa.Top -= vel;
                                pinkLast = 4;
                            }
                            else
                            {
                                Fantasma_Rosa.Top += vel;
                                pinkLast = 3;
                            }
                        }
                        else
                        {
                            if(!rosa_mangibile)
                            {
                                Fantasma_Rosa.Top += vel;
                                pinkLast = 3;
                            }
                            else
                            {
                                Fantasma_Rosa.Top -= vel;
                                pinkLast = 4;
                            }
                        }
                        break;
                }
            }
            else
            {

                switch (r.Next(1, 5))
                {
                    case 1:
                        Fantasma_Rosa.Left += vel;
                        pinkLast = 1;
                        break;
                    case 2:
                        Fantasma_Rosa.Left -= vel;
                        pinkLast = 2;
                        break;
                    case 3:
                        Fantasma_Rosa.Top += vel;
                        pinkLast = 3;
                        break;
                    case 4:
                        Fantasma_Rosa.Top -= vel;
                        pinkLast= 4;
                        break;
                }
            }
            foreach (Control item in this.Controls)
            {
                if (item is PictureBox && (string)item.Tag == "Muro" && Fantasma_Rosa.Bounds.IntersectsWith(item.Bounds))
                {
                    switch (pinkLast)
                    {
                        case 1:
                            Fantasma_Rosa.Left -= vel;
                            break;
                        case 2:
                            Fantasma_Rosa.Left += vel;
                            break;
                        case 3:
                            Fantasma_Rosa.Top -= vel;
                            break;
                        case 4:
                            Fantasma_Rosa.Top += vel;
                            break;
                    }
                }
            }
        }
        #endregion
        #region INPUT_PACMAN
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.None)
                pL = e.KeyCode;
            destra = false;
            sinistra = false;
            su = false;
            giu = false;
            if (pL == Keys.Left)
            {
                sinistra = true;
            }
            if (pL == Keys.Right)
            {
                destra = true;
            }
            if (pL == Keys.Up)
            {

                su = true;
            }
            if (pL == Keys.Down)
            {

                giu = true;
            }
        }
        void MuoviPacMan()
        {
            if (sinistra)
            {
                PacMan.Left -= velocitàPacMan;
                PacMan.Image = Properties.Resources.left;
            }
            else if (destra)
            {
                PacMan.Left += velocitàPacMan;

                PacMan.Image = Properties.Resources.right;
            }
            else if (su)
            {
                PacMan.Top -= velocitàPacMan;
                PacMan.Image = Properties.Resources.Up;
            }
            else if (giu)
            {
                PacMan.Top += velocitàPacMan;
                PacMan.Image = Properties.Resources.down;
            }
        }
        #endregion
        #region LOOP
        void Pacman_Moviment_Tick(object sender, EventArgs e)
        {
            if (LoginEffettuato())
            {
                lock (_lock)
                {
                    MuoviPacMan();
                    EffettoPacMan(false);
                }
            }
        }
        void Movimento_fantasmi_Tick(object sender, EventArgs e)
        {
            if (LoginEffettuato())
            {
                lock (_lock)
                {
                    Muovi_Rosa();
                    Muovi_Rosso();
                    EffettoPacMan(true);
                }
            }
        }
        void Form1_gameTimerTick(object sender, EventArgs e)
        {
            if (LoginEffettuato())
            {
                info.Text = $"{player.Nome} - Punti: {punti} - Record: {player.Record} - Livello: {livello} - Vite: {numeroVite}";
                GestisciMangiabili();
                ImmaginiFantasmi();
                lock (_lock)
                {
                    ControllaCollisioni();
                }
                #region AUMENTA PUNTI OGNI 10000
                if ((punti - 10000 * n) / 10000 > 0)
                {
                    n++;
                    numeroVite++;
                }
                #endregion
                if (vittoria())
                {
                    Vinto();
                }
            }
        }
        #endregion
        void ResetPosizioni()
        {
            Fantasma_Rosso.Left = rosso.X;
            Fantasma_Rosso.Top = rosso.Y;
            Fantasma_Rosa.Left = rosa.X;
            Fantasma_Rosa.Top = rosa.Y;
            PacMan.Left = pacman.X;
            PacMan.Top = pacman.Y;
        }
        void Reset()
        {
            if (!LoginEffettuato())
            {
                Inserimento_nome.Enabled = true;
                Login.Show();
                Login.BringToFront();
            }
            else
            {
                if (numeroVite > 0)
                {
                    ResetPosizioni();
                }
                else
                {
                    ResetPosizioni();
                    AggiornaFile();
                    Reset();
                }
            }
        }
    }
}
