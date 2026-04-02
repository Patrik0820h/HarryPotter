using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;

namespace HarryPotter_UI
{
    public partial class BooksForm : Form
    {

        public class Book
        {
            public string original_title { get; set; }
            public int pages { get; set; }
            public string release_date { get; set; }
            public string description { get; set; }
            public string cover { get; set; } 
        }

        public BooksForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://potterapi-fedeperin.vercel.app/en/books";
                    string json = await client.GetStringAsync(url);

                    List<Book> books = JsonConvert.DeserializeObject<List<Book>>(json);

                    dgvBooks.DataSource = books;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
            }

        }

        private void dgvBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBooks.CurrentRow == null) return;

            Book selectedBook = dgvBooks.CurrentRow.DataBoundItem as Book;
            if (selectedBook == null) return;

            label1.Text = selectedBook.description;

            try
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.Load(selectedBook.cover);
            }
            catch
            {
                pictureBox1.Image = null;
            }
        }

        private void BooksForm_Load(object sender, EventArgs e)
        {
            dgvBooks.AutoGenerateColumns = false;

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "original_title",
                HeaderText = "Eredeti cím"
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "pages",
                HeaderText = "Oldalak"
            });

            dgvBooks.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "release_date",
                HeaderText = "Kiadás dátuma"
            });
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
