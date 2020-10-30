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
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Drawing.Drawing2D;

namespace EOT_LTĐT
{
    public partial class Form1 : Form
    {
        Graphics g;
        int vertex;
        int[,] matrix;
        int[] x = { 551, 871, 551, 731, 231, 371, 231, 871, 371, 731 };
        int[] y = { 104, 404, 584, 154, 404, 154, 274, 274, 543, 534 };
        Point[] point;
        int[] visited;
        int nConnect;
        int[] path;
        int nP = 0;
        int[] connect;
        int nC = 0, check = -1;
        int[] hamilton = new int[20];
        int nH;
        Pen grayPen = new Pen(Color.Gray, 2);
        Pen redPen = new Pen(Color.Red, 2);
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
        }

        #region Methods
        private bool isSymmetryMatrix()
        {
            bool flag = true;
            for (int i = 0; i < vertex; i++)
            {
                for (int j = 0; j < vertex; j++)
                {
                    if (matrix[i, j] != 0 && matrix[i, j] != matrix[j, i])
                    {
                        flag = false;
                        break;
                    }
                }
            }
            return flag;
        }
        private void loadComboboxVertex()
        {
            int[] arrVertex = new int[vertex];
            for (int i = 0; i < vertex; i++)
            {
                arrVertex[i] = i + 1;
            }
            cbVertex.DataSource = arrVertex;
        }
        private void readMatrix(string fileName)
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
        private void writeMatrix(int n, int[,] a, string fileName)
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
        private void loadGraph()
        {
            this.Refresh();
            point = new Point[vertex];
            //Draw Vertex
            for (int i = 0; i < vertex; i++)
            {
                point[i] = new Point(x[i] - 1, y[i] - 14);
                Point drawPoint = new Point(x[i], y[i]);
                g.FillEllipse(new SolidBrush(Color.LightBlue), x[i] - 1, y[i] - 14, 13, 13);
                g.DrawString((i + 1).ToString(), new Font("Arial", 12), new SolidBrush(Color.Black), drawPoint);
            }
            //Draw Edge
            for (int i = 0; i < vertex; i++)
            {
                for (int j = 0; j < vertex; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        g.DrawString(matrix[i, j].ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), ((x[i] - 1) + (x[j] - 1)) / 2, ((y[i] - 14) + (y[j] - 14)) / 2);
                        if (!isSymmetryMatrix())
                        {
                            AdjustableArrowCap arrowSize = new AdjustableArrowCap(5, 5);
                            grayPen.EndCap = LineCap.ArrowAnchor;
                            grayPen.CustomEndCap = arrowSize;
                        }
                        g.DrawLine(grayPen, point[i], point[j]);
                    }
                }
            }
        }
        private bool addEdge(int edgeFrom, int edgeTo, int Weight)
        {
            if (matrix[edgeFrom - 1, edgeTo - 1] != 0)
            {
                MessageBox.Show("Đã có cạnh này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Point pointFrom = new Point(x[edgeFrom - 1] - 1, y[edgeFrom - 1] - 14);
                Point pointTo = new Point(x[edgeTo - 1] - 1, y[edgeTo - 1] - 14);
                if (!isSymmetryMatrix())
                {
                    matrix[edgeFrom - 1, edgeTo - 1] = Weight;
                }
                else
                {
                    matrix[edgeFrom - 1, edgeTo - 1] = Weight;
                    matrix[edgeTo - 1, edgeFrom - 1] = Weight;
                }
                g.DrawLine(grayPen, pointFrom, pointTo);
                g.DrawString(Weight.ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), ((x[edgeFrom - 1] - 1) + (x[edgeTo - 1] - 1)) / 2, ((y[edgeFrom - 1] - 14) + (y[edgeTo - 1] - 14)) / 2);
                writeMatrix(vertex, matrix, "Matrix.txt");
                readMatrix("Matrix.txt");
                loadGraph();
                return true;
            }
            return false;
        }
        private bool deleteEdge(int edgeFrom, int edgeTo)
        {
            if (matrix[edgeFrom - 1, edgeTo - 1] == 0)
            {
                MessageBox.Show("Không có cạnh này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (!isSymmetryMatrix())
                {
                    matrix[edgeFrom - 1, edgeTo - 1] = 0;
                }
                else
                {
                    matrix[edgeFrom - 1, edgeTo - 1] = 0;
                    matrix[edgeTo - 1, edgeFrom - 1] = 0;
                }
                writeMatrix(vertex, matrix, "Matrix.txt");
                readMatrix("Matrix.txt");
                loadGraph();
                return true;
            }
            return false;
        }
        private bool changeWeight(int edgeFrom, int edgeTo, int newWeight)
        {
            if (matrix[edgeFrom - 1, edgeTo - 1] == 0)
            {
                MessageBox.Show("Không có cạnh này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (!isSymmetryMatrix())
                {
                    matrix[edgeFrom - 1, edgeTo - 1] = newWeight;
                }
                else
                {
                    matrix[edgeFrom - 1, edgeTo - 1] = newWeight;
                    matrix[edgeTo - 1, edgeFrom - 1] = newWeight;
                }
                writeMatrix(vertex, matrix, "Matrix.txt");
                readMatrix("Matrix.txt");
                loadGraph();
                return true;
            }
            return false;
        }
        private bool addVertex()
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
        private bool deleteVertex(int _vertex)
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
        private bool BFS(int edgeFrom, int edgeTo)
        {
            visited = new int[vertex];
            path = new int[vertex];
            for (int i = 0; i < vertex; i++)
            {
                visited[i] = 0;
                path[i] = -1;
            }

            Queue queue = new Queue();
            queue.Enqueue(edgeFrom);
            while (queue.Count != 0)
            {
                int element = Convert.ToInt32(queue.Dequeue());
                visited[element] = 1;
                for (int i = 0; i < vertex; i++)
                {
                    if (visited[i] == 0 && matrix[element, i] != 0)
                    {
                        queue.Enqueue(i);
                        path[i] = element;
                    }
                }
            }
            int[] result = new int[vertex];
            int n = 0;
            if (visited[edgeTo] == 1)
            {
                int j = edgeTo;
                while (j != edgeFrom)
                {
                    result[n] = j;
                    j = path[j];
                    n++;
                }
                result[n] = edgeFrom;
                for (int i = 0; i < n; i++)
                {
                    if (!isSymmetryMatrix())
                    {
                        AdjustableArrowCap arrowSize = new AdjustableArrowCap(5, 5);
                        redPen.StartCap = LineCap.ArrowAnchor;
                        redPen.CustomStartCap = arrowSize;
                    }
                    g.DrawLine(redPen, point[result[i]], point[result[i + 1]]);
                    txbPath.Text += (result[i] + 1).ToString() + "<--";
                    Thread.Sleep(1000);
                }
                txbPath.Text += result[n] + 1;
                MessageBox.Show("Xong", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không có đường đi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return true;
        }
        private void backtracking(int edgeFrom)
        {
            visited[edgeFrom] = 1;
            for (int i = 0; i < vertex; i++)
            {
                if (visited[i] == 0 && matrix[edgeFrom, i] != 0)
                {
                    path[i] = edgeFrom;
                    backtracking(i);
                }
            }
        }
        private bool DFS(int vertexFrom, int vertexTo)
        {
            visited = new int[vertex];
            path = new int[vertex];
            for (int i = 0; i < vertex; i++)
            {
                visited[i] = 0;
                path[i] = -1;
            }

            backtracking(vertexFrom);

            int[] result = new int[vertex];
            int n = 0;
            if (visited[vertexTo] == 1)
            {
                int j = vertexTo;
                while (j != vertexFrom)
                {
                    result[n] = j;
                    j = path[j];
                    n++;
                }
                result[n] = vertexFrom;
                for (int i = 0; i < n; i++)
                {
                    if (!isSymmetryMatrix())
                    {
                        AdjustableArrowCap arrowSize = new AdjustableArrowCap(5, 5);
                        redPen.StartCap = LineCap.ArrowAnchor;
                        redPen.CustomStartCap = arrowSize;
                    }
                    g.DrawLine(redPen, point[result[i]], point[result[i + 1]]);
                    txbPath.Text += (result[i] + 1).ToString() + "<--";
                    Thread.Sleep(1000);
                }
                txbPath.Text += result[n] + 1;
                MessageBox.Show("Xong", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không có đường đi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return true;
        }
        //i là đỉnh, nConnect là nhãn dán theo liên thông của từng đỉnh
        private void visit(int i, int nConnect)
        {
            //đánh dấu đỉnh đã duyệt và gán nhãn dán
            visited[i] = nConnect;
            if (check == -1 || check < nConnect)
            {
                check = nConnect;
                if (nConnect != 1)
                {
                    connect[nC] = -1;
                    nC++;
                }
            }
            connect[nC] = i;
            nC++;

            //xét tiếp các đỉnh chưa duyệt và kề với đỉnh i, nếu liên thông thì gọi lại để gán nhãn dán cho các đỉnh kề
            for (int j = 0; j < vertex; j++)
            {
                if (visited[j] == 0 && matrix[i, j] != 0)
                {
                    visit(j, nConnect);
                }
            }
        }
        private bool Connect()
        {
            visited = new int[vertex];
            connect = new int[20];
            for (int i = 0; i < vertex; i++)
            {
                visited[i] = 0;
            }
            nConnect = 0;
            for (int i = 0; i < vertex; i++)
            {
                if (visited[i] == 0)
                {
                    nConnect++;
                    visit(i, nConnect);
                }
            }

            if (nConnect == 1) return true;
            return false;
        }
        private void backtracking1(int k)
        {
            if (nH == vertex && matrix[k, 0] != 0)
            {
                nP = nH;
                for (int i = 1; i < nH; i++)
                {
                    path[i] = hamilton[i];
                }
            }
            else
            {
                for (int i = 0; i < vertex; i++)
                {
                    if (visited[i] == 0 && matrix[k, i] != 0)
                    {
                        hamilton[nH] = i;
                        nH++;
                        visited[i] = 1;
                        backtracking1(i);
                        visited[i] = 0;
                        nH--;
                    }
                }
            }
        }
        private bool Hamilton(int edgeFrom)
        {
            visited = new int[vertex];
            path = new int[vertex + 1];
            for (int i = 0; i < vertex; i++)
            {
                visited[i] = 0;
            }
            visited[edgeFrom] = 1;
            path[0] = edgeFrom;
            nP++;
            hamilton[0] = edgeFrom;
            nH++;

            backtracking1(edgeFrom);

            if (nP < vertex) return false;
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
            if (txbVertexFrom.Text == "" || txbVertexTo.Text == "" || txbWeight.Text == "")
            {
                MessageBox.Show("Điền đầy đủ đỉnh và trọng số để thêm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (addEdge(int.Parse(txbVertexFrom.Text), int.Parse(txbVertexTo.Text), int.Parse(txbWeight.Text)))
                {
                    MessageBox.Show("Thêm cạnh thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void btnDeleteEdge_Click(object sender, EventArgs e)
        {
            if (txbVertexFrom.Text == "" || txbVertexTo.Text == "")
            {
                MessageBox.Show("Điền đầy đủ đỉnh để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (deleteEdge(int.Parse(txbVertexFrom.Text), int.Parse(txbVertexTo.Text)))
                {
                    MessageBox.Show("Xóa cạnh thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void btnChangeWeight_Click(object sender, EventArgs e)
        {
            if (txbVertexFrom.Text == "" || txbVertexTo.Text == "" || txbWeight.Text == "")
            {
                MessageBox.Show("Điền đầy đủ đỉnh và trọng số để thay đổi trọng số cho cạnh", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (changeWeight(int.Parse(txbVertexFrom.Text), int.Parse(txbVertexTo.Text), int.Parse(txbWeight.Text)))
                {
                    MessageBox.Show("Thay đối trọng số thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
        private void btnBFS_Click(object sender, EventArgs e)
        {
            txbPath.Clear();
            loadGraph();
            BFS(int.Parse(txbVertexFrom.Text) - 1, int.Parse(txbVertexTo.Text) - 1);
        }
        private void btnDFS_Click(object sender, EventArgs e)
        {
            txbPath.Clear();
            loadGraph();
            DFS(int.Parse(txbVertexFrom.Text) - 1, int.Parse(txbVertexTo.Text) - 1);
        }
        private void btnCheckConnect_Click(object sender, EventArgs e)
        {
            txbPath.Clear();
            loadGraph();
            if (Connect())
            {
                for (int i = 0; i < nC - 1; i++)
                {
                    g.DrawLine(redPen, point[connect[i]], point[connect[i + 1]]);
                    Thread.Sleep(1000);
                }
                if (matrix[connect[nC - 1], connect[0]] != 0)
                {
                    g.DrawLine(redPen, point[connect[nC - 1]], point[connect[0]]);
                }
                txbPath.Text = "Đồ thị có liên thông";
                MessageBox.Show("Xong", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Random random = new Random();
                Pen randomPen = new Pen(Color.Red, 2);
                for (int i = 0; i < nC - 1; i++)
                {
                    if (connect[i] != -1 && connect[i + 1] != -1)
                    {
                        g.DrawLine(randomPen, point[connect[i]], point[connect[i + 1]]);
                    }
                    else
                    {
                        randomPen = new Pen(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)), 2);
                    }
                    Thread.Sleep(1000);
                }
                txbPath.Text = "Đồ thị không liên thông và có " + nConnect + " thành phần liên thông" + Environment.NewLine;
                for (int i = 1; i <= nConnect; i++)
                {
                    for (int j = 0; j < vertex; j++)
                    {
                        if (visited[j] == i)
                        {
                            txbPath.Text += (j + 1).ToString() + " ";
                        }
                    }
                    txbPath.Text += Environment.NewLine;
                }
                MessageBox.Show("Xong", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnHamilton_Click(object sender, EventArgs e)
        {
            txbPath.Clear();
            loadGraph();
            if (Hamilton(int.Parse(cbVertex.SelectedItem.ToString()) - 1))
            {
                path[nP] = int.Parse(cbVertex.SelectedItem.ToString()) - 1;
                for (int i = 0; i < nP; i++)
                {
                    g.DrawLine(redPen, point[path[i]], point[path[i + 1]]);
                    Thread.Sleep(1000);
                    txbPath.Text += (path[i] + 1).ToString();
                    if (i < nP) txbPath.Text += "-->";
                }
                txbPath.Text += int.Parse(cbVertex.SelectedItem.ToString());
                
            }
            else
            {
                txbPath.Text = "Không có đường đi Hamilton.";
            }
            MessageBox.Show("Xong", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
