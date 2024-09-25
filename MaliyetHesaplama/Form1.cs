using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MaliyetHesaplama
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-32Q9FH5;Initial Catalog=DbMaliyet;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");

        void MalzemeListesi()
        {
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter("Select * FROM TBLMALZEMELER", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            baglanti.Close();
        }

        void UrunListesi()
        {
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter("Select * FROM TBLURUNLER", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            baglanti.Close();
        }

        void Kasa()
        {
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter("Select * FROM TBLKASA", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            baglanti.Close();
        }

        void UrunCekme()
        {
            //ürünleri çekme
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter("Select * FROM TBLURUNLER", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmburun.ValueMember = "URUNID";
            cmburun.DisplayMember = "AD";
            cmburun.DataSource = dt;
            baglanti.Close();

        }

        void MalzemeCekme()
        {
            //malzemeleri çekme
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter("Select * FROM TBLMALZEMELER", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmbmalzeme.ValueMember = "MALZEMEID";
            cmbmalzeme.DisplayMember = "AD";
            cmbmalzeme.DataSource = dt;
            baglanti.Close();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MalzemeListesi();
            UrunCekme();
            MalzemeCekme();

        }

        private void btnmalzemelistesi_Click(object sender, EventArgs e)
        {
            MalzemeListesi();
        }

        private void btnurunlistesi_Click(object sender, EventArgs e)
        {
            UrunListesi();
        }

        private void btnkasa_Click(object sender, EventArgs e)
        {
            Kasa();
        }

        private void btncikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnmalzemeekle_Click(object sender, EventArgs e)
        {
            string malzeme = txtmalzemead.Text.ToUpper();
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into TBLMALZEMELER (AD,STOK,FIYAT,NOTLAR) values (@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1", malzeme);
            komut.Parameters.AddWithValue("@p2", decimal.Parse(txtmalzemestok.Text));
            komut.Parameters.AddWithValue("@p3", decimal.Parse(txtmalzemefiyat.Text));
            komut.Parameters.AddWithValue("@p4", txtmalzemenot.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Malzeme Eklendi", "Bilgi");
            MalzemeListesi();
            MalzemeCekme();
        }

        private void btnurunekle_Click(object sender, EventArgs e)
        {
            string urun = txturunad.Text.ToUpper();
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into TBLURUNLER (AD) values (@p1)", baglanti);
            komut.Parameters.AddWithValue("@p1", urun);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ürün Eklendi", "Bilgi");
            UrunListesi();
            UrunCekme();
            txturunad.Text = "";
        }

        private void txturunekle_Click(object sender, EventArgs e)
        {


            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into TBLFIRIN (URUNID,MALZEMEID,MIKTAR,MALIYET) values (@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1", cmburun.SelectedValue);
            komut.Parameters.AddWithValue("@p2", cmbmalzeme.SelectedValue);
            komut.Parameters.AddWithValue("@p3", decimal.Parse(txturunmiktar.Text));
            komut.Parameters.AddWithValue("@p4", decimal.Parse(txturunmaliyet.Text));
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Malzeme Eklendi", "Bilgi");

            listBox1.Items.Add(cmbmalzeme.Text + " - " + txturunmaliyet.Text);

        }

        private void txturunmiktar_TextChanged(object sender, EventArgs e)
        {
            double maliyet;

            if (txturunmiktar.Text == "")
            {
                txturunmiktar.Text = "0";
            }

            baglanti.Open();

            SqlCommand komut = new SqlCommand("Select * from TBLMALZEMELER where MALZEMEID=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", cmbmalzeme.SelectedValue);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                txturunmaliyet.Text = dr[3].ToString();
            }
            baglanti.Close();

            if (cmbmalzeme.Text == "YUMURTA")
            {
                maliyet = Convert.ToDouble(txturunmaliyet.Text) * Convert.ToDouble(txturunmiktar.Text);
                txturunmaliyet.Text = maliyet.ToString("0.00");
            }
            else
            {
                maliyet = Convert.ToDouble(txturunmaliyet.Text) / 1000 * Convert.ToDouble(txturunmiktar.Text);
                txturunmaliyet.Text = maliyet.ToString("0.00");
            }


        }



        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            txturunid.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            txturunad.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();

            baglanti.Open();
            SqlCommand komut = new SqlCommand("Select SUM(MALIYET) from TBLFIRIN where URUNID=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", txturunid.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                txturunmaliyetfiyat.Text = dr[0].ToString();
            }
            baglanti.Close();
        }

        private void btnurunguncelle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update TBLURUNLER set MFIYAT=@p1, SFIYAT=@p2, STOK=@p3 where URUNID=@p4", baglanti);
            komut.Parameters.AddWithValue("@p1", decimal.Parse(txturunmaliyetfiyat.Text));
            komut.Parameters.AddWithValue("@p2", decimal.Parse(txturunsatisfiyat.Text));
            komut.Parameters.AddWithValue("@p3", txturunstok.Text);
            komut.Parameters.AddWithValue("@p4", txturunid.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();


            double giris = 0, cikis=0; 
            giris = Convert.ToDouble(txturunsatisfiyat.Text) * Convert.ToDouble(txturunstok.Text);
            cikis = Convert.ToDouble(txturunmaliyetfiyat.Text) * Convert.ToDouble(txturunstok.Text);

            baglanti.Open();
            SqlCommand komut1 = new SqlCommand("update TBLKASA set GIRIS=GIRIS+@s1,CIKIS=CIKIS+@s2", baglanti);
            komut1.Parameters.AddWithValue("@s1", giris);
            komut1.Parameters.AddWithValue("@s2", cikis);
            komut1.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ürün ve Kasa Güncellendi");
            UrunListesi();


        }
    }
}
