using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
//using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
//using ClosedXML.Excel;
using System.Globalization;

using System.Configuration;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace InfoKiosk
{
    public partial class LoginForm : Form
    {
        VideoCapture capture;
        Mat frame;
        Bitmap image;
        private Thread camera;
        bool isCameraRunning = false;


        public LoginForm()
        {
            InitializeComponent();
            this.ActiveControl = textBox1;
        }
        public string connectionString = @"Data Source=192.168.0.3;Initial Catalog=RFIND;User ID=sa;Password=AdminFX9.";
        public static string prezimer,lbltxt,idradnika,idradnika0,idfirme,idprijave;

        private void Button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Želite izaći iz programa?");
            System.Windows.Forms.Application.Exit();

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

            //MessageBox.Show("Želite izaći iz programa?");
            //System.Windows.Forms.Application.Exit();

        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Želite izaći iz programa?");
            SqlConnection cn1 = new SqlConnection(connectionString);
            cn1.Open();
            string sql1 = "insert into infokiosk_log ( datum,korisnik,korisnik_id,opis,idprijave) values ( getdate(),'" + lbltxt.Substring(0, 49) + "'," + idradnika0 + ",'Odjava','')";
            SqlCommand  sqlCommand1 = new SqlCommand( sql1 , cn1);
            SqlDataReader reader22 = sqlCommand1.ExecuteReader();
            cn1.Close();
            System.Windows.Forms.Application.Exit();

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            //private Timer timer1;
            //public void InitTimer()
            //{
            //    timer1 = new Timer();
            //    timer1.Tick += new EventHandler(timer1_Tick);
            //    timer1.Interval = 5000; // in miliseconds
            //    timer1.Start();
            //}
        
                string connectionString = @"Data Source=192.168.0.3;Initial Catalog=RFIND;User ID=sa;Password=AdminFX9.";
                SqlConnection cn0 = new SqlConnection(connectionString);
                cn0.Open();

                SqlCommand sqlCommand1 = new SqlCommand("insert into infokiosk_log (datum,korisnik,opis) values ( getdate(),'Sistem','Check connection')", cn0);
                SqlDataReader reader21 = sqlCommand1.ExecuteReader();
                cn0.Close();

            
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {

        }
        private void CaptureCamera()
        {
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.Start();
        }
        private void CaptureCameraCallback()
        {

            frame = new Mat();
            capture = new VideoCapture(0);
            capture.Open(0);

            if (capture.IsOpened())
            {
                while (isCameraRunning)
                {

                    capture.Read(frame);
                    image = BitmapConverter.ToBitmap(frame);
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }
                    pictureBox1.Image = image;
                }
            }

//            Bitmap snapshot = new Bitmap(pictureBox1.Image);
//            snapshot.Save(string.Format(@"C:\brisi\{0}.png", Guid.NewGuid()), ImageFormat.Png);
        }

        private void PictureBox3_Click_1(object sender, EventArgs e)
        {

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings["Showopt"].Value == "0")
            {
                settings["Showopt"].Value = "1";
                label1.BackColor = Color.Beige;

            }
            else
            {
                settings["Showopt"].Value = "0";
                label1.BackColor = Color.Bisque;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        }
        private void Button1_Click_1(object sender, EventArgs e)
        {

           
            button1.Text = "Stop";
            isCameraRunning = true;
            CaptureCamera();

            Bitmap snapshot = new Bitmap(pictureBox1.Image);

            // Save in some directory
            // in this example, we'll generate a random filename e.g 47059681-95ed-4e95-9b50-320092a3d652.png
            // snapshot.Save(@"C:\Users\sdkca\Desktop\mysnapshot.png", ImageFormat.Png);
            snapshot.Save(string.Format(@"C:\brisi\{0}.png", Guid.NewGuid()), ImageFormat.Png);

            
            

            // Declare required methods

        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
           
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.X))
            {
                
                MessageBox.Show("Želite izaći iz programa?");
                System.Windows.Forms.Application.Exit(); 
                //return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

                string rfind1 = textBox1.Text;
                
                if (rfind1.Length >= 10)
                {                
                    long rfidd = 0;
            //    MessageBox.Show("duzina stringa " + rfind1.Length.ToString(), " ");

                try
                    {
                        rfidd = (long.Parse)(textBox1.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Pokušajte ponovo, neuspjelo očitanje kartice !", "");
                        this.ActiveControl = textBox1 ;
                    return;
                        //  textBox1.SelectionStart = 1;
                        
                //        textBox1.Visible = true;
                        //this.ActiveControl= LoginForm.ActiveForm                            ;
                    }

                  //  textBox1.Visible = false;
                    string hexValue = "0000"+rfidd.ToString("X");
                    hexValue = hexValue.Substring(hexValue.Length - 8);
                    string prvi = hexValue.Substring(0, 2);
                    string drugi = hexValue.Substring(2, hexValue.Length - 2);
                    if (hexValue.Length == 9)
                    {
                        prvi = hexValue.Substring(0, 1);
                        drugi = hexValue.Substring(1, hexValue.Length - 1);
                    }

                    //string prvi = hexValue.Substring(0, 2);

                    int prvidec = int.Parse(prvi, System.Globalization.NumberStyles.HexNumber);
                    int drugidec = int.Parse(drugi, System.Globalization.NumberStyles.HexNumber);
                    string csn1 = prvidec.ToString() + "-" + drugidec.ToString();
                    string custid1 = prvidec.ToString();

                    SqlConnection cn1 = new SqlConnection(connectionString);
                    cn1.Open();
                    int citac = 2;   // čitač iz pogona 2, čitač iz  financija 1
                    SqlCommand sqlCommand1;

                    if (citac == 1)
                    {
                        sqlCommand1 = new SqlCommand("select * from radnici_  where rfid2='" + csn1 + "'", cn1);
                    }
                    else   // čitač iz pogona čita samo drugi dio rfid
                    {
                    //sqlCommand1 = new SqlCommand("select * from radnici_   where RIGHT(rfid2, LEN(rfid2) - CHARINDEX('-', rfid2))='" + rfidd.ToString() + "'", cn1);
                    sqlCommand1 = new SqlCommand("select r.* from badge b left join radnici_ r on r.id=b.extid  where b.active=1 and b.badgeno='" + hexValue+ "'", cn1);
                }


                    SqlDataReader reader21 = sqlCommand1.ExecuteReader();
                    reader21.Read();
                    string poduzece, ime0 = "", prezime0 = "", rv0, radnomjesto0 = "", id0 = "", poduzece0 = "", idradnika1 = "";
                    if (reader21.HasRows)
                    {
                        poduzece = reader21["poduzece"].ToString();
                        ime0 = reader21["ime"].ToString();
                        prezime0 = reader21["prezime"].ToString();
                        id0 = reader21["id"].ToString();
                        radnomjesto0 = reader21["radnomjesto"].ToString();
                        poduzece0 = reader21["poduzece"].ToString();
                        idradnika1 = reader21["id_radnika"].ToString().TrimEnd();

                        //rfid0 = reader21["rfid"].ToString();
                        //rfidhex0 = reader21["rfidhex"].ToString();
                        //rfid20 = reader21["rfid2"].ToString();
                        //custid0 = reader21["custid"].ToString();
                        rv0 = reader21["rv"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Pokušajte ponovo, vaša kartica nije važeća !", "");
                        this.ActiveControl = textBox1;
                        return;
                    }
                    lbltxt    = prezime0 + ' ' + ime0 + " (" + id0 + ")  - radno mjesto: " + radnomjesto0 + " poduzeće :" + poduzece0;

                    idfirme = "1";
                    if (poduzece0.Contains("okab"))
                    {
                            idfirme = "3";
                    }
                    prezimer  = prezime0 + ' ' + ime0;
                    idradnika = idradnika1;
                    idradnika0 = id0;   // id iz radnici_
                    idprijave = idradnika0 + '-' + DateTime.Now;
                cn1.Close();

                cn1.Open(); 
                sqlCommand1 = new SqlCommand("insert into infokiosk_log ( datum,korisnik,korisnik_id,opis,idprijave) values ( getdate(),'"+lbltxt.Substring(0,49)+"',"+idradnika0+",'Prijava','"+idprijave+"')" , cn1);
                SqlDataReader reader22 = sqlCommand1.ExecuteReader();
                cn1.Close() ;

                DialogResult = DialogResult.OK;

                

                //this.Close();        

            }
        }
    }
}