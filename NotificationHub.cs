using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WebApplication
{
    public interface INotificationHub
    {
        Task Notify(string message);
    }
    public class NotificationHub : Hub<INotificationHub>
    {
        public ChannelReader<int> TryStream(
          CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItemsAsync(channel.Writer, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
                     ChannelWriter<int> writer,

                     CancellationToken cancellationToken)
        {
            var count = 10;
            var delay = 1000;

            Exception localException = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    await writer.WriteAsync(i, cancellationToken);

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                localException = ex;
            }

            writer.Complete(localException);
        }

        //public async IAsyncEnumerable<int> TryStream(
        //   [EnumeratorCancellation]
        //    CancellationToken cancellationToken)
        //{
        //    var count = 10;
        //    var delay = 1000;

        //    for (var i = 0; i < count; i++)
        //    {
        //        // Check the cancellation token regularly so that the server will stop
        //        // producing items if the client disconnects.
        //        cancellationToken.ThrowIfCancellationRequested();

        //        yield return i;

        //        // Use the cancellationToken in other APIs that accept cancellation
        //        // tokens so the cancellation can flow down to them.
        //        await Task.Delay(delay, cancellationToken);
        //    }
        //}


        public async IAsyncEnumerable<int> TryAutoStream(
              [EnumeratorCancellation]
                CancellationToken cancellationToken)
        {
            var count = 10;
            var delay = 1000;

            for (var i = 0; i < count; i++)
            {
                // Check the cancellation token regularly so that the server will stop
                // producing items if the client disconnects.
                cancellationToken.ThrowIfCancellationRequested();

                yield return i;

                // Use the cancellationToken in other APIs that accept cancellation
                // tokens so the cancellation can flow down to them.
                await Task.Delay(delay, cancellationToken);
            }
        }

    }
}
