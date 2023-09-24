using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using SocketIOClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using OzerJewelry.Properties;

namespace OzerJewelry
{
    public partial class OzerJewelry : Form
    {
        Thread DataGetirThread;
        public OzerJewelry()
        {
            InitializeComponent();
        }

        public class Root
        {
            public data data { get; set; }
            public meta meta { get; set; }
        }
        public class data
        {
            public ALTIN ALTIN { get; set; }
            public USDTRY USDTRY { get; set; }
            public EURTRY EURTRY { get; set; }
        }
        public class meta
        {
            public string time { get; set; }
            public string tarih { get; set; }
        }
        public class ALTIN
        {
            public string code { get; set; }
            public string alis { get; set; }
            public string satis { get; set; }
        }
        public class USDTRY
        {
            public string code { get; set; }
            public string alis { get; set; }
            public string satis { get; set; }
        }
        public class EURTRY
        {
            public string code { get; set; }
            public string alis { get; set; }
            public string satis { get; set; }
        }


        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr,
        ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        internal static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Text = "Dark";
                checkBox1.BackColor = Color.Transparent;
                checkBox1.ForeColor = Color.White;
                INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
                ini.Yaz("Ayar", "Dark-Light", checkBox1.Text);
            }

            else
            {
                checkBox1.Text = "Light";
                checkBox1.BackColor = Color.Transparent;
                checkBox1.ForeColor = Color.Black;
                INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
                ini.Yaz("Ayar", "Dark-Light", checkBox1.Text);
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
        string hasaltinsatisfirst = "";
        decimal hasaltinsatis = 0;

        string hasaltinalisfirst = "";
        decimal hasaltinalis = 0;

        string dolaralisfirst = "";
        decimal dolaralis = 0;
        string dolarsatisfirst = "";
        decimal dolarsatis = 0;

        string euroalisfirst = "";
        decimal euroalis = 0;
        string eurosatisfirst = "";
        decimal eurosatis = 0;

        public static async Task haremaltingetir(decimal ALTINalis, decimal ALTINsatis)
        {
            var client = new SocketIO("wss://www.haremaltin.com:2088");


            string USDTRYalis = "0.0";
            string USDTRYsatis = "0.0";

            string EURTRYalis = "0.0";
            string EURTRYsatis = "0.0";

            //string ALTINalis = "0.0";
            //string ALTINsatis = "0.0";




            client.On("price_changed", response =>
            {

                string gelendata = response.ToString();


                var objResponse1 = JsonConvert.DeserializeObject<List<Root>>(gelendata);
                var a = objResponse1[0];


                if (a.data.USDTRY != null)
                {
                    USDTRYalis = a.data.USDTRY.alis;
                    USDTRYsatis = a.data.USDTRY.satis;


                    INIKaydet ini = new INIKaydet(AppDomain.CurrentDomain.BaseDirectory + @"Ayarlar.ini");
                    ini.Yaz("HAREMALTIN_USDTRY", "USDTRY Alis Degeri", a.data.USDTRY.alis);
                    ini.Yaz("HAREMALTIN_USDTRY", "USDTRY Satis Degeri", a.data.USDTRY.satis);
                }
                if (a.data.EURTRY != null)
                {
                    EURTRYalis = a.data.EURTRY.alis;
                    EURTRYsatis = a.data.EURTRY.satis;

                    INIKaydet ini = new INIKaydet(AppDomain.CurrentDomain.BaseDirectory + @"Ayarlar.ini");
                    ini.Yaz("HAREMALTIN_EURTRY", "EURTRY Alis Degeri", a.data.EURTRY.alis);
                    ini.Yaz("HAREMALTIN_EURTRY", "EURTRY Satis Degeri", a.data.EURTRY.satis);
                }
                if (a.data.ALTIN != null)
                {
                    ALTINalis = Convert.ToDecimal(a.data.ALTIN.alis);
                    ALTINsatis = Convert.ToDecimal(a.data.ALTIN.satis);

                    INIKaydet ini = new INIKaydet(AppDomain.CurrentDomain.BaseDirectory + @"Ayarlar.ini");
                    ini.Yaz("HAREMALTIN_HASALTIN", "HASALTIN Alis Degeri", a.data.ALTIN.alis);
                    ini.Yaz("HAREMALTIN_HASALTIN", "HASALTIN Satis Degeri", a.data.ALTIN.satis);
                }
                //Console.Clear();
                //Console.WriteLine("USDTRY");
                //Console.WriteLine("ALIŞ  : " + USDTRYalis);
                //Console.WriteLine("SATIŞ : " + USDTRYsatis);

                //Console.WriteLine("EURTRY");
                //Console.WriteLine("ALIŞ  : " + EURTRYalis);
                //Console.WriteLine("SATIŞ : " + EURTRYsatis);

                //Console.WriteLine("ALTIN");
                //Console.WriteLine("ALIŞ  : " + ALTINalis);
                //Console.WriteLine("SATIŞ : " + ALTINsatis);

            });

            client.OnConnected += async (sender, e) =>
            {
                // Emit a string
                await client.EmitAsync("");
                Console.WriteLine("Connected");


                // Emit a string and an object
            };
            await client.ConnectAsync();
        }

        public async void DataGetir()
        {

            decimal hasaltinaliscolor = Convert.ToDecimal(hasaltinalis);
            decimal hasaltinsatiscolor = Convert.ToDecimal(hasaltinsatis);


            decimal dolaraliscolor = Convert.ToDecimal(dolaralis);
            decimal dolarsatiscolor = Convert.ToDecimal(dolarsatis);

            decimal euroaliscolor = Convert.ToDecimal(euroalis);
            decimal eurosatiscolor = Convert.ToDecimal(eurosatis);


            var hasaltinson = "";
            var hasaltinson2 = "";



            var client = new SocketIO("wss://www.haremaltin.com:2088");


            string USDTRYalis = "0.0";
            string USDTRYsatis = "0.0";

            string EURTRYalis = "0.0";
            string EURTRYsatis = "0.0";

            string ALTINalis = "0.0";
            string ALTINsatis = "0.0";




            client.On("price_changed", response =>
            {

                string gelendata = response.ToString();


                var objResponse1 = JsonConvert.DeserializeObject<List<Root>>(gelendata);
                var a = objResponse1[0];


                if (a.data.USDTRY != null)
                {
                    dolaralisfirst = a.data.USDTRY.alis;
                    dolarsatisfirst = a.data.USDTRY.satis;


                    INIKaydet ini = new INIKaydet(AppDomain.CurrentDomain.BaseDirectory + @"Ayarlar.ini");
                    ini.Yaz("HAREMALTIN_USDTRY", "USDTRY Alis Degeri", a.data.USDTRY.alis);
                    ini.Yaz("HAREMALTIN_USDTRY", "USDTRY Satis Degeri", a.data.USDTRY.satis);
                }
                if (a.data.EURTRY != null)
                {
                    euroalisfirst = a.data.EURTRY.alis;
                    eurosatisfirst = a.data.EURTRY.satis;

                    INIKaydet ini = new INIKaydet(AppDomain.CurrentDomain.BaseDirectory + @"Ayarlar.ini");
                    ini.Yaz("HAREMALTIN_EURTRY", "EURTRY Alis Degeri", a.data.EURTRY.alis);
                    ini.Yaz("HAREMALTIN_EURTRY", "EURTRY Satis Degeri", a.data.EURTRY.satis);
                }
                if (a.data.ALTIN != null)
                {
                    hasaltinalisfirst = a.data.ALTIN.alis;
                    hasaltinsatisfirst = a.data.ALTIN.satis;

                    INIKaydet ini = new INIKaydet(AppDomain.CurrentDomain.BaseDirectory + @"Ayarlar.ini");
                    ini.Yaz("HAREMALTIN_HASALTIN", "HASALTIN Alis Degeri", a.data.ALTIN.alis);
                    ini.Yaz("HAREMALTIN_HASALTIN", "HASALTIN Satis Degeri", a.data.ALTIN.satis);
                }
                //Console.Clear();
                //Console.WriteLine("USDTRY");
                //Console.WriteLine("ALIŞ  : " + USDTRYalis);
                //Console.WriteLine("SATIŞ : " + USDTRYsatis);

                //Console.WriteLine("EURTRY");
                //Console.WriteLine("ALIŞ  : " + EURTRYalis);
                //Console.WriteLine("SATIŞ : " + EURTRYsatis);

                //Console.WriteLine("ALTIN");
                //Console.WriteLine("ALIŞ  : " + ALTINalis);
                //Console.WriteLine("SATIŞ : " + ALTINsatis);

            });

            client.OnConnected += async (sender, e) =>
            {
                // Emit a string
                await client.EmitAsync("");
                Console.WriteLine("Connected");



                // Emit a string and an object
            };
            await client.ConnectAsync();




            //try
            //{
            //    if (File.Exists(Application.StartupPath + @"\Ayarlar.ini"))
            //    {
            //        INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            //        hasaltinsatisfirst = ini.Oku("HAREMALTIN_HASALTIN", "HASALTIN Satis Degeri");


            //        hasaltinalisfirst = ini.Oku("HAREMALTIN_HASALTIN", "HASALTIN Alis Degeri");

            //        hasaltinson = ini.Oku("HAREMALTIN_HASALTIN", "HASALTIN Satis Degeri");
            //        hasaltinson2 = ini.Oku("HAREMALTIN_HASALTIN", "HASALTIN Satis Degeri");

            //        dolaralisfirst = ini.Oku("HAREMALTIN_USDTRY", "USDTRY Alis Degeri");
            //        dolarsatisfirst = ini.Oku("HAREMALTIN_USDTRY", "USDTRY Satis Degeri");

            //        euroalisfirst = ini.Oku("HAREMALTIN_EURTRY", "EURTRY Alis Degeri");
            //        eurosatisfirst = ini.Oku("HAREMALTIN_EURTRY", "EURTRY Satis Degeri");

            //        if (checkBox1.Text == "Dark")
            //        {
            //            checkBox1.Checked = true;
            //        }
            //        else if (checkBox1.Text == "Light")
            //        {
            //            checkBox1.Checked = false;
            //        }


            //    }
            //}
            //catch (Exception hata)
            //{
            //}


            if (hasaltinsatisfirst != "" || hasaltinalisfirst != "")
            {
                hasaltinsatis = Convert.ToDecimal(hasaltinsatisfirst.Replace(".", ","));
                hasaltinalis = Convert.ToDecimal(hasaltinalisfirst.Replace(".", ","));
            }


            if (dolarsatisfirst != "" || dolaralisfirst != "")
            {
                dolarsatis = Convert.ToDecimal(dolarsatisfirst.Replace(".", ","));
                dolaralis = Convert.ToDecimal(dolaralisfirst.Replace(".", ","));
            }


            if (eurosatisfirst != "" || euroalisfirst != "")
            {
                eurosatis = Convert.ToDecimal(eurosatisfirst.Replace(".", ","));
                euroalis = Convert.ToDecimal(euroalisfirst.Replace(".", ","));
            }





            labelHasAltinSatis.Text = hasaltinsatis.ToString("C3");
            labelHasAltinAlis.Text = hasaltinalis.ToString("C3");

            labeldolarsatis.Text = dolarsatis.ToString("C3");
            labeldolaralis.Text = dolaralis.ToString("C3");

            labeleurosatis.Text = eurosatis.ToString("C3");
            labeleuroalis.Text = euroalis.ToString("C3");






            decimal Decimal_hasaltinAlis = Convert.ToDecimal(hasaltinsatis);
            decimal Decimal_hasaltinSatis = Convert.ToDecimal(hasaltinsatis);
            labelHasAltinSon.Text = hasaltinson.ToString();


            if (System.Text.RegularExpressions.Regex.Match(labelHasAltinSon.Text, "up").Success)
            {
                textBox1.Text = "up";
            }
            else
            {
                textBox1.Text = "down";
            }


            //if (euroalis != null && eurosatis != null)
            //{
            //    try
            //    {
            //        labeleuroalis.Text = euroalis.ToString();
            //        labeleurosatis.Text = eurosatis.ToString();
            //    }
            //    catch
            //    {


            //    }

            //}
            //if (dolaralis != null && dolarsatis != null)
            //{
            //    labeldolaralis.Text = dolaralis.ToString();
            //    labeldolarsatis.Text = dolarsatis.ToString();
            //}
            if (hasaltinaliscolor < Convert.ToDecimal(hasaltinalis))
            {
                labelHasAltinAlis.ForeColor = Color.Green;
                pictureBox1.Image = Resource1.green;
            }
            else if (hasaltinaliscolor > Convert.ToDecimal(hasaltinalis))
            {
                labelHasAltinAlis.ForeColor = Color.Red;
                pictureBox1.Image = Resource1.red;
            }

            if (hasaltinsatiscolor < Convert.ToDecimal(hasaltinsatis))
            {
                labelHasAltinSatis.ForeColor = Color.Green;
                pictureBox2.Image = Resource1.green;
            }
            else if (hasaltinsatiscolor > Convert.ToDecimal(hasaltinsatis))
            {
                labelHasAltinSatis.ForeColor = Color.Red;
                pictureBox2.Image = Resource1.red;
            }


            if (euroaliscolor < Convert.ToDecimal(euroalis))
            {
                labeleuroalis.ForeColor = Color.White;
                labeleuroalis.BackColor = Color.MediumSeaGreen;
                pictureBoxEURAlis.Image = Resource1.green;
            }
            else if (euroaliscolor > Convert.ToDecimal(euroalis))
            {
                labeleuroalis.ForeColor = Color.White;
                labeleuroalis.BackColor = Color.LightCoral;
                pictureBoxEURAlis.Image = Resource1.red;
            }
            else if (euroaliscolor == Convert.ToDecimal(euroalis))
            {
                //labeleuroalis.ForeColor = Color.Black;
                labeleuroalis.BackColor = Color.Transparent;
                //pictureBoxEURAlis.Image = Resource1.black;
            }

            if (eurosatiscolor < Convert.ToDecimal(eurosatis))
            {
                labeleurosatis.ForeColor = Color.White;
                labeleurosatis.BackColor = Color.MediumSeaGreen;
                pictureBoxEURSatis.Image = Resource1.green;
            }
            else if (eurosatiscolor > Convert.ToDecimal(eurosatis))
            {
                labeleurosatis.ForeColor = Color.White;
                labeleurosatis.BackColor = Color.LightCoral;
                pictureBoxEURSatis.Image = Resource1.red;
            }
            else if (eurosatiscolor == Convert.ToDecimal(eurosatis))
            {
                //labeleurosatis.ForeColor = Color.Black;
                labeleurosatis.BackColor = Color.Transparent;
                //pictureBoxEURSatis.Image = Resource1.black;
            }




            if (dolarsatiscolor < Convert.ToDecimal(dolarsatis))
            {
                labeldolarsatis.ForeColor = Color.White;
                labeldolarsatis.BackColor = Color.MediumSeaGreen;
                pictureBoxUSDSatis.Image = Resource1.green;
            }
            else if (dolarsatiscolor > Convert.ToDecimal(dolarsatis))
            {
                labeldolarsatis.ForeColor = Color.White;
                labeldolarsatis.BackColor = Color.LightCoral;
                pictureBoxUSDSatis.Image = Resource1.red;
            }
            else if (dolarsatiscolor == Convert.ToDecimal(dolarsatis))
            {
                //labeldolarsatis.ForeColor = Color.Black;
                labeldolarsatis.BackColor = Color.Transparent;
                //pictureBoxUSDSatis.Image = Resource1.black;
            }



            if (dolaraliscolor < Convert.ToDecimal(dolaralis))
            {
                labeldolaralis.ForeColor = Color.White;
                labeldolaralis.BackColor = Color.MediumSeaGreen;
                pictureBoxUSDAlis.Image = Resource1.green;
            }
            else if (dolaraliscolor > Convert.ToDecimal(dolaralis))
            {
                labeldolaralis.ForeColor = Color.White;
                labeldolaralis.BackColor = Color.LightCoral;
                pictureBoxUSDAlis.Image = Resource1.red;
            }
            else if (dolaraliscolor == Convert.ToDecimal(dolaralis))
            {


                labeldolaralis.BackColor = Color.Transparent;
                //pictureBoxUSDAlis.Image = Resource1.black;
            }

            labelHasAltinSatis.Text = hasaltinsatis.ToString("C3");
            labelHasAltinAlis.Text = hasaltinalis.ToString("C3");

            labeldolarsatis.Text = dolarsatis.ToString("C3");
            labeldolaralis.Text = dolaralis.ToString("C3");

            labeleurosatis.Text = eurosatis.ToString("C3");
            labeleuroalis.Text = euroalis.ToString("C3");



        }
        public void darkLight()
        {
            if (checkBox1.Checked)
            {

                UseImmersiveDarkMode(this.Handle, true);
                this.BackColor = Color.FromArgb(30, 30, 30);

                // EUR Label
                label8.ForeColor = Color.White;
                label17.ForeColor = Color.White;
                label18.ForeColor = Color.White;

                // USD Label
                label9.ForeColor = Color.White;
                label20.ForeColor = Color.White;
                label21.ForeColor = Color.White;

                // Has Altın Label
                label23.ForeColor = Color.White;
                label24.ForeColor = Color.White;
                label26.ForeColor = Color.White;

                // Table Label
                label3.ForeColor = Color.White;
                label14.ForeColor = Color.White;
                label4.ForeColor = Color.White;
                label15.ForeColor = Color.White;

                labeleuroalis.ForeColor = Color.White;
                labeleurosatis.ForeColor = Color.White;
                labeldolarsatis.ForeColor = Color.White;
                labeldolaralis.ForeColor = Color.White;

                labelHasAltinAlis.ForeColor = Color.White;
                labelHasAltinSatis.ForeColor = Color.White;

                //Panel

                panel1.BackColor = Color.FromArgb(100, 100, 100);
                panel2.BackColor = Color.FromArgb(60, 60, 60);
                panel3.BackColor = Color.FromArgb(100, 100, 100);
                panel4.BackColor = Color.FromArgb(60, 60, 60);
                panel5.BackColor = Color.FromArgb(100, 100, 100);
                panel6.BackColor = Color.FromArgb(60, 60, 60);
                panel7.BackColor = Color.FromArgb(100, 100, 100);
                panel8.BackColor = Color.FromArgb(60, 60, 60);
                panel9.BackColor = Color.FromArgb(100, 100, 100);
                panel10.BackColor = Color.FromArgb(60, 60, 60);
                panel11.BackColor = Color.FromArgb(100, 100, 100);
                panel12.BackColor = Color.FromArgb(60, 60, 60);

                panel1.ForeColor = Color.White;
                panel2.ForeColor = Color.White;
                panel3.ForeColor = Color.White;
                panel4.ForeColor = Color.White;
                panel5.ForeColor = Color.White;
                panel6.ForeColor = Color.White;
                panel7.ForeColor = Color.White;
                panel8.ForeColor = Color.White;
                panel9.ForeColor = Color.White;
                panel10.ForeColor = Color.White;
                panel11.ForeColor = Color.White;
                panel12.ForeColor = Color.White;

                //Textbox
                textBoxCeyrekSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxCeyrekAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxAtaAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxAtaSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxResatAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxResatSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxResatikibucAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxResatikibucSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxResatBesSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxResatBesAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxBilezikAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxBilezikSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxZiynetSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxZiynetAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxZiynetikibucSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxZiynetikibucAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxIkiBucuklukSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxIkiBucuklukAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox8MuhtelifSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox8MuhtelifAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox14MuhtelifSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox14MuhtelifAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox1Gramlik24kSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox1Gramlik24kAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox1GramlikSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBox1GramlikAlDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxYarimGramlikSatDeg.BackColor = Color.FromArgb(30, 30, 30);
                textBoxYarimGramlikAlDeg.BackColor = Color.FromArgb(30, 30, 30);

                textBoxCeyrekSatDeg.ForeColor = Color.White;
                textBoxCeyrekAlDeg.ForeColor = Color.White;
                textBoxAtaAlDeg.ForeColor = Color.White;
                textBoxAtaSatDeg.ForeColor = Color.White;
                textBoxResatAlDeg.ForeColor = Color.White;
                textBoxResatSatDeg.ForeColor = Color.White;
                textBoxResatikibucAlDeg.ForeColor = Color.White;
                textBoxResatikibucSatDeg.ForeColor = Color.White;
                textBoxResatBesSatDeg.ForeColor = Color.White;
                textBoxResatBesAlDeg.ForeColor = Color.White;
                textBoxBilezikAlDeg.ForeColor = Color.White;
                textBoxBilezikSatDeg.ForeColor = Color.White;
                textBoxZiynetSatDeg.ForeColor = Color.White;
                textBoxZiynetAlDeg.ForeColor = Color.White;
                textBoxZiynetikibucSatDeg.ForeColor = Color.White;
                textBoxZiynetikibucAlDeg.ForeColor = Color.White;
                textBoxIkiBucuklukSatDeg.ForeColor = Color.White;
                textBoxIkiBucuklukAlDeg.ForeColor = Color.White;
                textBox8MuhtelifSatDeg.ForeColor = Color.White;
                textBox8MuhtelifAlDeg.ForeColor = Color.White;
                textBox14MuhtelifSatDeg.ForeColor = Color.White;
                textBox14MuhtelifAlDeg.ForeColor = Color.White;
                textBox1Gramlik24kSatDeg.ForeColor = Color.White;
                textBox1Gramlik24kAlDeg.ForeColor = Color.White;
                textBox1GramlikSatDeg.ForeColor = Color.White;
                textBox1GramlikAlDeg.ForeColor = Color.White;
                textBoxYarimGramlikSatDeg.ForeColor = Color.White;
                textBoxYarimGramlikAlDeg.ForeColor = Color.White;





            }
            else
            {

                UseImmersiveDarkMode(this.Handle, false);
                this.BackColor = SystemColors.Control;

                // EUR Label
                label8.ForeColor = Color.Black;
                label17.ForeColor = Color.Black;
                label18.ForeColor = Color.Black;

                // USD Label
                label9.ForeColor = Color.Black;
                label20.ForeColor = Color.Black;
                label21.ForeColor = Color.Black;

                // Has Altın Label
                label23.ForeColor = Color.Black;
                label24.ForeColor = Color.Black;
                label26.ForeColor = Color.Black;

                // Table Label
                label3.ForeColor = Color.Black;
                label14.ForeColor = Color.Black;
                label4.ForeColor = Color.Black;
                label15.ForeColor = Color.Black;

                labeleuroalis.ForeColor = Color.Black;
                labeleurosatis.ForeColor = Color.Black;
                labeldolarsatis.ForeColor = Color.Black;
                labeldolaralis.ForeColor = Color.Black;

                labelHasAltinAlis.ForeColor = Color.Black;
                labelHasAltinSatis.ForeColor = Color.Black;

                //Panel
                panel1.BackColor = Color.LightSlateGray;
                panel2.BackColor = Color.LightGray;
                panel3.BackColor = Color.LightSlateGray;
                panel4.BackColor = Color.LightGray;
                panel5.BackColor = Color.LightSlateGray;
                panel6.BackColor = Color.LightGray;
                panel7.BackColor = Color.LightSlateGray;
                panel8.BackColor = Color.LightGray;
                panel9.BackColor = Color.LightSlateGray;
                panel10.BackColor = Color.LightGray;
                panel11.BackColor = Color.LightSlateGray;
                panel12.BackColor = Color.LightGray;

                panel1.ForeColor = Color.White;
                panel2.ForeColor = Color.Black;
                panel3.ForeColor = Color.White;
                panel4.ForeColor = Color.Black;
                panel5.ForeColor = Color.White;
                panel6.ForeColor = Color.Black;
                panel7.ForeColor = Color.White;
                panel8.ForeColor = Color.Black;
                panel9.ForeColor = Color.White;
                panel10.ForeColor = Color.Black;
                panel11.ForeColor = Color.White;
                panel12.ForeColor = Color.Black;

                //Textbox

                textBoxCeyrekSatDeg.BackColor = Color.White;
                textBoxCeyrekAlDeg.BackColor = Color.White;
                textBoxAtaAlDeg.BackColor = Color.White;
                textBoxAtaSatDeg.BackColor = Color.White;
                textBoxResatAlDeg.BackColor = Color.White;
                textBoxResatSatDeg.BackColor = Color.White;
                textBoxResatikibucAlDeg.BackColor = Color.White;
                textBoxResatikibucSatDeg.BackColor = Color.White;
                textBoxResatBesSatDeg.BackColor = Color.White;
                textBoxResatBesAlDeg.BackColor = Color.White;
                textBoxBilezikAlDeg.BackColor = Color.White;
                textBoxBilezikSatDeg.BackColor = Color.White;
                textBoxZiynetSatDeg.BackColor = Color.White;
                textBoxZiynetAlDeg.BackColor = Color.White;
                textBoxZiynetikibucSatDeg.BackColor = Color.White;
                textBoxZiynetikibucAlDeg.BackColor = Color.White;
                textBoxIkiBucuklukSatDeg.BackColor = Color.White;
                textBoxIkiBucuklukAlDeg.BackColor = Color.White;
                textBox8MuhtelifSatDeg.BackColor = Color.White;
                textBox8MuhtelifAlDeg.BackColor = Color.White;
                textBox14MuhtelifSatDeg.BackColor = Color.White;
                textBox14MuhtelifAlDeg.BackColor = Color.White;
                textBox1Gramlik24kSatDeg.BackColor = Color.White;
                textBox1Gramlik24kAlDeg.BackColor = Color.White;
                textBox1GramlikSatDeg.BackColor = Color.White;
                textBox1GramlikAlDeg.BackColor = Color.White;
                textBoxYarimGramlikSatDeg.BackColor = Color.White;
                textBoxYarimGramlikAlDeg.BackColor = Color.White;

                textBoxCeyrekSatDeg.ForeColor = Color.Black;
                textBoxCeyrekAlDeg.ForeColor = Color.Black;
                textBoxAtaAlDeg.ForeColor = Color.Black;
                textBoxAtaSatDeg.ForeColor = Color.Black;
                textBoxResatAlDeg.ForeColor = Color.Black;
                textBoxResatSatDeg.ForeColor = Color.Black;
                textBoxResatikibucAlDeg.ForeColor = Color.Black;
                textBoxResatikibucSatDeg.ForeColor = Color.Black;
                textBoxResatBesSatDeg.ForeColor = Color.Black;
                textBoxResatBesAlDeg.ForeColor = Color.Black;
                textBoxBilezikAlDeg.ForeColor = Color.Black;
                textBoxBilezikSatDeg.ForeColor = Color.Black;
                textBoxZiynetSatDeg.ForeColor = Color.Black;
                textBoxZiynetAlDeg.ForeColor = Color.Black;
                textBoxZiynetikibucSatDeg.ForeColor = Color.Black;
                textBoxZiynetikibucAlDeg.ForeColor = Color.Black;
                textBoxIkiBucuklukSatDeg.ForeColor = Color.Black;
                textBoxIkiBucuklukAlDeg.ForeColor = Color.Black;
                textBox8MuhtelifSatDeg.ForeColor = Color.Black;
                textBox8MuhtelifAlDeg.ForeColor = Color.Black;
                textBox14MuhtelifSatDeg.ForeColor = Color.Black;
                textBox14MuhtelifAlDeg.ForeColor = Color.Black;
                textBox1Gramlik24kSatDeg.ForeColor = Color.Black;
                textBox1Gramlik24kAlDeg.ForeColor = Color.Black;
                textBox1GramlikSatDeg.ForeColor = Color.Black;
                textBox1GramlikAlDeg.ForeColor = Color.Black;
                textBoxYarimGramlikSatDeg.ForeColor = Color.Black;
                textBoxYarimGramlikAlDeg.ForeColor = Color.Black;


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
                    checkBox1.Text = ini.Oku("Ayar", "Dark-Light");


                    if (hasaltinsatisfirst != "" || hasaltinalisfirst != "")
                    {
                        hasaltinsatis = Convert.ToDecimal(hasaltinsatisfirst.Replace(".", ","));
                        hasaltinalis = Convert.ToDecimal(hasaltinalisfirst.Replace(".", ","));
                    }


                    hasaltinsatisfirst = ini.Oku("HAREMALTIN_HASALTIN", "HASALTIN Satis Degeri");
                    hasaltinsatis = Convert.ToDecimal(hasaltinsatisfirst.Replace(".", ","));
                    labelHasAltinSatis.Text = hasaltinsatis.ToString("C3");


                    hasaltinalisfirst = ini.Oku("HAREMALTIN_HASALTIN", "HASALTIN Alis Degeri");
                    hasaltinalis = Convert.ToDecimal(hasaltinalisfirst.Replace(".", ","));
                    labelHasAltinAlis.Text = hasaltinalis.ToString("C3");

                    dolaralisfirst = ini.Oku("HAREMALTIN_USDTRY", "USDTRY Alis Degeri");
                    dolaralis = Convert.ToDecimal(dolaralisfirst.Replace(".", ","));
                    labeldolaralis.Text = dolaralis.ToString("C3");


                    dolarsatisfirst = ini.Oku("HAREMALTIN_USDTRY", "USDTRY Satis Degeri");
                    dolarsatis = Convert.ToDecimal(dolarsatisfirst.Replace(".", ","));
                    labeldolarsatis.Text = dolarsatis.ToString("C3");



                    euroalisfirst = ini.Oku("HAREMALTIN_USDTRY", "USDTRY Alis Degeri");
                    euroalis = Convert.ToDecimal(euroalisfirst.Replace(".", ","));
                    labeleuroalis.Text = euroalis.ToString("C3");


                    eurosatisfirst = ini.Oku("HAREMALTIN_USDTRY", "USDTRY Satis Degeri");
                    eurosatis = Convert.ToDecimal(eurosatisfirst.Replace(".", ","));
                    labeleurosatis.Text = eurosatis.ToString("C3");




                    if (checkBox1.Text == "Dark")
                    {
                        checkBox1.Checked = true;
                    }
                    else if (checkBox1.Text == "Light")
                    {
                        checkBox1.Checked = false;
                    }
                }
                else
                {
                    INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
                    ini.Yaz("Ceyrek", "Ceyrek Satis Degeri", "0");
                    ini.Yaz("Ceyrek", "Ceyrek Alis Degeri", "0");
                    ini.Yaz("Ata", "Ata Satis Degeri", "0");
                    ini.Yaz("Ata", "Ata Alis Degeri", "0");
                    ini.Yaz("Resat", "Resat Satis Degeri", "0");
                    ini.Yaz("Resat", "Resat Alis Degeri", "0");
                    ini.Yaz("Resat 2,5", "Resat 2,5 Satis Degeri", "0");
                    ini.Yaz("Resat 2,5", "Resat 2,5 Alis Degeri", "0");
                    ini.Yaz("Bilezik", "Bilezik Satis Degeri", "0");
                    ini.Yaz("Bilezik", "Bilezik Alis Degeri", "0");
                    ini.Yaz("Ziynet", "Ziynet Satis Degeri", "0");
                    ini.Yaz("Ziynet", "Ziynet Alis Degeri", "0");
                    ini.Yaz("Ziynet 2,5", "Ziynet 2,5 Satis Degeri", "0");
                    ini.Yaz("Ziynet 2,5", "Ziynet 2,5 Alis Degeri", "0");
                    ini.Yaz("8Muhtelif", "8Muhtelif Satis Degeri", "0");
                    ini.Yaz("8Muhtelif", "8Muhtelif Alis Degeri", "0");
                    ini.Yaz("14Muhtelif", "14Muhtelif Satis Degeri", "0");
                    ini.Yaz("14Muhtelif", "14Muhtelif Alis Degeri", "0");
                    ini.Yaz("Beslik", "Beslik Satis Degeri", "0");
                    ini.Yaz("Beslik", "Beslik Alis Degeri", "0");
                    ini.Yaz("IkiBucukluk", "IkiBucukluk Satis Degeri", "0");
                    ini.Yaz("IkiBucukluk", "IkiBucukluk Alis Degeri", "0");
                    ini.Yaz("1Gramlik", "1Gramlik Satis Degeri", "0");
                    ini.Yaz("1Gramlik", "1Gramlik Alis Degeri", "0");
                    ini.Yaz("1Gramlik 24k", "1Gramlik 24k Satis Degeri", "0");
                    ini.Yaz("1Gramlik 24k", "1Gramlik 24k Alis Degeri", "0");
                    ini.Yaz("YarimGramlik", "YarimGramlik Satis Degeri", "0");
                    ini.Yaz("YarimGramlik", "YarimGramlik Alis Degeri", "0");
                    ini.Yaz("Ayar", "Dark-Light", "0");

                    ini.Yaz("HAREMALTIN_HASALTIN", "HASALTIN Satis Degeri", "0");
                    ini.Yaz("HAREMALTIN_HASALTIN", "HASALTIN Alis Degeri", "0");

                    ini.Yaz("HAREMALTIN_USDTRY", "USDTRY Alis Degeri", "0");
                    ini.Yaz("HAREMALTIN_USDTRY", "USDTRY Satis Degeri", "0");

                    ini.Yaz("HAREMALTIN_EURTRY", "EURTRY Alis Degeri", "0");
                    ini.Yaz("HAREMALTIN_EURTRY", "EURTRY Satis Degeri", "0");
                }
            }
            catch (Exception hata)
            {
                // MessageBox.Show("ini dosyası hasarlı" + hata.Message);
            }
        }
        public void change()
        {

            if (textBoxCeyrekSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxCeyrekSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxCeyrekSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelCeyrekSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxCeyrekAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxCeyrekAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelCeyrekAl.Text = moneyFormat2;
            }
            if (textBoxAtaSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxAtaSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxAtaSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelAtaSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxAtaAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxAtaAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelAtaAl.Text = moneyFormat2;
            }
            if (textBoxResatSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxResatSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxResatSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelResatSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxResatAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxResatAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelResatAl.Text = moneyFormat2;
            }
            if (textBoxResatikibucSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxResatikibucSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxResatikibucSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelResatikibucSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxResatikibucAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxResatikibucAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelResatikibucAl.Text = moneyFormat2;
            }
            if (textBoxBilezikSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxBilezikSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxBilezikSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelBilezikSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxBilezikAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxBilezikAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelBilezikAl.Text = moneyFormat2;
            }
            if (textBoxZiynetSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxZiynetSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxZiynetSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelZiynetSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxZiynetAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxZiynetAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelZiynetAl.Text = moneyFormat2;
            }
            if (textBoxZiynetikibucSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxZiynetikibucSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxZiynetikibucSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelZiynetikibucSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxZiynetikibucAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxZiynetikibucAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelZiynetikibucAl.Text = moneyFormat2;
            }
            if (textBox8MuhtelifSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBox8MuhtelifSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBox8MuhtelifSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                label8MuhtelifSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBox8MuhtelifAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBox8MuhtelifAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                label8MuhtelifAl.Text = moneyFormat2;
            }
            if (textBox14MuhtelifSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBox14MuhtelifSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBox14MuhtelifSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                label14MuhtelifSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBox14MuhtelifAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBox14MuhtelifAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                label14MuhtelifAl.Text = moneyFormat2;
            }
            if (textBoxResatBesSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxResatBesSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxResatBesSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelResatBesSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxResatBesAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxResatBesAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelResatBesAl.Text = moneyFormat2;
            }
            if (textBoxIkiBucuklukSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxIkiBucuklukSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxIkiBucuklukSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelIkiBucuklukSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxIkiBucuklukAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxIkiBucuklukAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelIkiBucuklukAl.Text = moneyFormat2;
            }
            if (textBox1GramlikSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBox1GramlikSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBox1GramlikSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                label1GramlikSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBox1GramlikAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBox1GramlikAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                label1GramlikAl.Text = moneyFormat2;
            }
            if (textBox1Gramlik24kSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBox1Gramlik24kSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBox1Gramlik24kSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                label1Gramlik24kSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBox1Gramlik24kAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBox1Gramlik24kAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                label1Gramlik24kAl.Text = moneyFormat2;
            }
            if (textBoxYarimGramlikSatDeg.Text != "")
            {
                decimal sayi1 = Convert.ToDecimal(hasaltinsatis);
                decimal sayi2;
                if (textBoxYarimGramlikSatDeg.Text == "")
                {
                    sayi2 = 0;
                }
                else
                {
                    sayi2 = Convert.ToDecimal(textBoxYarimGramlikSatDeg.Text);
                }

                decimal sonuc = sayi1 * sayi2;
                string moneyFormat = sonuc.ToString("C3");
                labelYarimGramlikSat.Text = moneyFormat;

                decimal sayi3 = Convert.ToDecimal(hasaltinalis);
                decimal sayi4;
                if (textBoxYarimGramlikAlDeg.Text == "")
                {
                    sayi4 = 0;
                }
                else
                {
                    sayi4 = Convert.ToDecimal(textBoxYarimGramlikAlDeg.Text);
                }

                decimal sonuc2 = sayi3 * sayi4;
                string moneyFormat2 = sonuc2.ToString("C3");
                labelYarimGramlikAl.Text = moneyFormat2;
            }

            if (labelHasAltinSatis.Text != "")
            {
                decimal money = Convert.ToDecimal(hasaltinsatis);
                string moneyFormat = money.ToString("C3");
                labelHasAltinSatis.Text = moneyFormat;
            }
        }


        public static string cfgFile = "config.json";
        class Settings
        {
            public string apiKey { get; set; }
            public string secretKey { get; set; }
            public bool tradeStatus { get; set; }

        }
        public static string readSettings()
        {
            if (!File.Exists(cfgFile))
            {
                MessageBox.Show("Cannot Found Config.json");

                Settings newSetting = new Settings
                {
                    apiKey = "",
                    secretKey = "",
                    tradeStatus = false,
                };

                string jsonString = JsonConvert.SerializeObject(newSetting);
                File.WriteAllText(cfgFile, jsonString);

                MessageBox.Show("Created config file but you must edit.");
                MessageBox.Show("You must edit config file");
                throw new Exception("You must edit config file");
                return "";
            }

            var cfgJson = System.IO.File.ReadAllText(cfgFile);
            Settings readcfg = JsonConvert.DeserializeObject<Settings>(cfgJson);
            if (string.IsNullOrEmpty(readcfg.apiKey))
            {
                MessageBox.Show("Api key cannot be empty. Edit config file.");
                MessageBox.Show("You must edit config file");
                throw new Exception("You must edit config file");
            }

            return cfgJson;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //Settings readcfg = JsonConvert.DeserializeObject<Settings>(readSettings());


            //label27.Text = readcfg.apiKey;
            //Process process = new Process();
            //process.StartInfo.FileName = @"OzerJewelry_DataGetir.exe";
            //process.StartInfo.CreateNoWindow = true;
            //process.StartInfo.UseShellExecute = false;
            //process.Start();

            haremaltingetir(hasaltinalis, hasaltinsatis);


            ayarcek();
            DataGetir();
            timer1.Start();

            darkLight();

        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            DataGetirThread = new Thread(new ThreadStart(DataGetir));
            DataGetirThread.IsBackground = true;
            DataGetirThread.Start();
            Control.CheckForIllegalCrossThreadCalls = false;
            //DataGetir();
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

        private void textBoxAtaSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxAtaAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxResatSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxResatAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxBilezikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxBilezikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxZiynetSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxZiynetAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox8MuhtelifSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox8MuhtelifAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox14MuhtelifSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox14MuhtelifAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxBeslikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxBeslikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxIkiBucuklukSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxIkiBucuklukAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox1GramlikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox1GramlikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxYarimGramlikSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxYarimGramlikAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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
        private void textBoxResatikibucSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxResatikibucAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxZiynetikibucSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBoxZiynetikibucAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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
        private void textBox1Gramlik24kSatDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox1Gramlik24kAlDeg_KeyPress(object sender, KeyPressEventArgs e)
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

        #endregion
        #region textchanged
        private void textBoxCeyrekSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
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
            string moneyFormat = sonuc.ToString("C3");
            labelCeyrekSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ceyrek", "Ceyrek Satis Degeri", textBoxCeyrekSatDeg.Text);
        }

        private void textBoxCeyrekAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxCeyrekAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxCeyrekAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelCeyrekAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ceyrek", "Ceyrek Alis Degeri", textBoxCeyrekAlDeg.Text);
        }

        private void textBoxAtaSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxAtaSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxAtaSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelAtaSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ata", "Ata Satis Degeri", textBoxAtaSatDeg.Text);
        }

        private void textBoxAtaAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxAtaAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxAtaAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelAtaAl.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ata", "Ata Alis Degeri", textBoxAtaAlDeg.Text);
        }

        private void textBoxResatSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxResatSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelResatSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat", "Resat Satis Degeri", textBoxResatSatDeg.Text);
        }

        private void textBoxResatAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxResatAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelResatAl.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat", "Resat Alis Degeri", textBoxResatAlDeg.Text);
        }
        private void textBoxResatikibucSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxResatikibucSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatikibucSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelResatikibucSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat 2,5", "Resat 2,5 Satis Degeri", textBoxResatikibucSatDeg.Text);
        }

        private void textBoxResatikibucAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxResatikibucAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatikibucAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelResatikibucAl.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Resat 2,5", "Resat 2,5 Alis Degeri", textBoxResatikibucAlDeg.Text);
        }

        private void textBoxBilezikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxBilezikSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxBilezikSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelBilezikSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Bilezik", "Bilezik Satis Degeri", textBoxBilezikSatDeg.Text);
        }

        private void textBoxBilezikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxBilezikAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxBilezikAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelBilezikAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Bilezik", "Bilezik Alis Degeri", textBoxBilezikAlDeg.Text);
        }

        private void textBoxZiynetSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxZiynetSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelZiynetSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet", "Ziynet Satis Degeri", textBoxZiynetSatDeg.Text);
        }

        private void textBoxZiynetAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxZiynetAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);
            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelZiynetAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet", "Ziynet Alis Degeri", textBoxZiynetAlDeg.Text);
        }
        private void textBoxZiynetikibucSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxZiynetikibucSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetikibucSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelZiynetikibucSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet 2,5", "Ziynet 2,5 Satis Degeri", textBoxZiynetikibucSatDeg.Text);
        }

        private void textBoxZiynetikibucAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxZiynetikibucAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxZiynetikibucAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelZiynetikibucAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Ziynet 2,5", "Ziynet 2,5 Alis Degeri", textBoxZiynetikibucAlDeg.Text);
        }

        private void textBox8MuhtelifSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBox8MuhtelifSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox8MuhtelifSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label8MuhtelifSat.Text = moneyFormat;


            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("8Muhtelif", "8Muhtelif Satis Degeri", textBox8MuhtelifSatDeg.Text);
        }

        private void textBox8MuhtelifAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBox8MuhtelifAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox8MuhtelifAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label8MuhtelifAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("8Muhtelif", "8Muhtelif Alis Degeri", textBox8MuhtelifAlDeg.Text);
        }

        private void textBox14MuhtelifSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBox14MuhtelifSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox14MuhtelifSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label14MuhtelifSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("14Muhtelif", "14Muhtelif Satis Degeri", textBox14MuhtelifSatDeg.Text);
        }

        private void textBox14MuhtelifAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBox14MuhtelifAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox14MuhtelifAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label14MuhtelifAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("14Muhtelif", "14Muhtelif Alis Degeri", textBox14MuhtelifAlDeg.Text);
        }

        private void textBoxBeslikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxResatBesSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatBesSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelResatBesSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Beslik", "Beslik Satis Degeri", textBoxResatBesSatDeg.Text);
        }

        private void textBoxBeslikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxResatBesAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxResatBesAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelResatBesAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("Beslik", "Beslik Alis Degeri", textBoxResatBesAlDeg.Text);
        }

        private void textBoxIkiBucuklukSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxIkiBucuklukSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxIkiBucuklukSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelIkiBucuklukSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("IkiBucukluk", "IkiBucukluk Satis Degeri", textBoxIkiBucuklukSatDeg.Text);
        }

        private void textBoxIkiBucuklukAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxIkiBucuklukAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxIkiBucuklukAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelIkiBucuklukAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("IkiBucukluk", "IkiBucukluk Alis Degeri", textBoxIkiBucuklukAlDeg.Text);
        }

        private void textBox1GramlikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBox1GramlikSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1GramlikSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label1GramlikSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik", "1Gramlik Satis Degeri", textBox1GramlikSatDeg.Text);
        }

        private void textBox1GramlikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBox1GramlikAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1GramlikAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label1GramlikAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik", "1Gramlik Alis Degeri", textBox1GramlikAlDeg.Text);
        }

        private void textBoxYarimGramlikSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBoxYarimGramlikSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxYarimGramlikSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelYarimGramlikSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("YarimGramlik", "YarimGramlik Satis Degeri", textBoxYarimGramlikSatDeg.Text);
        }

        private void textBoxYarimGramlikAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBoxYarimGramlikAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBoxYarimGramlikAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            labelYarimGramlikAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("YarimGramlik", "YarimGramlik Alis Degeri", textBoxYarimGramlikAlDeg.Text);
        }
        private void textBox1Gramlik24kAlDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinalis);
            double sayi2;
            if (textBox1Gramlik24kAlDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1Gramlik24kAlDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label1Gramlik24kAl.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik 24k", "1Gramlik 24k Alis Degeri", textBox1Gramlik24kAlDeg.Text);
        }
        private void textBox1Gramlik24kSatDeg_TextChanged(object sender, EventArgs e)
        {
            double sayi1 = Convert.ToDouble(hasaltinsatis);
            double sayi2;
            if (textBox1Gramlik24kSatDeg.Text == "")
            {
                sayi2 = 0;
            }
            else
            {
                sayi2 = Convert.ToDouble(textBox1Gramlik24kSatDeg.Text);
            }

            (sender as System.Windows.Forms.TextBox).Text = (sender as System.Windows.Forms.TextBox).Text.Replace('.', ',');
            (sender as System.Windows.Forms.TextBox).Select((sender as System.Windows.Forms.TextBox).Text.Length, 0);

            double sonuc = sayi1 * sayi2;
            string moneyFormat = sonuc.ToString("C3");
            label1Gramlik24kSat.Text = moneyFormat;

            INIKaydet ini = new INIKaydet(Application.StartupPath + @"\Ayarlar.ini");
            ini.Yaz("1Gramlik 24k", "1Gramlik 24k Satis Degeri", textBox1Gramlik24kSatDeg.Text);
        }


        #endregion


        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

            foreach (var process in Process.GetProcessesByName("OzerJewelry_DataGetir"))
            {
                process.Kill();
            }

        }


    }
}
