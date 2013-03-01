using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AddressMatch;
using AddressMatch.Training;

namespace AddressMatch.Test
{
    public class ConcurrentTest
    {
        private static void Match()
        {
            AddrSet addrset = AddrSet.GetInstance();

            //Match
            MatchMachine m = new MatchMachine(addrset);

            MatchResult result = m.Match(new string[] { "B" });

            MatchHelper.rwLockDashboard(addrset);


        }
        private static void Train()
        {
            AddrSet addrset = AddrSet.GetInstance();

            TrainMachine t = new TrainMachine(addrset);

            List<InsertElement> list = new List<InsertElement>();

            Random rnd = new Random();

            list.Add(new InsertElement(rnd.Next().ToString("0.00"), LEVEL.City, InsertMode.AutoPlace | InsertMode.ExactlyLevel));

            t.Train(list, true);

            MatchHelper.rwLockDashboard(addrset);


        }

        public static void Test()
        {

            Thread[] thread_match = new Thread[10];
            for (int i = 0; i < 10;i++ )
            {
                thread_match[i] = new Thread(new ThreadStart(Match));
                thread_match[i].Start();
            }
            Thread[] thread_train = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                thread_train[i] = new Thread(new ThreadStart(Train));
                thread_train[i].Start();
            }
            for (int i = 0; i < 10; i++)
            {
                thread_match[i].Join();
                thread_train[i].Join();
            }
            AddrSet addrset = AddrSet.GetInstance();
            Console.WriteLine("end");
        }

        public static void Test2()
        {
            Train();
            Train();
        }


    }

    public class ReaderWriterLockSlimDemo
    {
        static ReaderWriterLockSlim rw = new ReaderWriterLockSlim();
        static List<int> items = new List<int>();
        static Random rand = new Random();

        public static void Test()
        {
            new Thread(Write).Start("A");
            new Thread(Write).Start("B");
            new Thread(Read).Start();
            new Thread(Read).Start();
            new Thread(Read).Start();

        }

        static void Read()
        {
            while (true)
            {
                rw.EnterReadLock();
                foreach (int i in items) Thread.Sleep(10);
                Thread currentThread = Thread.CurrentThread;
                Console.WriteLine("==DEBUG== CurrentReadThreadsCount =   " + rw.CurrentReadCount +
                        " , currentThread = " + currentThread.ManagedThreadId);
                Console.WriteLine("==DEBUG== WaitingReadThreadsCount =   " + rw.WaitingReadCount +
                        " , currentThread = " + currentThread.ManagedThreadId);
                Console.WriteLine("==DEBUG== WaitingWriteThreadsCount =   " + rw.WaitingWriteCount +
                        " , currentThread = " + currentThread.ManagedThreadId);
                Console.WriteLine("==DEBUG== //////////////");
                rw.ExitReadLock();
            }
        }

        static void Write(object threadID)
        {
            while (true)
            {
                int newNumber = GetRandNum(100);
                rw.EnterWriteLock();
                items.Add(newNumber);
                rw.ExitWriteLock();
                Console.WriteLine("Thread " + threadID + " added " + newNumber);
                
            }
        }

        static int GetRandNum(int max)
        {
            //lock (rand) ;
            return rand.Next(max);
        }
    }  


}
