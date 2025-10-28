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
                    C_matrix_dgv.Rows[i].Cells[j].Value =
                        Convert.ToString(Math.Round(A[i + 1, j + 1], 4));
        }

        private void Solve(int Change, int N)
        {
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
            for (int k = 0; k < N - 1; k++)
            {
                for (int i = k + 1; i < N; i++)
                {
                    double factor = A[i, k] / A[k, k];
                    for (int j = k; j < N; j++)
                        A[i, j] -= factor * A[k, j];
                    B[i] -= factor * B[k];
                }
            }

            double[] X = new double[N];
            for (int i = N - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < N; j++)
                    sum += A[i, j] * X[j];
                X[i] = (B[i] - sum) / A[i, i];
            }

            return X;
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
            A_matrix_dgv.RowCount = N;
            A_matrix_dgv.ColumnCount = N;
            X_vector_dgv.RowCount = N;
            B_vector_dgv.RowCount = N;
            C_matrix_dgv.RowCount = N;
            C_matrix_dgv.ColumnCount = N;
        }

        private void BСreateGrid_Click(object sender, EventArgs e)
        {
            bool exc_A = false;
            bool exc_B = false;

            for (i = 1; i <= N; i++)
            {
                for (j = 1; j <= N; j++)
                {
                    try
                    {
                        A[i, j] = Convert.ToDouble(A_matrix_dgv[j - 1, i - 1].Value);
                    }
                    catch
                    {
                        A_matrix_dgv[j - 1, i - 1].Style.ForeColor = Color.Red;
                        exc_A = true;
                    }
                }
            }

            for (j = 0; j < N; j++)
            {
                try
                {
                    B[j + 1] = Convert.ToDouble(B_vector_dgv[0, j].Value);
                }
                catch
                {
                    B_vector_dgv[0, j].Style.ForeColor = Color.Red;
                    exc_B = true;
                }
            }

            if (exc_A || exc_B)
            {
                MessageBox.Show("Помилка введення!");
                return;
            }

            string method = MethodSelector.SelectedItem.ToString();

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
                        A_copy[r, c] = Convert.ToDouble(A_matrix_dgv[c, r].Value);
                    B_copy[r] = Convert.ToDouble(B_vector_dgv[0, r].Value);
                }

                double[] X_g = GaussSolve(A_copy, B_copy, N);

                for (int k = 0; k < N; k++)
                    X_vector_dgv[0, k].Value = Math.Round(X_g[k], 4);
            }

            MessageBox.Show("Розв’язок знайдено");
        }

        private void BClear_Click(object sender, EventArgs e)
        {
            for (i = 0; i < N; i++)
                for (j = 0; j < N; j++)
                {
                    A_matrix_dgv[j, i].Value = "";
                    C_matrix_dgv[j, i].Value = "";
                }

            for (j = 0; j < N; j++)
            {
                B_vector_dgv[0, j].Value = "";
                X_vector_dgv[0, j].Value = "";
            }
        }

        private void BClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void A_matrix_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            A_matrix_dgv.CurrentCell.Style.ForeColor = Color.Black;
        }

        private void B_vector_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            B_vector_dgv.CurrentCell.Style.ForeColor = Color.Black;
        }
    }
}
