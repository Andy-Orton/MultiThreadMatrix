using System;
using System.Collections.Generic;
using System.Threading;

namespace Matrix
{
    class Program
    {
        private static int cols = 115;
        private readonly static string possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*(){}|";
        private static char[] chars = new char[cols+1];
        private readonly static Semaphore sem1 = new Semaphore(0, cols);
        private readonly static Semaphore sem2 = new Semaphore(0, cols);

        private static int repetitions = 100;
        private static List<Thread> threads = new List<Thread>();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting {0} Threads", cols);

            for(int i = 0; i < cols; i++){
                threads.Add(new Thread(new ThreadState(i).ThreadProc));
            }

            

            foreach(var thread in threads){
                thread.Start();
            }

            
            Thread.Sleep(1000);

            for(int i = 0; i < repetitions; i++){
                for (int j = 0; j < cols; j++){
                    sem1.Release(1);
                }
                for(int g = 0; g < cols; g++){
                    sem2.WaitOne();
                }

                Console.WriteLine(chars);
            }
            
            foreach(var thread in threads){
                thread.Interrupt();
            }

            Console.WriteLine("Yay Matrix");
            return;
        }



        class ThreadState{
            public int colId { get; set; }
            public ThreadState(int id)
            {
                colId = id;
            }
            public void ThreadProc(){
                Console.WriteLine("Thread {0}: Started", colId);
                Random rGenerator = new Random();
                int currentThreadProgress = 0;
                bool win = false;
                for(int k = 0; k < cols; k++){
                    sem1.WaitOne();
                    if(rGenerator.Next() % 500 == 0 || win){
                        win = true;
                        chars[colId] = possibleChars[currentThreadProgress];
                        if(possibleChars[currentThreadProgress] == '|'){
                            win = false;
                            currentThreadProgress = 0;
                        }
                        else{
                            currentThreadProgress++;
                        }
                        sem2.Release(1);
                    }
                    else{
                        chars[colId] = ' ';
                        sem2.Release(1);
                    }
                }
            }
        }
    }
}
