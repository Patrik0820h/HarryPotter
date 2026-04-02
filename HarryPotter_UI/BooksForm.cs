using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarryPotter_UI
{
    public partial class BooksForm : Form
    {

        public class Book
        {
            [JsonPropertyName("original_title")]
            public string OriginalTitle { get; set; }

            [JsonPropertyName("pages")]
            public int Pages { get; set; }

            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("image")]
            public string CoverUrl { get; set; }
        }

        public BooksForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string apiUrl = "https://potterapi-fedeperin.vercel.app/en/books";

            using (HttpClient client = new HttpClient())
            {
                
            }

        }

        private void dgvBooks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0) { return;};

            Book selectedBook = dgvBooks.SelectedRows[0].DataBoundItem as Book;
            if (selectedBook != null)
            {
                label1.Text = selectedBook.Description;

                try
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Load(selectedBook.CoverUrl);
                }
                catch
                {
                    pictureBox1.Image = null;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BooksForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}
