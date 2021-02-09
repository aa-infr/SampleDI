using System;
using System.Threading.Tasks;

namespace Infrabel.ICT.Framework.Extension
{
    //https://stackoverflow.com/questions/32112418/how-to-design-fluent-async-operations
    public static class AsyncExtensions
    {
        public static TR Pipe<T, TR>(this T target, Func<T, TR> func) => func(target);

        public static async Task<TR> PipeAsync<T, TR>(this Task<T> target, Func<T, TR> func) => func(await target);

        public static async Task<TR> PipeAsync<T, TR>(this Task<T> target, Func<T, Task<TR>> func) => await func(await target);
    }
}