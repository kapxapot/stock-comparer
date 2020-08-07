using System;
using System.Threading;
using System.Threading.Tasks;
using StockComparer.Exceptions;

namespace StockComparer.Infrastructure
{
    public class Retrier
    {
        public static async Task<T> Retry<T>(Func<Task<T>> func, Action<string> log = null, int retries = 6, int initialDelay = 2000)
        {
            var delay = initialDelay;
            var retriesLeft = retries;
            var attempt = 1;

            while (retriesLeft > 0)
            {
                if (log != null)
                {
                    log($"Executing func, attempt: {attempt++}");
                }

                try
                {
                    return await func();
                }
                catch (CriticalException)
                {
                    throw;
                }
                catch
                {
                    retriesLeft--;

                    if (retriesLeft == 0)
                    {
                        throw;
                    }

                    Thread.Sleep(delay);

                    delay *= 2;
                }
            }

            throw new Exception("No retries left");
        }
    }
}
