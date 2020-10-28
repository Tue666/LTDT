using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOT_LTĐT
{
    public partial class Form1 : Form
    {
        Graphics g;
        int vertex;
        int[,] matrix;
        int[] x = { 551, 871, 551, 731, 231, 371, 231, 871, 371, 731 };
        int[] y = { 104, 404, 584, 154, 404, 154, 274, 274, 543, 534 };
        Pen grayPen = new Pen(Color.Gray, 2);
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
        }

        #region Methods
        void loadComboboxVertex()
        {
            int[] arrVertex = new int[vertex];
            for (int i = 0; i < vertex; i++)
            {
                arrVertex[i] = i + 1;
            }
            cbVertex.DataSource = arrVertex;
        }
        void readMatrix(string fileName)
        {
            StreamReader read = new StreamReader(fileName);
            txbMatrix.Text = read.ReadToEnd();
            string[] lines = File.ReadAllLines(fileName);
            vertex = int.Parse(lines[0]);
            matrix = new int[vertex, vertex];
            int n = 1;
            for (int i = 0; i < vertex; i++)
            {
                string[] elements = lines[n++].Split(' ');
                for (int j = 0; j < vertex; j++)
                {
                    matrix[i, j] = int.Parse(elements[j]);
                }
            }
            loadComboboxVertex();
            read.Close();
        }
        void writeMatrix(int n, int[,] a, string fileName)
        {
            StreamWriter write = new StreamWriter(fileName);
            write.WriteLine(n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    write.Write(a[i, j] + " ");
                }
                if (i < n - 1) write.WriteLine();
            }
            write.Close();
        }
        void loadGraph()
        {
            this.Refresh();
            Point[] point = new Point[vertex];
            for (int i = 0; i < vertex; i++)
            {
                point[i] = new Point(x[i] - 1, y[i] - 14);
                Point drawPoint = new Point(x[i], y[i]);
                g.FillEllipse(new SolidBrush(Color.LightBlue), x[i] - 1, y[i] - 14, 13, 13);
                g.DrawString((i + 1).ToString(), new Font("Arial", 12), new SolidBrush(Color.Black), drawPoint);
            }
            for (int i = 0; i < vertex; i++)
            {
                for (int j = 0; j < vertex; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        g.DrawString(matrix[i, j].ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), ((x[i] - 1) + (x[j] - 1)) / 2, ((y[i] - 14) + (y[j] - 14)) / 2);
                        g.DrawLine(grayPen, point[i], point[j]);
                    }
                }
            }
        }
        bool addEdge(int edgeFrom, int edgeTo, int Weight)
        {
            Point pointFrom = new Point(x[edgeFrom - 1] - 1, y[edgeFrom - 1] - 14);
            Point pointTo = new Point(x[edgeTo - 1] - 1, y[edgeTo - 1] - 14);
            g.DrawLine(grayPen, pointFrom, pointTo);
            g.DrawString(Weight.ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), ((x[edgeFrom - 1] - 1) + (x[edgeTo - 1] - 1)) / 2, ((y[edgeFrom - 1] - 14) + (y[edgeTo - 1] - 14)) / 2);
            matrix[edgeFrom - 1, edgeTo - 1] = Weight;
            writeMatrix(vertex, matrix, "Matrix.txt");
            readMatrix("Matrix.txt");
            return true;
        }
        bool deleteEdge(int edgeFrom, int edgeTo)
        {
            matrix[edgeFrom - 1, edgeTo - 1] = 0;
            writeMatrix(vertex, matrix, "Matrix.txt");
            readMatrix("Matrix.txt");
            loadGraph();
            return true;
        }
        bool changeWeight(int edgeFrom, int edgeTo, int newWeight)
        {
            matrix[edgeFrom - 1, edgeTo - 1] = newWeight;
            writeMatrix(vertex, matrix, "Matrix.txt");
            readMatrix("Matrix.txt");
            loadGraph();
            return true;
        }
        bool addVertex()
        {
            int saveVertex = vertex;
            int[,] saveMatrix = matrix;
            vertex = vertex + 1;
            matrix = new int[vertex, vertex];
            for (int i = 0; i < saveVertex; i++)
            {
                for (int j = 0; j < saveVertex; j++)
                {
                    matrix[i, j] = saveMatrix[i, j];
                }
            }
            for (int i = 0; i < vertex; i++)
            {
                matrix[i, vertex - 1] = 0;
                matrix[vertex - 1, i] = 0;
            }
            writeMatrix(vertex, matrix, "Matrix.txt");
            readMatrix("Matrix.txt");
            loadGraph();
            return true;
        }
        bool deleteVertex(int _vertex)
        {
            int nRow = vertex;
            int nColumn = vertex;
            //Delete Row
            for (int i = _vertex - 1; i < nRow - 1; i++)
            {
                for (int j = 0; j < nColumn; j++)
                {
                    matrix[i, j] = matrix[i + 1, j];
                }
            }
            nRow--;
            //Delete Column
            for (int i = 0; i < nRow; i++)
            {
                for (int j = _vertex - 1; j < nColumn - 1; j++)
                {
                    matrix[i, j] = matrix[i, j + 1];
                }
            }
            nColumn--;
            int[,] saveMatrix = matrix;
            vertex = vertex - 1;
            matrix = new int[vertex, vertex];
            for (int i = 0; i < vertex; i++)
            {
                for (int j = 0; j < vertex; j++)
                {
                    matrix[i, j] = saveMatrix[i, j];
                }
            }
            writeMatrix(vertex, matrix, "Matrix.txt");
            readMatrix("Matrix.txt");
            loadGraph();
            return true;
        }
        #endregion

        #region Events
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                readMatrix(open.FileName);
            }
        }

        private void btnDrawGraph_Click(object sender, EventArgs e)
        {
            loadGraph();
        }

        private void btnAddEdge_Click(object sender, EventArgs e)
        {
            if (addEdge(int.Parse(txbEdgeFrom.Text), int.Parse(txbEdgeTo.Text), int.Parse(txbWeight.Text))){
                MessageBox.Show("Thêm cạnh thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnDeleteEdge_Click(object sender, EventArgs e)
        {
            if (deleteEdge(int.Parse(txbEdgeFrom.Text), int.Parse(txbEdgeTo.Text)))
            {
                MessageBox.Show("Xóa cạnh thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnChangeWeight_Click(object sender, EventArgs e)
        {
            if (changeWeight(int.Parse(txbEdgeFrom.Text), int.Parse(txbEdgeTo.Text), int.Parse(txbWeight.Text)))
            {
                MessageBox.Show("Thay đối trọng số thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnAddVertex_Click(object sender, EventArgs e)
        {
            if (addVertex())
            {
                MessageBox.Show("Thêm đỉnh thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnDeleteVertex_Click(object sender, EventArgs e)
        {
            if (deleteVertex(int.Parse(cbVertex.SelectedItem.ToString())))
            {
                MessageBox.Show("Xóa đỉnh thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Ngang: " + this.Size.Width + ", Doc: " + this.Size.Height);
            MessageBox.Show("Ngang: " + MousePosition.X + ", Doc: " + MousePosition.Y);
        }

        private void ptbDrawArea_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ngang: " + this.Size.Width + ", Doc: " + this.Size.Height);
        }
        #endregion
    }
}
