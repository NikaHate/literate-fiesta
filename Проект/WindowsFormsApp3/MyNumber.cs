using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public class MyNumber 
    {

        private int numerator;
        private int denominator;
        private int sign;

        public int Numerator
        {
            get => numerator;
            set => numerator = value;
        }

        public int Denominator
        {
            get => denominator;
            set => denominator = value;
        }

        public int Sign
        {
            get => sign;
            set => sign = value;
        }




        public MyNumber()
        {
        }


        public MyNumber(int num, int denom)
        {
            if (num < 0)
            {
                sign = -1;
                numerator = -num;
            }
            else
            {
                sign = 1;
                numerator = num;
            }

            denominator = denom;
        }


        public MyNumber(int k)
        {
            numerator = Math.Abs(k);
            denominator = 1;
            sign = k < 0 ? -1 : 1;
        }


        public MyNumber(string str)
        {
            str.Replace(" ", "");
            if (str.StartsWith("-"))
            {
                sign = -1;
                str = str.Remove(0, 1);
            }
            else
                sign = 1;

            //целое
            if (!str.Contains(",") && !str.Contains(".") && !str.Contains("/"))
            {
                int k = Convert.ToInt32(str);
                this.numerator = k;
                this.denominator = 1;
            }

            //десятичная
            else if (str.Contains(",") || str.Contains("."))
            {
                string[] str1 = str.Split(new char[] { ',', '.' });
                if (str1.Length != 2)
                {
                    throw new Exception();
                }
                else
                {
                    numerator = Convert.ToInt32(str1[0] + str1[1]);
                    denominator = Convert.ToInt32(Math.Pow(10, str1[1].Length));
                }
            }
            //обыкновенная
            else if (str.Contains("/"))
            {
                string[] str1 = str.Split('/');
                if (str1.Length != 2 || Convert.ToInt32(str1[1]) == 0)
                {
                    throw new Exception();

                }
                else
                {
                    try
                    {
                        numerator = Convert.ToInt32(str1[0]);
                        denominator = Convert.ToInt32(str1[1]);
                    }
                    catch 
                    {
                        throw new Exception();

                    }
                }
            }

        }
        






        public override string ToString()
        {
            Reduce();
            if (numerator == 0)
            {
                return "0";
            }

            string result;
            if (sign < 0)
            {
                result = "-";
            }
            else
            {
                result = "";
            }

            if (numerator == denominator)
            {
                return result + "1";
            }

            if (denominator == 1)
            {
                return result + numerator;
            }

            return result + numerator + "/" + denominator;
        }
        public string toDouble()
        {
            return ((double)sign * (double)numerator / (double)denominator).ToString();
        
        
        }


        public int gcd(int c, int b)
        {
            while (b != 0)
            {
                int d = c % b;
                c = b;
                b = d;
            }

            return c;
        }

     
        


        public MyNumber Reduce()
        {
            MyNumber result = this;
            if (result.numerator == 0)
            {
                return this;
            }

            int k = gcd(result.numerator, result.denominator);
            result.denominator /= k;
            result.numerator /= k;
            return result;
        }

      






        public static MyNumber operator +(MyNumber a, MyNumber b)
        {
            int c = a.sign * a.numerator * b.denominator + b.sign * b.numerator * a.denominator;
            int d = a.denominator * b.denominator;
            return new MyNumber(c, d);
        }

        public static MyNumber operator +(int a, MyNumber b)
        {
            return new MyNumber(a) + b;
        }

        public static MyNumber operator +(MyNumber a, int b)
        {
            return b + a;
        }

        public static MyNumber operator -(MyNumber a, MyNumber b)
        {
            MyNumber c = new MyNumber();
            int k = a.sign * a.numerator * b.denominator - b.sign * (b.numerator * a.denominator);
            c.numerator = Math.Abs(k);
            c.denominator = a.denominator * b.denominator;
            c.sign = (k > 0) ? 1 : -1;
            return c;
        }

        public static MyNumber operator -(int a, MyNumber b)
        {
            return new MyNumber(a) - b;
        }

        public static MyNumber operator -(MyNumber a, int b)
        {
            return b - a;
        }



        public static MyNumber operator /(MyNumber a, MyNumber b)

        {
            MyNumber c = new MyNumber();
            c.numerator = a.numerator * b.denominator;
            c.denominator = a.denominator * b.numerator;
            c.sign = a.sign * b.sign;
            return c;
        }

        public static MyNumber operator /(int a, MyNumber b)
        {
            return new MyNumber(a) / b;
        }

        public static MyNumber operator /(MyNumber a, int b)
        {
            return b / a;
        }

        public static MyNumber operator *(MyNumber a, MyNumber b)
        {
            MyNumber c = new MyNumber();
            c.numerator = a.numerator * b.numerator;
            c.denominator = a.denominator * b.denominator;
            c.sign = a.sign * b.sign;
            return c;
        }

        public static MyNumber operator *(int a, MyNumber b)
        {
            return new MyNumber(a) * b;
        }

        public static MyNumber operator *(MyNumber a, int b)
        {
            return b * a;
        }


        // унарный минус

        public static MyNumber operator -(MyNumber a)
        {
            MyNumber res = new MyNumber(-1 * a.numerator * a.sign, a.denominator);
            return res;
        }


        public static MyNumber operator ++(MyNumber a)
        {
            return a + 1;
        }

        
        public static MyNumber operator --(MyNumber a)
        {
            return a - 1;
        }

        public bool Equals(MyNumber that)
        {
            MyNumber a = Reduce();
            MyNumber b = that.Reduce();
            return a.numerator == b.numerator && a.denominator == b.denominator && a.sign == b.sign;
        }

        
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is MyNumber)
            {
                result = this.Equals(obj as MyNumber);
            }

            return result;
        }

        
       
        public static bool operator ==(MyNumber a, MyNumber b)
        {

            
            if (a.numerator == 0 && b.numerator == 0)
                return true;
            

            return a.Equals(b);
        }

       
        public static bool operator ==(MyNumber a, int b)
        {
            return a == new MyNumber(b);
        }

        
        public static bool operator ==(int a, MyNumber b)
        {
            return new MyNumber(a) == b;
        }

       
        public static bool operator !=(MyNumber a, MyNumber b)
        {
            return !(a == b);
        }

       
        public static bool operator !=(MyNumber a, int b)
        {
            return a != new MyNumber(b);
        }

       
        public static bool operator !=(int a, MyNumber b)
        {
            return new MyNumber(a) != b;
        }



        

     
        private int CompareTo(MyNumber that)
        {
            if (Equals(that)|| this==that)
            {
                return 0;
            }
            

            MyNumber a = Reduce();
            MyNumber b = that.Reduce();
            
            if (a.numerator * a.sign * b.denominator > b.numerator * b.sign * a.denominator)
            {
                
                return 1;
            }

            return -1;
        }

      
        public static bool operator >(MyNumber a, MyNumber b)
        {
            return a.CompareTo(b) > 0;
        }

       
        public static bool operator >(MyNumber a, int b)
        {
            return a > new MyNumber(b);
        }

        public static bool operator >(int a, MyNumber b)
        {
            return new MyNumber(a) > b;
        }

        
        public static bool operator <(MyNumber a, MyNumber b)
        {
            return a.CompareTo(b) < 0;
        }

        
        public static bool operator <(MyNumber a, int b)
        {
            return a < new MyNumber(b);
        }

      
        public static bool operator <(int a, MyNumber b)
        {
            return new MyNumber(a) < b;
        }

       
        public static bool operator >=(MyNumber a, MyNumber b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >=(MyNumber a, int b)
        {
            return a >= new MyNumber(b);
        }

 
        public static bool operator >=(int a, MyNumber b)
        {
            return new MyNumber(a) >= b;
        }

       
        public static bool operator <=(MyNumber a, MyNumber b)
        {
            return a.CompareTo(b) <= 0;
        }

   
        public static bool operator <=(MyNumber a, int b)
        {
            return a <= new MyNumber(b);
        }

        
        public static bool operator <=(int a, MyNumber b)
        {
            return new MyNumber(a) <= b;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }



    }
}
