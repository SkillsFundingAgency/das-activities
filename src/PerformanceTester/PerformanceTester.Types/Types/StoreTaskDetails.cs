// Decompiled with JetBrains decompiler
// Type: PerformanceTester.Types.Types.StoreTaskDetails
// Assembly: PerformanceTester.Types, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 811E2CD7-D50B-4F41-93E3-583B7CD698DE
// Assembly location: C:\temp\scrap\PerformanceTester\PerformanceTester\bin\Debug\PerformanceTester.Types.dll

using PerformanceTester.Types.Interfaces;
using System;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Types
{
    public class StoreTaskDetails
    {
        public StoreTaskDetails(IStoreCommand command, IStore store, Task task)
        {
            this.Command = command;
            this.Store = store;
            this.Task = task;
        }

        public IStoreCommand Command { get; }

        public IStore Store { get; }

        public Task Task { get; }

        public bool Success
        {
            get
            {
                Task task = this.Task;
                if (task == null)
                    return false;
                return task.Status == TaskStatus.RanToCompletion;
            }
        }

        public TimeSpan Elapsed { get; set; }

        public IOperationCost Cost { get; set; }
    }
}
