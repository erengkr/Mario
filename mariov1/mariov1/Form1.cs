using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace mariov1
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// OleDb sınıfı yardımıyla veri tabanına bağlantı sağlıyoruz 
        /// int olarak gerekli elemanları tanımladım
        /// true false değeri döndürmesi gerekern değerleri ise bool ile tnaımladım
        /// </summary>
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0; Data Source=Veri.accdb");
        OleDbCommand baglan = new OleDbCommand();

        bool gitSol, gitSag, zıpla, anahtar_al;
        int zıplamahızı = 10;
        int force = 8;
        int skor = 0;
        int karakter_hız = 10;
        int arkaplanhızı = 8;



        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Oyunumuzda timer kullandık ve karakterin boyutlarına göre zıplama mesafesini ayarladık
        /// sağ ve sola hareketlerini yukarı kısımda berlirlediğimiz karakter hızı ile belirledik.
        /// Karakter sağa yada sola hareketini gerçekleştirirken arkaplanın ve arka plan üstündeki diğer yapıların karakterle orantılı olarak sağa yada sola gitmesini belirledik
        /// Karakterimizle altınları topladığımızda skoru 1 artırdık.
        /// Bölümün en sonunda yer alan anahtarı alıp geri geldiğimizde ise kapı açılıyor (yeşile dönüyor) ve bölümü başarılı bir şekilde tamamlamış oluyoruz.
        /// Eğer yapının üstünde duramayıp aşağı düşersek ekrana "Öldün Tekrar oynamak için tıklayın" yazısı geliyor ve ölene kadar topladığımız skoru veri tabanına kaydediyor
        /// Bölümü başarılı bir şekilde tamamladğımızda ise bölüm tamamlana kadar toplanan skor veri tabanına kaydediliyor ve "Tebrikler Tekrar oynamak için tıklayın" yazısı geliyor
        /// baglantı.open yapısıyla veri tabanına kayıt tamamlanıyor
        /// 
        /// </summary>

        private void Zamanlayıcı_Tick(object sender, EventArgs e)
        {
            txtScore.Text = "Skor:" + skor;
            oyuncu.Top += zıplamahızı;

            if (gitSol==true && oyuncu.Left>60)
            {
                oyuncu.Left -= karakter_hız;
            }
            if (gitSag==true && oyuncu.Left+(oyuncu.Width+60)<this.ClientSize.Width)
            {
                oyuncu.Left += karakter_hız;
            }


            if (gitSol==true&&arkaplan.Left<0)
            {
                arkaplan.Left += arkaplanhızı;
                Ogehareket("ileri");
            }
            if (gitSag == true && arkaplan.Left > -1377)
            {
                arkaplan.Left -= arkaplanhızı;
                Ogehareket("geri");
            }
            if (zıpla == true)
            {
                zıplamahızı = -12;
                force -= 1;

            }
            else
            {
                zıplamahızı = 12;
            }

            if (zıpla == true && force < 0)
            {
                zıpla = false;
            }
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "platform")
                {


                    if (oyuncu.Bounds.IntersectsWith(x.Bounds) && zıpla == false)
                    {
                        force = 8;
                        oyuncu.Top = x.Top - oyuncu.Height;
                        zıplamahızı = 0;

                    }
                    x.BringToFront();
                }
                if (x is PictureBox&&(string)x.Tag=="altın")
                {
                    if (oyuncu.Bounds.IntersectsWith(x.Bounds)&&x.Visible==true)
                    {
                        x.Visible = false;
                        skor += 1;
                    }
                }

            }
            if (oyuncu.Bounds.IntersectsWith(key.Bounds))
            {
                key.Visible = false;
                anahtar_al = true;
            }

            if (oyuncu.Bounds.IntersectsWith(kapı.Bounds)&&anahtar_al==true)
            {
                kapı.Image = Properties.Resources.kapı_acık;
                Zamanlayıcı.Stop();
                MessageBox.Show("Tebrikler" + Environment.NewLine + "Tekrar oynamak için tıklayın");
                YenidenBaslat();
                baglanti.Open();
                baglan.Connection = baglanti;
                baglan.CommandText = "INSERT INTO Mario(skor) VALUES('" + skor + "')";
                baglan.ExecuteNonQuery();


            }
            if (oyuncu.Top+oyuncu.Height>this.ClientSize.Height)
            {
                Zamanlayıcı.Stop();
                MessageBox.Show("Öldün" + Environment.NewLine + "Tekrar oynamak için tıklayın");
                YenidenBaslat();
                baglanti.Open();
                baglan.Connection = baglanti;
                baglan.CommandText="INSERT INTO Mario(skor) VALUES('"+skor+"')";
                baglan.ExecuteNonQuery();

            }
        }
        /// <summary>
        /// space tusuna basıldığında zıplamasını ve sağ sol tuslarıyla hareketi tamamlıyor
        /// </summary>
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Left)
            {
                gitSol = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                gitSag = true;
            }
            if (e.KeyCode == Keys.Space && zıpla==false)
            {
                zıpla = true;
            }

        }

        /// <summary>
        /// space tusuna basıldığında zıplamasını ve sağ sol tuslarıyla hareketi tamamlıyor
        /// </summary>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Left)
            {
                gitSol = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                gitSag = false;
            }
            if (zıpla == true)
            {
                zıpla = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// Eğer oyunu kapatırsak formun kapatılmasını sağlıyor
        /// </summary>
        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// oyunu yeniden baslamasını istedğimiz durumlarda yani ya bölüm tamamlandığında yada öldüğümüzde oyunu bastan baslatıyor.
        /// </summary>
        private void YenidenBaslat()
        {
            Form1 newWindow = new Form1();
            newWindow.Show();
            this.Hide();



        }
        private void Ogehareket(string direction)
        {
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag=="platform"||x is PictureBox&& (string)x.Tag=="altın"|| x is PictureBox && (string)x.Tag == "key"|| x is PictureBox && (string)x.Tag == "kapı")
                {
                    if (direction == "geri")
                    {
                        x.Left -= arkaplanhızı;
                    }
                    if (direction=="ileri")
                    {
                        x.Left += arkaplanhızı;
                    }


                }
            }
        }
    }
}
