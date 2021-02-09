using System;
using System.Threading;

namespace Infrabel.ICT.Framework.Extension
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static T ReadExecute<T>(this ReaderWriterLockSlim locker, Func<T> func)
        {
            if(func == null)
                throw new ArgumentNullException(nameof(func));
            try
            {
                locker.EnterReadLock();
                return func();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static void ReadExecute(this ReaderWriterLockSlim locker, Action action)
        {
            ReadExecute(locker, () =>
                                {
                                    action();
                                    return 0;
                                });
        }

        public static T UpgradableReadExecute<T>(this ReaderWriterLockSlim locker, Func<T> func)
        {
            if(func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                locker.EnterUpgradeableReadLock();
                return func();
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public static void UpgradableReadExecute(this ReaderWriterLockSlim locker, Action action)
        {
            UpgradableReadExecute(locker, () =>
                                          {
                                              action();
                                              return 0;
                                          });
        }

        public static T WriteExecute<T>(this ReaderWriterLockSlim locker, Func<T> func)
        {
            if(func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                locker.EnterWriteLock();
                return func();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public static void WriteExecute(this ReaderWriterLockSlim locker, Action action)
        {
            WriteExecute(locker, () =>
                                 {
                                     action();
                                     return 0;
                                 });
        }

        public static T UpgradableWriteExecute<T>(this ReaderWriterLockSlim locker, Func<T> func)
        {
            if(!locker.IsUpgradeableReadLockHeld)
                throw new InvalidOperationException();

            return WriteExecute(locker, func);
        }

        public static void UpgradableWriteExecute(this ReaderWriterLockSlim locker, Action action)
        {
            UpgradableWriteExecute(locker, () =>
                                           {
                                               action();
                                               return 0;
                                           });
        }
    }
}