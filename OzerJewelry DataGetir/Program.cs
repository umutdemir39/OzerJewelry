using System;
using System.Threading;
using System.Text.Json;
using SocketIOClient;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SocketIOClient.JsonSerializer;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;

namespace OzerJewelry
{
    public class DataGetir
    {

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

        public static async Task Main(string[] args)
        {

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
                    USDTRYalis = a.data.USDTRY.alis;
                    USDTRYsatis = a.data.USDTRY.satis;


                    INIKaydet ini = new INIKaydet(AppDomain.CurrentDomain.BaseDirectory+@"Ayarlar.ini");
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
                    ALTINalis = a.data.ALTIN.alis;
                    ALTINsatis = a.data.ALTIN.satis;

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

                Console.ReadKey();

                // Emit a string and an object
            };
            await client.ConnectAsync();
            Console.ReadKey();
        }


    }
}
