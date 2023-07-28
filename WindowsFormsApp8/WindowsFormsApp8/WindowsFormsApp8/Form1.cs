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


namespace WindowsFormsApp8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Veri Tabanı bağlantısı için conn isminde bir nesne oluşturuldu ve Veri tabanı bilgileri girildi
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-M0KLB6Q\SQLEXPRESS;
                                    Initial Catalog=gorselProgramalama;
                                    Integrated Security=True");

        //Giriş yapılabilmesi için textNumberGiris ve password için oluşturulmuş "textBox1" VE "textNumberGiris"
        //isimli textBox'laraa girilen
        //değerlerin TBL_users tablosunda bulunan number ve password sütunlarındaki değerlerle tutarlı olması gerekmektedir.
        private void button1_Click(object sender, EventArgs e)
        {
            conn.Open();
            SqlCommand command = new SqlCommand("select * from TBL_Users where number = @p1 and password=@p2", conn);

            command.Parameters.AddWithValue("@p1", textNumberGiris.Text);
            command.Parameters.AddWithValue("@p2", textBox1.Text);
            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                //bilgilerin doğru gildiği durumda Form2 ekranı açılacaktır.
                Form1.ActiveForm.Hide();
                Form2 frm = new Form2();
                frm.number = textNumberGiris.Text;
                frm.Show();
            }
            else
            {
                //bilgilerin yanlış girildiği durumda ise MessageBox ile uyarı verilmektedir.
                MessageBox.Show("Hatalı Bilgi");
            }
            conn.Close();
        }

        //Kullanıcı kaydının bulunmadığı durumlarda "Sign Up" butonuna basılması durumunda kayıt oluşturulabilen Form3 ekranına
        //yönlendirilme sağlanmakdadır.
        private void button2_Click(object sender, EventArgs e)
        {
            Form1.ActiveForm.Hide();
            Form3 frm = new Form3();
            frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.PasswordChar = '*';
        }
    }
}
