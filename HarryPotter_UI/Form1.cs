using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarryPotter_UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ListViewLoader();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BooksForm booksForm = new BooksForm();
            booksForm.Show();
        }
        private void ListViewLoader()
        {

            string connectionString = "Server=localhost;Database=harrypotter;UId=root;Password=;";

            listView1.View = View.List;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Characters";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    listView1.Items.Clear();

                    while (reader.Read())
                    {
                        DateTime birthform = DateTime.Parse(reader["BirthDate"].ToString());
                        string shortDate = birthform.ToString("yyyy-MM-dd");
                        string rowText = $"{reader["CharacterID"]} - {reader["FullName"]} - {reader["NickName"]} - {reader["HogWartsHouse"]} - {reader["InterPretedBy"]} - {shortDate}";
                        ListViewItem item = new ListViewItem(rowText);
                        item.Tag = reader["Image"].ToString();
                        listView1.Items.Add(item);
                        Console.WriteLine(item.Tag);
                    }
                }
                listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            }
        }
        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { return; }

            ListViewItem selected = listView1.SelectedItems[0];
            string imageUrl = selected.Tag.ToString();

            if (!string.IsNullOrEmpty(imageUrl))
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Load(imageUrl);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListViewLoader();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
