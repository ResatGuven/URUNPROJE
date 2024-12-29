using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJESQL
{
    public partial class Form1 : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox searchTextBox;
        private Button btnLogin;
        private Label lblMessage;
        private string connectionString = $"Server={System.Net.Dns.GetHostName()};Database=veritabani;Trusted_Connection=True;";
        private DataGridView dataGridView;
        private Button btnEkle, btnSil, btnDuzenle, btnIstatistik, btnResimEkle, btnTemizle, btnYorumlar, btnResimSil;
        private TextBox[] textBoxes;
        private TextBox[] textBoxes2;
        private string[] labels = { "Marka", "Model", "Ürün Rengi", "Malzeme", "Fiyat", "Agirlik", "Stok", "Garanti", "Ekstra", "Aciklama" };
        private string[] labels2 = { "Marka", "Model", "Ürün Rengi", "Malzeme", "Fiyat", "Agirlik", "Stok", "Garanti", "Ekstra", "Aciklama" };
        private int selectedRowId = -1; // Seçilen satırın ID'si
        private PictureBox pictureBox;
        private bool resimDegisti = false; // Resim değişip değişmediğini kontrol etmek için bayrak
        private DataTable dataTable;
        private DataView dataView;
        private SqlConnection connection;
        private DataGridView dataGridViewUrunler;
        private DataGridView dataGridViewYorumlar;
        private DataGridView dataGridViewIstatislikler;
        private TextBox txtYorum;
        private bool admin = false;
        private FlowLayoutPanel starPanel;
        private Button star;
        private int selectedRating = 0;  // Varsayılan puan
        private Button btnKaydet;


        public Form1()
        {
            InitializeComponent();
            InitializeLoginComponents();
        }

        private void InitializeLoginComponents()
        {
            this.Controls.Clear();
            this.Text = "Kullanıcı Girişi";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black; // Arka planı siyah yapıyoruz

            // Başlık
            Label lblTitle = new Label
            {
                Text = "GİRİŞ PANELİ",
                Font = new Font("Segoe UI", 24, FontStyle.Bold), // Daha büyük başlık
                ForeColor = Color.White, // Başlık rengi beyaz
                AutoSize = true,
                Location = new Point(110, 20)
            };
            this.Controls.Add(lblTitle);

            // Kullanıcı Adı
            Label lblUsername = new Label
            {
                Text = "Kullanıcı Adı:",
                Location = new Point(50, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Regular), // Daha büyük metin boyutu
                ForeColor = Color.White
            };
            this.Controls.Add(lblUsername);

            // Kullanıcı Adı Textbox
            txtUsername = new TextBox
            {
                Location = new Point(160, 88),
                Width = 180,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.White, // Kontrast artırıldı
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtUsername);

            // Şifre
            Label lblPassword = new Label
            {
                Text = "Şifre:",
                Location = new Point(50, 140),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.White
            };
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(160, 138),
                Width = 180,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '*'
            };
            this.Controls.Add(txtPassword);

            // Giriş Yap butonu
            btnLogin = new Button
            {
                Text = "Giriş Yap",
                Location = new Point(160, 200),
                Width = 180,
                Height = 40,
                BackColor = Color.Red, // Kırmızı buton arka planı
                ForeColor = Color.White, // Yazılar net görünüyor
                Font = new Font("Segoe UI", 14, FontStyle.Bold), // Daha büyük buton yazısı
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // Enter tuşu desteği
            txtPassword.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnLogin.PerformClick();
                }
            };

            // Hata mesajı
            lblMessage = new Label
            {
                ForeColor = Color.FromArgb(231, 76, 60), // Dikkat çeken kırmızı
                AutoSize = true,
                Location = new Point(50, 250),
                Font = new Font("Segoe UI", 10),
                Text = ""
            };
            this.Controls.Add(lblMessage);
        }




        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (username == "admin" && password == "1234")
            {
                admin = true;
                InitializeForm2Components();
            }
            else if (username == "user" && password == "1234")
            {
                InitializeForm3Components();
            }
            else if (username == "admin" && password == "123")
            {
                InitializeForm6Components();
            }
            else
            {
                lblMessage.Text = "Hatalı kullanıcı adı veya şifre!";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void InitializeForm2Components()
        {
            this.Size = new Size(1120, 700);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );

            this.Controls.Clear();
            this.Text = "Admin Paneli";
            this.BackColor = Color.Black; // Arka planı siyah yapıyoruz

            // DataGridView tasarımı
            dataGridView = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(1070, 250),
                AllowUserToAddRows = false,
                ReadOnly = true,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                MultiSelect = false,
                BackgroundColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.Fixed3D,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Black,
                    BackColor = Color.White
                }
            };

            // Sütun başlıklarını mavi yapma
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dataGridView.CellClick += DataGridView_CellClick;
            this.Controls.Add(dataGridView);
            LoadData();

            // Label ve TextBox düzenlemeleri
            textBoxes = new TextBox[labels.Length];
            int labelColumn1X = 20, labelColumn2X = 400, textBoxColumn1X = 130, textBoxColumn2X = 520;

            for (int i = 0; i < labels.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = labels[i] + ":",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.White
                };

                if (i < 5)
                {
                    lbl.Location = new Point(labelColumn1X, 300 + i * 50);
                    this.Controls.Add(lbl);

                    textBoxes[i] = new TextBox
                    {
                        Location = new Point(textBoxColumn1X, 300 + i * 50),
                        Width = 250,
                        BorderStyle = BorderStyle.FixedSingle,
                        Font = new Font("Arial", 10),
                        BackColor = Color.LightGray,
                        ForeColor = Color.Black
                    };
                    this.Controls.Add(textBoxes[i]);
                }
                else
                {
                    lbl.Location = new Point(labelColumn2X, 300 + (i - 5) * 50);
                    this.Controls.Add(lbl);

                    textBoxes[i] = new TextBox
                    {
                        Location = new Point(textBoxColumn2X, 300 + (i - 5) * 50),
                        Width = 250,
                        BorderStyle = BorderStyle.FixedSingle,
                        Font = new Font("Arial", 10),
                        BackColor = Color.LightGray,
                        ForeColor = Color.Black
                    };
                    this.Controls.Add(textBoxes[i]);
                }
            }

            // PictureBox ve Butonlar
            pictureBox = new PictureBox
            {
                Location = new Point(855, 300),
                Size = new Size(200, 200),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBox);

            btnResimEkle = new Button
            {
                Text = "Resim Ekle",
                Location = new Point(825, pictureBox.Bottom + 20),
                Width = 120,
                Height = 40,
                BackColor = Color.Teal,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnResimSil = new Button
            {
                Text = "Resimi Sil",
                Location = new Point(955, pictureBox.Bottom + 20),
                Width = 120,
                Height = 40,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnResimEkle.Click += BtnResimEkle_Click;
            btnResimSil.Click += BtnResimSil_Click;

            this.Controls.Add(btnResimEkle);
            this.Controls.Add(btnResimSil);

            // Diğer butonlar
            btnEkle = new Button { Text = "Ekle", Location = new Point(20, 600), Width = 140, Height = 40, BackColor = Color.LightGreen, Font = new Font("Arial", 12, FontStyle.Bold) };
            btnSil = new Button { Text = "Sil", Location = new Point(180, 600), Width = 140, Height = 40, BackColor = Color.Salmon, Font = new Font("Arial", 12, FontStyle.Bold) };
            btnDuzenle = new Button { Text = "Düzenle", Location = new Point(340, 600), Width = 140, Height = 40, BackColor = Color.LightBlue, Font = new Font("Arial", 12, FontStyle.Bold) };
            btnIstatistik = new Button { Text = "İstatistik", Location = new Point(500, 600), Width = 140, Height = 40, BackColor = Color.Khaki, Font = new Font("Arial", 12, FontStyle.Bold) };
            btnTemizle = new Button { Text = "Temizle", Location = new Point(340, 550), Width = 140, Height = 40, BackColor = Color.LightGray, Font = new Font("Arial", 12, FontStyle.Bold) };
            btnYorumlar = new Button { Text = "Yorumlar", Location = new Point(660, 600), Width = 140, Height = 40, BackColor = Color.LightCoral, Font = new Font("Arial", 12, FontStyle.Bold) };

            btnEkle.Click += BtnEkle_Click;
            btnSil.Click += BtnSil_Click;
            btnDuzenle.Click += BtnDuzenle_Click;
            btnIstatistik.Click += istatistik;
            btnYorumlar.Click += Yorumlar;
            btnTemizle.Click += Temizle;

            this.Controls.Add(btnEkle);
            this.Controls.Add(btnSil);
            this.Controls.Add(btnDuzenle);
            this.Controls.Add(btnIstatistik);
            this.Controls.Add(btnTemizle);
            this.Controls.Add(btnYorumlar);

            // Çıkış Yap Butonu
            Button exitButton = new Button
            {
                Text = "Çıkış Yap",
                Location = new Point(this.ClientSize.Width - 140, this.ClientSize.Height - 60),
                Size = new Size(120, 40),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);
        }


        private void InitializeForm3Components()
        {
            // **Form Özellikleri**
            this.Size = new Size(1160, 800);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );
            this.Controls.Clear();
            this.Text = "Veri Tablosu Görüntülemesi";

            // **Arka Plan Rengi Admin Paneline Uygun Renk**
            this.BackColor = Color.FromArgb(34, 34, 34); // Admin panelinin koyu gri rengi

            // **Formda Genel Font Ayarı (Admin Panelindeki gibi)**
            this.Font = new Font("Arial", 12); // Arial fontu, genellikle admin panellerinde tercih edilir

            // **"Çıkış Yap" Butonu**
            Button exitButton = new Button
            {
                Text = "Çıkış Yap",
                Location = new Point(this.ClientSize.Width - 120, 10),
                Size = new Size(100, 40),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(204, 0, 0), // Admin paneline uyumlu kırmızı
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);

            // **"Yorumlar" Butonu**
            Button commentsButton = new Button
            {
                Text = "Yorumlar",
                Location = new Point(this.ClientSize.Width - 240, 10),
                Size = new Size(100, 40),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 123, 255), // Admin paneline uyumlu mavi
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            commentsButton.FlatAppearance.BorderSize = 0;
            commentsButton.Click += Yorumlar;
            this.Controls.Add(commentsButton);

            // **DataGridView Kontrolü (Admin Paneli Uyumu)**
            dataGridView = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(1103, 370),
                AllowUserToAddRows = false,
                ReadOnly = true,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                MultiSelect = false,
                Font = new Font("Arial", 10), // Admin panelindeki font stili
                ForeColor = Color.White, // Beyaz yazılar
                BackgroundColor = Color.FromArgb(47, 47, 47), // Koyu gri arka plan
                GridColor = Color.Gray, // Grid çizgisi
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(50, 50, 50) // Alternatif satırlar için koyu gri
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    SelectionBackColor = Color.FromArgb(0, 123, 255), // Seçili satır için mavi
                    SelectionForeColor = Color.White,
                    BackColor = Color.FromArgb(47, 47, 47), // Koyu gri arka plan
                    ForeColor = Color.White // Beyaz yazılar
                }
            };

            // **Sütun Başlıkları (Column Headers) İçin Renk Ayarı**
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 123, 255); // Mavi arka plan
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Beyaz yazılar
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold); // Font ve kalın yazı
            dataGridView.EnableHeadersVisualStyles = false; // Başlıkları özelleştir

            dataGridView.CellClick += DataGridView_CellClick2;
            this.Controls.Add(dataGridView);
            LoadData2();

            // **"Özel Arama" Label**
            Label specialSearchLabel = new Label
            {
                Text = "Özel Arama:",
                Location = new Point(300, dataGridView.Bottom + 10),
                Size = new Size(140, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(204, 0, 0) // Admin paneline uyumlu kırmızı
            };
            this.Controls.Add(specialSearchLabel);

            // **TextBox Array ve Etiketler**
            textBoxes = new TextBox[labels.Length];
            for (int i = 0; i < labels.Length; i++)
            {
                int column = i / 5;
                int row = i % 5;

                Label label = new Label
                {
                    Text = labels[i],
                    Location = new Point(20 + (column * 400), 15 + (specialSearchLabel.Bottom + 10) + (row * 45)),
                    Size = new Size(100, 30),
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.White // Beyaz yazı rengi
                };
                this.Controls.Add(label);

                textBoxes[i] = new TextBox
                {
                    Location = new Point(120 + (column * 400), 15 + (specialSearchLabel.Bottom + 10) + (row * 45)),
                    Size = new Size(200, 30),
                    Font = new Font("Arial", 12),
                    ForeColor = Color.Black,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    MaxLength = 100
                };
                textBoxes[i].TextChanged += SearchTextBox_TextChanged2;
                this.Controls.Add(textBoxes[i]);
            }

            // **"Temizle" Butonu**
            Button clearButton = new Button
            {
                Text = "Temizle",
                Location = new Point(this.ClientSize.Width / 2 - 250, specialSearchLabel.Bottom + 18 + (5 * 45)),
                Size = new Size(120, 40),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 79, 79), // Koyu kırmızı
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.Click += ClearButton_Click;
            this.Controls.Add(clearButton);

            // **"Genel Arama" Label ve TextBox**
            Label generalSearchLabel = new Label
            {
                Text = "Genel Arama:",
                Location = new Point(20, 20),
                Size = new Size(140, 30),
                Font = new Font("Arial", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(204, 0, 0) // Admin paneline uyumlu kırmızı
            };
            this.Controls.Add(generalSearchLabel);

            searchTextBox = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(generalSearchLabel.Right + 5, 20),
                Font = new Font("Arial", 12),
                ForeColor = Color.Black,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 100
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            this.Controls.Add(searchTextBox);

            // **PictureBox Kontrolü**
            pictureBox = new PictureBox
            {
                Location = new Point(this.ClientSize.Width - 300, this.ClientSize.Height - 290),
                Size = new Size(250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBox);
        }


        private void InitializeForm5Components()
        {
            // Form Yapılandırması
            this.Text = "Ürün Değerlendirme";
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            this.Size = new Size(1180, 800);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(20, 20, 20);  // Siyah ton
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );

            this.Controls.Clear(); // Tüm kontrolleri kaldır

            connection = new SqlConnection(connectionString);

            // DataGridViews
            Label lblUrunler = new Label
            {
                Text = "Ürün Listesi:",
                Location = new Point(20, 20),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White // Metin rengi beyaz
            };
            this.Controls.Add(lblUrunler);

            dataGridViewUrunler = new DataGridView
            {
                Location = new Point(20, lblUrunler.Bottom),
                Size = new Size(1120, 250),
                BackgroundColor = Color.FromArgb(40, 40, 40),  // Koyu gri
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(60, 90, 150),  // Mavi sütun başlıkları
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(50, 50, 50),  // Koyu gri hücreler
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(100, 150, 200),
                    SelectionForeColor = Color.White
                },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                MultiSelect = false,
                AllowUserToResizeColumns = false, // Boyutlandırmayı devre dışı bırak
                AllowUserToResizeRows = false, // Boyutlandırmayı devre dışı bırak
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing // Sütun başlıklarının boyutunu değiştirilememe
            };
            dataGridViewUrunler.SelectionChanged += DataGridViewUrunler_SelectionChanged;
            this.Controls.Add(dataGridViewUrunler);

            Label lblYorumlar = new Label
            {
                Text = "Ürünün Yorumları:",
                Location = new Point(20, dataGridViewUrunler.Bottom + 10),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White // Metin rengi beyaz
            };
            this.Controls.Add(lblYorumlar);

            dataGridViewYorumlar = new DataGridView
            {
                Location = new Point(20, lblYorumlar.Bottom),
                Size = new Size(1120, 250),
                BackgroundColor = Color.FromArgb(40, 40, 40),  // Koyu gri
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(90, 60, 150),  // Mavi sütun başlıkları
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(50, 50, 50),  // Koyu gri hücreler
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(120, 80, 180),
                    SelectionForeColor = Color.White
                },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                MultiSelect = false,
                AllowUserToResizeColumns = false, // Boyutlandırmayı devre dışı bırak
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing, // Sütun başlıklarının boyutunu değiştirilememe
                AllowUserToResizeRows = false // Boyutlandırmayı devre dışı bırak

            };
            this.Controls.Add(dataGridViewYorumlar);

            if (admin == true)
            {
                this.Size = new Size(1180, 690);
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(
                    (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                    (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
                );
                // Yorum Sil Butonu
                Button btnYorumSil = new Button
                {
                    Text = "Yorumu Sil",
                    Location = new Point(500, dataGridViewYorumlar.Bottom + 15),
                    Size = new Size(150, 40),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.FromArgb(200, 60, 60), // Kırmızı buton
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnYorumSil.Click += BtnYorumSil_Click;
                this.Controls.Add(btnYorumSil);
            }
            else
            {
                dataGridViewYorumlar.Enabled = false;

                // Yorum TextBox
                Label lblYorum = new Label
                {
                    Text = "Yorumunuz:",
                    Location = new Point(20, dataGridViewYorumlar.Bottom + 10),
                    Size = new Size(150, 25),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White // Metin rengi beyaz
                };
                this.Controls.Add(lblYorum);

                txtYorum = new TextBox
                {
                    Location = new Point(20, lblYorum.Bottom),
                    Size = new Size(780, 80),
                    Multiline = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(60, 60, 60),  // Koyu gri arka plan
                    ForeColor = Color.White  // Beyaz yazı rengi
                };
                this.Controls.Add(txtYorum);

                // Puan Yıldızları
                Label lblPuan = new Label
                {
                    Text = "Puan:",
                    Location = new Point(840, dataGridViewYorumlar.Bottom + 10),
                    Size = new Size(100, 25),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White // Metin rengi beyaz
                };
                this.Controls.Add(lblPuan);

                starPanel = new FlowLayoutPanel
                {
                    Location = new Point(840, lblPuan.Bottom),
                    Size = new Size(280, 50),
                    AutoSize = true
                };

                for (int i = 1; i <= 5; i++)
                {
                    star = new Button
                    {
                        Text = "☆",
                        Size = new Size(50, 50),
                        FlatStyle = FlatStyle.Flat,
                        ForeColor = Color.Gold,
                        Font = new Font("Segoe UI", 30, FontStyle.Regular),
                        BackColor = Color.Transparent,
                        FlatAppearance = { BorderSize = 0 },
                        Tag = i
                    };
                    star.Click += (sender, e) =>
                    {
                        selectedRating = (int)((Button)sender).Tag;  // Seçilen puanı ayarla
                        foreach (Button s in starPanel.Controls)
                        {
                            s.Text = int.Parse(s.Tag.ToString()) <= selectedRating ? "★" : "☆";
                        }
                    };
                    starPanel.Controls.Add(star);
                }
                this.Controls.Add(starPanel);

                // Yorum Ekle Butonu
                Button btnYorumEkle = new Button
                {
                    Text = "Yorum Ekle",
                    Location = new Point(500, txtYorum.Bottom + 10),
                    Size = new Size(150, 40),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.FromArgb(60, 150, 90),  // Yeşil buton
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnYorumEkle.Click += BtnYorumEkle_Click;
                this.Controls.Add(btnYorumEkle);
                LoadDataFromFile2();
            }

            Button exitButton = new Button
            {
                Text = "Çıkış Yap",
                Location = new Point(this.ClientSize.Width - 110, 5), // Sağ üst köşeye yerleştiriyoruz
                Size = new Size(90, 40), // Buton boyutunu ayarlıyoruz
                Font = new Font("Arial", 10, FontStyle.Bold), // Fontu kalın yapıyoruz
                ForeColor = Color.White, // Yazı rengini beyaz yapıyoruz
                BackColor = Color.Red, // Arka plan rengini kırmızı yapıyoruz
                FlatStyle = FlatStyle.Flat, // Butonun stili düz yapılıyor
                Cursor = Cursors.Hand // Fareyi üzerine getirdiğinde el simgesi görünsün
            };
            exitButton.Click += ExitButton_Click; // Çıkış butonuna tıklandığında çalışacak olan event
            this.Controls.Add(exitButton);

            Button btnGeri = new Button
            {
                Text = "Geri",
                Location = new Point(exitButton.Left - 70, 5), // Sağ üst köşeye biraz daha sol tarafa
                Size = new Size(60, 40), // Buton boyutunu ayarlıyoruz
                Font = new Font("Arial", 12, FontStyle.Bold), // Fontu kalın yapıyoruz
                ForeColor = Color.White, // Yazı rengini beyaz yapıyoruz
                BackColor = Color.Blue, // Arka plan rengini mavi yapıyoruz
                FlatStyle = FlatStyle.Flat, // Butonun stili düz yapılıyor
                Cursor = Cursors.Hand // Fareyi üzerine getirdiğinde el simgesi görünsün
            };
            btnGeri.Click += Geri;
            this.Controls.Add(btnGeri);

            LoadDataUrunler();
        }


        private void BtnYorumSil_Click(object sender, EventArgs e)
        {
            if (dataGridViewYorumlar.SelectedRows.Count > 0)
            {
                // Seçilen satırdan Yorum ID'sini al
                int yorumId = Convert.ToInt32(dataGridViewYorumlar.SelectedRows[0].Cells["YorumId"].Value);

                // Ayrıca UrunId'yi de al
                int urunId = Convert.ToInt32(dataGridViewYorumlar.SelectedRows[0].Cells["UrunId"].Value);

                // Silme işlemini onaylat
                var confirmResult = MessageBox.Show("Seçilen yorumu silmek istediğinize emin misiniz?",
                                                    "Yorumu Sil",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM Yorumlar WHERE YorumId = @YorumID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@YorumID", yorumId);
                        command.ExecuteNonQuery();

                        // Yorum silindikten sonra, UrunId'yi de kullanarak LoadDataYorumlar fonksiyonunu çağır
                        LoadDataYorumlar(urunId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Silmek için bir yorum seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void LoadDataUrunler()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM [table]"; // Ürünlerin listeleneceği sorgu
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    dataAdapter.Fill(dataTable);
                    dataGridViewUrunler.DataSource = dataTable;

                    // ID sütununu gizle
                    if (dataGridViewUrunler.Columns.Contains("ID"))
                    {
                        dataGridViewUrunler.Columns["ID"].Visible = false;
                    }
                }
                LoadDataFromFile2();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DataGridView'de bir ürün seçildiğinde yorumları yükleme
        private void DataGridViewUrunler_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewUrunler.SelectedRows.Count > 0)
            {
                // Seçilen satırdan UrunId'yi al
                int urunId = Convert.ToInt32(dataGridViewUrunler.SelectedRows[0].Cells["ID"].Value);

                // O ürüne ait yorumları yükle
                LoadDataYorumlar(urunId);
            }
        }

        // Seçilen ürüne ait yorumları veritabanından alıp DataGridView'e yükleme
        private void LoadDataYorumlar(int urunId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Puan, Yorum, YorumId, UrunId FROM Yorumlar WHERE UrunId = @UrunId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UrunId", urunId);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    dataAdapter.Fill(dataTable);
                    dataGridViewYorumlar.DataSource = dataTable;
                    // Sonra sütun genişliklerini ayarla
                    dataGridViewYorumlar.Columns[0].Width = 50; // İlk sütunun genişliğini küçük yap
                    dataGridViewYorumlar.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridViewYorumlar.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;// İlk sütunun başlık yazısını ortala
                    dataGridViewYorumlar.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // İkinci sütun kalan alanı kaplasın
                    dataGridViewYorumlar.Columns[2].Visible = false;
                    dataGridViewYorumlar.Columns[3].Visible = false;

                }
                if (admin == false)
                {
                    dataGridViewYorumlar.ClearSelection(); // Mevcut seçimleri temizler
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Yorum ekleme fonksiyonu
        private void BtnYorumEkle_Click(object sender, EventArgs e)
        {
            if (selectedRating == 0) // Puan seçilmemişse
            {
                MessageBox.Show("Lütfen bir puan seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridViewUrunler.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir ürün seçin.");
                return;
            }

            int urunId = Convert.ToInt32(dataGridViewUrunler.SelectedRows[0].Cells["ID"].Value);
            string yorum = txtYorum.Text;
            int puan = selectedRating;

            if (string.IsNullOrEmpty(yorum))
            {
                MessageBox.Show("Lütfen yorumunuzu girin.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Yorumlar (UrunId, Yorum, Puan) VALUES (@UrunId, @Yorum, @Puan)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UrunId", urunId);
                    command.Parameters.AddWithValue("@Yorum", yorum);
                    command.Parameters.AddWithValue("@Puan", puan);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    txtYorum.Clear();
                    selectedRating = 0; // Puanı sıfırla
                    foreach (Button star in starPanel.Controls)
                    {
                        star.Text = "☆";  // Tüm yıldızları boş yap
                    }
                    LoadDataYorumlar(urunId); // Yorumları yeniden yükle

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Temizleme butonunun click event'i
        private void ClearButton_Click(object sender, EventArgs e)
        {
            // Tüm TextBox'ları temizliyoruz
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Clear();
            }
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            admin = false;
            InitializeLoginComponents();
        }


        private void SearchTextBox_TextChanged2(object sender, EventArgs e)
        {
            // Her TextBox'ı kontrol etmeden önce null olup olmadığını kontrol edelim
            if (textBoxes == null)
            {
                MessageBox.Show("TextBox dizisi null!");
                return;
            }

            // DataGridView'in DataSource'unun null olmadığından ve DataView olduğundan emin olalım
            if (dataGridView == null || dataGridView.DataSource == null || !(dataGridView.DataSource is DataView dataView))
            {
                MessageBox.Show("DataGridView'in DataSource'u bir DataView değil!");
                return;
            }

            List<string> filterConditions = new List<string>();

            for (int i = 0; i < textBoxes.Length; i++)
            {
                // Her TextBox'ın null olup olmadığını kontrol et
                if (textBoxes[i] != null)
                {
                    string textBoxText = textBoxes[i].Text.Trim(); // TextBox'tan gelen metni alıyoruz
                    if (!string.IsNullOrEmpty(textBoxText)) // Eğer TextBox boş değilse
                    {
                        // Eğer boş değilse, filtreleme koşulu ekliyoruz
                        filterConditions.Add($"{labels2[i]} LIKE '%{textBoxText}%'");
                    }
                }
            }

            // Eğer en az bir koşul varsa, RowFilter'ı uyguluyoruz
            if (filterConditions.Count > 0)
            {
                string fullFilter = string.Join(" AND ", filterConditions);
                dataView.RowFilter = fullFilter;  // DataView üzerinden filtre uyguluyoruz
            }
            else
            {
                // Eğer hiç koşul yoksa, tüm veriyi gösteriyoruz
                dataView.RowFilter = string.Empty;
            }
        }

        private void DataGridView_CellClick2(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                if (row.Cells["Resim"].Value != DBNull.Value)
                {
                    byte[] imageBytes = (byte[])row.Cells["Resim"].Value;
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox.Image = null; // Resim yoksa, Image'ı null yap
                }


                selectedRowId = Convert.ToInt32(row.Cells["ID"].Value);
            }
        }
        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string filter = searchTextBox.Text.Trim();

            // Eğer arama kutusu boşsa, filtreyi temizle
            if (string.IsNullOrEmpty(filter))
            {
                dataView.RowFilter = string.Empty;
            }
            else
            {
                // Filtreleme sorgusunu oluştur (örneğin, tüm kolonlarda arama yap)
                dataView.RowFilter = $"Marka LIKE '%{filter}%' OR " +
                                  $"Model LIKE '%{filter}%' OR " +
                                  $"Renk LIKE '%{filter}%' OR " +
                                  $"Malzeme LIKE '%{filter}%' OR " +
                                  $"Fiyat LIKE '%{filter}%' OR " +
                                  $"Agirlik LIKE '%{filter}%' OR " +
                                  $"Stok LIKE '%{filter}%' OR " +
                                  $"Garanti LIKE '%{filter}%' OR " +
                                  $"Ekstra LIKE '%{filter}%' OR " +
                                  $"Aciklama LIKE '%{filter}%'";

            }
        }
        private void LoadData2()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL sorgusu
                    string query = "SELECT * FROM [table]"; // Burada [table] yerine tablonuzun adını yazın

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    dataTable = new DataTable();


                    // Verileri al
                    dataAdapter.Fill(dataTable);

                    // DataView ile filtreleme yapabilmek için DataTable'ı bağla
                    dataView = new DataView(dataTable);
                    dataGridView.DataSource = dataView;
                    // ID sütununu gizle
                    if (dataGridView.Columns.Contains("ID"))
                    {
                        dataGridView.Columns["ID"].Visible = false; // ID sütununu gizler
                    }
                }

                LoadDataFromFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Temizle(object sender, EventArgs e)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.Clear();
            }
            selectedRowId = -1; // Seçilen satır ID'sini sıfırla
            pictureBox.Image = null;
        }
        private void Yorumlar(object sender, EventArgs e)
        {
            InitializeForm5Components();
        }
        private void istatistik(object sender, EventArgs e)
        {
            InitializeForm4Components();
        }
        private void Geri(object sender, EventArgs e)
        {
            if (admin == true)
            {
                InitializeForm2Components();
            }
            else
            {
                InitializeForm3Components();
            }
        }
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                for (int i = 0; i < labels.Length; i++)
                {
                    textBoxes[i].Text = row.Cells[i].Value.ToString().Trim();
                }

                if (row.Cells["Resim"].Value != DBNull.Value)
                {
                    byte[] imageBytes = (byte[])row.Cells["Resim"].Value;
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox.Image = null; // Resim yoksa, Image'ı null yap
                }


                selectedRowId = Convert.ToInt32(row.Cells["ID"].Value);
            }
        }


        private void BtnResimEkle_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Resim Seçin"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Resmi yükle
                Image img = Image.FromFile(openFileDialog.FileName);
                // Resmi boyutlandır
                Image resizedImg = new Bitmap(img, new Size(150, 150));  // Fotoğrafı 150x150 boyutunda sıkıştır
                pictureBox.Image = resizedImg; // PictureBox'a yükle
                resimDegisti = true;
            }
        }
        private void BtnResimSil_Click(object sender, EventArgs e)
        {
            pictureBox.Image = null;
            resimDegisti = true;
        }


        private void BtnEkle_Click(object sender, EventArgs e)
        {
            // Textbox'lardan herhangi biri boş mu kontrol et
            for (int i = 0; i < textBoxes.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(textBoxes[i].Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Eğer bir textbox boşsa, işlemi durdur
                }
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO [table] (Marka, Model, Renk, Malzeme, Fiyat, Agirlik, Stok, Garanti, Ekstra, Aciklama, Resim) " +
                               "VALUES (@Marka, @Model, @UrunRengi, @Malzeme, @Fiyat, @Agirlik, @Stok, @Garanti, @Ekstra, @Aciklama, @Resim)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    for (int i = 0; i < labels.Length; i++)
                    {
                        string paramName = labels[i]
            .Replace(" ", "")       // Boşlukları kaldır
            .Replace("ü", "u")      // Türkçe karakter dönüşümleri
            .Replace("ö", "o")
            .Replace("ğ", "g")
            .Replace("ş", "s")
            .Replace("ç", "c")
            .Replace("ı", "i")
            .Replace("İ", "I")
            .Replace("Ü", "U")      // Türkçe karakter dönüşümleri
            .Replace("Ö", "O")
            .Replace("Ğ", "G")
            .Replace("Ş", "S")
            .Replace("Ç", "C")
            .Replace("İ", "I");

                        cmd.Parameters.AddWithValue("@" + paramName, textBoxes[i].Text);
                    }

                    // Resim ekleme
                    // Eğer resim değiştiyse, resim verisini ekle
                    if (resimDegisti && pictureBox.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            try
                            {
                                pictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                cmd.Parameters.Add("@Resim", SqlDbType.VarBinary).Value = ms.ToArray();
                            }
                            catch (ArgumentNullException ex)
                            {
                                // Hata durumunda, mevcut formatı kullanıyoruz
                                Console.WriteLine("Hata: " + ex.Message);
                                pictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                cmd.Parameters.Add("@Resim", SqlDbType.VarBinary).Value = ms.ToArray();
                            }
                        }
                    }
                    else
                    {
                        // Resim yoksa NULL değeri gönderiyoruz
                        cmd.Parameters.Add("@Resim", SqlDbType.VarBinary).Value = DBNull.Value;
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            LoadData();
        }


        private void BtnDuzenle_Click(object sender, EventArgs e)
        {
            if (selectedRowId != -1)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE [table] SET Marka = @Marka, Model = @Model, Renk = @Renk, Malzeme = @Malzeme, Fiyat = @Fiyat, Agirlik = @Agirlik, Stok = @Stok, Garanti = @Garanti, Ekstra = @Ekstra, Aciklama = @Aciklama";

                    // Eğer resim değiştiyse, query'e Resim alanını ekle
                    if (resimDegisti)
                    {
                        query += ", Resim = @Resim";
                    }

                    query += " WHERE ID = @ID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Parametreleri ekle
                        string[] labels = { "Marka", "Model", "Renk", "Malzeme", "Fiyat", "Agirlik", "Stok", "Garanti", "Ekstra", "Aciklama" };

                        for (int i = 0; i < labels.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@" + labels[i], textBoxes[i].Text); // TextBox'tan değerleri al
                        }

                        cmd.Parameters.AddWithValue("@ID", selectedRowId); // ID parametresini ekle

                        // Eğer resim değiştiyse, resim verisini ekle
                        if (resimDegisti)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                try
                                {
                                    // Eğer resim null değilse, resmi kaydet
                                    if (pictureBox.Image != null)
                                    {
                                        pictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        cmd.Parameters.Add("@Resim", SqlDbType.VarBinary).Value = ms.ToArray();  // Resmi veritabanına ekle
                                    }
                                    else
                                    {
                                        // Resim null ise, null olarak ekle (veritabanında null kalır)
                                        cmd.Parameters.Add("@Resim", SqlDbType.VarBinary).Value = DBNull.Value;  // null değeri gönder
                                    }
                                }
                                catch (ArgumentNullException ex)
                                {
                                    // Hata durumunda, mevcut formatı kullanıyoruz
                                    Console.WriteLine("Hata: " + ex.Message);

                                    // Eğer hata olsa bile resim null ise, null değeri ekle
                                    cmd.Parameters.Add("@Resim", SqlDbType.VarBinary).Value = DBNull.Value;  // null değeri gönder
                                }
                            }
                        }


                        // Sorguyu çalıştır
                        cmd.ExecuteNonQuery();
                        resimDegisti = false;
                    }
                }

                // Verileri yeniden yükle ve görseli temizle
                LoadData();
                pictureBox.Image = null;
                resimDegisti = false;  // Bayrağı sıfırla
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["ID"].Value);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM [table] WHERE ID = @ID";
                        using (SqlCommand command = new SqlCommand(query, conn))
                        {
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("Kayıt başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme işlemi sırasında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }






        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL sorgusu
                    string query = "SELECT * FROM [table]"; // Burada [table] yerine tablonuzun adını yazın

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    // Verileri al
                    dataAdapter.Fill(dataTable);

                    // DataView ile filtreleme yapabilmek için DataTable'ı bağla
                    DataView dataView = new DataView(dataTable);
                    dataGridView.DataSource = dataView;

                    // ID sütununu gizle
                    if (dataGridView.Columns.Contains("ID"))
                    {
                        dataGridView.Columns["ID"].Visible = false; // ID sütununu gizler
                    }
                    dataGridView.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // Başlıkları ortala
                    }

                    LoadDataFromFile();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void InitializeForm4Components()
        {

            // Form ayarları
            this.Text = "Ürün İstatistikleri";
            this.Size = new Size(800, 580);

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );

            this.Controls.Clear(); // Tüm kontrolleri kaldır

            // Veritabanı bağlantısı
            connection = new SqlConnection(connectionString);

            // Ürün adı label


            // DataGridView: Ürünleri listeleme
            dataGridViewUrunler = new DataGridView
            {
                Location = new Point(10, 50),       // Konum ayarı
                Size = new Size(760, 300),           // Boyut ayarı
                AllowUserToAddRows = false,
                ReadOnly = true,  // Hücreleri sadece okunur yapmak
                AllowUserToResizeColumns = false, // Sütun boyutlarını değiştirmeyi engelle
                AllowUserToResizeRows = false, // Satır boyutlarını değiştirmeyi engelle
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, // Bir satır seçildiğinde tüm satır seçilsin
                RowHeadersVisible = false, // En soldaki boş sütunu gizle
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing, // Sütun başlıklarının boyutunu değiştirilememe
                MultiSelect = false, // Birden fazla satır seçilemiyor
                Font = new Font("Arial", 10), // DataGridView fontunu ayarladık
                ForeColor = Color.Black, // Yazı rengini siyah yaptık
                BackgroundColor = Color.LightGray // Arka plan rengini gri yapalım


            };

            // Ürünler DataGridView seçim değişikliği olay işleyicisi
            dataGridViewUrunler.SelectionChanged += DataGridViewUrunler_SelectionChanged2;

            // Ürünler DataGridView'i forma ekleme
            this.Controls.Add(dataGridViewUrunler);

            // İstatistikler DataGridView: İstatistikleri listeleme
            dataGridViewIstatislikler = new DataGridView
            {
                Location = new Point(10, 370),      // Konum ayarı
                Size = new Size(760, 160),           // Boyut ayarı
                AllowUserToAddRows = false,
                ReadOnly = true,  // Hücreleri sadece okunur yapmak
                AllowUserToResizeColumns = false, // Sütun boyutlarını değiştirmeyi engelle
                AllowUserToResizeRows = false, // Satır boyutlarını değiştirmeyi engelle
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, // Bir satır seçildiğinde tüm satır seçilsin
                RowHeadersVisible = false, // En soldaki boş sütunu gizle
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing, // Sütun başlıklarının boyutunu değiştirilememe
                MultiSelect = false, // Birden fazla satır seçilemiyor
                Font = new Font("Arial", 10), // DataGridView fontunu ayarladık
                ForeColor = Color.Black, // Yazı rengini siyah yaptık
                BackgroundColor = Color.LightGray // Arka plan rengini gri yapalım
            };

            // İstatistikler DataGridView'i forma ekleme
            this.Controls.Add(dataGridViewIstatislikler);

            LoadDataUrunler2();



            Button exitButton = new Button
            {
                Text = "Çıkış Yap",
                Location = new Point(this.ClientSize.Width - 102, 5), // Sağ üst köşeye yerleştiriyoruz
                Size = new Size(90, 40), // Buton boyutunu ayarlıyoruz
                Font = new Font("Arial", 10, FontStyle.Bold), // Fontu kalın yapıyoruz
                ForeColor = Color.White, // Yazı rengini beyaz yapıyoruz
                BackColor = Color.Red, // Arka plan rengini kırmızı yapıyoruz
                FlatStyle = FlatStyle.Flat, // Butonun stili düz yapılıyor
                Cursor = Cursors.Hand // Fareyi üzerine getirdiğinde el simgesi görünsün
            };
            exitButton.Click += ExitButton_Click; // Çıkış butonuna tıklandığında çalışacak olan event
            this.Controls.Add(exitButton);

            Button btnGeri = new Button
            {
                Text = "Geri",
                Location = new Point(exitButton.Left - 70, 5), // Sağ üst köşeye biraz daha sol tarafa
                Size = new Size(60, 40), // Buton boyutunu ayarlıyoruz
                Font = new Font("Arial", 12, FontStyle.Bold), // Fontu kalın yapıyoruz
                ForeColor = Color.White, // Yazı rengini beyaz yapıyoruz
                BackColor = Color.Blue, // Arka plan rengini mavi yapıyoruz
                FlatStyle = FlatStyle.Flat, // Butonun stili düz yapılıyor
                Cursor = Cursors.Hand // Fareyi üzerine getirdiğinde el simgesi görünsün
            };
            btnGeri.Click += Geri;
            this.Controls.Add(btnGeri);
            LoadDataFromFile();
        }


        // Ürün seçildiğinde istatistikleri güncelleme

        private void DataGridViewUrunler_SelectionChanged2(object sender, EventArgs e)
        {
            if (dataGridViewUrunler.SelectedRows.Count > 0)
            {
                int urunId = Convert.ToInt32(dataGridViewUrunler.SelectedRows[0].Cells["ID"].Value);

                LoadDataIstatislikler(urunId);
            }
        }
        private void LoadDataIstatislikler(int urunId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT 
                        COUNT(*) AS YorumSayisi, 
                        AVG(Puan) AS PuanOrtalamasi
                    FROM Yorumlar 
                    WHERE UrunId = @UrunId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UrunId", urunId);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    dataAdapter.Fill(dataTable);
                    dataGridViewIstatislikler.DataSource = dataTable;

                    // Başlıkları düzenleme
                    if (dataGridViewIstatislikler.Columns.Contains("YorumSayisi"))
                        dataGridViewIstatislikler.Columns["YorumSayisi"].HeaderText = "Toplam Yorum Sayısı";
                    if (dataGridViewIstatislikler.Columns.Contains("PuanOrtalamasi"))
                        dataGridViewIstatislikler.Columns["PuanOrtalamasi"].HeaderText = "Puan Ortalaması";
                    dataGridViewIstatislikler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // İki sütunu eşit şekilde kaplamak için her iki sütunu da "Fill" moduna ayarlayalım
                    dataGridViewIstatislikler.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridViewIstatislikler.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadDataUrunler2()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM [table]"; // Tüm sütunları alır
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    dataAdapter.Fill(dataTable);
                    dataGridViewUrunler.DataSource = dataTable;

                    // ID sütununu gizle
                    if (dataGridViewUrunler.Columns.Contains("ID"))
                    {
                        dataGridViewUrunler.Columns["ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeForm6Components()
        {

            // Form ayarları
            this.Text = "Sütun Düzeltme";
            this.Size = new Size(800, 350);

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
            );

            this.Controls.Clear(); // Tüm kontrolleri kaldır

            textBoxes2 = new TextBox[labels.Length];


            // Etiket ve TextBox'ları oluşturuyoruz
            for (int i = 0; i < labels.Length; i++)
            {
                // Her 5 elemanda bir sütun değişiyor
                int column = i / 5;  // 2 sütun için 5'er 5'er yerleştiriyoruz
                int row = i % 5;

                // Etiketleri yerleştiriyoruz
                Label label = new Label
                {
                    Text = labels[i],
                    Location = new Point(20 + (column * 400), 15 + (20) + (row * 45)), // Etiketlerin yeni konumu
                    Size = new Size(100, 30),
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.Black // Etiket rengini siyah yaptık
                };
                this.Controls.Add(label);

                // TextBox'ları yerleştiriyoruz
                textBoxes2[i] = new TextBox
                {
                    Location = new Point(120 + (column * 400), 15 + (20) + (row * 45)), // TextBox'ları etiketlerin yanına yerleştiriyoruz
                    Size = new Size(200, 30),
                    Font = new Font("Arial", 12),
                    ForeColor = Color.Black,
                    BackColor = Color.White, // TextBox'ların arka plan rengini beyaz yaptık
                    BorderStyle = BorderStyle.FixedSingle, // TextBox kenarlarını sabitledik
                    MaxLength = 10 // Maksimum uzunluğu 10 karakterle sınırladık
                };
                this.Controls.Add(textBoxes2[i]);
            }

            btnKaydet = new Button
            {
                Text = "Kaydet",
                Location = new Point(350, 30 + (5 * 45)),  // Buton, etiketlerin altına yerleştirilecek
                Size = new Size(100, 40),
                Font = new Font("Arial", 12),
                BackColor = Color.LightGreen,
                ForeColor = Color.Black
            };
            btnKaydet.Click += btnKaydet_Click;  // Butona tıklama olayını bağlıyoruz
            this.Controls.Add(btnKaydet);
            LoadDataFromFile();
            // Çıkış Yap Butonu Tasarımı
            Button exitButton = new Button
            {
                Text = "Çıkış Yap",
                Location = new Point(this.ClientSize.Width - 140, this.ClientSize.Height - 60),
                Size = new Size(120, 40),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);
            LoadDataFromFile();
        }
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // TextBox'lardaki değerlerle Label'ları güncelliyoruz
            for (int i = 0; i < labels.Length; i++)
            {
                // TextBox'ı boş bırakmamaya dikkat ediyoruz
                if (textBoxes2[i] != null && textBoxes2[i].Text != null)
                {
                    labels[i] = textBoxes2[i].Text; // Label'ı TextBox'ın metni ile güncelliyoruz
                }
            }

            // Verileri dosyaya kaydediyoruz
            SaveDataToFile();
            LoadDataFromFile();
        }



        private void SaveDataToFile()
        {
            // Geçerli kullanıcının Belgeler (Documents) dizinini alıyoruz
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UygulamaVerileri");

            // Eğer dizin yoksa oluşturuyoruz
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Dosyanın tam yolunu belirliyoruz
            string filePath = Path.Combine(directory, "labelData.txt");

            // Dosyayı açıp verileri kaydediyoruz
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    writer.WriteLine(labels[i]); // Her label'ı bir satıra yazıyoruz
                }
            }
        }
        private void LoadDataFromFile()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UygulamaVerileri", "labelData.txt");

            // Dosya varsa okumaya başlıyoruz
            if (File.Exists(filePath))
            {
                // Dosyadan satırları okuyoruz
                string[] fileData = File.ReadAllLines(filePath);

                // labels dizisinin başlatıldığından emin olalım
                if (labels == null || labels.Length != fileData.Length)
                {
                    labels = new string[fileData.Length];
                }

                // Dosyadaki verileri labels dizisine atıyoruz
                for (int i = 0; i < fileData.Length && i < labels.Length; i++)
                {
                    labels[i] = fileData[i]; // Dosyadan alınan her satırı labels dizisine yerleştiriyoruz
                }
            }

            // Eğer dosya bulunmazsa, Label'ların metinlerini TextBox'lara yerleştiriyoruz
            for (int i = 0; i < labels.Length; i++)
            {
                // Eğer dosya yoksa, textBoxes2 dizisinin null olup olmadığını kontrol ederek Label'ların metinlerini TextBox'lara taşıyoruz
                if (textBoxes2 != null && textBoxes2[i] != null)
                {
                    // Eğer dosyadan veri alınamadıysa, Label'daki metni TextBox'a aktar
                    textBoxes2[i].Text = (labels != null && i < labels.Length) ? labels[i] : string.Empty;
                }

                // Aynı zamanda Label'ı da güncelliyoruz
                if (i < this.Controls.OfType<Label>().Count()) // Label var mı kontrolü
                {
                    Label label = this.Controls.OfType<Label>().ElementAt(i);
                    label.Text = (labels != null && i < labels.Length) ? labels[i] : label.Text; // Eğer labelData.txt dosyasında bir veri varsa, onu kullanıyoruz
                }
            }

            // dataGridView geçerli mi kontrol edelim
            if (dataGridView != null)
            {
                for (int i = 0; i < labels.Length && i < dataGridView.Columns.Count; i++)
                {
                    // DataGridView sütun başlıklarını labels dizisine göre güncelliyoruz
                    dataGridView.Columns[i].HeaderText = labels[i];
                }
            }
            if (dataGridViewUrunler != null)
            {
                for (int i = 0; i < labels.Length && i < dataGridViewUrunler.Columns.Count; i++)
                {
                    // DataGridView sütun başlıklarını labels dizisine göre güncelliyoruz
                    dataGridViewUrunler.Columns[i].HeaderText = labels[i];
                }
            }

        }
        private void LoadDataFromFile2()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UygulamaVerileri", "labelData.txt");

            // Dosya varsa okumaya başlıyoruz
            if (File.Exists(filePath))
            {
                // Dosyadan satırları okuyoruz
                string[] fileData = File.ReadAllLines(filePath);

                // labels dizisinin başlatıldığından emin olalım
                if (labels == null || labels.Length != fileData.Length)
                {
                    labels = new string[fileData.Length];
                }

                // Dosyadaki verileri labels dizisine atıyoruz
                for (int i = 0; i < fileData.Length && i < labels.Length; i++)
                {
                    labels[i] = fileData[i]; // Dosyadan alınan her satırı labels dizisine yerleştiriyoruz
                }
            }

            // Sadece DataGridView başlıklarını güncelleyeceğiz
            if (dataGridView != null)
            {
                for (int i = 0; i < labels.Length && i < dataGridView.Columns.Count; i++)
                {
                    // DataGridView sütun başlıklarını labels dizisine göre güncelliyoruz
                    dataGridView.Columns[i].HeaderText = labels[i];
                }
            }

            if (dataGridViewUrunler != null)
            {
                for (int i = 0; i < labels.Length && i < dataGridViewUrunler.Columns.Count; i++)
                {
                    // DataGridView sütun başlıklarını labels dizisine göre güncelliyoruz
                    dataGridViewUrunler.Columns[i].HeaderText = labels[i];
                }
            }
        }

    }

}
