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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        // Veri Tabanına bağlan
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-M0KLB6Q\SQLEXPRESS;
        Initial Catalog=gorselProgramalama;Integrated Security=True");
        private void btnSignup_Click(object sender, EventArgs e)
        {
            conn.Open(); //bağlantıyı aç

            //Tabloya kayıt için gerekli SQL komutu.
            SqlCommand command = new SqlCommand("insert into TBL_Users (name,surname,number,password) " +
                "                               values (@p1, @p2, @p3, @p4)", conn);
            command.Parameters.AddWithValue("@p1", txtName.Text);
            command.Parameters.AddWithValue("@p2", txtSurname.Text);
            command.Parameters.AddWithValue("@p3", txtMail.Text);
            command.Parameters.AddWithValue("@p4", txtPassword.Text);
            command.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Account Created");
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            Form3.ActiveForm.Hide();
            Form1 frm = new Form1();
            frm.Show(); 
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '*';

        }
    }
}
