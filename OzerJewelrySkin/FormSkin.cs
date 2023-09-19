using MaterialSkin;
using MaterialSkin.Controls;
using OzerJewelry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OzerJewelrySkin
{
    public partial class FormSkin : MaterialForm
    {
        Thread DataGetirThread;
        public FormSkin()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            //materialSkinManager.ColorScheme = new ColorScheme(Primary.Orange800, Primary.Orange900, Primary.Orange500, Accent.Orange200, TextShade.WHITE);
        }

        MaterialSkinManager TManager = MaterialSkinManager.Instance;

        private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialSwitch1.Checked)
            {
                TManager.Theme = MaterialSkinManager.Themes.DARK;
                materialSwitch1.Text = "Dark";
                materialSwitch1.BackColor = Color.Transparent;
                INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
                ini.Yaz("Ayar", "Dark-Light", materialSwitch1.Text);
            }

            else
            {
                TManager.Theme = MaterialSkinManager.Themes.LIGHT;
                materialSwitch1.Text = "Light";
                materialSwitch1.BackColor = Color.Transparent;
                INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
                ini.Yaz("Ayar", "Dark-Light", materialSwitch1.Text);
            }

        }


        public class INIKaydet
        {
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            public INIKaydet(string dosyaYolu)
            {
                DOSYAYOLU = dosyaYolu;
            }
            private string DOSYAYOLU = String.Empty;
            public string Varsayilan { get; set; }
            public string Oku(string bolum, string ayaradi)
            {
                Varsayilan = Varsayilan ?? string.Empty;
                StringBuilder StrBuild = new StringBuilder(256);
                GetPrivateProfileString(bolum, ayaradi, Varsayilan, StrBuild, 255, DOSYAYOLU);
                return StrBuild.ToString();
            }
            public long Yaz(string bolum, string ayaradi, string deger)
            {
                return WritePrivateProfileString(bolum, ayaradi, deger, DOSYAYOLU);
            }
        }

        public void DataGetir()
        {
            double hasaltinaliscolor = Convert.ToDouble(labelHasAltinAlis.Text);
            double hasaltinsatiscolor = Convert.ToDouble(labelHasAltinSatis.Text);

            double dolaraliscolor = Convert.ToDouble(labeldolaralis.Text);
            double dolarsatiscolor = Convert.ToDouble(labeldolarsatis.Text);

            double euroaliscolor = Convert.ToDouble(labeleuroalis.Text);
            double eurosatiscolor = Convert.ToDouble(labeleurosatis.Text);

            var url = new Uri("https://www.hakanaltin.com/"); // url oluştruduk
            WebClient client = new WebClient(); // siteye erişim için client tanımladık
            var html = client.DownloadString(url); //sitenin html lini indirdik
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); //burada HtmlAgilityPack Kütüphanesini kullandık
            doc.LoadHtml(html); // indirdiğimiz sitenin html lini oluşturduğumuz dokumana dolduruyoruz
            var hasaltinalis = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[1]/table/tbody/tr[1]/td[4]/span")[0]; // siteden aldığımız xpath i buraya yazıp kaynak kısmını seçiyoruz
            var hasaltinsatis = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[1]/table/tbody/tr[1]/td[5]/span")[0];

            var hasaltinson = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[1]/table/tbody/tr[1]/td[5]/span")[0];
            var hasaltinson2 = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[1]/table/tbody/tr[1]/td[5]/span")[0];

            var dolaralis = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]/td[4]/span")[0]; ;
            var dolarsatis = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]/td[5]/span")[0]; ;

            var euroalis = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[4]/span")[0]; ;
            var eurosatis = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[5]/span")[0]; ;

            //pictureBox1.ImageLocation = @"green.png";
            //pictureBox2.ImageLocation = @"red.png";




            if (hasaltinalis != null && hasaltinsatis != null)
            {
                labelHasAltinAlis.Text = hasaltinalis.InnerHtml.ToString();
                labelHasAltinSatis.Text = hasaltinsatis.InnerHtml.ToString();
                labelHasAltinSon.Text = hasaltinson.InnerHtml.ToString();
                if (System.Text.RegularExpressions.Regex.Match(labelHasAltinSon.Text, "up").Success)
                {
                    textBox1.Text = "up";
                }
                else
                {
                    textBox1.Text = "down";
                }
            }

            if (euroalis != null && eurosatis != null)
            {
                labeleuroalis.Text = euroalis.InnerText.ToString();
                labeleurosatis.Text = eurosatis.InnerText.ToString();
            }
            if (dolaralis != null && dolarsatis != null)
            {
                labeldolaralis.Text = dolaralis.InnerText.ToString();
                labeldolarsatis.Text = dolarsatis.InnerText.ToString();
            }
            if (hasaltinaliscolor < Convert.ToDouble(labelHasAltinAlis.Text))
            {
                labelHasAltinAlis.ForeColor = Color.Green;
                pictureBox1.Image = Resource1.green;
            }
            else if (hasaltinaliscolor > Convert.ToDouble(labelHasAltinAlis.Text))
            {
                labelHasAltinAlis.ForeColor = Color.Red;
                pictureBox1.Image = Resource1.red;
            }

            if (hasaltinsatiscolor < Convert.ToDouble(labelHasAltinSatis.Text))
            {
                labelHasAltinSatis.ForeColor = Color.Green;
                pictureBox2.Image = Resource1.green;
            }
            else if (hasaltinsatiscolor > Convert.ToDouble(labelHasAltinSatis.Text))
            {
                labelHasAltinSatis.ForeColor = Color.Red;
                pictureBox2.Image = Resource1.red;
            }


            if (euroaliscolor < Convert.ToDouble(labeleuroalis.Text))
            {
                labeleuroalis.ForeColor = Color.White;
                labeleuroalis.BackColor = Color.MediumSeaGreen;
                pictureBoxEURAlis.Image = Resource1.green;
            }
            else if (euroaliscolor > Convert.ToDouble(labeleuroalis.Text))
            {
                labeleuroalis.ForeColor = Color.White;
                labeleuroalis.BackColor = Color.LightCoral;
                pictureBoxEURAlis.Image = Resource1.red;
            }
            else if (euroaliscolor == Convert.ToDouble(labeleuroalis.Text))
            {
                //labeleuroalis.ForeColor = Color.Black;
                labeleuroalis.BackColor = Color.Transparent;
                //pictureBoxEURAlis.Image = Resource1.black;
            }

            if (eurosatiscolor < Convert.ToDouble(labeleurosatis.Text))
            {
                labeleurosatis.ForeColor = Color.White;
                labeleurosatis.BackColor = Color.MediumSeaGreen;
                pictureBoxEURSatis.Image = Resource1.green;
            }
            else if (eurosatiscolor > Convert.ToDouble(labeleurosatis.Text))
            {
                labeleurosatis.ForeColor = Color.White;
                labeleurosatis.BackColor = Color.LightCoral;
                pictureBoxEURSatis.Image = Resource1.red;
            }
            else if (eurosatiscolor == Convert.ToDouble(labeleurosatis.Text))
            {
                //labeleurosatis.ForeColor = Color.Black;
                labeleurosatis.BackColor = Color.Transparent;
                //pictureBoxEURSatis.Image = Resource1.black;
            }




            if (dolarsatiscolor < Convert.ToDouble(labeldolarsatis.Text))
            {
                labeldolarsatis.ForeColor = Color.White;
                labeldolarsatis.BackColor = Color.MediumSeaGreen;
                pictureBoxUSDSatis.Image = Resource1.green;
            }
            else if (dolarsatiscolor > Convert.ToDouble(labeldolarsatis.Text))
            {
                labeldolarsatis.ForeColor = Color.White;
                labeldolarsatis.BackColor = Color.LightCoral;
                pictureBoxUSDSatis.Image = Resource1.red;
            }
            else if (dolarsatiscolor == Convert.ToDouble(labeldolarsatis.Text))
            {
                //labeldolarsatis.ForeColor = Color.Black;
                labeldolarsatis.BackColor = Color.Transparent;
                //pictureBoxUSDSatis.Image = Resource1.black;
            }



            if (dolaraliscolor < Convert.ToDouble(labeldolaralis.Text))
            {
                labeldolaralis.ForeColor = Color.White;
                labeldolaralis.BackColor = Color.MediumSeaGreen;
                pictureBoxUSDAlis.Image = Resource1.green;
            }
            else if (dolaraliscolor > Convert.ToDouble(labeldolaralis.Text))
            {
                labeldolaralis.ForeColor = Color.White;
                labeldolaralis.BackColor = Color.LightCoral;
                pictureBoxUSDAlis.Image = Resource1.red;
            }
            else if (dolaraliscolor == Convert.ToDouble(labeldolaralis.Text))
            {


                labeldolaralis.BackColor = Color.Transparent;
                //pictureBoxUSDAlis.Image = Resource1.black;
            }


        }
        public void darkLight()
        {
            if (materialSwitch1.Checked)
            {
                labeleuroalis.ForeColor = Color.White;
                labeleurosatis.ForeColor = Color.White;
                labeldolarsatis.ForeColor = Color.White;
                labeldolaralis.ForeColor = Color.White;
            }
            else
            {
                labeleuroalis.ForeColor = Color.Black;
                labeleurosatis.ForeColor = Color.Black;
                labeldolarsatis.ForeColor = Color.Black;
                labeldolaralis.ForeColor = Color.Black;
            }
        }
        public void ayarcek()
        {
            try
            {
                if (File.Exists(Application.StartupPath + @"\Ayarlar.ini"))
                {
                    INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
                    textBoxCeyrekSatDeg.Text = ini.Oku("Ceyrek", "Ceyrek Satis Degeri");
                    textBoxCeyrekAlDeg.Text = ini.Oku("Ceyrek", "Ceyrek Alis Degeri");
                    textBoxAtaSatDeg.Text = ini.Oku("Ata", "Ata Satis Degeri");
                    textBoxAtaAlDeg.Text = ini.Oku("Ata", "Ata Alis Degeri");
                    textBoxResatSatDeg.Text = ini.Oku("Resat", "Resat Satis Degeri");
                    textBoxResatAlDeg.Text = ini.Oku("Resat", "Resat Alis Degeri");
                    textBoxResatikibucSatDeg.Text = ini.Oku("Resat 2,5", "Resat 2,5 Satis Degeri");
                    textBoxResatikibucAlDeg.Text = ini.Oku("Resat 2,5", "Resat 2,5 Alis Degeri");
                    textBoxBilezikSatDeg.Text = ini.Oku("Bilezik", "Bilezik Satis Degeri");
                    textBoxBilezikAlDeg.Text = ini.Oku("Bilezik", "Bilezik Alis Degeri");
                    textBoxZiynetSatDeg.Text = ini.Oku("Ziynet", "Ziynet Satis Degeri");
                    textBoxZiynetAlDeg.Text = ini.Oku("Ziynet", "Ziynet Alis Degeri");
                    textBoxZiynetikibucSatDeg.Text = ini.Oku("Ziynet 2,5", "Ziynet 2,5 Satis Degeri");
                    textBoxZiynetikibucAlDeg.Text = ini.Oku("Ziynet 2,5", "Ziynet 2,5 Alis Degeri");
                    textBox8MuhtelifSatDeg.Text = ini.Oku("8Muhtelif", "8Muhtelif Satis Degeri");
                    textBox8MuhtelifAlDeg.Text = ini.Oku("8Muhtelif", "8Muhtelif Alis Degeri");
                    textBox14MuhtelifSatDeg.Text = ini.Oku("14Muhtelif", "14Muhtelif Satis Degeri");
                    textBox14MuhtelifAlDeg.Text = ini.Oku("14Muhtelif", "14Muhtelif Alis Degeri");
                    textBoxResatBesSatDeg.Text = ini.Oku("Beslik", "Beslik Satis Degeri");
                    textBoxResatBesAlDeg.Text = ini.Oku("Beslik", "Beslik Alis Degeri");
                    textBoxIkiBucuklukSatDeg.Text = ini.Oku("IkiBucukluk", "IkiBucukluk Satis Degeri");
                    textBoxIkiBucuklukAlDeg.Text = ini.Oku("IkiBucukluk", "IkiBucukluk Alis Degeri");
                    textBox1GramlikSatDeg.Text = ini.Oku("1Gramlik", "1Gramlik Satis Degeri");
                    textBox1GramlikAlDeg.Text = ini.Oku("1Gramlik", "1Gramlik Alis Degeri");
                    textBox1Gramlik24kSatDeg.Text = ini.Oku("1Gramlik 24k", "1Gramlik 24k Satis Degeri");
                    textBox1Gramlik24kAlDeg.Text = ini.Oku("1Gramlik 24k", "1Gramlik 24k Alis Degeri");
                    textBoxYarimGramlikSatDeg.Text = ini.Oku("YarimGramlik", "YarimGramlik Satis Degeri");
                    textBoxYarimGramlikAlDeg.Text = ini.Oku("YarimGramlik", "YarimGramlik Alis Degeri");
                    materialSwitch1.Text = ini.Oku("Ayar", "Dark-Light");

                    if (materialSwitch1.Text == "Dark")
                    {
                        materialSwitch1.Checked = true;
                        TManager.Theme = MaterialSkinManager.Themes.DARK;
                    }
                    else if (materialSwitch1.Text == "Light")
                    {
                        materialSwitch1.Checked = false;
                        TManager.Theme = MaterialSkinManager.Themes.LIGHT;
                    }
                }
            }
            catch (Exception hata)
            {
                MessageBox.Show("ini dosyası hasarlı" + hata.Message);
            }
        }
        public void change()
        {
            if (textBoxCeyrekSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxCeyrekSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxCeyrekSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelCeyrekSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxCeyrekAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxCeyrekAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelCeyrekAl.Text = moneyFormat2;
            }
            if (textBoxAtaSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxAtaSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxAtaSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelAtaSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxAtaAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxAtaAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelAtaAl.Text = moneyFormat2;
            }
            if (textBoxResatSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxResatSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxResatSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelResatSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxResatAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxResatAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelResatAl.Text = moneyFormat2;
            }
            if (textBoxResatikibucSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxResatikibucSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxResatikibucSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelResatikibucSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxResatikibucAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxResatikibucAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelResatikibucAl.Text = moneyFormat2;
            }
            if (textBoxBilezikSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxBilezikSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxBilezikSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelBilezikSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxBilezikAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxBilezikAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelBilezikAl.Text = moneyFormat2;
            }
            if (textBoxZiynetSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxZiynetSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxZiynetSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelZiynetSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxZiynetAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxZiynetAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelZiynetAl.Text = moneyFormat2;
            }
            if (textBoxZiynetikibucSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxZiynetikibucSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxZiynetikibucSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelZiynetikibucSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxZiynetikibucAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxZiynetikibucAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelZiynetikibucAl.Text = moneyFormat2;
            }
            if (textBox8MuhtelifSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBox8MuhtelifSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBox8MuhtelifSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                label8MuhtelifSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBox8MuhtelifAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBox8MuhtelifAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                label8MuhtelifAl.Text = moneyFormat2;
            }
            if (textBox14MuhtelifSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBox14MuhtelifSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBox14MuhtelifSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                label14MuhtelifSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBox14MuhtelifAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBox14MuhtelifAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                label14MuhtelifAl.Text = moneyFormat2;
            }
            if (textBoxResatBesSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxResatBesSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxResatBesSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelResatBesSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxResatBesAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxResatBesAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelResatBesAl.Text = moneyFormat2;
            }
            if (textBoxIkiBucuklukSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxIkiBucuklukSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxIkiBucuklukSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelIkiBucuklukSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxIkiBucuklukAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxIkiBucuklukAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelIkiBucuklukAl.Text = moneyFormat2;
            }
            if (textBox1GramlikSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBox1GramlikSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBox1GramlikSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                label1GramlikSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBox1GramlikAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBox1GramlikAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                label1GramlikAl.Text = moneyFormat2;
            }
            if (textBox1Gramlik24kSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBox1Gramlik24kSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBox1Gramlik24kSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                label1Gramlik24kSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBox1Gramlik24kAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBox1Gramlik24kAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                label1Gramlik24kAl.Text = moneyFormat2;
            }
            if (textBoxYarimGramlikSatDeg.Text != "")
            {
                double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
                double sayi2;
                if (textBoxYarimGramlikSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDouble(textBoxYarimGramlikSatDeg.Text);
                }

                double sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("#,##0.00");
                labelYarimGramlikSat.Text = moneyFormat;

                double sayi3 = Convert.ToDouble(labelHasAltinAlis.Text);
                double sayi4;
                if (textBoxYarimGramlikAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDouble(textBoxYarimGramlikAlDeg.Text);
                }

                double sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("#,##0.00");
                labelYarimGramlikAl.Text = moneyFormat2;
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            DataGetir();
            ayarcek();
            timer1.Start();

            darkLight();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            DataGetirThread = new Thread(new ThreadStart(DataGetir));
            DataGetirThread.IsBackground = true;
            DataGetirThread.Start();

            change();
            darkLight();
        }
        #region keypress
        private void textBoxCeyrekSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxCeyrekAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxAtaSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxAtaAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxResatSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxResatAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxBilezikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxBilezikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxZiynetSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxZiynetAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox8MuhtelifSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox8MuhtelifAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox14MuhtelifSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox14MuhtelifAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxBeslikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxBeslikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxIkiBucuklukSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxIkiBucuklukAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox1GramlikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox1GramlikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxYarimGramlikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxYarimGramlikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }
        private void textBoxResatikibucSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxResatikibucAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxZiynetikibucSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxZiynetikibucAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }
        private void textBox1Gramlik24kSatDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox1Gramlik24kAlDeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        #endregion
        #region textchanged
        private void textBoxCeyrekSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxCeyrekSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxCeyrekSatDeg.Text);
            }
            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelCeyrekSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ceyrek", "Ceyrek Satis Degeri", textBoxCeyrekSatDeg.Text);
        }

        private void textBoxCeyrekAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxCeyrekAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxCeyrekAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelCeyrekAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ceyrek", "Ceyrek Alis Degeri", textBoxCeyrekAlDeg.Text);
        }

        private void textBoxAtaSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxAtaSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxAtaSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelAtaSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ata", "Ata Satis Degeri", textBoxAtaSatDeg.Text);
        }

        private void textBoxAtaAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxAtaAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxAtaAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelAtaAl.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ata", "Ata Alis Degeri", textBoxAtaAlDeg.Text);
        }

        private void textBoxResatSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxResatSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelResatSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat", "Resat Satis Degeri", textBoxResatSatDeg.Text);
        }

        private void textBoxResatAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxResatAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelResatAl.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat", "Resat Alis Degeri", textBoxResatAlDeg.Text);
        }
        private void textBoxResatikibucSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxResatikibucSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatikibucSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelResatikibucSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat 2,5", "Resat 2,5 Satis Degeri", textBoxResatikibucSatDeg.Text);
        }

        private void textBoxResatikibucAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxResatikibucAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatikibucAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelResatikibucAl.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat 2,5", "Resat 2,5 Alis Degeri", textBoxResatikibucAlDeg.Text);
        }

        private void textBoxBilezikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxBilezikSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxBilezikSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelBilezikSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Bilezik", "Bilezik Satis Degeri", textBoxBilezikSatDeg.Text);
        }

        private void textBoxBilezikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxBilezikAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxBilezikAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelBilezikAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Bilezik", "Bilezik Alis Degeri", textBoxBilezikAlDeg.Text);
        }

        private void textBoxZiynetSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxZiynetSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelZiynetSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet", "Ziynet Satis Degeri", textBoxZiynetSatDeg.Text);
        }

        private void textBoxZiynetAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxZiynetAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelZiynetAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet", "Ziynet Alis Degeri", textBoxZiynetAlDeg.Text);
        }
        private void textBoxZiynetikibucSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxZiynetikibucSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetikibucSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelZiynetikibucSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet 2,5", "Ziynet 2,5 Satis Degeri", textBoxZiynetikibucSatDeg.Text);
        }

        private void textBoxZiynetikibucAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxZiynetikibucAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetikibucAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelZiynetikibucAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet 2,5", "Ziynet 2,5 Alis Degeri", textBoxZiynetikibucAlDeg.Text);
        }

        private void textBox8MuhtelifSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBox8MuhtelifSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox8MuhtelifSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label8MuhtelifSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("8Muhtelif", "8Muhtelif Satis Degeri", textBox8MuhtelifSatDeg.Text);
        }

        private void textBox8MuhtelifAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBox8MuhtelifAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox8MuhtelifAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label8MuhtelifAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("8Muhtelif", "8Muhtelif Alis Degeri", textBox8MuhtelifAlDeg.Text);
        }

        private void textBox14MuhtelifSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBox14MuhtelifSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox14MuhtelifSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label14MuhtelifSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("14Muhtelif", "14Muhtelif Satis Degeri", textBox14MuhtelifSatDeg.Text);
        }

        private void textBox14MuhtelifAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBox14MuhtelifAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox14MuhtelifAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label14MuhtelifAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("14Muhtelif", "14Muhtelif Alis Degeri", textBox14MuhtelifAlDeg.Text);
        }

        private void textBoxBeslikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxResatBesSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatBesSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelResatBesSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Beslik", "Beslik Satis Degeri", textBoxResatBesSatDeg.Text);
        }

        private void textBoxBeslikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxResatBesAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatBesAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelResatBesAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Beslik", "Beslik Alis Degeri", textBoxResatBesAlDeg.Text);
        }

        private void textBoxIkiBucuklukSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxIkiBucuklukSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxIkiBucuklukSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelIkiBucuklukSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("IkiBucukluk", "IkiBucukluk Satis Degeri", textBoxIkiBucuklukSatDeg.Text);
        }

        private void textBoxIkiBucuklukAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxIkiBucuklukAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxIkiBucuklukAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelIkiBucuklukAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("IkiBucukluk", "IkiBucukluk Alis Degeri", textBoxIkiBucuklukAlDeg.Text);
        }

        private void textBox1GramlikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBox1GramlikSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1GramlikSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label1GramlikSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik", "1Gramlik Satis Degeri", textBox1GramlikSatDeg.Text);
        }

        private void textBox1GramlikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBox1GramlikAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1GramlikAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label1GramlikAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik", "1Gramlik Alis Degeri", textBox1GramlikAlDeg.Text);
        }

        private void textBoxYarimGramlikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBoxYarimGramlikSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxYarimGramlikSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelYarimGramlikSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("YarimGramlik", "YarimGramlik Satis Degeri", textBoxYarimGramlikSatDeg.Text);
        }

        private void textBoxYarimGramlikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBoxYarimGramlikAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxYarimGramlikAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            labelYarimGramlikAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("YarimGramlik", "YarimGramlik Alis Degeri", textBoxYarimGramlikAlDeg.Text);
        }
        private void textBox1Gramlik24kAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinAlis.Text);
            double sayi2;
            if (textBox1Gramlik24kAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1Gramlik24kAlDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label1Gramlik24kAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik 24k", "1Gramlik 24k Alis Degeri", textBox1Gramlik24kAlDeg.Text);
        }
        private void textBox1Gramlik24kSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(labelHasAltinSatis.Text);
            double sayi2;
            if (textBox1Gramlik24kSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1Gramlik24kSatDeg.Text);
            }

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("#,##0.00");
            label1Gramlik24kSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik 24k", "1Gramlik 24k Satis Degeri", textBox1Gramlik24kSatDeg.Text);
        }


        #endregion



    }
}
