using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    class MySimplex 
    {

        public Matrix matrix;
        
        public List<MyNumber> function = new List<MyNumber>();//функция
        public List<int> IndexFreeVariables = new List<int>(); // Индексы свободных переменных
        public List<int> IndexBasisVariables = new List<int>(); // Индексы базисных переменных
        public List<List<MyNumber>> TableSimplex = new List<List<MyNumber>>(); //Симплекс таблица
        public List<MyNumber> lastStroka = new List<MyNumber>();//последняя строка
        public bool Answer = false; //ответ?
        public bool NoAnswer = false; //решения нет?
        public int MainColumn = 0;//главная строка
        public int MainRow = 0;//главный столбец
        public Dictionary<int, List<int>> MainElements = new Dictionary<int, List<int>>();//опорные элементы
        
        public List<MyNumber> basis;
        public bool ArtificialOver = false;//конец иск. базиса?

        //конструктор копирования
        public MySimplex(MySimplex s)
        {
            
            this.Answer = s.Answer;
            this.NoAnswer = s.NoAnswer;
            this.MainRow = s.MainRow;

            matrix = (Matrix)s.matrix.Clone();

                function = new List<MyNumber>(s.function);
            IndexFreeVariables = new List<int>(s.IndexFreeVariables); // Индексы свободных переменных
            IndexBasisVariables = new List<int>(s.IndexBasisVariables); // Индексы базисных переменных
             TableSimplex = new List<List<MyNumber>>(s.TableSimplex); //Симплекс таблица
             lastStroka = new List<MyNumber>(s.lastStroka);
       
       
             MainColumn = s.MainColumn;
       
             MainElements = new Dictionary<int, List<int>>(s.MainElements);//опорные элементы
       
             basis= new List<MyNumber>(s.basis);
             ArtificialOver = s.ArtificialOver;




    }

        public MySimplex(List<MyNumber> Function, Matrix Matrix)
        {
            matrix = Matrix;
            
            function = Function;

            basis = new List<MyNumber>();


            InitBasisArt();
            InitIndexVariables();
            InitTableSimplex();
            InitLastStrokaArtificial();
           


        }

        //таблица
        public void InitTableSimplex()
        {
            for (var i = 0; i < IndexBasisVariables.Count; i++)
            {
                TableSimplex.Add(OutStrTable(i));
            }
        }
        //последняя строка после оканчания иск. базиса
        public void InitLastStroka1()
        {
            lastStroka = new List<MyNumber>();
            MyNumber temp = new MyNumber(0);
            for (var i = 0; i < IndexFreeVariables.Count; i++)
            {
                for (var j = 0; j < TableSimplex.Count; j++)
                {
                    temp += function[IndexBasisVariables[j]] * TableSimplex[j][i];
                }

                temp -= function[IndexFreeVariables[i]];
                lastStroka.Add(-temp);
                temp = new MyNumber(0);
            }

            int t = 0;
            foreach (var j in IndexBasisVariables)
            {
                temp += function[j] * basis[j];
                t++;
            }

            lastStroka.Add(-temp);
            temp = new MyNumber(0);
            if (lastStroka.Count == 0)
            {
                for (var i = 0; i < basis.Count; i++)
                {
                    temp += function[i] * basis[i];
                }

                lastStroka.Add(-temp);
            }
            
            lastStroka[lastStroka.Count - 1] -= function[function.Count - 1];
           


        }

     
        //последняя строка в начале алгоритма
        public void InitLastStrokaArtificial()
        {
            MyNumber temp = new MyNumber(0);
            for (var i = 0; i < TableSimplex[0].Count; i++)
            {
                for (int j = 0; j < TableSimplex.Count; j++)
                {
                    temp += TableSimplex[j][i];
                }
                lastStroka.Add(-temp);
                temp = new MyNumber(0);
            }
        }
        
        //базис
        public void InitBasisArt()
        {
         
            for (int i = 0; i < matrix.values[0].Count-1; i++)
            {
                basis.Add(new MyNumber(0));
            }
          
            for(int i = 0; i < matrix.values.Count; i++)
            {
                basis.Add(new MyNumber(1));// в принципе это не важно
              
            }

        }


        
        //   раскидывает индексы переменных на свободные и базисные переменные.
        
        public void InitIndexVariables()
        {
            for (int i = 0; i < basis.Count; i++)
            {
                if (basis[i] != 0)
                    IndexBasisVariables.Add(i);
                else
                {
                    IndexFreeVariables.Add(i);
                }
            }
        }
       
        //строка таблицы
        public List<MyNumber> OutStrTable(int IndexBasis)
        {
            List<MyNumber> temp = new List<MyNumber>();
            foreach (var element in IndexFreeVariables)
            {
                temp.Add(matrix.values[IndexBasis][element]);
            }

            temp.Add(matrix.values[IndexBasis][matrix.values[0].Count - 1]);
            return temp;
        }


        //удаление колонки с искуственной переменной
        private void DeleteColumn(int Column)
        {
            for (var i = 0; i < TableSimplex.Count; i++)
            {
                TableSimplex[i].RemoveAt(Column);
            }
            lastStroka.RemoveAt(Column);
            basis.RemoveAt(IndexFreeVariables[Column]);


            for (var i = 0; i < IndexBasisVariables.Count; i++)
            {
                if (IndexFreeVariables[Column] < IndexBasisVariables[i])
                    IndexBasisVariables[i] -= 1;
            }

            IndexFreeVariables.RemoveAt(Column);

        }

        //Поиск всех опорных элементов в таблице
        

        //шаг всего алгоритма
        public void SuperStep(int r, int c)
        {
            isNoAnswer1();
            if (NoAnswer)
            {

                return;

            }
            if (ArtificialOver)
            {
                StepSimplexTable(r, c);
                return;
            }
            StepArtificialTable(r, c);
            if (ArtificialOver)
            {
                InitLastStroka1();
            
            
            }

            





        }


        //Поиск всех опорных элементов в таблице
        public void SearchAllMainElement1() 
        {
           

            MainElements = new Dictionary<int, List<int>>();
            for (int i = 0; i < IndexBasisVariables.Count; i++)
            {
                MainElements.Add(i, new List<int>());
            }

            
            for (int i = 0; i < IndexBasisVariables.Count; i++)
                for (int k = 0; k < IndexFreeVariables.Count; k++)
                {
                    if (lastStroka[k] < 0 && TableSimplex[i][k]>0 && findMainRow(k)==i)
                    {
                        MainElements[i].Add(k);
                    
                    }
                
                }  
                
                
        }

        //шаг симплекс метода
        public void StepSimplexTable( int row, int column)
        {
            List<MyNumber> HelpVector = new List<MyNumber>();
            List<MyNumber> Multi = new List<MyNumber>();
            MyNumber valueMain = new MyNumber(0);

            MainColumn = column;
            MainRow = row;

            valueMain = TableSimplex[MainRow][MainColumn];

           

            //это вычитаем(опорная строка)
            for (var i = 0; i < TableSimplex[0].Count; i++)
            {
                if (i != MainColumn)
                    HelpVector.Add(TableSimplex[MainRow][i] / valueMain);
                else
                {
                    HelpVector.Add(new MyNumber(0));
                }
            }

            //на это умножаем, то что вычитаем(опорный столбец)
            for (var i = 0; i < TableSimplex.Count; i++)
            {
                if (i != MainRow)
                    Multi.Add(TableSimplex[i][MainColumn]);
                else
                {
                    Multi.Add(new MyNumber(0));
                }
            }


            //Cоздание новой таблицы
            List<List<MyNumber>> NewTableSimplex = new List<List<MyNumber>>();
            List<MyNumber> newLastStroka = new List<MyNumber>();


            for (var i = 0; i < TableSimplex.Count; i++)
            {
                List<MyNumber> list = new List<MyNumber>();
                for (var j = 0; j < TableSimplex[0].Count; j++)
                {
                    if (j == MainColumn && i == MainRow)
                    {
                        list.Add(1 / valueMain); //если обрабатывается опорный элемент
                        continue;
                    }

                    if (i == MainRow) //если обрабатывается опорная строка
                    {
                        list.Add(HelpVector[j]);
                        continue;
                    }

                    if (j == MainColumn) //если обрабатывается опорная столбецц
                    {
                        list.Add(-Multi[i] / valueMain);
                        continue;
                    }

                    list.Add(TableSimplex[i][j] - (Multi[i] * HelpVector[j]));
                }

                NewTableSimplex.Add(list);
            }

            for (var j = 0; j < TableSimplex[0].Count; j++)
            {
                if (j == MainColumn) //если обрабатывается опорная строка
                {
                    newLastStroka.Add(-lastStroka[MainColumn] / valueMain);
                    continue;
                }

                newLastStroka.Add(lastStroka[j] - (lastStroka[MainColumn] * HelpVector[j]));
            }

            //Меняем базисные индексы и сам базис
            int val = IndexBasisVariables[MainRow];
            IndexBasisVariables[MainRow] = IndexFreeVariables[MainColumn];
            IndexFreeVariables[MainColumn] = val;
            basis = new List<MyNumber>();
            for (int i = 0; i < function.Count-1; i++)
            {
                basis.Add(new MyNumber(0));
            }

            int k = 0;
            foreach (var indexBasisVariable in IndexBasisVariables)
            {
                basis[indexBasisVariable] = NewTableSimplex[k][NewTableSimplex[0].Count - 1];
                k++;
            }
         


            //Меняем значения таблицы
            TableSimplex = NewTableSimplex;
            lastStroka = newLastStroka;
        }

        // шаг с искуственным базисом
        public void StepArtificialTable(int row, int column)
        {
            List<MyNumber> HelpVector = new List<MyNumber>();
            List<MyNumber> Multi = new List<MyNumber>();
            MyNumber valueMain = new MyNumber(0);
            
            MainColumn = column;
            MainRow = row;

         

            valueMain = TableSimplex[MainRow][MainColumn];

        
            //так же как в симплекс шаге
            
            for (var i = 0; i < TableSimplex[0].Count; i++)
            {
                if (i != MainColumn)
                    HelpVector.Add(TableSimplex[MainRow][i] / valueMain);
                else
                {
                    HelpVector.Add(new MyNumber(0));
                }
            }

          
            for (var i = 0; i < TableSimplex.Count; i++)
            {
                if (i != MainRow)
                    Multi.Add(TableSimplex[i][MainColumn]);
                else
                {
                    Multi.Add(new MyNumber(0));
                }
            }


            
            List<List<MyNumber>> NewTableSimplex = new List<List<MyNumber>>();
            List<MyNumber> newLastStroka = new List<MyNumber>();


            for (var i = 0; i < TableSimplex.Count; i++)
            {
                List<MyNumber> list = new List<MyNumber>();
                for (var j = 0; j < TableSimplex[0].Count; j++)
                {
                    if (j == MainColumn && i == MainRow)
                    {
                        list.Add(1 / valueMain);
                        continue;
                    }

                    if (i == MainRow) 
                    {
                        list.Add(HelpVector[j]);
                        continue;
                    }

                    if (j == MainColumn) 
                    {
                        list.Add(-Multi[i] / valueMain);
                        continue;
                    }

                    list.Add(TableSimplex[i][j] - (Multi[i] * HelpVector[j]));
                }

                NewTableSimplex.Add(list);
            }

            for (var j = 0; j < TableSimplex[0].Count; j++)
            {
                if (j == MainColumn) 
                {
                    newLastStroka.Add(-lastStroka[MainColumn] / valueMain);
                    continue;
                }

                newLastStroka.Add(lastStroka[j] - (lastStroka[MainColumn] * HelpVector[j]));
            }

            
            int val = IndexBasisVariables[MainRow];
            IndexBasisVariables[MainRow] = IndexFreeVariables[MainColumn];
            IndexFreeVariables[MainColumn] = val;
            int v = basis.Count;
            basis = new List<MyNumber>();
            for (int i = 0; i < v; i++)
            {
                basis.Add(new MyNumber(0));
            }


            int k = 0;
            foreach (var indexBasisVariable in IndexBasisVariables)
            {
                basis[indexBasisVariable] = NewTableSimplex[k][NewTableSimplex[0].Count - 1];
                k++;
            }
           
            
            TableSimplex = NewTableSimplex;
            lastStroka = newLastStroka; 

            //если это дополнительная переменная (искусственная) , то удаляем столбец в таблице с ней


            if (val >= function.Count-1)  
                DeleteColumn(MainColumn);
            isEndArt();
           
        }

        // поиск опорной строки
        public int findMainRow(int mainCol)
        {
            int mainRow = -1;

            int last = TableSimplex[0].Count - 1;

            for (int i = 0; i < TableSimplex.Count; i++)
                if (TableSimplex[i][mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }

            if (mainRow == -1)
            {
                NoAnswer = true;
            }

            for (int i = mainRow + 1; i < TableSimplex.Count; i++)
                if ((TableSimplex[i][mainCol] > 0) && ((TableSimplex[i][last] / TableSimplex[i][mainCol]) < (TableSimplex[mainRow][last] / TableSimplex[mainRow][mainCol])))
                    mainRow = i;

            return mainRow;
        }
        //поиск опорного столбца
        public int findMainCol()
        {
            int mainCol = 0;
            for (int j = 1; j < lastStroka.Count - 1; j++)
            {
                if (lastStroka[j] < lastStroka[mainCol])
                    mainCol = j;
            }

            return mainCol;
        }

        //проверка на конец алгоритма
        public void isEnd()
        {
            Answer = true;
            for (int j = 0; j < lastStroka.Count-1; j++)
            {
                if (lastStroka[j] < 0)
                { Answer = false; break; }



            }

        }
        //исскуств. базис выход
        public void isEndArt()
        {
            ArtificialOver = true;
            for (int j = 0; j < lastStroka.Count; j++)
            {
                if (lastStroka[j] != 0)
                {
                    ArtificialOver = false;//- продолжаем с исскуственным базисом 
                   return;
                }


            }
            //foreach (var x in IndexBasisVariables)//
            //{ 
            //    if(x >)
            
            //}
            //InitLastStroka1();

        }
        //решений нет
        public void isNoAnswer(int mainCol)
        {
          
            NoAnswer = true;

            
            for (int i = 0; i < TableSimplex.Count; i++)
                if (TableSimplex[i][mainCol] > 0)
                {
                    NoAnswer = false;
                    break;
                }


        }
        public void isNoAnswer1()
        {
           
            NoAnswer = true;


            for (int i = 0; i < IndexBasisVariables.Count; i++)
                for (int k = 0; k < IndexFreeVariables.Count; k++)
                {
                    if (lastStroka[k] < 0 && TableSimplex[i][k] > 0)
                    {

                        NoAnswer = false;
                    }



                }


        }
        //Строковое представление таблицы 
        public override string ToString()
        {
            return $"{StrTable()}\n"

            +$"{StrFunction()}";
        }

        

       
        public string StrTable()
        {
            string temp = "__|" + StrFreeVariable() + " |\n";
            int i = 0;
            foreach (var indexBasis in IndexBasisVariables)
            {
                temp += $"{indexBasis + 1}x|";
                foreach (var element in OutTable(i))
                {
                    temp += $"{element}|";
                }

                i++;
                temp += "\n";
            }

            
            temp += "f|";
            foreach (var t in lastStroka)
                temp += $"{t}!!!|";

            return temp;
        }

        public List<MyNumber> OutTable(int IndexBasis)
        {
            List<MyNumber> temp = new List<MyNumber>();

            foreach (var V in TableSimplex[IndexBasis])
            {
                temp.Add(V);
            }

            return temp;
        }
       
      


        
        public string StrFreeVariable()
        {
            string temp = "";

            foreach (var element in IndexFreeVariables)
            {
                temp += $"x{element + 1}|";
            }

            return temp;
        }


      
        public string StrFunction()
        {
            string temp = "f=";


            for (var i = 0; i < function.Count; i++)
            {
                if (function[i] != 0)
                {
                    if (function[i].Sign == 1)
                        temp += "+";
                    temp += $"{function[i]}x";
                }
            }

            return temp;
        }

        public string StrAnswer( int k, int m)
        {
            string temp = "f=";

            if (m == 0) m--;
            if (k == 0) { 
            temp += (m*lastStroka[lastStroka.Count - 1])+"  x=(";
            for (int i = 0; i < basis.Count; i++)
            {
                
                temp += basis[i] + " ";
                

            }
            temp += ")";
            }
            if (k == 1)
            {

                temp += (m*lastStroka[lastStroka.Count - 1]).toDouble() + "  x=(";
                for (int i = 0; i < basis.Count; i++)
                {

                    temp += basis[i].toDouble() + " ";


                }
                temp += ")";
                
            }
            return temp;


        }




           
        

        



    }
}
