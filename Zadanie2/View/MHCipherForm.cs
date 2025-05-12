using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Algorithm;

namespace MHCipherUI
{
    public class MHCipherForm : Form
    {
        // ============================
        //  Sekcja: Klucze
        // ============================
        private Label labelPrivateKey;
        private Label labelPublicKey;
        private Label labelMultiplier;
        private Label labelModulus;

        private TextBox textBoxPrivateKey;
        private TextBox textBoxPublicKey;
        private TextBox textBoxMultiplier;
        private TextBox textBoxModulus;

        private Button buttonGenerateKeys;
        private Button buttonLoadKeys;
        private Button buttonSaveKeys;

        // ============================
        //  Sekcja: Szyfrowanie / Deszyfrowanie
        // ============================
        private Label labelPlainText;
        private Label labelCipherText;

        private TextBox textBoxPlainText;
        private TextBox textBoxCipherText;

        private Button buttonEncrypt;
        private Button buttonDecrypt;

        private Button buttonSavePlainText;
        private Button buttonSaveCipherText;
        private Button buttonLoadPlainText;
        private Button buttonLoadCipherText;

        // Tryb pracy
        private RadioButton radioTextMode;
        private RadioButton radioBinaryMode;
        private Label labelModeInfo;

        // Dane binarne
        private byte[] binaryPlainData;
        private byte[] binaryCipherData;

        // Obiekt MHCipher
        private MHCipher mhCipher;
        private SimpleKeyGenerator keyGenerator;

        public MHCipherForm()
        {
            InitializeComponent();
            keyGenerator = new SimpleKeyGenerator();
            GenerateNewKeys();
            AttachEventHandlers();
            UpdateMode();
        }

        private void InitializeComponent()
        {
            // Ustawienia głównego okna
            this.Text = "Algorytm Plecakowy (Merkle-Hellman)";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // ============================
            //  Inicjalizacja kontrolek (Klucze)
            // ============================
            labelPrivateKey = new Label { Text = "Klucz prywatny:", AutoSize = true, Location = new Point(20, 20) };
            textBoxPrivateKey = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 60,
                Location = new Point(140, 20)
            };

            labelPublicKey = new Label { Text = "Klucz publiczny:", AutoSize = true, Location = new Point(20, 90) };
            textBoxPublicKey = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 60,
                Location = new Point(140, 90)
            };

            labelMultiplier = new Label { Text = "Mnożnik:", AutoSize = true, Location = new Point(20, 160) };
            textBoxMultiplier = new TextBox { Width = 100, Location = new Point(140, 160) };

            labelModulus = new Label { Text = "Modulus:", AutoSize = true, Location = new Point(240, 160) };
            textBoxModulus = new TextBox { Width = 100, Location = new Point(320, 160) };

            buttonGenerateKeys = new Button { Text = "Generuj nowe klucze", Width = 150, Location = new Point(500, 20) };
            buttonLoadKeys = new Button { Text = "Wczytaj klucze", Width = 150, Location = new Point(500, 60) };
            buttonSaveKeys = new Button { Text = "Zapisz klucze", Width = 150, Location = new Point(500, 100) };

            // ============================
            //  Inicjalizacja kontrolek (Szyfrowanie/Deszyfrowanie)
            // ============================
            labelPlainText = new Label { Text = "Tekst jawny:", AutoSize = true, Location = new Point(20, 200) };
            textBoxPlainText = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 150,
                Location = new Point(140, 200)
            };

            labelCipherText = new Label { Text = "Tekst zaszyfrowany:", AutoSize = true, Location = new Point(20, 400) };
            textBoxCipherText = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 150,
                Location = new Point(140, 400)
            };

            buttonEncrypt = new Button { Text = "Szyfruj >>", Width = 100, Location = new Point(500, 250) };
            buttonDecrypt = new Button { Text = "<< Deszyfruj", Width = 100, Location = new Point(500, 290) };

            buttonLoadPlainText = new Button { Text = "Wczytaj", Width = 80, Location = new Point(140, 360) };
            buttonSavePlainText = new Button { Text = "Zapisz", Width = 80, Location = new Point(210, 360) };

            buttonLoadCipherText = new Button { Text = "Wczytaj", Width = 80, Location = new Point(140, 560) };
            buttonSaveCipherText = new Button { Text = "Zapisz", Width = 80, Location = new Point(210, 560) };

            // Tryb pracy
            radioTextMode = new RadioButton { Text = "Tryb tekstowy", AutoSize = true, Checked = true, Location = new Point(500, 200) };
            radioBinaryMode = new RadioButton { Text = "Tryb binarny", AutoSize = true, Location = new Point(500, 220) };
            labelModeInfo = new Label
            {
                Text = "Tryb tekstowy - dla tekstu. Tryb binarny - dla dowolnych plików.",
                AutoSize = true,
                Location = new Point(500, 180)
            };

            // Dodawanie wszystkich kontrolek do okna
            this.Controls.Add(labelPrivateKey);
            this.Controls.Add(textBoxPrivateKey);
            this.Controls.Add(labelPublicKey);
            this.Controls.Add(textBoxPublicKey);
            this.Controls.Add(labelMultiplier);
            this.Controls.Add(textBoxMultiplier);
            this.Controls.Add(labelModulus);
            this.Controls.Add(textBoxModulus);
            this.Controls.Add(buttonGenerateKeys);
            this.Controls.Add(buttonLoadKeys);
            this.Controls.Add(buttonSaveKeys);

            this.Controls.Add(labelPlainText);
            this.Controls.Add(textBoxPlainText);
            this.Controls.Add(labelCipherText);
            this.Controls.Add(textBoxCipherText);

            this.Controls.Add(buttonEncrypt);
            this.Controls.Add(buttonDecrypt);

            this.Controls.Add(buttonLoadPlainText);
            this.Controls.Add(buttonSavePlainText);
            this.Controls.Add(buttonLoadCipherText);
            this.Controls.Add(buttonSaveCipherText);

            this.Controls.Add(radioTextMode);
            this.Controls.Add(radioBinaryMode);
            this.Controls.Add(labelModeInfo);
        }

        private void AttachEventHandlers()
        {
            // Klucze
            buttonGenerateKeys.Click += (s, e) => GenerateNewKeys();
            buttonLoadKeys.Click += (s, e) => LoadKeys();
            buttonSaveKeys.Click += (s, e) => SaveKeys();

            // Tekst
            buttonLoadPlainText.Click += (s, e) => OpenFile(textBoxPlainText, ref binaryPlainData);
            buttonSavePlainText.Click += (s, e) => SaveFile(textBoxPlainText, binaryPlainData);
            buttonLoadCipherText.Click += (s, e) => OpenFile(textBoxCipherText, ref binaryCipherData);
            buttonSaveCipherText.Click += (s, e) => SaveFile(textBoxCipherText, binaryCipherData);

            // Szyfrowanie / Deszyfrowanie
            buttonEncrypt.Click += (s, e) =>
            {
                if (radioTextMode.Checked) EncryptText();
                else EncryptBinary();
            };

            buttonDecrypt.Click += (s, e) =>
            {
                if (radioTextMode.Checked) DecryptText();
                else DecryptBinary();
            };

            // Tryb pracy
            radioTextMode.CheckedChanged += (s, e) => UpdateMode();
            radioBinaryMode.CheckedChanged += (s, e) => UpdateMode();
        }

        private void UpdateMode()
        {
            if (radioTextMode.Checked)
            {
                textBoxPlainText.Text = "Wprowadź tekst jawny...";
                textBoxCipherText.Text = "Zaszyfrowany tekst w HEX...";
                textBoxPlainText.Enabled = true;
            }
            else
            {
                textBoxPlainText.Text = "[Dane binarne - użyj przycisku 'Wczytaj']";
                textBoxCipherText.Text = "[Dane binarne zaszyfrowane - HEX]";
                textBoxPlainText.Enabled = false;
            }
        }

        private void GenerateNewKeys()
        {
            try
            {
                long multiplier, modulus;
                if (long.TryParse(textBoxMultiplier.Text, out multiplier) &&
                    long.TryParse(textBoxModulus.Text, out modulus))
                {
                    keyGenerator = new SimpleKeyGenerator(multiplier, modulus);
                }
                else
                {
                    keyGenerator = new SimpleKeyGenerator();
                }

                long[] privateKey = keyGenerator.generateDefaultPrivateKey();
                long[] publicKey = keyGenerator.generatePublicKey(privateKey);

                mhCipher = new MHCipher(keyGenerator, privateKey);

                UpdateKeyTextBoxes(privateKey, publicKey);
                UpdateModulusAndMultiplier();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas generowania kluczy: {ex.Message}");
            }
        }

        private void UpdateKeyTextBoxes(long[] privateKey, long[] publicKey)
        {
            textBoxPrivateKey.Text = string.Join(", ", privateKey);
            textBoxPublicKey.Text = string.Join(", ", publicKey);
        }

        private void UpdateModulusAndMultiplier()
        {
            textBoxMultiplier.Text = keyGenerator.multiplier.ToString();
            textBoxModulus.Text = keyGenerator.modulus.ToString();
        }

        private void LoadKeys()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(ofd.FileName);
                        if (lines.Length >= 4)
                        {
                            textBoxPrivateKey.Text = lines[0];
                            textBoxPublicKey.Text = lines[1];
                            textBoxMultiplier.Text = lines[2];
                            textBoxModulus.Text = lines[3];

                            // Reinitialize cipher with loaded keys
                            long[] privateKey = Array.ConvertAll(lines[0].Split(','), long.Parse);
                            keyGenerator = new SimpleKeyGenerator(
                                long.Parse(lines[2]),
                                long.Parse(lines[3]));

                            mhCipher = new MHCipher(keyGenerator, privateKey);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas wczytywania kluczy: " + ex.Message);
                    }
                }
            }
        }

        private void SaveKeys()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] lines = {
                            textBoxPrivateKey.Text,
                            textBoxPublicKey.Text,
                            textBoxMultiplier.Text,
                            textBoxModulus.Text
                        };
                        File.WriteAllLines(sfd.FileName, lines);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas zapisywania kluczy: " + ex.Message);
                    }
                }
            }
        }

        private void OpenFile(TextBox targetTextBox, ref byte[] binaryData)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Wszystkie pliki (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        binaryData = File.ReadAllBytes(ofd.FileName);

                        if (radioTextMode.Checked)
                        {
                            targetTextBox.Text = Encoding.UTF8.GetString(binaryData);
                        }
                        else
                        {
                            if (targetTextBox == textBoxPlainText)
                            {
                                targetTextBox.Text = $"[Dane binarne - rozmiar: {binaryData.Length} bajtów]";
                            }
                            else
                            {
                                // Dla szyfrogramu pokazujemy HEX
                                targetTextBox.Text = BitConverter.ToString(binaryData).Replace("-", " ");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas wczytywania pliku: " + ex.Message);
                    }
                }
            }
        }

        private void SaveFile(TextBox sourceTextBox, byte[] binaryData)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Wszystkie pliki (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (binaryData != null && binaryData.Length > 0)
                        {
                            File.WriteAllBytes(sfd.FileName, binaryData);
                            MessageBox.Show("Plik został zapisany pomyślnie!");
                        }
                        else if (radioTextMode.Checked)
                        {
                            File.WriteAllText(sfd.FileName, sourceTextBox.Text);
                            MessageBox.Show("Plik został zapisany pomyślnie!");
                        }
                        else
                        {
                            MessageBox.Show("Brak danych do zapisania.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas zapisywania pliku: " + ex.Message);
                    }
                }
            }
        }

        private void EncryptText()
        {
            try
            {
                string plainText = textBoxPlainText.Text;
                string cipherText = mhCipher.Encrypt(plainText);

                // Konwersja na format HEX
                byte[] cipherBytes = Encoding.UTF8.GetBytes(cipherText);
                textBoxCipherText.Text = BitConverter.ToString(cipherBytes).Replace("-", " ");
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
                // Konwersja z HEX na string
                string hexString = textBoxCipherText.Text.Replace(" ", "");
                byte[] cipherBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < cipherBytes.Length; i++)
                {
                    cipherBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                string cipherText = Encoding.UTF8.GetString(cipherBytes);

                string plainText = mhCipher.Decrypt(cipherText);
                textBoxPlainText.Text = plainText;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas deszyfrowania: " + ex.Message);
            }
        }

        private void EncryptBinary()
        {
            try
            {
                if (binaryPlainData == null || binaryPlainData.Length == 0)
                {
                    MessageBox.Show("Najpierw wczytaj plik do zaszyfrowania.");
                    return;
                }

                // Konwersja bajtów na string (Base64) i szyfrowanie
                string base64PlainText = Convert.ToBase64String(binaryPlainData);
                string cipherText = mhCipher.Encrypt(base64PlainText);

                // Konwersja wyniku na bajty i zapis
                binaryCipherData = Encoding.UTF8.GetBytes(cipherText);
                textBoxCipherText.Text = BitConverter.ToString(binaryCipherData).Replace("-", " ");

                MessageBox.Show("Plik został zaszyfrowany pomyślnie!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas szyfrowania: " + ex.Message);
            }
        }

        private void DecryptBinary()
        {
            try
            {
                if (binaryCipherData == null || binaryCipherData.Length == 0)
                {
                    // Spróbuj przekonwertować z HEX, jeśli nie ma danych binarnych
                    string hexString = textBoxCipherText.Text.Replace(" ", "");
                    binaryCipherData = new byte[hexString.Length / 2];
                    for (int i = 0; i < binaryCipherData.Length; i++)
                    {
                        binaryCipherData[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                }

                // Konwersja bajtów na string i deszyfrowanie
                string cipherText = Encoding.UTF8.GetString(binaryCipherData);
                string base64PlainText = mhCipher.Decrypt(cipherText);

                // Konwersja Base64 z powrotem na bajty
                binaryPlainData = Convert.FromBase64String(base64PlainText);
                textBoxPlainText.Text = $"[Odszyfrowane dane binarne - rozmiar: {binaryPlainData.Length} bajtów]";

                MessageBox.Show("Plik został odszyfrowany pomyślnie!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas deszyfrowania: " + ex.Message);
            }
        }
    }
}