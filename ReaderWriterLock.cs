using System;
using System.Threading;

namespace ReaderWriterLock
{
    /// <summary>
    /// Driver class 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            TestClass obj = new TestClass();

            // Start some reader threads
            for (int i = 0; i < 5; i++)
            {
                Thread readerThread = new Thread(new ThreadStart(obj.Writer));
                readerThread.Start();
            }

            // Start few writer threads
            for (int i = 0; i < 5; i++)
            {
                Thread writerThread = new Thread(new ThreadStart(obj.Reader));
                writerThread.Start();
            }

            Console.Read();
        }

    }

    /// <summary>
    /// Test class who's read and write methods need to be synchronized
    /// </summary>
    class TestClass
    {
        ReaderWriterLock m_rwLock = new ReaderWriterLock();

        public void Writer()
        {
            Console.WriteLine("Writer Thread Starting - " + Thread.CurrentThread.ManagedThreadId);
            m_rwLock.AcquireWriterLock();
            // Simulate Work
            Thread.Sleep(1000 * 5);
            m_rwLock.ReleaseWriterLock();

            Console.WriteLine("Writer Thread Ending - " + Thread.CurrentThread.ManagedThreadId);
        }

        public void Reader()
        {
            Console.WriteLine("Reader Thread Starting - " + Thread.CurrentThread.ManagedThreadId);
            m_rwLock.AcquireReaderLock();
            // Simulate Work
            Thread.Sleep(1000 * 5);
            m_rwLock.ReleaseReaderLock();

            Console.WriteLine("Reader Thread Ending - " + Thread.CurrentThread.ManagedThreadId);
        }
    }


    /// <summary>
    /// ReaderWriter lock class implementation
    /// </summary>
    public class ReaderWriterLock
    {
        private int nReadersReading = 0;
        private bool bIsWriterWriting = false;
        private object syncRoot = new Object();

        public void AcquireReaderLock()
        {
            // Enter critical section
            Monitor.Enter(syncRoot);

            // If a writer is writing wait until it finishes 
            while (bIsWriterWriting)
            {
                Monitor.Wait(syncRoot);
            }

            // Increase active readers count by 1
            nReadersReading++;
            // Print to console for testing purpose
            Console.WriteLine("Acquired Reader's lock - " + Thread.CurrentThread.ManagedThreadId);
            // Exit critical section
            Monitor.Exit(syncRoot);

        }

        public void ReleaseReaderLock()
        {
            // Enter critical section
            Monitor.Enter(syncRoot);

            // Decrease active readers count by 1
            nReadersReading--;

            // If there are no active readers signal waiting writer threads
            if (nReadersReading == 0)
            {
                Monitor.PulseAll(syncRoot);
            }
            // Print to console for testing purpose
            Console.WriteLine("Released Reader's lock - " + Thread.CurrentThread.ManagedThreadId);
            // Exit critical section
            Monitor.Exit(syncRoot);

        }

        public void AcquireWriterLock()
        {
            // Enter critical section
            Monitor.Enter(syncRoot);

            // If there are active readers or a writer is still writing wait for the signal
            while ((nReadersReading != 0) || (bIsWriterWriting == true))
            {
                Monitor.Wait(syncRoot);
            }

            // Acquire the lock
            bIsWriterWriting = true;
            Console.WriteLine("Acquired Writers's lock - " + Thread.CurrentThread.ManagedThreadId);
            // Exit critical section
            Monitor.Exit(syncRoot);

        }

        public void ReleaseWriterLock()
        {
            // Enter critical section
            Monitor.Enter(syncRoot);

            // Release writer lock
            bIsWriterWriting = false;

            // Signal the Readers as well as writers
            Monitor.PulseAll(syncRoot);
            Console.WriteLine("Released Writer's lock - " + Thread.CurrentThread.ManagedThreadId);
            // Exit critical section
            Monitor.Exit(syncRoot);
        }
    }
}
