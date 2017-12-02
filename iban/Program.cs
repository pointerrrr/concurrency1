using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static System.Threading.Interlocked;

namespace iban
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] line = Console.ReadLine().Split(' ');
            int l = int.Parse(line[0]);
            int b = int.Parse(line[1]);
            int e = int.Parse(line[2]);
            int m = int.Parse(line[3]);
            int p = int.Parse(line[4]);
            int u = int.Parse(line[5]);
            string h = line[6];
            int dif = e - b;
            int remainder = dif % p;
            Thread[] threads = new Thread[p];
            for (int i = 0; i < p; i++)
            {
                switch (u)
                {
                    case 0:
                        threads[i] = new Thread(unused => count(b + (i * dif), b + (i + 1) * dif));
                        threads[i].Start();
                        break;
                    case 1:
                        threads[i] = new Thread(unused => list(e + dif * i, dif));
                        threads[i].Start();
                        break;
                    case 2:
                        threads[i] = new Thread(unused => search(e + dif * i, dif, h));
                        threads[i].Start();
                        break;
                }
            }
        }

        static void count(int start, int end)
        {

        }

        static void list(int start, int dif)
        {

        }

        static void search(int start, int dif, string hash)
        {

        }
    }
}
