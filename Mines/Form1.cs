using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mines
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Button btn;
        int[] arrayMines;
        int countX = 0;
        int countY = 0;
        int mineCount = 0;
        int timer = 0;

        private int[] RandomNumbersGenerator(int count, int min, int max)
        {
            int[] array = new int[count];
            Random rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                int nextNumber = rnd.Next(min, max);
                if (array.Contains(nextNumber))
                {
                    i--;
                    continue;
                }
                else
                {
                    array[i] = nextNumber;
                }
            }
            return array;
        }

        private void ButtonGenerator(int countX, int countY)
        {
            for (int i = 0; i < countY; i++)
            {
                for (int j = 0; j < countX; j++)
                {
                    btn = new Button();
                    btn.Size = new Size(25, 25);
                    btn.BackColor = Color.Black;
                    btn.Tag = (j + i * countX).ToString();
                    btn.Location = new Point(j * 25, i * 25);

                    btn.MouseUp += Btn_MouseUp;
                    this.grpButtons.Controls.Add(btn);
                }
            }
        }

        private int CountMinesNearby(Button _btn, int btnCountX, int btnCountY)
        {
            int mineCount = 0;

            int btnNumber = int.Parse((string)_btn.Tag);

            int[] toCheck = { btnNumber - btnCountX - 1, btnNumber - btnCountX, btnNumber - btnCountX + 1, btnNumber - 1, btnNumber + 1, btnNumber + btnCountX - 1, btnNumber + btnCountX, btnNumber + btnCountX + 1 };

            for (int i = 0; i < toCheck.Length; i++)
            {
                if (toCheck[i] < 0 || toCheck[i] >= btnCountX * btnCountY)
                    continue;

                Button __btn = grpButtons.Controls[toCheck[i]] as Button;

                if (Math.Abs(_btn.Location.X - grpButtons.Controls[toCheck[i]].Location.X) > 30)
                {
                    continue;
                }

                if ((string)grpButtons.Controls[toCheck[i]].Tag == "mine")
                {
                    mineCount++;
                }
            }

            return mineCount;
        }

        private Button[] ButtonsNearby(Button _btn, int btnCountX, int btnCountY)
        {
            Button[] btnsNearby = new Button[0];

            int btnNumber = int.Parse((string)_btn.Tag);

            int[] toCheck = { btnNumber - btnCountX - 1, btnNumber - btnCountX, btnNumber - btnCountX + 1, btnNumber - 1, btnNumber + 1, btnNumber + btnCountX - 1, btnNumber + btnCountX, btnNumber + btnCountX + 1 };

            for (int i = 0; i < toCheck.Length; i++)
            {
                if (toCheck[i] < 0 || toCheck[i] >= btnCountX * btnCountY)
                {
                    continue;
                }
                else if (Math.Abs(_btn.Location.X - grpButtons.Controls[toCheck[i]].Location.X) > 30)
                {
                    continue;
                }
                else
                {
                    Button __btn = grpButtons.Controls[toCheck[i]] as Button;
                    Array.Resize(ref btnsNearby, btnsNearby.Length + 1);
                    btnsNearby[btnsNearby.Length - 1] = __btn;
                }
            }

            return btnsNearby;
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case (MouseButtons.Left):

                    Button btnClicked = sender as Button;

                    btnClicked.BackColor = Color.White;
                    btnClicked.Image = null;

                    if ((string)btnClicked.Tag == "mine")
                    {
                        timer1.Enabled = false;
                        btnFinish.Enabled = false;

                        foreach (Button item in grpButtons.Controls)
                        {
                            Button _btn = item as Button;
                            if ((string)_btn.Tag == "mine")
                            {
                                _btn.BackColor = Color.Red;
                                _btn.Image = images.Images[0];
                            }

                            _btn.Enabled = false;
                        }
                    }

                    else
                    {
                        int nearbyMines = CountMinesNearby(btnClicked, countX, countY);

                        if (nearbyMines > 0)
                        {
                            btnClicked.Image = images.Images[nearbyMines];
                        }

                        else
                        {
                            NonMineClicked(btnClicked, countX, countY);
                        }
                    }

                    break;

                case (MouseButtons.Right):

                    Button buttonClicked = sender as Button;

                    if (buttonClicked.Image == null)
                    {
                        buttonClicked.BackColor = Color.White;
                        buttonClicked.Image = images.Images[9];
                    }
                    else
                    {
                        buttonClicked.Image = null;
                        buttonClicked.BackColor = Color.Black;
                    }

                    break;
            }
        }

        private void NonMineClicked(Button btnC, int btnCountX, int btnCountY)
        {
            List<Button> gtCheck = new List<Button>();
            Button btnActive;

            gtCheck.Add(btnC);

            while (gtCheck.Count > 0)
            {
                btnActive = gtCheck[0];
                btnActive.BackColor = Color.White;
                btnActive.Enabled = false;

                int btnNumber = int.Parse((string)btnActive.Tag);
                Button[] btnsAround = ButtonsNearby(btnActive, btnCountX, btnCountY);
                int nearbyMines = CountMinesNearby(btnActive, btnCountX, btnCountY);

                if (nearbyMines > 0)
                {
                    btnActive.Image = images.Images[nearbyMines];
                }
                else
                {
                    foreach (Button b in btnsAround)
                    {
                        if (b.Enabled == false)
                            continue;

                        int cmn = CountMinesNearby(b, btnCountX, btnCountY);
                        if (cmn > 0)
                        {
                            b.BackColor = Color.White;
                            b.Image = images.Images[cmn];
                        }
                        else if (!gtCheck.Contains(b))
                        {
                            gtCheck.Add(b);
                        }
                    }
                }

                gtCheck.RemoveAt(0);
            }
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            switch (cmbSize.SelectedIndex)
            {
                case (0):
                    countX = 12;
                    countY = 12;
                    mineCount = 22;
                    this.Size = new Size(316, 360);
                    this.grpButtons.Size = new Size(300, 300);
                    break;
                case (1):
                    countX = 20;
                    countY = 20;
                    mineCount = 70;
                    this.Size = new Size(516, 560);
                    this.grpButtons.Size = new Size(500, 500);
                    break;
                case (2):
                    countX = 40;
                    countY = 25;
                    mineCount = 250;
                    this.Size = new Size(1016, 685);
                    this.grpButtons.Size = new Size(1000, 625);
                    break;
            }

            timer = 0;
            lblTimer.Text = timer.ToString();
            btnFinish.Enabled = true;
            timer1.Enabled = true;
            this.grpButtons.Controls.Clear();
            ButtonGenerator(countX, countY);
            arrayMines = RandomNumbersGenerator(mineCount, 0, countX * countY);

            for (int i = 0; i < arrayMines.Length; i++)
            {
                this.grpButtons.Controls[arrayMines[i]].Tag = "mine";
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            int markedMines = 0;

            for (int i = 0; i < grpButtons.Controls.Count; i++)
            {
                Button _btn = grpButtons.Controls[i] as Button;
                if ((string)_btn.Tag == "mine" && _btn.Image != null)
                {
                    markedMines++;
                }
            }

            if (markedMines == arrayMines.Length)
            {
                timer1.Enabled = false;
                MessageBox.Show("Tebrikler, " + timer + " saniyede kazandınız!");
            }
            else
            {
                MessageBox.Show("Tekrar deneyin.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbSize.SelectedIndex = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
            lblTimer.Text = timer.ToString();
        }
    }
}