using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        int[] arrayMines;   //mayınların numaralarını tutan array
        int countX = 0;     //yatay buton sayısı
        int countY = 0;     //dikey buton sayısı
        int mineCount = 0;  //toplam mayın sayısı
        int timer = 0;      //timer


        /// <summary>
        /// Rastgele sayılar dizisi oluşturur
        /// </summary>
        /// <param name="count">sayı miktarı</param>
        /// <param name="min">minimum sayı</param>
        /// <param name="max">maksimum sayı + 1 (144 girersen maks 143)</param>
        /// <returns></returns>
        int[] RandomNumbersGenerator(int count, int min, int max)
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
                    array[i] = nextNumber;
            }
            return array;
        }

        /// <summary>
        /// Ekranda butonları oluşturur
        /// </summary>
        /// <param name="countX">yatay buton sayısı</param>
        /// <param name="countY">dikey buton sayısı</param>
        void ButtonGenerator(int countX, int countY)
        {
            for (int i = 0; i < countY; i++)
            {
                for (int j = 0; j < countX; j++)
                {
                    btn = new Button();
                    btn.Size = new Size(25, 25);
                    btn.BackColor = Color.Black;
                    btn.Tag = (j + i * countX).ToString();  //12x12 için 0-143
                    btn.Location = new Point(j * 25, i * 25);

                    //hem sağ tık hem de sol tık eventlerini almamız lazım.
                    btn.MouseUp += Btn_MouseUp;
                    //butonu groupbox'a ekle
                    this.grpButtons.Controls.Add(btn);
                }
            }
        }

        /// <summary>
        /// Butonun etrafındaki mayın sayısını verir
        /// </summary>
        /// <param name="_btn">seçilen buton</param>
        /// <param name="btnCountX">yataydaki buton sayısı</param>
        /// <param name="btnCountY">dikeydeki buton sayısı</param>
        /// <param name="arrayMines">mayınlı butonların numaraları dizisi</param>
        /// <returns></returns>
        int CountMinesNearby(Button _btn, int btnCountX, int btnCountY, int[] arrayMines)
        {
            //mayın sayısı
            int count = 0;

            //butonun numarası
            int btnNumber = int.Parse((string)_btn.Tag);

            //butonun etrafındaki, mayın olup olmadığı kontrol edilecek butonların numaraları => alttaki toCheck arrayinde
            //normal şartlarda etrafında 8 buton var fakat kenarlardakilerin etrafında 5, köşelerdekinin etrafında 3 buton oluyor.
            //örn; 12x12 durumda 0 numaralı sol üst köşedeki buton için kontrol edilecek butonların numaraları: "-13,-12,-11,-1,1,11,12,13"
            //fakat kontrol edilmesi gereken butonlar:"1,12,13". (negatif numaralı buton yok. 11 numaralı buton ise sağ üst köşedeki buton.)
            //bu durumu engellemek için aşağıdaki for döngüsünün içinde bazı kontroller uyguladım.
            //1: kontrol edilecek buton numarası negatif olamaz
            //2: kontrol edilecek buton numarası maksimum buton numarasından büyük olamaz.(örn; 12x12 için 143)
            //3: basılan buton ile kontrol edilen buton arasındaki mesafe 30 pikselden büyük olamaz.(buton boyutları 25x25 piksel) (yukarıdaki örnekte 11 numaralı butonu kontrol etmesini engelleyen şart)
            int[] toCheck = { btnNumber - btnCountX - 1, btnNumber - btnCountX, btnNumber - btnCountX + 1, btnNumber - 1, btnNumber + 1, btnNumber + btnCountX - 1, btnNumber + btnCountX, btnNumber + btnCountX + 1 };

            for (int i = 0; i < toCheck.Length; i++)
            {
                //1 veya 2 numaralı şartlar
                if (toCheck[i] < 0 || toCheck[i] >= btnCountX * btnCountY)
                    continue;
                Button __btn = grpButtons.Controls[toCheck[i]] as Button;
                //3 numaralı mesafe kontrol eden şart
                if (Math.Abs(_btn.Location.X - grpButtons.Controls[toCheck[i]].Location.X) > 30)
                    continue;
                //kontrol edilen butonun tag'i "mine" ise count arttır
                if ((string)grpButtons.Controls[toCheck[i]].Tag == "mine")
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Girilen butonun etrafındaki butonları array olarak döner
        /// </summary>
        /// <param name="_btn">girilen buton</param>
        /// <param name="btnCountX">yatay buton sayısı</param>
        /// <param name="btnCountY">dikey buton sayısı</param>
        /// <returns></returns>
        Button[] ButtonsNearby(Button _btn, int btnCountX, int btnCountY)
        {
            //burada da CountMinesNearby metodundaki şartlar geçerli
            //(ortadaysa 8, kenardaysa 5, köşedeyse 3 buton atar arraye)
            Button[] btnsNearby = new Button[0];
            int btnNumber = int.Parse((string)_btn.Tag);
            int[] toCheck = { btnNumber - btnCountX - 1, btnNumber - btnCountX, btnNumber - btnCountX + 1, btnNumber - 1, btnNumber + 1, btnNumber + btnCountX - 1, btnNumber + btnCountX, btnNumber + btnCountX + 1 };

            for (int i = 0; i < toCheck.Length; i++)
            {
                if (toCheck[i] < 0 || toCheck[i] >= btnCountX * btnCountY)
                    continue;
                else if (Math.Abs(_btn.Location.X - grpButtons.Controls[toCheck[i]].Location.X) > 30)
                    continue;
                else
                {
                    Button __btn = grpButtons.Controls[toCheck[i]] as Button;
                    Array.Resize(ref btnsNearby, btnsNearby.Length + 1);
                    btnsNearby[btnsNearby.Length - 1] = __btn;
                }
            }
            return btnsNearby;
        }

        //butonlara tıklanınca çalışan event handler
        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                //sol tık ise:
                case (MouseButtons.Left):

                    Button btnClicked = sender as Button;
                    btnClicked.BackColor = Color.White;
                    //bayraklı butona tıklanırsa bayrağı silsin
                    btnClicked.Image = null;

                    //mayına tıklama durumu (oyun kaybedilir ve bütün mayınlar görünür olmalı)
                    if ((string)btnClicked.Tag == "mine")
                    {
                        //oyun bitince kronometre dursun
                        timer1.Enabled = false;
                        btnFinish.Enabled = false;

                        //groupboxın içindeki her buton için
                        foreach (Button item in grpButtons.Controls)
                        {
                            Button _btn = item as Button;
                            if ((string)_btn.Tag == "mine")
                            {
                                //tag'i "mine" olanların fotosunu mayın yap
                                _btn.BackColor = Color.Red;
                                _btn.Image = images.Images[0];
                            }
                            //hepsini disable yap ki oyun biterse butonlara tıklanamasın
                            _btn.Enabled = false;
                        }
                    }

                    //mayın olmayan butona tıklama durumu
                    else
                    {
                        //etrafındaki mayın sayısı
                        int nearbyMines = CountMinesNearby(btnClicked, countX, countY, arrayMines);
                        
                        //etrafında kaç mayın varsa resmini o numara yap
                        if (nearbyMines > 0)
                            btnClicked.Image = images.Images[nearbyMines];
                        
                        //etrafında mayın yoksa toplu açılış yapacak çılgın metoda git :)
                        else
                            NonMineClicked(btnClicked, countX, countY);
                    }

                    break;

                //sağ tık ise:
                case (MouseButtons.Right):

                    Button buttonClicked = sender as Button;

                    //sağ tıklanan butonun resmi yoksa bayrak yap
                    if (buttonClicked.Image == null)
                    {
                        buttonClicked.BackColor = Color.White;
                        buttonClicked.Image = images.Images[9];
                    }
                    //resmi varsa resmini silip tekrar siyah yap
                    else
                    {
                        buttonClicked.Image = null;
                        buttonClicked.BackColor = Color.Black;
                    }

                    break;
            }
        }

        /// <summary>
        /// Tıklanan butonun etrafında mayın yoksa çalışan metot
        /// </summary>
        /// <param name="btnC">tıklanan buton</param>
        /// <param name="btnCountX">yatay buton sayısı</param>
        /// <param name="btnCountY">dikey buton sayısı</param>
        void NonMineClicked(Button btnC, int btnCountX, int btnCountY)
        {
            //kontrol edilecek butonlar listesi
            List<Button> gtCheck = new List<Button>();

            //şuan kontrol edilen buton
            Button btnActive;

            //tıklanan buton kontrol edilecekler listesine eklendi
            gtCheck.Add(btnC);
            
            //kontrol edilecek butonlar listesinde eleman olduğu sürece çalışacak döngü
            while (gtCheck.Count > 0)
            {
                //listenin başındaki buton şuan kontrol edilen
                btnActive = gtCheck[0];
                btnActive.BackColor = Color.White;
                //kontrol edilen buton disable ediliyor. sonraki adımlarda disable olanları kontrol edilecekler listesine eklememesi için şart var. aynı butonları sürekli kontrol etmesin diye.
                btnActive.Enabled = false;

                //şuan kontrol edilen butona göre; buton numarası, etrafındaki butonlar ve etrafındaki mayın sayısı güncellendi
                int btnNumber = int.Parse((string)btnActive.Tag);
                Button[] btnsAround = ButtonsNearby(btnActive, btnCountX, btnCountY);
                int nearbyMines = CountMinesNearby(btnActive, btnCountX, btnCountY, arrayMines);

                //etrafında mayın varsa resmi sayı olsun
                if (nearbyMines > 0)
                    btnActive.Image = images.Images[nearbyMines];
                //etrafında mayın yoksa;
                else
                {
                    //etrafındaki her b butonu için(b => btnActive'in etrafındaki butonlar)
                    //b'nin etrafında mayın varsa resmini ayarla
                    //b'nin etrafında da mayın yoksa b'yi kontrol edilecekler listesine ekle(while döngüsünün ileriki adımlarında btnActive(şuanki adımda kontrol edilen buton) b olacak)
                    foreach (Button b in btnsAround)
                    {
                        //disable olanı atla(zaten daha önce kontrol edilmiş)
                        if (b.Enabled == false)
                            continue;

                        int cmn = CountMinesNearby(b, btnCountX, btnCountY, arrayMines);
                        //b'nin etrafında mayın varsa;
                        if (cmn > 0)
                        {                            
                            b.BackColor = Color.White;
                            b.Image = images.Images[cmn];
                        }
                        //yoksa listeye ekle
                        else
                        {
                            if (!gtCheck.Contains(b))
                                gtCheck.Add(b);
                        }
                    }
                }
                //kontrol edilen butonu kontrol edilecekler listesinden sil ki sonsuz döngüye girme
                gtCheck.RemoveAt(0);
            }
        }


        //new game butonu
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            //combobox'tan seçilen zorluk seviyesine göre buton sayısı, form boyutu, grpbox boyutu ve mayın sayısını belirle
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
            timer = 0; //kronometreyi sıfırla
            lblTimer.Text = timer.ToString();
            btnFinish.Enabled = true;
            timer1.Enabled = true; //kronometreyi başlat
            this.grpButtons.Controls.Clear();  //groupboxtaki butonları temizle
            ButtonGenerator(countX, countY);   //butonları oluştur
            //mayınların numaralarını oluştur
            arrayMines = RandomNumbersGenerator(mineCount, 0, countX * countY);

            //buton numarası mayın numarası olanın tag'ini "mine" yap
            for (int i = 0; i < arrayMines.Length; i++)
            {
                this.grpButtons.Controls[arrayMines[i]].Tag = "mine";
            }
        }

        //finish butonu
        private void btnFinish_Click(object sender, EventArgs e)
        {
            //finish butonuna tıklanınca "mine" tagli yani mayın butonların hepsinin image'i boş değilse(bayraklıysa) oyun kazanılır
            //markedMines => bayraklanmış mayınlı buton sayısı
            int markedMines = 0;
            for (int i = 0; i < grpButtons.Controls.Count; i++)
            {
                Button _btn = grpButtons.Controls[i] as Button;
                //groupboxtaki butonlardan mine tagli ve fotosu olan her biri için markedMines 1 arttır
                if ((string)_btn.Tag == "mine" && _btn.Image != null)
                    markedMines++;
            }
            //Kazanma durumu
            if (markedMines == arrayMines.Length)
            {
                timer1.Enabled = false;
                MessageBox.Show("Tebrikler, " + timer + " saniyede kazandınız!");
            }
            else
                MessageBox.Show("Tekrar deneyin.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //combobox default seçili index 0
            cmbSize.SelectedIndex = 0;
        }

        //kronometre
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
            lblTimer.Text = timer.ToString();
        }
    }
}
