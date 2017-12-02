using System;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using static System.Threading.Interlocked;


namespace iban
{
    static class Program
    {
        // global variables
        static int counter = 0;
        static int lockmode, mtestnumber;
        static int foundnr = -1;
        static bool found = false;
        static Thread[] threads;
        static TaS_Lock taslock = new TaS_Lock();
        // object used for c# locking
        static object locker = new object();

        static void Main(string[] args)
        {
            // parse info
            string[] line = Console.ReadLine().Split(' ');
            lockmode = int.Parse(line[0]);
            int b = int.Parse(line[1]);
            int e = int.Parse(line[2]);
            mtestnumber = int.Parse(line[3]);
            int p = int.Parse(line[4]);
            int u = int.Parse(line[5]);
            string h = line[6];
            threads = new Thread[p];
            // setup info for threads
            long dif = e - b;
            // initialize threads
            for (int i = 0; i < p; i++)
            {
                int start = b + (int)((dif * i) / p);
                int end = b + (int)((dif * (i + 1)) / p);
                switch (u)
                {
                    case 0:
                        threads[i] = new Thread(unused => count(start, end));
                        break;
                    case 1:
                        threads[i] = new Thread(unused => list(start, end));
                        break;
                    case 2:
                        threads[i] = new Thread(unused => search(start, end, h));
                        break;
                }
            }
            // runs threads
            foreach (Thread t in threads)
                t.Start();
            // wait for threads to finish
            foreach (Thread t in threads)
                t.Join();
            // print info, if necessary
            switch (u)
            {
                case 0:
                    Console.Write(counter);
                    break;
                case 2:
                    Console.Write(foundnr);
                    break;
                default:
                    break;
            }
        }

        // count all numbers in given range that satisfy the m-test
        static void count(int start, int end)
        {
            for (int i = start; i < end; i++)
                if (mtest(i))
                    count();
        }
        
        // prints all number that satisfy the m-test in the given range
        static void list(int start, int end)
        {
            for (int i = start; i < end; i++)
                if (mtest(i))
                    print(i);
        }
        
        // tries to find the iban that matches with the given hash in the given range
        static void search(int start, int end, string hash)
        {
            SHA1 sha = SHA1.Create();
            byte[] hashArray;
            string newHash;
            for(int i = start; i < end; i++)
            {
                if (!found)
                {
                    if (mtest(i))
                    {
                        // hashcode from course website
                        hashArray = sha.ComputeHash(Encoding.ASCII.GetBytes(i.ToString()));
                        newHash = "";
                        for (int hashPos = 0; hashPos < hashArray.Length; hashPos++)
                            newHash += hashArray[hashPos].ToString("x2");
                        if (newHash == hash)
                        {
                            foundnr = i;
                            found = true;
                        }
                    }
                }
                else
                    break;
            }
        }

        // check if a number holds for the m-test
        static bool mtest(int nr)
        {
            string number = nr.ToString();
            int total = 0;
            for (int i = 1; i <= number.Length; i++)
                total += (number[number.Length-i] - '0') * i;
            return total % mtestnumber == 0;
        }

        // increase the counter of found numbers, while making sure no other thread does the same
        static void count()
        {
            if (lockmode == 0)
            {
                taslock.Lock();
                counter++;
                taslock.Unlock();
            }
            else
                lock (locker)
                {
                    counter++;
                }
        }

        // print a number, while making sure no other thread also prints a number at the same time
        static void print(int nr)
        {
            if (lockmode == 0)
            {
                taslock.Lock();
                counter++;
                Console.WriteLine(counter + " " + nr);
                taslock.Unlock();
            }
            else
                lock (locker)
                {
                    counter++;
                    Console.WriteLine(counter + " " + nr);
                }
        }
    }

    public class TaS_Lock
    {
        private int locked = 0;

        // wait until the lock is free and take it when it is, in an atomic operation
        public void Lock()
        {
            while(0 != CompareExchange(ref locked, 1, 0))
            { }
        }

        // unlock the lock
        public void Unlock()
        {
            Exchange(ref locked, 0);
        }
    }
}
