using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    public class Simpl
    {
        //source - симплекс таблица без базисных переменных
        double[,] table; //симплекс таблица
        int kStrok, kStolb;
        List<int> basis; //список базисных переменных
        public Simpl(double[,] source)
        {
            kStrok = source.GetLength(0);
            kStolb = source.GetLength(1);
            table = new double[kStrok, kStolb + kStrok - 1];
            basis = new List<int>();
            // Добавление фиктивных переменных
            for (int i = 0; i < kStrok; i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (j < kStolb)
                        table[i, j] = source[i, j];
                    else
                        table[i, j] = 0;
                }
                //выставляем коэффициент 1 перед базисной переменной в строке, это для правильного выстраивания фиктивных переменных, проверка
                if ((kStolb + i) < table.GetLength(1))
                {
                    table[i, kStolb + i] = 1;
                    basis.Add(kStolb + i);
                }
            }
            kStolb = table.GetLength(1);
        }
        //result - в этот массив будут записаны полученные значения X
        public double[,] CycleDec(double[] result)
        {
            int mainCol, mainRow; //результирующие столбец и строка
            while (!CyclEnd())
            {
                mainCol = FindMainCol();
                mainRow = FindMainRow(mainCol);
                basis[mainRow] = mainCol;
                double[,] new_table = new double[kStrok, kStolb];
                for (int j = 0; j < kStolb; j++)
                    new_table[mainRow, j] = table[mainRow, j] / table[mainRow, mainCol];
                for (int i = 0; i < kStrok; i++)
                {
                    if (i == mainRow)
                        continue;
                    for (int j = 0; j < kStolb; j++)
                        new_table[i, j] = table[i, j] - table[i, mainCol] * new_table[mainRow, j];
                }
                table = new_table;
            }
            //заносим в result найденные значения X
            for (int i = 0; i < result.Length; i++)
            {
                int k = basis.IndexOf(i + 1);
                if (k != -1)
                    result[i] = table[k, 0];
                else
                    result[i] = 0;
            }
            return table;
        }
        private bool CyclEnd() //остановка программы, если строка оценок меньше 0
        {
            bool flag = true;
            for (int j = 1; j < kStolb; j++)
            {
                if (table[kStrok - 1, j] < 0)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        private int FindMainCol()//Ищем разрешающую столбец
        {
            int mainStolb = 1;
            for (int j = 2; j < kStolb; j++)
                if (table[kStrok - 1, j] < table[kStrok - 1, mainStolb])
                    mainStolb = j;
            Debug.WriteLine("Разрешающий столбец: "+ mainStolb);
            return mainStolb;
        }
        private int FindMainRow(int mainCol)//Ищем разрещающую строку
        {
            int mainRow = 0;
            for (int i = 0; i < kStrok - 1; i++)
                if (table[i, mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }
            for (int i = mainRow + 1; i < kStrok - 1; i++)
                if ((table[i, mainCol] > 0) && ((table[i, 0] / table[i, mainCol]) < (table[mainRow, 0] / table[mainRow, mainCol])))
                    mainRow = i;
            Debug.WriteLine("Разрешающая строка: " + mainRow);
            return mainRow;
        }
    }
    public class ValueZnachenie
    {
        public double[,] MaS;
        public double[] bufMass = { };
        public double[,] Tab_Rezultat;
        /// <summary>
        /// Метод ввода и вывода данных
        /// </summary>
        public void simplexBol()
        {
            double[] MSOnE = { };
            string STROnE = "";
            int RAZOnE = 0, d = 0;
            //Запись из csv в массив
            try
            {
                using (StreamReader SReader = new StreamReader(@"Ввод.csv"))
                {
                    SReader.ReadLine();
                    STROnE = SReader.ReadToEnd();
                    string[] st = STROnE.Split('\n');
                    RAZOnE = st.Length;
                    MSOnE = Array.ConvertAll(st[0].Split(';'), double.Parse);
                    d = MSOnE.Length;
                    MaS = new double[RAZOnE, d];
                    for (int i = 0; i < RAZOnE; i++)
                    {
                        MSOnE = Array.ConvertAll(st[i].Split(';'), double.Parse);
                        for (int j = 0; j < d; j++)
                        {
                            MaS[i, j] = MSOnE[j];

                        }
                    }

                    // Меняем первый и последний столбцы местами для того что бы удобно вводить в csv файл ограничения
                    for (int i = 0; i < RAZOnE; i++)
                    {
                        for (int j = 0; j < d; j += d - 1)
                        {
                            double tmp = MaS[i, j];
                            MaS[i, j] = MaS[i, d - 1];
                            MaS[i, d - 1] = tmp;
                        }

                    }
                    // делаем строку оценок отрицательной для корректного вывода
                    for (int i = 0; i < RAZOnE; i++)
                    {
                        for (int j = 0; j < d; j++)
                        {
                            if (i == RAZOnE - 1)
                            {
                                MaS[i, j] = MaS[i, j] * (-1);
                            }
                        }
                    }
                    Console.WriteLine("Исходная матрица");
                    for (int i = 0; i < RAZOnE; i++)
                    {
                        for (int j = 0; j < d; j++)
                        {
                            Console.Write($"{MaS[i, j],5}");
                        }
                        Console.WriteLine();
                    }
                }

                //Объявляем массив размерностью в два раза больше, чем введенный массив для фиктивных переменных
                double[] result = new double[RAZOnE * 2];
                //Конструктор класса
                Simpl S = new Simpl(MaS);
                //Основной метод программы
                Tab_Rezultat = S.CycleDec(result);
                for (int i = 0; i < Tab_Rezultat.GetLength(0); i++)
                {
                    for (int j = 0; j < Tab_Rezultat.GetLength(1); j++)
                    {
                        if (i == RAZOnE - 1)
                        {
                            Tab_Rezultat[i, j] = Tab_Rezultat[i, j] * (-1);
                        }
                    }
                }
                Console.WriteLine("Решение:");
                for (int i = 0; i < Tab_Rezultat.GetLength(0); i++)
                {
                    for (int j = 0; j < Tab_Rezultat.GetLength(1); j++)
                        Console.Write($"{Math.Round(Tab_Rezultat[i, j]),5}" + ";");
                    Console.WriteLine("");
                }
                int ind1 = 1;
                for (int j = d - 2; j >= 0; j--)
                {
                    Console.WriteLine("X[{0}] = {1}", ind1, result[j]);
                    ind1++;
                }
                Console.WriteLine("F = " + (Tab_Rezultat[Tab_Rezultat.GetLength(0) - 1, 0] * -1));
                Console.WriteLine("F' = " + (Tab_Rezultat[Tab_Rezultat.GetLength(0) - 1, 0]));
                using (StreamWriter sw = new StreamWriter(@"Вывод.csv"))
                {
                    sw.WriteLine("reshenie:");
                    for (int i = 0; i < Tab_Rezultat.GetLength(0); i++)
                    {
                        for (int j = 0; j < Tab_Rezultat.GetLength(1); j++)
                            sw.Write($"{Math.Round(Tab_Rezultat[i, j]),5}" + ";");
                        sw.WriteLine();
                    }
                    ind1 = 1;
                    for (int j = d - 2; j >= 0; j--)
                    {
                        sw.WriteLine("X[{0}] = {1}", ind1, result[j]);
                        ind1++;
                    }
                    sw.WriteLine("F = " + (Tab_Rezultat[Tab_Rezultat.GetLength(0) - 1, 0] * -1));
                    sw.WriteLine("F' = " + (Tab_Rezultat[Tab_Rezultat.GetLength(0) - 1, 0]));
                }
            }
            catch
            {
                Console.WriteLine("В файле ошибка, измените данные");
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(File.CreateText("Промежуточные.txt")));
            Debug.AutoFlush = true;
            ValueZnachenie vz = new ValueZnachenie();
            vz.simplexBol();
            Console.ReadKey();
        }
    }
}
