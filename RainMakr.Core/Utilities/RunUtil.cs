namespace RainMakr.Core.Utilities
{
    using System.Threading;

    public static class RunUtil
    {
        /// <summary>
        ///     Empty while loop to keep the thread running
        /// </summary>
        public static void KeepRunning()
        {
            while (true)
            {
                Thread.Sleep(250);
            }
        }
    }
}