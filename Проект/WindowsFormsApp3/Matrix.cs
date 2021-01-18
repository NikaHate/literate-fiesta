using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public class Matrix
    {
        public List<List<MyNumber>> values;//матрица дробных чисел
       
        public bool isBasis;
        public bool isOnes;
        public Matrix()
        {
        }

        public Matrix(List<List<MyNumber>> table)
        {
            this.values = table;


        }

        //метод гаусса
       

        public object Clone()
        {
            return new Matrix
            {
                values = this.values
            };
        }
    }
}
