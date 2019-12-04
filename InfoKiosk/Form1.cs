using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
//using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
//using ClosedXML.Excel;
using System.Globalization;
using OpenCvSharp;
using OpenCvSharp.Extensions;


namespace InfoKiosk
{
    public partial class Form1 : Form
    {
        public string connectionString = @"Data Source=192.168.0.3;Initial Catalog=fx_RFIND;User ID=sa;Password=AdminFX9.";
        public static string pprezimer, idradnika1, idfirme, idradnika0,idprijave;
        public int idporuke;

        public Form1()
        {
            InitializeComponent();
            idprijave = LoginForm.idprijave.Trim();        // user
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            var MyReader = new System.Configuration.AppSettingsReader();
            string keyvalue = MyReader.GetValue("showopt", typeof(string)).ToString();

            if (keyvalue == "0")
            {
                dolasciToolStripMenuItem.Visible = false;
                godišnjiToolStripMenuItem.Visible = false;
                rasporedToolStripMenuItem.Visible = false;
                prijavaToolStripMenuItem.Visible = false;
                prijavaZaVikendToolStripMenuItem.Visible = false;
            }


            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //this.ActiveControl = textBox1;
            label1.Text = LoginForm.lbltxt.Trim();    // text u naslovu    
            pprezimer = LoginForm.prezimer.Trim();    // prezime
            idradnika1 = LoginForm.idradnika.Trim();    // grupa            
            idradnika0 = LoginForm.idradnika0.Trim();    // id radnika iz radnici_
            idfirme = LoginForm.idfirme.Trim();    // id firme

            SqlConnection cn1 = new SqlConnection(connectionString);
            cn1.Open();
            SqlCommand sqlCommand1 = new SqlCommand("select count(*) brojporuka from poruke  where rtrim(userid)='" + idradnika1 + "' and status is null", cn1);
            SqlDataReader reader21 = sqlCommand1.ExecuteReader();
            reader21.Read();
            int bp = (int.Parse)(reader21["brojporuka"].ToString());
            novostiToolStripMenuItem.Text = "Novosti ("+bp.ToString()+")";

        }
        

        private void button6_Click(object sender, EventArgs e)
        {
            pprezimer = "";
            LoginForm.ActiveForm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

       public void cistipanele()
        {
            panel2.Visible =false;
            panel3.Visible = false;
            pnl_poruke.Visible = false;
            btn_pomoc.Visible = false;
        }

        private void normeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Pregled izvršenja normi";
            cistipanele();
            panel2.Visible = true;
            panel3.Visible = true;
            btn_godisnji.Visible = false;
            btn_subota.Visible = false;
            btn_nedjelja.Visible = false;
            btn_praznici.Visible = false;


            DateTime danas = DateTime.Now;
            DateTime prije = danas.AddDays(-100);
            string sdanas = danas.Year.ToString() + '-' + danas.Month.ToString() + '-' + danas.Day.ToString();
            string sprije = prije.Year.ToString() + '-' + prije.Month.ToString() + '-' + prije.Day.ToString();

            SqlConnection connection = new SqlConnection(connectionString);
           // pprezimer = "Karabelj";
            string sql1 = "select  Radnik,Vrsta,Firma,Datum,Hala,Smjena,LInija,NazivPar,BrojRn,Proizvod,Norma,cast( norma*minutaradaradnika/480 as int) Planirano, KOlicinaok,OtpadObrada,OtpadMat,kolicinaPorozno,MinutaRadaradnika,Napomena1,Napomena2,Napomena3,MT,PomocniRadnik from rfind.dbo.evidnormiradad('" + sprije + "','" + sdanas + "') where radnik like '" + pprezimer.TrimEnd() + "%'  order by datum desc";

            //sql1 = "select e.Datum,e.Linija,e.Hala,e.Brojrn,e.Norma,e.Kolicinaok , e.OtpadObrada,Napomena1,e.Id_pro,p.nazivpro,e.Vrijemeod,'' UkupnoMinuta,e.Vrijemedo " +
            //        "from feroapp.dbo.evidencijanormiview e left join feroapp.dbo.radnici r on r.id_radnika = e.id_radnika left join feroapp.dbo.Proizvodi p on p.id_pro = e.id_pro where r.ID_Fink= 973  and r.id_firme=1 and DATEDIFF(month,e.datum, GETDATE()) <= 13 order by e.datum desc";

            sql1 = "select e.Datum,e.Hala,e.smjena,e.linija,e.Brojrn,e.Norma,0 Planirano,e.Kolicinaok , e.OtpadObrada,Napomena1,e.Id_pro,p.nazivpro,e.Vrijemeod,e.Vrijemedo,''  UkupnoMinuta " +
                                "from feroapp.dbo.evidencijanormiview e left join feroapp.dbo.radnici r on r.id_radnika = e.id_radnika left join feroapp.dbo.Proizvodi p on p.id_pro = e.id_pro where r.ID_Fink= " + idradnika0 + "  and r.id_firme=" + idfirme + "  and DATEDIFF(month,e.datum, GETDATE()) <= 13 order by e.datum desc";


            int id1 = int.Parse(idradnika0);

            if (id1 > 8000)
                  idradnika0 = (id1 - 8000).ToString();

            sql1 ="fx_rfind.dbo.InfoKiosk_Norme "+idradnika0+", " + idfirme ;
            
            SqlDataAdapter dataadapter = new SqlDataAdapter(sql1, connection);
            DataSet ds = new DataSet();
            connection.Open();
            dataadapter.Fill(ds, "eventn");
            connection.Close();
            //            label92.BackColor = System.Drawing.Color.LawnGreen;

            dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgv_zbirni.DataSource = ds;
            dgv_zbirni.DataMember = "eventn";
            double minuta = 0.0;
            int norm1 = 0, norm = 0,kol=0;

            foreach (DataGridViewRow row in dgv_zbirni.Rows)
            {
                if (row.Cells[1].Value == null)
                { }
                else
                {
                    string s1 = row.Cells[0].Value.ToString(); ;
                    DateTime dat0 = DateTime.Parse(s1);
                    DayOfWeek dat1 = dat0.DayOfWeek;

                    //foreach (var item in praznicii)                   // norma praznici
                    //{
                    //    if (item.dan.ToString() == s1)
                    //        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
                    //}


                    if (dat1 == DayOfWeek.Saturday)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.Yellow;
                    }
                    if (dat1 == DayOfWeek.Sunday)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                    }

                    norm1 = (int.Parse)(row.Cells[5].Value.ToString());
                    
                    if (row.Cells[7].Value == DBNull.Value)
                    {
                        kol = 0;
                    }
                    else
                    {
                        kol = (int.Parse)(row.Cells[7].Value.ToString());
                    }

                    //string ts1 = row.Cells[12].Value.ToString();
                    //string ts2 = row.Cells[13].Value.ToString();
                    //minuta = 0;
                    //if (ts1.Length > 0 && ts2.Length > 0)
                    //{
                    //    TimeSpan t1 = (TimeSpan.Parse(ts1));
                    //    TimeSpan t2 = (TimeSpan.Parse(ts2));
                    //    if (t2 < t1)
                    //    {
                    //        minuta = t2.TotalMinutes + 1440 - t1.TotalMinutes;
                    //    }
                    //    else
                    //    {
                    //        minuta = t2.TotalMinutes - t1.TotalMinutes;
                    //    }
                    //}

                    string min1 = row.Cells[11].Value.ToString().Replace(',','.');
                    double minuta1 = (double.Parse)(min1)/100;
                    //row.Cells[14].Value = minuta.ToString();
                    norm = (int)(norm1 * minuta1 / (480.0));
                    row.Cells[6].Value = norm.ToString();


                    if ((kol) >= (norm))
                    {
                        //                            row.Cells[6].Style.BackColor = System.Drawing.Color.LawnGreen;
                        row.Cells[6].Style.BackColor = System.Drawing.Color.LawnGreen;
                        row.Cells[5].Style.BackColor = System.Drawing.Color.LawnGreen;
                        row.Cells[7].Style.BackColor = System.Drawing.Color.LawnGreen;
                        row.Cells[8].Style.BackColor = System.Drawing.Color.LawnGreen;
                    }
                    if ((kol) <= (norm*0.9))
                    {
                        //                            row.Cells[6].Style.BackColor = System.Drawing.Color.LawnGreen;
                        
                        row.Cells[6].Style.BackColor = System.Drawing.Color.Chocolate;
                        row.Cells[5].Style.BackColor = System.Drawing.Color.Chocolate;
                        row.Cells[7].Style.BackColor = System.Drawing.Color.Chocolate;
                        row.Cells[8].Style.BackColor = System.Drawing.Color.Chocolate;
                    }
                    if ((kol) <= (norm * 0.7))
                    {
                        //                            row.Cells[6].Style.BackColor = System.Drawing.Color.LawnGreen;
                        row.Cells[6].Style.BackColor = System.Drawing.Color.Red;
                        row.Cells[5].Style.BackColor = System.Drawing.Color.Red;
                        row.Cells[7].Style.BackColor = System.Drawing.Color.Red;
                        row.Cells[8].Style.BackColor = System.Drawing.Color.Red;
                    }


                }
            }
            dgv_zbirni.AutoResizeColumns();
            //dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            //

            //SqlDataAdapter dataadapter = new SqlDataAdapter(sql1, connection);
            //DataSet ds = new DataSet();
            //connection.Open();
            //dataadapter.Fill(ds, "event");
            connection.Close();
            dgv_zbirni.Width = panel2.Width - 100;
            dgv_zbirni.Height = panel2.Height - 100;

            dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            //dgv_zbirni.DataSource = ds;
            //dgv_zbirni.DataMember = "event";
        }

        private void Odjava_Click(object sender, EventArgs e)
        {
            pprezimer = "";
            //this.Hide();
            SqlConnection cn1 = new SqlConnection(connectionString);
            cn1.Open();
            int citac = 2;   // čitač iz pogona 2, čitač iz  financija 1
            SqlCommand sqlCommand1;
            cn1.Close();
            cn1.Open();
            
            sqlCommand1 = new SqlCommand("insert into infokiosk_log ( datum,korisnik,korisnik_id,opis,idprijave) values ( getdate(),'" + label1.Text.Substring(0, 49) + "'," + idradnika0 + ",'Odjava','"+idprijave+"')", cn1);
            SqlDataReader reader22 = sqlCommand1.ExecuteReader();
            cn1.Close();
            Form LoginForm = new LoginForm();
            LoginForm.ShowDialog();
            Form f1 = new Form1();
            f1.Show();
            //panel2.Visible = true;



        }

        private void dolasciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //label2.Text = "Pregled dolazaka";
            //cistipanele();
            //panel2.Visible = true;
            

            //DateTime danas = DateTime.Now;
            //DateTime prije = danas.AddDays(-100);
            //string sdanas = danas.Year.ToString() + '-' + danas.Month.ToString() + '-' + danas.Day.ToString();
            //string sprije = prije.Year.ToString() + '-' + prije.Month.ToString() + '-' + prije.Day.ToString();

            //SqlConnection connection = new SqlConnection(connectionString);
            ////pprezimer = "Karabelj";
            
            //string sql1 = "select Convert(VarChar, p.datum, 104) Datum,r.id,r.prezime,r.ime,p.hala,p.smjena,p.radnomjesto,p.dosao,p.otisao,p.Kasni,p.Ukupno_minuta,p.Ukupno_sati from pregledvremena p left join radnici_ r on r.id = p.idradnika where datediff(day, p.datum,getdate())<100 and (prezime+' '+IME) like  '" + pprezimer.TrimEnd() + "%' order by p.datum desc";

            //SqlDataAdapter dataadapter = new SqlDataAdapter(sql1, connection);
            //DataSet ds = new DataSet();
            //connection.Open();
            //dataadapter.Fill(ds, "event");
            //connection.Close();
            //dgv_zbirni.Width = panel2.Width - 100;
            //dgv_zbirni.Height = panel2.Height - 100;

            //dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            //dgv_zbirni.DataSource = ds;
            //dgv_zbirni.DataMember = "event";
        }

        private void rasporedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Pregled rasporeda po radnim mjestima";
            cistipanele();
            panel2.Visible = true;
            btn_godisnji.Visible = false;
            btn_subota.Visible = false;
            btn_nedjelja.Visible = false;
            btn_praznici.Visible = false;

            DateTime danas = DateTime.Now;
            DateTime prije = danas.AddDays(-100);
            string sdanas = danas.Year.ToString() + '-' + danas.Month.ToString() + '-' + danas.Day.ToString();
            string sprije = prije.Year.ToString() + '-' + prije.Month.ToString() + '-' + prije.Day.ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            //pprezimer = "Karabelj";

            string sql1 = "select top 100 Convert(VarChar, p.datum, 104) Datum,r.id,r.prezime,r.ime,p.hala,p.smjena,p.radnomjesto,p.dosao,p.otisao,p.Kasni,p.preranootisao,p.Ukupno_minuta,p.Ukupno_minuta/60 Ukupno_sati from pregledvremena p left join radnici_ r on r.id = p.idradnika where (rtrim(r.prezime)+' ' +rtrim(r.ime)) like  '" + pprezimer.TrimEnd() + "%' order by p.datum desc";

            SqlDataAdapter dataadapter = new SqlDataAdapter(sql1, connection);
            DataSet ds = new DataSet();
            connection.Open();
            dataadapter.Fill(ds, "event");
            connection.Close();
            dgv_zbirni.Width = panel2.Width - 100;
            dgv_zbirni.Height = panel2.Height - 100;

            dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgv_zbirni.DataSource = ds;
            dgv_zbirni.DataMember = "event";
        }

        private void prijavaZaVikendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cistipanele();
        }

        private void godišnjiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cistipanele();
        }

        private void novostiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cistipanele();
            
            SqlConnection cn1 = new SqlConnection(connectionString);
            cn1.Open();
            
            SqlCommand sqlCommand1 = new SqlCommand("select * from poruke  where rtrim(userid)='" + idradnika1 + "' and status is null", cn1);

            SqlDataReader reader21 = sqlCommand1.ExecuteReader();
            reader21.Read();
            string sadrzaj = "", author = "", Oddatuma = "";
            
            if (reader21.HasRows)
            {
                sadrzaj = reader21["Sadržaj"].ToString();
                author = reader21["Author"].ToString();
                Oddatuma = reader21["DatumU"].ToString();
                idporuke = (int.Parse)(reader21["id"].ToString());

                pnl_poruke.Visible = true;
                Poruke_text.Text = sadrzaj;
                lbl_author.Text = author;
                lbl_Oddatuma.Text = Oddatuma;
                
            }
            else
            {
                MessageBox.Show("Nema poruka !");
                cistipanele();                
                
            }
            cn1.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string rfind1 = textBox1.Text;
            if (rfind1.Length == 14)
            {
                textBox1.Visible = false;
                long rfidd = (long.Parse)(textBox1.Text);
                string hexValue = rfidd.ToString("X");
                string prvi = hexValue.Substring(0, 2);
                string drugi = hexValue.Substring(2, hexValue.Length - 2);
                int prvidec = int.Parse(prvi, System.Globalization.NumberStyles.HexNumber);
                int drugidec = int.Parse(drugi, System.Globalization.NumberStyles.HexNumber);
                string csn1 = prvidec.ToString() + "-" + drugidec.ToString();
                string custid1 = prvidec.ToString();

                SqlConnection cn1 = new SqlConnection(connectionString);
                cn1.Open();
                SqlCommand sqlCommand1 = new SqlCommand("select * from radnici_  where rfid2='" + csn1 + "'", cn1);

                SqlDataReader reader21 = sqlCommand1.ExecuteReader();
                reader21.Read();
                string poduzece, ime0 = "", prezime0 = "", rv0, radnomjesto0 = "", id0 = "", poduzece0 = "";
                if (reader21.HasRows)
                {
                    poduzece = reader21["poduzece"].ToString();
                    ime0 = reader21["ime"].ToString();
                    prezime0 = reader21["prezime"].ToString();
                    id0 = reader21["id"].ToString();
                    radnomjesto0 = reader21["radnomjesto"].ToString();
                    poduzece0 = reader21["poduzece"].ToString();

                    //rfid0 = reader21["rfid"].ToString();
                    //rfidhex0 = reader21["rfidhex"].ToString();
                    //rfid20 = reader21["rfid2"].ToString();
                    //custid0 = reader21["custid"].ToString();
                    rv0 = reader21["rv"].ToString();
                }
                label1.Text = prezime0 + ' ' + ime0 + " (" + id0 + ")  - radno mjesto: " + radnomjesto0 + " poduzeće :" + poduzece0;
                pprezimer = prezime0 + ' ' + ime0;
                cn1.Close();
                DialogResult = DialogResult.OK;
                //  Form f1 = new Form1();
                //  f1.Show();

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dolasciToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            label2.Text = "Pregled dolazaka";
            cistipanele();
            panel2.Visible = true;
            btn_godisnji.Visible = false;
            btn_subota.Visible = false;
            btn_nedjelja.Visible = false;
            btn_praznici.Visible = false;


            DateTime danas = DateTime.Now;
            DateTime prije = danas.AddDays(-100);
            string sdanas = danas.Year.ToString() + '-' + danas.Month.ToString() + '-' + danas.Day.ToString();
            string sprije = prije.Year.ToString() + '-' + prije.Month.ToString() + '-' + prije.Day.ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            //pprezimer = "Karabelj";

            string sql1 = "select Convert(VarChar, p.datum, 104) Datum,r.id,r.prezime,r.ime,p.hala,p.smjena,p.radnomjesto,p.dosao,p.otisao,p.Kasni,p.Ukupno_minuta,p.ukupno_minuta/60 UkupnoSati from pregledvremena p left join radnici_ r on r.id = p.idradnika where datediff(day, p.datum,getdate())<100 and (prezime+' '+IME) like  '" + pprezimer.TrimEnd() + "%' order by p.datum desc";

            SqlDataAdapter dataadapter = new SqlDataAdapter(sql1, connection);
            DataSet ds = new DataSet();
            connection.Open();
            dataadapter.Fill(ds, "event");
            connection.Close();
            dgv_zbirni.Width = panel2.Width - 100;
            dgv_zbirni.Height = panel2.Height - 100;

            dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgv_zbirni.DataSource = ds;
            dgv_zbirni.DataMember = "event";
        }

        private void upisaniSatiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Pregled upisanih sati";
            cistipanele();
            panel2.Visible = true;
            btn_godisnji.Visible = true;
            btn_subota.Visible = true;
            btn_nedjelja.Visible = true;
            btn_praznici.Visible = true;
            btn_subota.BackColor = Color.LightYellow;
            btn_nedjelja.BackColor = Color.LightSeaGreen;
            btn_praznici.BackColor = Color.LightPink;
            btn_godisnji.BackColor = Color.LightBlue;


            DateTime danas = DateTime.Now;
            DateTime prije = danas.AddDays(-100);
            string sdanas = danas.Year.ToString() + '-' + danas.Month.ToString() + '-' + danas.Day.ToString();
            string sprije = prije.Year.ToString() + '-' + prije.Month.ToString() + '-' + prije.Day.ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            //pprezimer = "Karabelj";

            string sql1 = "select radnikid,p.ime,godina,mjesec,dan01,dan02,dan03,dan04,dan05,dan06,dan07,dan08,dan09,dan10,dan11,dan12,dan13,dan14,dan15,dan16,dan17,dan18,dan19,dan20,dan21,dan22,dan23,dan24,dan25,dan26,dan27,dan28,dan29,dan30,dan31,r2.fixnaisplata Sati,'' ukupno_sati from feroapp.dbo.radnici r left join fxsap.dbo.plansatirada p on r.ID_Fink = p.radnikid and r.ID_Firme = p.Firma left join rfind.dbo.radnici_ r2 on r2.id_radnika=r.id_radnika where r.id_radnika = " + idradnika1.ToString() + " order by (godina*100+mjesec) desc";

            SqlDataAdapter dataadapter = new SqlDataAdapter(sql1, connection);
            DataSet ds = new DataSet();
            connection.Open();
            dataadapter.Fill(ds, "event");
            connection.Close();
            dgv_zbirni.Width = panel2.Width - 100;
            dgv_zbirni.Height = panel2.Height - 100;

            dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);            
            dgv_zbirni.DataSource = ds;
            
            int count = 0;

            dgv_zbirni.DataMember = "event";
            dgv_zbirni.Columns["Sati"].Visible = false;
            DateTime dat1;

            foreach (DataGridViewRow row in dgv_zbirni.Rows)
            {
                if (row.Cells[3].Value == null || row.Cells[3].Value == DBNull.Value || String.IsNullOrWhiteSpace(row.Cells[3].Value.ToString()))
                {
                    continue;
                }

                int mjesec = (int.Parse)(row.Cells[3].Value.ToString());
                int godina = (int.Parse)(row.Cells[2].Value.ToString());
                int rezija = (int.Parse)(row.Cells[35].Value.ToString());
                int usati = 0;

                for (int i = 0; i < row.Cells.Count; i++)
                {

                    int dan = i - 3;
                    int usatiold = usati;

                    if (row.Cells[i].Value == null || row.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(row.Cells[i].Value.ToString()))
                    {

                        try
                        {
                            dat1 = new DateTime(godina, mjesec, dan);
                        }
                        catch
                        {
                            dat1 = DateTime.Now;
                            continue;
                        }

                        if (dat1.DayOfWeek == DayOfWeek.Saturday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightYellow;
                        }
                        if (dat1.DayOfWeek == DayOfWeek.Sunday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightSeaGreen;
                        }

                    }
                    else
                    {                       
                            try
                                {
                                    dat1 = new DateTime(godina, mjesec, dan);
                                }        
                            catch
                                {
                                        dat1 =  DateTime.Now ;
                                        continue;
                                }

                        if (dat1.DayOfWeek == DayOfWeek.Saturday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightYellow;
                        }
                        if (dat1.DayOfWeek == DayOfWeek.Sunday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightSeaGreen;
                        }
                        
                        string[] ozn = row.Cells[i].Value.ToString().Split(':');
                        string ozn10 = row.Cells[i].Value.ToString();

                        for (int ii = 0; ii < ozn.Length; ii++)
                        {
                            string ozn1 = ozn[ii];
                            

                            if (dat1.DayOfWeek == DayOfWeek.Saturday)
                            {
                                if (ozn1.Contains("j"))
                                {
                                    string ozn11 = ozn1.Replace("j", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 5;
                                }
                                if (ozn1.Contains("p"))
                                {
                                    string ozn11 = ozn1.Replace("p", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 5;
                                }
                                if (ozn1.Contains("n"))
                                {

                                    string ozn11 = ozn1.Replace("n", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 5;
                                }
                                if (ozn1.Contains("g"))
                                {
                                    string ozn11 = ozn1.Replace("g", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 ;
                                }
                                if (ozn1.Contains("b"))
                                {
                                    string ozn11 = ozn1.Replace("b", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 ;
                                }
                                if (ozn1.Contains("y"))
                                {
                                    string ozn11 = ozn1.Replace("y", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 5;
                                }
                                row.Cells[i].Value = usati - usatiold;
                            }
                            else
                            {
                                if (ozn1.Contains("j"))
                                {
                                    string ozn11 = ozn1.Replace("j", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 7;
                                }
                                if (ozn1.Contains("p"))
                                {
                                    string ozn11 = ozn1.Replace("p", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 7;
                                }
                                if (ozn1.Contains("n"))
                                {

                                    string ozn11 = ozn1.Replace("n", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 7;
                                }
                                if (ozn1.Contains("g"))
                                {
                                    string ozn11 = ozn1.Replace("g", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 ;
                                }
                                if (ozn1.Contains("b"))
                                {
                                    string ozn11 = ozn1.Replace("b", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 ;
                                }
                                if (ozn1.Contains("y"))
                                {
                                    string ozn11 = ozn1.Replace("y", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + sati1 + 7;
                                }


                                row.Cells[i].Value = (usati - usatiold).ToString()+" - "+ozn10;
                            }


                            if (ozn1.Contains("g"))
                            {
                                row.Cells[i].Style.BackColor = Color.LightBlue;
                            }

                            if (ozn1.Contains("b"))
                            {
                                row.Cells[i].Style.BackColor = Color.WhiteSmoke;
                            }


                            if (ozn1.Contains("0y"))
                            {
                                row.Cells[i].Style.BackColor = Color.LightPink;
                            }

                            row.Cells[i].Value = (usati - usatiold).ToString() + " = " + ozn10  ;

                        }
                    }

                    //if (qtyEntered <= 0)
                    //{
                    //    dgv_zbirni[0, count].Style.BackColor = Color.Red;//to color the row
                    //    dgv_zbirni[1, count].Style.BackColor = Color.Red;

                    //    dgv_zbirni[0, count].ReadOnly = true;//qty should not be enter for 0 inventory                       
                    //}
                    //dgv_zbirni[0, count].Value = "0";//assign a default value to quantity enter
                    //count++;
                }
                row.Cells[36].Value = usati.ToString();
            }
        }

        private void pregledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label2.Text = "Pregled Godišnjih";
            cistipanele();
            btn_pomoc.Visible = true;
            panel2.Visible = true;
            btn_godisnji.Visible = true;
            btn_subota.Visible = true;
            btn_nedjelja.Visible = true;
            btn_praznici.Visible = true;
            btn_subota.BackColor = Color.LightYellow;
            btn_nedjelja.BackColor = Color.LightSeaGreen;
            btn_praznici.BackColor = Color.LightPink;
            btn_godisnji.BackColor = Color.LightBlue;


            DateTime danas = DateTime.Now;
            DateTime prije = danas.AddDays(-100);
            string sdanas = danas.Year.ToString() + '-' + danas.Month.ToString() + '-' + danas.Day.ToString();
            string sprije = prije.Year.ToString() + '-' + prije.Month.ToString() + '-' + prije.Day.ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            //pprezimer = "Karabelj";

            string sql1 = "select radnikid,p.ime,godina,mjesec,dan01,dan02,dan03,dan04,dan05,dan06,dan07,dan08,dan09,dan10,dan11,dan12,dan13,dan14,dan15,dan16,dan17,dan18,dan19,dan20,dan21,dan22,dan23,dan24,dan25,dan26,dan27,dan28,dan29,dan30,dan31,r2.fixnaisplata Sati,'' ukupno_dana from feroapp.dbo.radnici r left join fxsap.dbo.plansatirada p on r.ID_Fink = p.radnikid and r.ID_Firme = p.Firma left join rfind.dbo.radnici_ r2 on r2.id_radnika=r.id_radnika where r.id_radnika = " + idradnika1.ToString() + " order by (godina*100+mjesec) desc";

            SqlDataAdapter dataadapter = new SqlDataAdapter(sql1, connection);
            DataSet ds = new DataSet();
            connection.Open();
            dataadapter.Fill(ds, "event");
            connection.Close();
            dgv_zbirni.Width = panel2.Width - 100;
            dgv_zbirni.Height = panel2.Height - 100;

            dgv_zbirni.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgv_zbirni.DataSource = ds;
            int count = 0;

            dgv_zbirni.DataMember = "event";
            DateTime dat1;

            foreach (DataGridViewRow row in dgv_zbirni.Rows)
            {
                if (row.Cells[3].Value == null || row.Cells[3].Value == DBNull.Value || String.IsNullOrWhiteSpace(row.Cells[3].Value.ToString()))
                {
                    continue;
                }

                int mjesec = (int.Parse)(row.Cells[3].Value.ToString());
                int godina = (int.Parse)(row.Cells[2].Value.ToString());
                int rezija = (int.Parse)(row.Cells[35].Value.ToString());
                int usati = 0;

                for (int i = 0; i < row.Cells.Count; i++)
                {

                    int dan = i - 3;
                    int usatiold = usati;

                    if (row.Cells[i].Value == null || row.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(row.Cells[i].Value.ToString()))
                    {

                        try
                        {
                            dat1 = new DateTime(godina, mjesec, dan);
                        }
                        catch
                        {
                            dat1 = DateTime.Now;
                            continue;
                        }

                        if (dat1.DayOfWeek == DayOfWeek.Saturday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightYellow;
                        }
                        if (dat1.DayOfWeek == DayOfWeek.Sunday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightSeaGreen;
                        }

                    }
                    else
                    {
                        try
                        {
                            dat1 = new DateTime(godina, mjesec, dan);
                        }
                        catch
                        {
                            dat1 = DateTime.Now;
                            continue;
                        }

                        if (dat1.DayOfWeek == DayOfWeek.Saturday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightYellow;
                        }
                        if (dat1.DayOfWeek == DayOfWeek.Sunday)
                        {
                            row.Cells[i].Style.BackColor = Color.LightSeaGreen;
                        }

                        string[] ozn = row.Cells[i].Value.ToString().Split(':');

                        for (int ii = 0; ii < ozn.Length; ii++)
                        {
                            string ozn1 = ozn[ii];

                            if (dat1.DayOfWeek == DayOfWeek.Saturday)
                            {
                                if (ozn1.Contains("j"))
                                {
                                    string ozn11 = ozn1.Replace("j", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                if (ozn1.Contains("p"))
                                {
                                    string ozn11 = ozn1.Replace("p", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                if (ozn1.Contains("n"))
                                {

                                    string ozn11 = ozn1.Replace("n", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                row.Cells[i].Value = "";
                                if (ozn1.Contains("g"))
                                {
                                    string ozn11 = ozn1.Replace("g", "");
                                    int sati1 = (int.Parse(ozn11));
                                    usati = usati + 1;
                                    row.Cells[i].Value = sati1;
                                }
                                if (ozn1.Contains("b"))
                                {
                                    string ozn11 = ozn1.Replace("b", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                if (ozn1.Contains("y"))
                                {
                                    string ozn11 = ozn1.Replace("y", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                
                            }
                            else
                            {
                                if (ozn1.Contains("j"))
                                {
                                    string ozn11 = ozn1.Replace("j", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                if (ozn1.Contains("p"))
                                {
                                    string ozn11 = ozn1.Replace("p", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                if (ozn1.Contains("n"))
                                {

                                    string ozn11 = ozn1.Replace("n", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                row.Cells[i].Value = "";
                                if (ozn1.Contains("g"))
                                {
                                    string ozn11 = ozn1.Replace("g", "");
                                    int sati1 = (int.Parse(ozn11));
                                    row.Cells[i].Value = sati1;
                                    usati = usati + 1;

                                }
                                if (ozn1.Contains("b"))
                                {
                                    string ozn11 = ozn1.Replace("b", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }
                                if (ozn1.Contains("y"))
                                {
                                    string ozn11 = ozn1.Replace("y", "");
                                    int sati1 = (int.Parse(ozn11));
                                 
                                }


                               
                            }


                            if (ozn1.Contains("g"))
                            {
                                row.Cells[i].Style.BackColor = Color.LightBlue;
                               
                            }

                            if (ozn1.Contains("b"))
                            {
                                row.Cells[i].Style.BackColor = Color.WhiteSmoke;
                            }


                            if (ozn1.Contains("0y"))
                            {
                          //      row.Cells[i].Style.BackColor = Color.LightPink;
                            }

                            //row.Cells[i].Value = (usati - usatiold).ToString() + " - " + ozn1;

                        }
                    }

                    //if (qtyEntered <= 0)
                    //{
                    //    dgv_zbirni[0, count].Style.BackColor = Color.Red;//to color the row
                    //    dgv_zbirni[1, count].Style.BackColor = Color.Red;

                    //    dgv_zbirni[0, count].ReadOnly = true;//qty should not be enter for 0 inventory                       
                    //}
                    //dgv_zbirni[0, count].Value = "0";//assign a default value to quantity enter
                    //count++;
                }
                row.Cells[36].Value = usati.ToString();
                if (usati>0)
                    row.Cells[36].Style.BackColor= Color.LightBlue;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            panel5.Visible = false;
        }

        private void btn_pomoc_Click(object sender, EventArgs e)
        {
            panel5.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection cn1 = new SqlConnection(connectionString);
            cn1.Open();

            SqlCommand sqlCommand1 = new SqlCommand("update poruke set status='P' where rtrim(userid)='" + idradnika1.ToString() + "' and id="+idporuke, cn1);
            SqlDataReader reader21 = sqlCommand1.ExecuteReader();
            cistipanele();
            
        }
    }
    }


