using System;
using System.Windows.Forms;

namespace Alg_Hill
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {

            string inputText = textBoxInput.Text;
            string key = textBoxKey.Text;

            string encryptedText = HillCipher.Encrypt(inputText, key);
            labelResult.Text = "Зашифрованный текст: " + encryptedText;
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            string inputText = textBoxInput.Text;
            string key = textBoxKey.Text;

            string decryptedText = HillCipher.Decrypt(inputText, key);
            labelResult.Text = "Расшифрованный текст: " + decryptedText;
        }
    }

    public static class HillCipher
    {
        public static string Encrypt(string text, string key)
        {
            text = text.ToUpper();
            key = key.ToUpper();

            // Проверка на валидность ключа (должна быть квадратной матрицей)
            int m = (int)Math.Sqrt(key.Length);
            if (m * m != key.Length)
            {
                throw new ArgumentException("Ключ должен быть квадратной матрицей.");
            }

            // Создание матрицы ключа
            int[,] keyMatrix = new int[m, m];
            int k = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    keyMatrix[i, j] = key[k++] - 'A';
                }
            }

            // Дополнение текста до длины, кратной размеру матрицы
            while (text.Length % m != 0)
            {
                text += 'X';
            }

            // Шифрование
            string ciphertext = "";
            for (int i = 0; i < text.Length; i += m)
            {
                // Преобразование блока текста в вектор
                int[] vector = new int[m];
                for (int j = 0; j < m; j++)
                {
                    vector[j] = text[i + j] - 'A';
                }

                // Умножение вектора на матрицу ключа
                int[] result = new int[m];
                for (int j = 0; j < m; j++)
                {
                    for (int l = 0; l < m; l++)
                    {
                        result[j] += keyMatrix[j, l] * vector[l];
                    }
                    result[j] %= 26; // Модуль 26 для букв алфавита
                }

                // Преобразование результата в буквы
                for (int j = 0; j < m; j++)
                {
                    ciphertext += (char)(result[j] + 'A');
                }
            }

            return ciphertext;
        }

        public static string Decrypt(string ciphertext, string key)
        {
            ciphertext = ciphertext.ToUpper();
            key = key.ToUpper();

            // Проверка на валидность ключа
            int m = (int)Math.Sqrt(key.Length);
            if (m * m != key.Length)
            {
                throw new ArgumentException("Ключ должен быть квадратной матрицей.");
            }

            // Создание матрицы ключа
            int[,] keyMatrix = new int[m, m];
            int k = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    keyMatrix[i, j] = key[k++] - 'A';
                }
            }

            // Нахождение обратной матрицы ключа
            int[,] inverseKeyMatrix = InverseMatrix(keyMatrix);

            // Расшифровка
            string plaintext = "";
            for (int i = 0; i < ciphertext.Length; i += m)
            {
                // Преобразование блока шифртекста в вектор
                int[] vector = new int[m];
                for (int j = 0; j < m; j++)
                {
                    vector[j] = ciphertext[i + j] - 'A';
                }

                // Умножение вектора на обратную матрицу ключа
                int[] result = new int[m];
                for (int j = 0; j < m; j++)
                {
                    for (int l = 0; l < m; l++)
                    {
                        result[j] += inverseKeyMatrix[j, l] * vector[l];
                    }
                    result[j] = (result[j] % 26 + 26) % 26; // Модуль 26 для букв алфавита
                }

                // Преобразование результата в буквы
                for (int j = 0; j < m; j++)
                {
                    plaintext += (char)(result[j] + 'A');
                }
            }

            return plaintext;
        }

        // Метод для нахождения обратной матрицы
        public static int[,] InverseMatrix(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            if (n != matrix.GetLength(1))
            {
                throw new ArgumentException("Матрица должна быть квадратной.");
            }

            // Создаем расширенную матрицу
            int[,] extendedMatrix = new int[n, 2 * n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    extendedMatrix[i, j] = matrix[i, j];
                }
                extendedMatrix[i, i + n] = 1;
            }

            // Приводим расширенную матрицу к верхнетреугольному виду
            for (int i = 0; i < n; i++)
            {
                // Поиск ведущего элемента
                int pivotRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(extendedMatrix[k, i]) > Math.Abs(extendedMatrix[pivotRow, i]))
                    {
                        pivotRow = k;
                    }
                }

                // Перестановка строк
                if (pivotRow != i)
                {
                    for (int j = 0; j < 2 * n; j++)
                    {
                        int temp = extendedMatrix[i, j];
                        extendedMatrix[i, j] = extendedMatrix[pivotRow, j];
                        extendedMatrix[pivotRow, j] = temp;
                    }
                }

                // Нормализация ведущего элемента
                int pivotValue = extendedMatrix[i, i];
                if (pivotValue != 0)
                {
                    for (int j = 0; j < 2 * n; j++)
                    {
                        extendedMatrix[i, j] /= pivotValue;
                    }
                }
                else
                {
                    throw new ArgumentException("Матрица вырожденная. Обратная матрица не существует.");
                }

                // Обнуление элементов ниже ведущего
                for (int k = i + 1; k < n; k++)
                {
                    int factor = extendedMatrix[k, i];
                    for (int j = 0; j < 2 * n; j++)
                    {
                        extendedMatrix[k, j] -= factor * extendedMatrix[i, j];
                    }
                }
            }

            // Приводим к единичной матрице
            for (int i = n - 1; i >= 0; i--)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    int factor = extendedMatrix[k, i];
                    for (int j = 0; j < 2 * n; j++)
                    {
                        extendedMatrix[k, j] -= factor * extendedMatrix[i, j];
                    }
                }
            }

            // Извлечение обратной матрицы
            int[,] inverseMatrix = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    inverseMatrix[i, j] = extendedMatrix[i, j + n];
                }
            }
            
            return inverseMatrix;
            
        }
    }
}

