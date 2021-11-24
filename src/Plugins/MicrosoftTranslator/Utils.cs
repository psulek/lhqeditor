// /* Copyright (C) 2021 ScaleHQ Solutions s.r.o. - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Peter Šulek <peter.sulek@scalehq.sk> / ScaleHQ Solutions company
//  */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace LHQ.Plugin.MicrosoftTranslator
{
    public static class Utils
    {
        public static Task ParallelForEachAsync<T>(IEnumerable<T> source, Func<T, Task> body)
        {
            return Task.WhenAll(
                from item in source
                select Task.Run(async () => await body(item)));
        }

        public static Task ParallelPartitionForEachAsync<T>(IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            async Task AwaitPartition(IEnumerator<T> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    { await body(partition.Current); }
                }
            }
            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(dop)
                    .AsParallel()
                    .Select(AwaitPartition));
        }
    }
}
