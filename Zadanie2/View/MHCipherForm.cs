//Jakub Gawrysiak - 252935
//Dawid Gradowski - 251524

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Algorithm;

namespace MHCipherUI
{
    public class MHCipherForm : Form
    {
        // Sekcja: Klucze
        private Label labelPrivateKey;
        private Label labelPublicKey;
        private Label labelMultiplier;
        private Label labelModulus;
        private Label labelKeySize;

        private TextBox textBoxPrivateKey;
        private TextBox textBoxPublicKey;
        private TextBox textBoxMultiplier;
        private TextBox textBoxModulus;
        private TextBox textBoxKeySize;

        private Button buttonGenerateKeys;
        private Button buttonLoadKeys;
        private Button buttonSaveKeys;
        private Button buttonUpdatePublicKey;

        // Sekcja: Szyfrowanie / Deszyfrowanie
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

        // Format wyświetlania szyfrogramu
        private RadioButton radioFormatBinary;
        private RadioButton radioFormatHex;
        private RadioButton radioFormatNumeric;
        private GroupBox groupBoxFormat;

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
            this.Size = new Size(900, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Sekcja: Klucze
            labelKeySize = new Label { Text = "Rozmiar klucza:", AutoSize = true, Location = new Point(20, 20) };
            textBoxKeySize = new TextBox { Width = 50, Location = new Point(120, 20), Text = "8" };

            labelPrivateKey = new Label { Text = "Klucz prywatny:", AutoSize = true, Location = new Point(20, 50) };
            textBoxPrivateKey = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 60,
                Location = new Point(120, 50)
            };

            labelPublicKey = new Label { Text = "Klucz publiczny:", AutoSize = true, Location = new Point(20, 120) };
            textBoxPublicKey = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 60,
                Location = new Point(120, 120)
            };

            labelMultiplier = new Label { Text = "Mnożnik:", AutoSize = true, Location = new Point(20, 190) };
            textBoxMultiplier = new TextBox { Width = 100, Location = new Point(120, 190) };

            labelModulus = new Label { Text = "Modulus:", AutoSize = true, Location = new Point(230, 190) };
            textBoxModulus = new TextBox { Width = 100, Location = new Point(300, 190) };

            buttonGenerateKeys = new Button { Text = "Generuj nowe klucze", Width = 150, Location = new Point(500, 20) };
            buttonUpdatePublicKey = new Button { Text = "Aktualizuj klucz publiczny", Width = 150, Location = new Point(500, 60) };
            buttonLoadKeys = new Button { Text = "Wczytaj klucze", Width = 150, Location = new Point(500, 100) };
            buttonSaveKeys = new Button { Text = "Zapisz klucze", Width = 150, Location = new Point(500, 140) };

            // Sekcja: Szyfrowanie/Deszyfrowanie
            labelPlainText = new Label { Text = "Tekst jawny:", AutoSize = true, Location = new Point(20, 230) };
            textBoxPlainText = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 150,
                Location = new Point(120, 230)
            };

            labelCipherText = new Label { Text = "Szyfrogram:", AutoSize = true, Location = new Point(20, 430) };
            textBoxCipherText = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 350,
                Height = 150,
                Location = new Point(120, 430)
            };

            buttonEncrypt = new Button { Text = "Szyfruj >>", Width = 100, Location = new Point(500, 280) };
            buttonDecrypt = new Button { Text = "<< Deszyfruj", Width = 100, Location = new Point(500, 320) };

            buttonLoadPlainText = new Button { Text = "Wczytaj", Width = 80, Location = new Point(120, 390) };
            buttonSavePlainText = new Button { Text = "Zapisz", Width = 80, Location = new Point(210, 390) };

            buttonLoadCipherText = new Button { Text = "Wczytaj", Width = 80, Location = new Point(120, 590) };
            buttonSaveCipherText = new Button { Text = "Zapisz", Width = 80, Location = new Point(210, 590) };

            // Tryb pracy
            radioTextMode = new RadioButton { Text = "Tryb tekstowy", AutoSize = true, Checked = true, Location = new Point(500, 230) };
            radioBinaryMode = new RadioButton { Text = "Tryb binarny", AutoSize = true, Location = new Point(500, 250) };
            labelModeInfo = new Label
            {
                Text = "Tryb tekstowy - dla tekstu. Tryb binarny - dla dowolnych plików.",
                AutoSize = true,
                Location = new Point(500, 200)
            };

            // Format wyświetlania szyfrogramu
            groupBoxFormat = new GroupBox
            {
                Text = "Format szyfrogramu",
                Location = new Point(500, 360),
                Size = new Size(150, 100)
            };

            radioFormatBinary = new RadioButton { Text = "Binarny", AutoSize = true, Location = new Point(10, 20) };
            radioFormatHex = new RadioButton { Text = "HEX", AutoSize = true, Location = new Point(10, 40) };
            radioFormatNumeric = new RadioButton { Text = "Liczbowy", AutoSize = true, Location = new Point(10, 60), Checked = true };

            
            groupBoxFormat.Controls.Add(radioFormatNumeric);

            // Dodawanie kontrolek
            this.Controls.AddRange(new Control[] {
                labelKeySize, textBoxKeySize,
                labelPrivateKey, textBoxPrivateKey,
                labelPublicKey, textBoxPublicKey,
                labelMultiplier, textBoxMultiplier,
                labelModulus, textBoxModulus,
                buttonGenerateKeys, buttonUpdatePublicKey,
                buttonLoadKeys, buttonSaveKeys,
                labelPlainText, textBoxPlainText,
                labelCipherText, textBoxCipherText,
                buttonEncrypt, buttonDecrypt,
                buttonLoadPlainText, buttonSavePlainText,
                buttonLoadCipherText, buttonSaveCipherText,
                radioTextMode, radioBinaryMode, labelModeInfo,
                groupBoxFormat
            });
        }

        private void AttachEventHandlers()
        {
            buttonGenerateKeys.Click += (s, e) => GenerateNewKeys();
            buttonUpdatePublicKey.Click += (s, e) => UpdatePublicKey();
            buttonLoadKeys.Click += (s, e) => LoadKeys();
            buttonSaveKeys.Click += (s, e) => SaveKeys();

            buttonLoadPlainText.Click += (s, e) => OpenFile(textBoxPlainText, ref binaryPlainData);
            buttonSavePlainText.Click += (s, e) => SaveFile(textBoxPlainText, binaryPlainData);
            buttonLoadCipherText.Click += (s, e) => OpenFile(textBoxCipherText, ref binaryCipherData);
            buttonSaveCipherText.Click += (s, e) => SaveFile(textBoxCipherText, binaryCipherData);

            buttonEncrypt.Click += (s, e) =>
            {
                if (radioTextMode.Checked)
                    EncryptText();
                else
                    EncryptBinary();
            };

            buttonDecrypt.Click += (s, e) =>
            {
                if (radioTextMode.Checked)
                    DecryptText();
                else
                    DecryptBinary();
            };

            radioTextMode.CheckedChanged += (s, e) => UpdateMode();
            radioBinaryMode.CheckedChanged += (s, e) => UpdateMode();

            
            radioFormatNumeric.CheckedChanged += (s, e) => UpdateCipherTextDisplay();
        }

        private void UpdateMode()
        {
            if (radioTextMode.Checked)
            {
                textBoxPlainText.Text = "Wprowadź tekst jawny...";
                textBoxCipherText.Text = "Zaszyfrowany tekst...";
                textBoxPlainText.Enabled = true;
            }
            else
            {
                textBoxPlainText.Text = "[Dane binarne - użyj przycisku 'Wczytaj']";
                textBoxCipherText.Text = "[Dane binarne zaszyfrowane]";
                textBoxPlainText.Enabled = false;
            }
        }

        private void UpdateCipherTextDisplay()
        {
            if (binaryCipherData == null || binaryCipherData.Length == 0)
                return;

            string originalText = Encoding.UTF8.GetString(binaryCipherData);

            if (radioFormatBinary.Checked)
            {
                // Zachowaj przecinki, resztę zamień na binarny
                string[] parts = originalText.Split(',');
                StringBuilder result = new StringBuilder();
                foreach (string part in parts)
                {
                    if (result.Length > 0) result.Append(",");
                    byte[] partBytes = Encoding.UTF8.GetBytes(part);
                    result.Append(ConvertToBinaryString(partBytes));
                }
                textBoxCipherText.Text = result.ToString();
            }
            else if (radioFormatHex.Checked)
            {
                // Zachowaj przecinki, resztę zamień na HEX
                string[] parts = originalText.Split(',');
                StringBuilder result = new StringBuilder();
                foreach (string part in parts)
                {
                    if (result.Length > 0) result.Append(",");
                    byte[] partBytes = Encoding.UTF8.GetBytes(part);
                    result.Append(BitConverter.ToString(partBytes).Replace("-", " "));
                }
                textBoxCipherText.Text = result.ToString();
            }
            else if (radioFormatNumeric.Checked)
            {
                // Tryb liczbowy - oryginalny tekst z przecinkami
                textBoxCipherText.Text = originalText;
            }
        }

        private string ConvertToBinaryString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0') + " ");
            }
            return sb.ToString().Trim();
        }

        private void GenerateNewKeys()
        {
            try
            {
                int keySize = int.Parse(textBoxKeySize.Text);
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

                // Wygeneruj klucz prywatny o żądanej długości
                long[] privateKey = new long[keySize];
                Random rand = new Random();
                long sum = 0;

                for (int i = 0; i < keySize; i++)
                {
                    long next = sum + rand.Next(1, 10);
                    privateKey[i] = next;
                    sum += next;
                }

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

        private void UpdatePublicKey()
        {
            try
            {
                long[] privateKey = Array.ConvertAll(textBoxPrivateKey.Text.Split(','), long.Parse);
                long multiplier = long.Parse(textBoxMultiplier.Text);
                long modulus = long.Parse(textBoxModulus.Text);

                keyGenerator = new SimpleKeyGenerator(multiplier, modulus);
                long[] publicKey = keyGenerator.generatePublicKey(privateKey);
                textBoxPublicKey.Text = string.Join(", ", publicKey);

                mhCipher = new MHCipher(keyGenerator, privateKey);
                MessageBox.Show("Klucz publiczny został zaktualizowany!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas aktualizacji klucza publicznego: {ex.Message}");
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
                                UpdateCipherTextDisplay();
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

                binaryCipherData = Encoding.UTF8.GetBytes(cipherText);
                UpdateCipherTextDisplay();
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
                string cipherText = textBoxCipherText.Text;
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

                string base64PlainText = Convert.ToBase64String(binaryPlainData);
                string cipherText = mhCipher.Encrypt(base64PlainText);

                binaryCipherData = Encoding.UTF8.GetBytes(cipherText);
                UpdateCipherTextDisplay();
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
                    string hexString = textBoxCipherText.Text.Replace(" ", "");
                    binaryCipherData = new byte[hexString.Length / 2];
                    for (int i = 0; i < binaryCipherData.Length; i++)
                    {
                        binaryCipherData[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                }

                string cipherText = Encoding.UTF8.GetString(binaryCipherData);
                string base64PlainText = mhCipher.Decrypt(cipherText);

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