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
        Giocatore? player = null;
        bool su, giu, sinistra, destra;
        int cont = 0;
        int pacman_mangiati = 0;
        int punti, velocitàPacMan = 40, numeroVite = 3, livello = 1;
        bool rosa_mangibile = false, rosso_mangibile = false;
        bool perso = false;
        int difficoltà = 3;
        Keys pL = Keys.None;
        int redlast = 10;
        int pinkLast = 10;
        const int speed_facile = 40;
        const int speed_difficile = 50;
        Random r = new Random();
        List<Giocatore> giocatores = new List<Giocatore>();
        const string path = "../../../dati.csv";
        void caricaUtenti()
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
        public Form1()
        {
            InitializeComponent();

            caricaUtenti();

            Reset();
        }
        bool èConsumabile(string tag)
        {
            return tag == "Coin" || tag == "Bonus" || tag == "Mangia";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string nome = Inserimento_nome.Text;
            if(SceltaDifficoltà.SelectedItem == null)
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
                    item.Visible = true;
                }
            }
            PacMan.Visible = true;
            Fantasma_Rosa.Visible = true;
            Fantasma_Rosso.Visible = true;
            livello = 1;
            punti = 0;
            perso = false;
            Reset();
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
                            Fantasma_Rosso.Left -= vel;
                            redlast = 2;
                        }
                        else
                        {
                            Fantasma_Rosso.Left += vel;
                            redlast = 1;
                        }
                        break;
                    case 2:
                        if (PacMan.Top < Fantasma_Rosso.Top)
                        {
                            Fantasma_Rosso.Top -= vel;
                            redlast = 4;
                        }
                        else
                        {
                            Fantasma_Rosso.Top += vel;
                            redlast = 3;
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
                            Fantasma_Rosa.Left -= vel;
                            pinkLast = 2;
                        }
                        else
                        {
                            Fantasma_Rosa.Left += vel;
                            pinkLast = 1;
                        }
                        break;
                    case 2:
                        if (PacMan.Top < Fantasma_Rosa.Top)
                        {
                            Fantasma_Rosa.Top -= vel;
                            pinkLast = 4;
                        }
                        else
                        {
                            Fantasma_Rosa.Top += vel;
                            pinkLast = 3;
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
        private void Form1_gameTimerTick(object sender, EventArgs e)
        {
            if (player != null)
            {
                info.Text = $"{player.Nome} - Punti: {punti} - Record: {player.Record} - Livello: {livello} - Vite: {numeroVite}";
                if (cont < 12 && (rosso_mangibile || rosa_mangibile))
                {
                    cont++;
                }
                else
                {
                    cont = 0;
                    pacman_mangiati = 0;
                    rosso_mangibile = false;
                    rosa_mangibile = false;
                }
                #region Immagini e Movimenti
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
                //MUOVO FANTASMI
                Muovi_Rosa();
                Muovi_Rosso();
                //MUOVO PACMAN
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
                if (PacMan.Left < -10)
                    PacMan.Left = 1140;
                else if (PacMan.Left > 1140)
                    PacMan.Left = -10;
                if (Fantasma_Rosa.Left < -10)
                    Fantasma_Rosa.Left = 1140;
                else if (Fantasma_Rosa.Left > 1140)
                    Fantasma_Rosa.Left -= 10;
                if (Fantasma_Rosso.Left < -10)
                    Fantasma_Rosso.Left = -10;
                else if (Fantasma_Rosso.Left > 1140)
                    Fantasma_Rosso.Left = -10;
                #endregion
                #region Collisioni
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
                                    Reset();
                                }
                            }
                        }
                        else if ((string)item.Tag == "Mangia" && !rosso_mangibile && !rosa_mangibile && item.Visible)
                        {
                            if (PacMan.Bounds.IntersectsWith(item.Bounds))
                            {
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
                    }
                }
                #endregion
                #region Controllo Vittoria
                if (vittoria())
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
                #endregion
            }
        }
        private void Reset()
        {
            if (player == null)
            {
                if (!perso)
                {
                    ImmagineAvvio.BackgroundImage = Properties.Resources.Pacman_schermata_avvio;
                }
                else
                {
                    ImmagineAvvio.BackgroundImage = Properties.Resources.pac_man_game_over;
                }
                Inserimento_nome.Enabled = true;
                Login.Show();
                Login.BringToFront();
            }
            else
            {
                Login.Hide();
                Inserimento_nome.Enabled = false;
                PacMan.BringToFront();
                Fantasma_Rosa.BringToFront();
                Fantasma_Rosso.BringToFront();
                if (numeroVite > 0)
                {
                    Fantasma_Rosso.Left = 902;
                    Fantasma_Rosso.Top = 522;
                    Fantasma_Rosa.Left = 813;
                    Fantasma_Rosa.Top = 522;
                    PacMan.Left = 49;
                    PacMan.Top = 86;
                }
                else
                {
                    PacMan.Visible = false;
                    Fantasma_Rosa.Visible = false;
                    Fantasma_Rosso.Visible = false;
                    Fantasma_Rosso.Left = 902;
                    Fantasma_Rosso.Top = 522;
                    Fantasma_Rosa.Left = 813;
                    Fantasma_Rosa.Top = 522;
                    PacMan.Left = 49;
                    PacMan.Top = 86;
                    perso = true;
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
                        player = null;
                    }
                    Reset();
                }
            }
        }
    }
}
