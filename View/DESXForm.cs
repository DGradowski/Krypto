using Model;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DESXUI
{
    public class DESXForm : Form
    {
        // ============================
        //  Sekcja: Klucze
        // ============================
        private Label labelKey1;
        private Label labelKey2;
        private Label labelKey3;

        private TextBox textBoxKey1;
        private TextBox textBoxKey2;
        private TextBox textBoxKey3;

        private Button buttonLoadKey1;
        private Button buttonLoadKey2;
        private Button buttonLoadKey3;
        private Button buttonSaveKey1;
        private Button buttonSaveKey2;
        private Button buttonSaveKey3;

        private Button buttonGenerateKeys;

        // ============================
        //  Sekcja: Szyfrowanie / Deszyfrowanie
        // ============================
        private Label labelOpenPlainFile;
        private TextBox textBoxOpenPlainFile;
        private Button buttonOpenPlainFile;

        private Label labelOpenCipherFile;
        private TextBox textBoxOpenCipherFile;
        private Button buttonOpenCipherFile;

        private TextBox textBoxPlainContent;
        private TextBox textBoxCipherContent;

        private Button buttonEncrypt;
        private Button buttonDecrypt;

        private RadioButton radioButtonFile;
        private RadioButton radioButtonWindow;

        private Label labelSavePlainFile;
        private TextBox textBoxSavePlainFile;
        private Button buttonSavePlainFile;

        private Label labelSaveCipherFile;
        private TextBox textBoxSaveCipherFile;
        private Button buttonSaveCipherFile;

        // Obiekt DESX
        private DESX desx;

        public DESXForm()
        {
            InitializeComponent();
            desx = new DESX();
            AttachEventHandlers();
        }

        private void InitializeComponent()
        {
            // Ustawienia głównego okna
            this.Text = "Algorytm DESX";
            this.Size = new Size(1100, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // ============================
            //  Inicjalizacja kontrolek (Klucze)
            // ============================
            labelKey1 = new Label { Text = "Wartość I klucza", AutoSize = true };
            textBoxKey1 = new TextBox { Text = "0123456789ABCDEF", Width = 150 };
            buttonLoadKey1 = new Button { Text = "Wczytaj", Width = 60 };
            buttonSaveKey1 = new Button { Text = "Zapisz", Width = 60 };

            labelKey2 = new Label { Text = "Wartość II klucza", AutoSize = true };
            textBoxKey2 = new TextBox { Text = "1133557799BBDDFF", Width = 150 };
            buttonLoadKey2 = new Button { Text = "Wczytaj", Width = 60 };
            buttonSaveKey2 = new Button { Text = "Zapisz", Width = 60 };

            labelKey3 = new Label { Text = "Wartość III klucza", AutoSize = true };
            textBoxKey3 = new TextBox { Text = "0022446688AACCEE", Width = 150 };
            buttonLoadKey3 = new Button { Text = "Wczytaj", Width = 60 };
            buttonSaveKey3 = new Button { Text = "Zapisz", Width = 60 };

            buttonGenerateKeys = new Button { Text = "Wygeneruj wartość klucza", Width = 180 };

            // ============================
            //  Inicjalizacja kontrolek (Szyfrowanie/Deszyfrowanie)
            // ============================
            labelOpenPlainFile = new Label { Text = "Otwórz plik zawierający tekst jawny", AutoSize = true };
            textBoxOpenPlainFile = new TextBox { Width = 220 };
            buttonOpenPlainFile = new Button { Text = "Otwórz", Width = 60 };

            labelOpenCipherFile = new Label { Text = "Otwórz plik zawierający szyfrogram", AutoSize = true };
            textBoxOpenCipherFile = new TextBox { Width = 220 };
            buttonOpenCipherFile = new Button { Text = "Otwórz", Width = 60 };

            textBoxPlainContent = new TextBox
            {
                Text = "Tu podaj tekst jawny",
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 300,
                Height = 150
            };
            textBoxCipherContent = new TextBox
            {
                Text = "Tu podaj szyfrogram",
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 300,
                Height = 150
            };

            buttonEncrypt = new Button { Text = "<Szyfruj>", Width = 80 };
            buttonDecrypt = new Button { Text = "<Deszyfruj>", Width = 80 };

            radioButtonFile = new RadioButton { Text = "Plik", AutoSize = true, Checked = true };
            radioButtonWindow = new RadioButton { Text = "Okno", AutoSize = true };

            labelSavePlainFile = new Label { Text = "Zapisz plik zawierający tekst jawny", AutoSize = true };
            textBoxSavePlainFile = new TextBox { Width = 220 };
            buttonSavePlainFile = new Button { Text = "Zapisz", Width = 60 };

            labelSaveCipherFile = new Label { Text = "Zapisz plik zawierający szyfrogram", AutoSize = true };
            textBoxSaveCipherFile = new TextBox { Width = 220 };
            buttonSaveCipherFile = new Button { Text = "Zapisz", Width = 60 };

            // ============================
            //  Rozmieszczenie kontrolek
            // ============================
            int marginLeft = 20;
            int currentY = 15;

            // --- Sekcja kluczy (góra okna) ---
            labelKey1.Location = new Point(marginLeft, currentY);
            textBoxKey1.Location = new Point(marginLeft + 90, currentY - 3);

            labelKey2.Location = new Point(marginLeft + 260, currentY);
            textBoxKey2.Location = new Point(marginLeft + 370, currentY - 3);

            labelKey3.Location = new Point(marginLeft + 540, currentY);
            textBoxKey3.Location = new Point(marginLeft + 650, currentY - 3);

            currentY += 30;
            buttonLoadKey1.Location = new Point(marginLeft, currentY);
            buttonSaveKey1.Location = new Point(marginLeft + 70, currentY);

            buttonLoadKey2.Location = new Point(marginLeft + 320, currentY);
            buttonSaveKey2.Location = new Point(marginLeft + 390, currentY);

            buttonLoadKey3.Location = new Point(marginLeft + 630, currentY);
            buttonSaveKey3.Location = new Point(marginLeft + 700, currentY);

            buttonGenerateKeys.Location = new Point(marginLeft + 860, currentY);

            // --- Sekcja szyfrowania/deszyfrowania (dół okna) ---
            currentY += 50;
            labelOpenPlainFile.Location = new Point(marginLeft, currentY);
            textBoxOpenPlainFile.Location = new Point(marginLeft + 210, currentY - 3);
            buttonOpenPlainFile.Location = new Point(marginLeft + 440, currentY - 5);

            labelOpenCipherFile.Location = new Point(marginLeft + 500, currentY);
            textBoxOpenCipherFile.Location = new Point(marginLeft + 700, currentY - 3);
            buttonOpenCipherFile.Location = new Point(marginLeft + 930, currentY - 5);

            currentY += 30;
            textBoxPlainContent.Location = new Point(marginLeft, currentY);
            textBoxCipherContent.Location = new Point(marginLeft + 550, currentY);

            buttonEncrypt.Location = new Point(marginLeft + 360, currentY + 40);
            buttonDecrypt.Location = new Point(marginLeft + 360, currentY + 80);

            radioButtonFile.Location = new Point(marginLeft + 365, currentY + 120);
            radioButtonWindow.Location = new Point(marginLeft + 410, currentY + 120);

            currentY += 160;
            labelSavePlainFile.Location = new Point(marginLeft, currentY);
            textBoxSavePlainFile.Location = new Point(marginLeft + 210, currentY - 3);
            buttonSavePlainFile.Location = new Point(marginLeft + 440, currentY - 5);

            labelSaveCipherFile.Location = new Point(marginLeft + 500, currentY);
            textBoxSaveCipherFile.Location = new Point(marginLeft + 700, currentY - 3);
            buttonSaveCipherFile.Location = new Point(marginLeft + 930, currentY - 5);

            // Dodawanie wszystkich kontrolek do okna
            this.Controls.Add(labelKey1);
            this.Controls.Add(textBoxKey1);
            this.Controls.Add(buttonLoadKey1);
            this.Controls.Add(buttonSaveKey1);

            this.Controls.Add(labelKey2);
            this.Controls.Add(textBoxKey2);
            this.Controls.Add(buttonLoadKey2);
            this.Controls.Add(buttonSaveKey2);

            this.Controls.Add(labelKey3);
            this.Controls.Add(textBoxKey3);
            this.Controls.Add(buttonLoadKey3);
            this.Controls.Add(buttonSaveKey3);

            this.Controls.Add(buttonGenerateKeys);

            this.Controls.Add(labelOpenPlainFile);
            this.Controls.Add(textBoxOpenPlainFile);
            this.Controls.Add(buttonOpenPlainFile);

            this.Controls.Add(labelOpenCipherFile);
            this.Controls.Add(textBoxOpenCipherFile);
            this.Controls.Add(buttonOpenCipherFile);

            this.Controls.Add(textBoxPlainContent);
            this.Controls.Add(textBoxCipherContent);

            this.Controls.Add(buttonEncrypt);
            this.Controls.Add(buttonDecrypt);

            this.Controls.Add(radioButtonFile);
            this.Controls.Add(radioButtonWindow);

            this.Controls.Add(labelSavePlainFile);
            this.Controls.Add(textBoxSavePlainFile);
            this.Controls.Add(buttonSavePlainFile);

            this.Controls.Add(labelSaveCipherFile);
            this.Controls.Add(textBoxSaveCipherFile);
            this.Controls.Add(buttonSaveCipherFile);
        }

        private void AttachEventHandlers()
        {
            // Klucze
            buttonLoadKey1.Click += (s, e) => LoadKey(textBoxKey1);
            buttonSaveKey1.Click += (s, e) => SaveKey(textBoxKey1);
            buttonLoadKey2.Click += (s, e) => LoadKey(textBoxKey2);
            buttonSaveKey2.Click += (s, e) => SaveKey(textBoxKey2);
            buttonLoadKey3.Click += (s, e) => LoadKey(textBoxKey3);
            buttonSaveKey3.Click += (s, e) => SaveKey(textBoxKey3);
            buttonGenerateKeys.Click += (s, e) => GenerateKeys();

            byte[] plainFileBytes = null;
            byte[] cipherFileBytes = null;
            // Otwarcie plików
            buttonOpenPlainFile.Click += (s, e) => plainFileBytes = OpenFile(textBoxOpenPlainFile);
            buttonOpenCipherFile.Click += (s, e) => cipherFileBytes = OpenFile(textBoxOpenCipherFile);

            // Szyfrowanie / Deszyfrowanie
            buttonEncrypt.Click += (s, e) => EncryptText();
            buttonDecrypt.Click += (s, e) => DecryptText();

            // Zapisywanie plików
            buttonSavePlainFile.Click += (s, e) => SaveFile(textBoxSavePlainFile, plainFileBytes);
            buttonSaveCipherFile.Click += (s, e) => SaveFile(textBoxSaveCipherFile, cipherFileBytes);
        }

        // ---------------------------
        // Metody pomocnicze dla kluczy
        // ---------------------------
        private void LoadKey(TextBox keyTextBox)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    keyTextBox.Text = File.ReadAllText(ofd.FileName).Trim();
                }
            }
        }

        private void SaveKey(TextBox keyTextBox)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, keyTextBox.Text);
                }
            }
        }

        private void GenerateKeys()
        {
            // Prosty generator losowych kluczy w formacie szesnastkowym (16 znaków)
            textBoxKey1.Text = GenerateRandomHexString(16);
            textBoxKey2.Text = GenerateRandomHexString(16);
            textBoxKey3.Text = GenerateRandomHexString(16);
        }

        private string GenerateRandomHexString(int length)
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(16).ToString("X"));
            }
            return sb.ToString();
        }

        // ---------------------------
        // Metody pomocnicze dla otwierania / zapisywania plików
        // ---------------------------
        private byte[] OpenFile(TextBox pathTextBox)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Wszystkie pliki (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pathTextBox.Text = ofd.FileName;
                    return File.ReadAllBytes(ofd.FileName);
                }
            }
            return null;
        }

        private void SaveFile(TextBox pathTextBox, byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Wszystkie pliki (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pathTextBox.Text = sfd.FileName;
                    File.WriteAllBytes(sfd.FileName, fileBytes);
                }
            }
        }



        public byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Nieprawidłowa długość ciągu hex.");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }


        // ---------------------------
        // Metody szyfrowania / deszyfrowania
        // ---------------------------
        private void EncryptText()
        {
            try
            {
                // Pobierz klucze z pól tekstowych (konwersja hex -> bajty)
                byte[] keyX1 = HexStringToByteArray(textBoxKey1.Text);
                byte[] desKey = HexStringToByteArray(textBoxKey2.Text);
                byte[] keyX2 = HexStringToByteArray(textBoxKey3.Text);

                // Ustaw klucze w obiekcie DESX
                desx.setKeys(keyX1, desKey, keyX2);

                // Ustaw tekst jawny i wykonaj szyfrowanie
                string plainText = textBoxPlainContent.Text;
                desx.setMsg(plainText);
                desx.prepareMsgPackages();
                desx.run(true);

                // Pobierz zaszyfrowane bajty i zakoduj je jako Base64 (lub w inny preferowany sposób)
                byte[] encryptedBytes = desx.getMsg();
                string encryptedText = Convert.ToBase64String(encryptedBytes);
                textBoxCipherContent.Text = encryptedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas szyfrowania: " + ex.Message);
            }
        }

        private void DecryptText()
        {
            try
            {
                // Pobierz klucze z pól tekstowych (konwersja hex -> bajty)
                byte[] keyX1 = HexStringToByteArray(textBoxKey1.Text);
                byte[] desKey = HexStringToByteArray(textBoxKey2.Text);
                byte[] keyX2 = HexStringToByteArray(textBoxKey3.Text);

                // Ustaw klucze w obiekcie DESX
                desx.setKeys(keyX1, desKey, keyX2);

                // Odczytaj zaszyfrowany tekst (Base64) i konwertuj na bajty
                string encryptedText = textBoxCipherContent.Text;
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

                // Ustaw zaszyfrowane bajty i wykonaj odszyfrowanie
                desx.setMsg(encryptedBytes);
                desx.prepareMsgPackages();
                desx.run(false);

                // Pobierz odszyfrowane bajty i skonwertuj na tekst ASCII
                byte[] decryptedBytes = desx.getMsg();
                string decryptedText = Encoding.ASCII.GetString(decryptedBytes);
                textBoxPlainContent.Text = decryptedText;
            }
            catch (FormatException)
            {
                MessageBox.Show("Błąd konwersji Base64. Upewnij się, że szyfrogram jest poprawnie zakodowany.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas odszyfrowywania: " + ex.Message);
            }
        }

    }
}
