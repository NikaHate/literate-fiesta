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

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {

        MySimplex simplex;
        List<MySimplex> history = new List<MySimplex>();
        public Form1()
        {
            InitializeComponent();
           
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            openFileDialog1.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

        }

     
        //задать задачу
        private void button5_Click(object sender, EventArgs e)
        {
            
            int n = (int)numericUpDown1.Value;
            int m = (int)numericUpDown2.Value;
            SizeSet(n, m);
            history.Clear();


        }

        //сохранить
        private void button4_Click(object sender, EventArgs e)
        {
            Stream myStream;
            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {

                    StreamWriter myWritet = new StreamWriter(myStream);
                    try
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            for (int j = 0; j < dataGridView1.ColumnCount; j++)
                            {
                                myWritet.Write(dataGridView1.Rows[i].Cells[j].Value.ToString() + " ");
                                //str += dataGridView1.Rows[i].Cells[j].Value.ToString() + " ";

                            }
                            myWritet.WriteLine();



                        }



                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                    finally
                    {
                        myWritet.Close();
                    
                    }
                    myStream.Close();
                    
                
                
                }
            
            
            
            }
        }

        //задать размер
        private void SizeSet(int n, int m)
        {
            dataGridView1.Columns.Clear();

            for (int i = 0; i < n; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = "x" + (i + 1);
                column.HeaderText = "x" + (i + 1);
                column.Width = 50;
                dataGridView1.Columns.Add(column);

            }
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
            c.Name = "b";
            c.HeaderText = "b";
            c.Width = 50;
            dataGridView1.Columns.Add(c);


            dataGridView1.Rows.Add(m);

            DataGridViewRow row = new DataGridViewRow();
            row.HeaderCell.Value = "F";
            dataGridView1.Rows.Add(row);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1[j, i].Value = 0;
                }
            }

        }

        //открыть файл
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                history.Clear();
                Stream str = null;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((str = openFileDialog1.OpenFile()) != null)
                    {

                        StreamReader myread = new StreamReader(str);
                        string[] s;
                        int num = 0;



                        try
                        {
                            string[] s1 = myread.ReadToEnd().Split('\n');
                            num = s1.Count();
                            s = s1[0].Split(' ');
                            int num2 = s.Count();

                            dataGridView1.Columns.Clear();
                            for (int i = 0; i < num2 - 2; i++)
                            {
                                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                column.Name = "x" + (i + 1);
                                column.HeaderText = "x" + (i + 1);
                                column.Width = 50;
                                dataGridView1.Columns.Add(column);

                            }
                            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                            c.Name = "b";
                            c.HeaderText = "b";
                            c.Width = 50;
                            dataGridView1.Columns.Add(c);



                            dataGridView1.RowCount = num;

                            for (int i = 0; i < num - 1; i++)
                            {
                                s = s1[i].Split(' ');
                                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                                {
                                    try
                                    {
                                        dataGridView1.Rows[i].Cells[j].Value = s[j];

                                    }
                                    catch
                                    { }


                                }
                            }
                            dataGridView1.Rows.RemoveAt(num - 1);
                            dataGridView1.Rows[num - 2].HeaderCell.Value = "F";


                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);

                        }
                        finally
                        {

                            myread.Close();
                        }

                    }

                }
            }
            catch { }
            

        }

        //подготовка к решению
        private void button3_Click(object sender, EventArgs e)
        {
          
                List<MyNumber> function = ReadFunc(dataGridView1);
                if (comboBox2.SelectedIndex == 1)
                    for (var i = 0; i < function.Count; i++)
                    {
                        function[i] *= -1;
                    }
                Matrix TabSimp = readMatrix(dataGridView1);
                simplex = new MySimplex(function, TabSimp);
                SimplexPrint(simplex, dataGridView1);
                ColorElement(simplex);
            //    MySimplex el = new MySimplex(simplex);
            //    history.Add(el);
            //label4.Text = history.Count + "";

        }


        //автоматическое решение
        private void button7_Click(object sender, EventArgs e)
        {
            

                List<MyNumber> function = ReadFunc(dataGridView1);



                if (comboBox2.SelectedIndex == 1)
                    for (var i = 0; i < function.Count; i++)
                    {
                        function[i] *= -1;
                    }

                Matrix TabSimp = readMatrix(dataGridView1);

                simplex = new MySimplex(function, TabSimp);



                while (true)
                {
                    simplex.isEndArt();
                    if (simplex.ArtificialOver)
                    {

                        break;

                    }
                    int c = simplex.findMainCol();
                    simplex.isNoAnswer1();
                    //такого быть не может
                    if (simplex.NoAnswer)
                    {
                        MessageBox.Show("Решения нет!!!");
                        return;


                    }
                    int r = simplex.findMainRow(c);



                    simplex.StepArtificialTable(r, c);
                    simplex.ToString();




                }

                simplex.InitLastStroka1();

                while (true)
                {

                    simplex.isEnd();
                    if (simplex.Answer)
                    {


                        MessageBox.Show("Ответ\n" + simplex.StrAnswer(comboBox3.SelectedIndex, comboBox2.SelectedIndex));
                        break;

                    }
                    int c = simplex.findMainCol();




                    simplex.isNoAnswer(c);

                    if (simplex.NoAnswer)
                    {
                        MessageBox.Show("Решения нет!!");
                        break;


                    }

                    int r = simplex.findMainRow(c);



                    simplex.StepSimplexTable(r, c);
                    //MessageBox.Show(simplex.ToString());



                }

           


        }

 

        //записать в табличку
        private void SimplexPrint( MySimplex s, DataGridView dgv)
        {

            dgv.Columns.Clear();
            dgv.Rows.Clear();
            int n = s.IndexFreeVariables.Count;

            int m = s.IndexBasisVariables.Count;
            foreach (var iv in s.IndexFreeVariables)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = "x" + (iv+1);
                column.HeaderText = "x" + (iv + 1);
                column.Width = 50;
                dataGridView1.Columns.Add(column);

            }
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
            c.Name = "b";
            c.HeaderText = "b";
            c.Width = 50;
            dataGridView1.Columns.Add(c);
            
            foreach (var ib in s.IndexBasisVariables)
            {
                DataGridViewRow r = new DataGridViewRow();
                r.HeaderCell.Value = "x"+(ib+1).ToString();
                dataGridView1.Rows.Add(r);
            }
            DataGridViewRow row = new DataGridViewRow();
            row.HeaderCell.Value = "F";
            dataGridView1.Rows.Add(row);
            if (comboBox3.SelectedIndex == 0) { 
          
           for(int i = 0; i<s.lastStroka.Count;i++)
            {
                dgv[i, m].Value = s.lastStroka[i];

            }

            for (int i = 0; i < dgv.RowCount-1; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1[j, i].Value = s.TableSimplex[i][j];
                }
            }
            }
            if (comboBox3.SelectedIndex == 1)
            {

                for (int i = 0; i < s.lastStroka.Count; i++)
                {
                    dgv[i, m].Value = s.lastStroka[i].toDouble();

                }

                for (int i = 0; i < dgv.RowCount - 1; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        dataGridView1[j, i].Value = s.TableSimplex[i][j].toDouble();
                    }
                }
            }



        }



        //считать функцию
        private List<MyNumber> ReadFunc(DataGridView dgv)
        {
            List<MyNumber> function = new List<MyNumber>();
            int n = dgv.RowCount-1;
            for (int j = 0; j < dgv.ColumnCount; j++)
            {
                MyNumber num = new MyNumber(dgv[j, n].Value.ToString());
                
                function.Add(num);
            }
            return function;

          





        }
        //считать симплекс таблицу
        private Matrix readMatrix(DataGridView dgv)
        {

            //Matrix = [dgv.RowCount, dgv.ColumnCount];

            List<List<MyNumber>> matrx = new List<List<MyNumber>>();

              for (int i = 0; i < dgv.RowCount-1; i++)
             {
                List < MyNumber > m= new List<MyNumber>();
                for (int j = 0; j < dgv.ColumnCount; j++)
                  {
                      
                    MyNumber num = new MyNumber(dgv[j, i].Value.ToString());
                    m.Add(num);
                  }
                matrx.Add(m);
            
            }
            Matrix nm = new Matrix(matrx);
           // MessageBox.Show(nm.ToString());



             return nm;




        }

      
        //покрасить опорные элементы
        private void ColorElement(MySimplex s)
        {
      
            s.SearchAllMainElement1();
            for (int i = 0; i < dataGridView1.RowCount-1; i++)
              foreach (var j in s.MainElements[i])
            {
                  // MessageBox.Show(i+" "+j+" ");
                    dataGridView1[j, i].Style.BackColor = Color.GreenYellow;


            }
        }
        // шаг симплекс метода
        private void button2_Click(object sender, EventArgs e)
        {

            MySimplex el = new MySimplex(simplex);
            history.Add(el);
            //label4.Text = history.Count + "";

            simplex.isNoAnswer1();
                if (simplex.NoAnswer)
                {
                    MessageBox.Show("Решения нет");
                
                }
                

                int i = dataGridView1.CurrentCell.ColumnIndex;
                int j = dataGridView1.CurrentCell.RowIndex;
                //MessageBox.Show(i + " " + j);
                //MessageBox.Show(dataGridView1[i, j].ToString());
                //MessageBox.Show(simplex.TableSimplex[j][i].ToString());
                simplex.SuperStep(j,i);
                
                SimplexPrint(simplex, dataGridView1);
                ColorElement(simplex);
                // MessageBox.Show( simplex.StrBasis());

                //string str = "";
                //for (int k = 0; k < simplex.basis.Count; k++)
                //    str += simplex.basis[k].ToString() + " ";
                //MessageBox.Show(str);
                //string str1 = "";
                //for (int k = 0; k < simplex.IndexBasisVariables.Count; k++)
                //    str1 += simplex.IndexBasisVariables[k].ToString() + " ";
                //MessageBox.Show(str1);


                simplex.isEnd();
                if (simplex.Answer && simplex.ArtificialOver)
                {
                    MessageBox.Show("Ответ\n" + simplex.StrAnswer(comboBox3.SelectedIndex, comboBox2.SelectedIndex));


                }
                else { 
                    simplex.isNoAnswer1();
                    if (simplex.NoAnswer)
                    {
                        MessageBox.Show("Решения нет");

                    }
                }


                







        }

        /// <summary>
        ///  шаг назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(history.Count + "");
            
           
                simplex = history[history.Count-1];
            if (history.Count > 1)
            {
                history.RemoveAt(history.Count - 1);
            }


               // MessageBox.Show(history.Count+"");
                SimplexPrint(simplex, dataGridView1);
                ColorElement(simplex);
                //history.RemoveAt(history.Count - 1);


           
            //label4.Text = history.Count + "";
            //String str1 = "";
            //for (int k = 0; k < history.Count; k++)
            //    str1 += history[k].ToString() + "\n ";
            //MessageBox.Show(str1);
        }

































    }
}
