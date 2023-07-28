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
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace WindowsFormsApp8
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-M0KLB6Q\SQLEXPRESS;
        Initial Catalog=gorselProgramalama;Integrated Security=True");

        void gelenKutusu()
        {
            SqlDataAdapter da1 = new SqlDataAdapter(@"SELECT [messageId] ,[name], [surname], [topic], [message], [attachment] FROM TBL_Messages
                                        LEFT JOIN TBL_Users ON TBL_Messages.sender = TBL_Users.number WHERE receiver = "
                                                    + number + " ORDER BY MessageId DESC", conn);

            // DataTable oluşturma ve verileri doldurma
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            // DataGridView'e veri kaynağı olarak DataTable'ı atama
            dataGridView1.DataSource = dt1;

            // Sütun isimlerini değiştirme
            dataGridView1.Columns["name"].HeaderText = "Sender Name";
            dataGridView1.Columns["surname"].HeaderText = "Sender Surname";
            dataGridView1.Columns["topic"].HeaderText = "Topic";
            dataGridView1.Columns["message"].HeaderText = "Message";
            dataGridView1.Columns["attachment"].HeaderText = "Attachment";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        void gidenKutusu()
        {
            SqlDataAdapter da2 = new SqlDataAdapter(@"SELECT [messageId] , [name], [surname], [topic], [message], [attachment] FROM TBL_Messages
                                                    LEFT JOIN TBL_Users ON TBL_Messages.receiver = TBL_Users.number WHERE sender = "
                                                    + number + " ORDER BY MessageId DESC", conn);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);

            // DataGridView'e veri kaynağı olarak DataTable'ı atama
            dataGridView2.DataSource = dt2;

            // Sütun isimlerini değiştirme
            dataGridView2.Columns["name"].HeaderText = "Receiver Name";
            dataGridView2.Columns["surname"].HeaderText = "Receiver Surname";
            dataGridView2.Columns["topic"].HeaderText = "Topic";
            dataGridView2.Columns["message"].HeaderText = "Message";
            dataGridView2.Columns["attachment"].HeaderText = "Attachment";

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        public string number;
        private void Form2_Load(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FileOk += new CancelEventHandler(openFileDialog1_FileOk);

            lblNumber.Text = number;
            gelenKutusu();
            gidenKutusu();

            conn.Open();
            SqlCommand command = new SqlCommand("select name, surname from TBL_Users where number = " + number, conn);
            SqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                lblName.Text = dr[0] + " " + dr[1];

            }
            conn.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            conn.Open();

            SqlCommand command = new SqlCommand("insert into TBL_Messages (sender,receiver,topic,message) values (@p1, @p2, @p3, @p4)", conn);
            command.Parameters.AddWithValue("@p1", number);
            command.Parameters.AddWithValue("@p2", boxAlici.Text);
            command.Parameters.AddWithValue("@p3", textBox1.Text);
            command.Parameters.AddWithValue("@p4", richTextBox1.Text);
            command.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Mesajınız iletildi");
            gidenKutusu();
            gelenKutusu();
        }
        private void btnAttach_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string dosyaYolu = openFileDialog1.FileName; // Seçilen dosyanın yolu

                // Dosyayı bir klasöre kopyala
                string hedefKlasor = Path.Combine(Application.StartupPath, "dosya_klasoru");

                // Hedef klasörü oluştur (eğer yoksa)
                if (!Directory.Exists(hedefKlasor))
                {
                    Directory.CreateDirectory(hedefKlasor);
                }
                string hedefDosyaYolu = Path.Combine(hedefKlasor, Path.GetFileName(dosyaYolu));
                File.Copy(dosyaYolu, hedefDosyaYolu, true);

                // Veritabanına dosya yolunu kaydet
                conn.Open();
                SqlCommand command = new SqlCommand("insert into TBL_Messages (sender, receiver, topic, message, attachment) " +
                    "values (@p1, @p2, @p3, @p4, @p5)", conn);
                command.Parameters.AddWithValue("@p1", number);
                command.Parameters.AddWithValue("@p2", boxAlici.Text);
                command.Parameters.AddWithValue("@p3", textBox1.Text);
                command.Parameters.AddWithValue("@p4", richTextBox1.Text);
                command.Parameters.AddWithValue("@p5", hedefDosyaYolu);
                command.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Dosya eklendi ve mesaj gönderildi: " + dosyaYolu);
                gidenKutusu();
                gelenKutusu();
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0) // İndirilecek bir dosya seçildiğinden emin olun
            {
                string dosyaYolu = dataGridView2.SelectedRows[0].Cells["attachment"].Value.ToString(); // Seçilen dosyanın yolu
                //Dosya varsa, SaveFileDialog ile bir dosya kaydetme iletişim kutusu açılır. Kullanıcı,
                //indirilen dosyanın kaydedileceği hedef yol ve dosya adını seçer.
                if (File.Exists(dosyaYolu))
                {

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = Path.GetFileName(dosyaYolu); // İndirilen dosyanın adı
                    saveFileDialog.Filter = $"Dosya (*{Path.GetExtension(dosyaYolu)})|*{Path.GetExtension(dosyaYolu)}";                     
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string hedefDosyaYolu = saveFileDialog.FileName;

                        // Dosyayı belirtilen hedefe kopyala
                        File.Copy(dosyaYolu, hedefDosyaYolu, true);

                        MessageBox.Show("Dosya indirildi: " + hedefDosyaYolu);
                    }
                }
                else
                {
                    MessageBox.Show("Dosya bulunamadı!");
                }
            }
            else
            {
                MessageBox.Show("Lütfen indirmek için bir dosya seçin!");
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string dosyaYolu = openFileDialog1.FileName;
            MessageBox.Show("Dosya eklendi: " + dosyaYolu);
        }



        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Seçili mesajı silmek istediğinize emin misiniz?", "Mesaj Silme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string messageId = dataGridView1.SelectedRows[0].Cells["messageId"].Value.ToString();

                    conn.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM TBL_Messages WHERE messageId = @messageId", conn);
                    command.Parameters.AddWithValue("@messageId", messageId);
                    command.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("Mesaj silindi.");
                    gelenKutusu();
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir mesaj seçin!");
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView2.Columns["attachment"].Index) // Link hücresine tıklandığından emin olun
            {
                string dosyaYolu = dataGridView2.Rows[e.RowIndex].Cells["attachment"].Value.ToString(); // Dosyanın yolu

                if (File.Exists(dosyaYolu))
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = dosyaYolu;
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya açılamadı: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Dosya bulunamadı!");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2.ActiveForm.Hide();
            Form1 frm = new Form1();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Seçili mesajı silmek istediğinize emin misiniz?", "Mesaj Silme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string messageId = dataGridView2.SelectedRows[0].Cells["messageId"].Value.ToString();

                    conn.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM TBL_Messages WHERE messageId = @messageId", conn);
                    command.Parameters.AddWithValue("@messageId", messageId);
                    command.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("Mesaj silindi.");
                    gidenKutusu();
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir mesaj seçin!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) // İndirilecek bir dosya seçildiğinden emin olun
            {
                string dosyaYolu = dataGridView1.SelectedRows[0].Cells["attachment"].Value.ToString(); // Seçilen dosyanın yolu
                //Dosya varsa, SaveFileDialog ile bir dosya kaydetme iletişim kutusu açılır. Kullanıcı,
                //indirilen dosyanın kaydedileceği hedef yol ve dosya adını seçer.
                if (File.Exists(dosyaYolu))
                {

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = Path.GetFileName(dosyaYolu); // İndirilen dosyanın adı
                    saveFileDialog.Filter = $"Dosya (*{Path.GetExtension(dosyaYolu)})|*{Path.GetExtension(dosyaYolu)}"; if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string hedefDosyaYolu = saveFileDialog.FileName;

                        // Dosyayı belirtilen hedefe kopyala
                        File.Copy(dosyaYolu, hedefDosyaYolu, true);

                        MessageBox.Show("Dosya indirildi: " + hedefDosyaYolu);
                    }
                }
                else
                {
                    MessageBox.Show("Dosya bulunamadı!");
                }
            }
            else
            {
                MessageBox.Show("Lütfen indirmek için bir dosya seçin!");
            }
        }
    }
}
