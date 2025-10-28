using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int N = 1;
        int i = 0, j = 0;
        int Change;
        double[,] A = new double[6, 6];
        double[] B = new double[6];
        double[] X = new double[6];

        public Form1()
        {
            InitializeComponent();
        }

        private void Decomp(int N, ref int Change)
        {
            const double EPS = 1e-12;

            if (N == 1)
            {
                if (Math.Abs(A[1, 1]) < EPS)
                    throw new InvalidOperationException("Елемент A[1,1] approximately 0. Матриця вироджена.");
                Change = 1;
                if (C_matrix_dgv.Rows.Count > 0 && C_matrix_dgv.Columns.Count > 0)
                    C_matrix_dgv[0, 0].Value = Convert.ToString(Math.Round(A[1, 1], 4));
                return;
            }

            int i, j, k;
            double s;
            Change = 1;
            double max = Math.Abs(A[1, 1]);
            int imax = 1;

            for (i = 2; i <= N; i++)
            {
                if (Math.Abs(A[i, 1]) > max)
                {
                    max = Math.Abs(A[i, 1]);
                    imax = i;
                }
            }

            if (max < EPS)
                throw new InvalidOperationException("Усі елементи першого стовпця approximately 0. Матриця вироджена.");

            if (imax != 1)
            {
                Change = imax;
                for (j = 1; j <= N; j++)
                {
                    double temp = A[1, j];
                    A[1, j] = A[imax, j];
                    A[imax, j] = temp;
                }
            }

            for (i = 2; i <= N; i++)
            {
                for (j = 1; j < i; j++)
                {
                    if (Math.Abs(A[j, j]) < EPS)
                        throw new InvalidOperationException($"Елемент A[{j},{j}] approximately 0 під час обчислення L.");
                    s = 0;
                    for (k = 1; k <= j - 1; k++)
                        s += A[i, k] * A[k, j];
                    A[i, j] = (A[i, j] - s) / A[j, j];
                }
                for (j = i; j <= N; j++)
                {
                    s = 0;
                    for (k = 1; k <= i - 1; k++)
                        s += A[i, k] * A[k, j];
                    A[i, j] = A[i, j] - s;
                }
            }

            for (i = 0; i < N; i++)
                for (j = 0; j < N; j++)
                    C_matrix_dgv.Rows[i].Cells[j].Value = Convert.ToString(Math.Round(A[i + 1, j + 1], 4));
        }

        private void Solve(int Change, int N)
        {
            if (N == 1)
            {
                X[1] = B[1] / A[1, 1];
                if (X_vector_dgv.Rows.Count > 0)
                    X_vector_dgv[0, 0].Value = Math.Round(X[1], 4);
                return;
            }

            int i, j;
            double s;
            double[] y = new double[6];

            if (Change != 1)
            {
                double temp = B[1];
                B[1] = B[Change];
                B[Change] = temp;
            }

            y[1] = B[1];
            for (i = 2; i <= N; i++)
            {
                s = 0;
                for (j = 1; j <= i - 1; j++)
                    s += A[i, j] * y[j];
                y[i] = B[i] - s;
            }

            X[N] = y[N] / A[N, N];
            for (i = N - 1; i >= 1; i--)
            {
                s = 0;
                for (j = i + 1; j <= N; j++)
                    s += A[i, j] * X[j];
                X[i] = (y[i] - s) / A[i, i];
            }

            for (i = 0; i < N; i++)
                X_vector_dgv[0, i].Value = Math.Round(X[i + 1], 4);
        }

        private double[] GaussSolve(double[,] A, double[] B, int N)
        {
            const double EPS = 1e-12;
            double[] result = new double[N];

            if (N == 1)
            {
                if (Math.Abs(A[0, 0]) < EPS)
                    throw new InvalidOperationException("Елемент A[0,0] approximately 0. Матриця вироджена.");
                result[0] = B[0] / A[0, 0];
                return result;
            }

            for (int k = 0; k < N - 1; k++)
            {
                if (Math.Abs(A[k, k]) < EPS)
                    throw new InvalidOperationException($"Головний елемент A[{k},{k}] approximately 0. Метод нестабільний.");

                for (int i = k + 1; i < N; i++)
                {
                    double factor = A[i, k] / A[k, k];
                    for (int j = k; j < N; j++)
                        A[i, j] -= factor * A[k, j];
                    B[i] -= factor * B[k];
                }
            }

            for (int i = N - 1; i >= 0; i--)
            {
                if (Math.Abs(A[i, i]) < EPS)
                    throw new InvalidOperationException($"Діагональний елемент A[{i},{i}] approximately 0 після виключення.");

                double sum = 0;
                for (int j = i + 1; j < N; j++)
                    sum += A[i, j] * result[j];
                result[i] = (B[i] - sum) / A[i, i];
            }

            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            X_vector_dgv.ReadOnly = true;
            A_matrix_dgv.AllowUserToAddRows = false;
            B_vector_dgv.AllowUserToAddRows = false;
            X_vector_dgv.AllowUserToAddRows = false;
            A_matrix_dgv.ColumnCount = 1;
            A_matrix_dgv.RowCount = 1;
            X_vector_dgv.ColumnCount = 1;
            X_vector_dgv.RowCount = 1;
            B_vector_dgv.ColumnCount = 1;
            B_vector_dgv.RowCount = 1;
            MethodSelector.Items.Clear();
            MethodSelector.Items.Add("Метод Гауса");
            MethodSelector.Items.Add("LU-перетворення");
            MethodSelector.SelectedIndex = 0;
        }

        private void NUD_rozmir_ValueChanged(object sender, EventArgs e)
        {
            N = Convert.ToInt16(NUD_rozmir.Value);

            A_matrix_dgv.Rows.Clear();
            B_vector_dgv.Rows.Clear();
            X_vector_dgv.Rows.Clear();
            C_matrix_dgv.Rows.Clear();

            A_matrix_dgv.RowCount = N;
            A_matrix_dgv.ColumnCount = N;
            B_vector_dgv.RowCount = N;
            X_vector_dgv.RowCount = N;
            C_matrix_dgv.RowCount = N;
            C_matrix_dgv.ColumnCount = N;

            foreach (DataGridViewRow row in A_matrix_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";

            foreach (DataGridViewRow row in B_vector_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";

            foreach (DataGridViewRow row in X_vector_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";

            foreach (DataGridViewRow row in C_matrix_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";
        }

        private void BСreateGrid_Click(object sender, EventArgs e)
        {
            bool exc_A = false;
            bool exc_B = false;

            for (i = 1; i <= N; i++)
            {
                for (j = 1; j <= N; j++)
                {
                    var cell = A_matrix_dgv[j - 1, i - 1];
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        cell.Style.ForeColor = Color.Red;
                        exc_A = true;
                    }
                    else
                    {
                        try
                        {
                            A[i, j] = Convert.ToDouble(cell.Value);
                        }
                        catch
                        {
                            cell.Style.ForeColor = Color.Red;
                            exc_A = true;
                        }
                    }
                }
            }

            for (j = 0; j < N; j++)
            {
                var cell = B_vector_dgv[0, j];
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    cell.Style.ForeColor = Color.Red;
                    exc_B = true;
                }
                else
                {
                    try
                    {
                        B[j + 1] = Convert.ToDouble(cell.Value);
                    }
                    catch
                    {
                        cell.Style.ForeColor = Color.Red;
                        exc_B = true;
                    }
                }
            }

            if (exc_A || exc_B)
            {
                MessageBox.Show("Помилка введення!");
                return;
            }

            string method = MethodSelector.SelectedItem.ToString();

            try
            {
                foreach (DataGridViewRow row in C_matrix_dgv.Rows)
                    foreach (DataGridViewCell cell in row.Cells)
                        cell.Value = "";

                if (method == "LU-перетворення")
                {
                    Decomp(N, ref Change);
                    Solve(Change, N);
                }
                else if (method == "Метод Гауса")
                {
                    double[,] A_copy = new double[N, N];
                    double[] B_copy = new double[N];

                    for (int r = 0; r < N; r++)
                    {
                        for (int c = 0; c < N; c++)
                            A_copy[r, c] = A[r + 1, c + 1];
                        B_copy[r] = B[r + 1];
                    }

                    double[] X_g = GaussSolve(A_copy, B_copy, N);
                    for (int k = 0; k < N; k++)
                        X_vector_dgv[0, k].Value = Math.Round(X_g[k], 4);
                }

                MessageBox.Show("Розв’язок знайдено");
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Помилка: індекс поза межами. Перевірте розмірність.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Матриця некоректна", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Невідома помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BClear_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in A_matrix_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";

            foreach (DataGridViewRow row in C_matrix_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";

            foreach (DataGridViewRow row in B_vector_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";

            foreach (DataGridViewRow row in X_vector_dgv.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";
        }

        private void BClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void A_matrix_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            A_matrix_dgv.CurrentCell.Style.ForeColor = Color.Black;
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void B_vector_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            B_vector_dgv.CurrentCell.Style.ForeColor = Color.Black;
        }
    }
}