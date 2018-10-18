using CommandLine;
using PerformanceTester.Types.Commands;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using PerformanceTester.CommandLIne;
using PerformanceTester.Types;
using PerformanceTester.Types.Parameters;
using PerformanceTester.Types.ResultLogger;

namespace PerformanceTester
{
    internal class Program
    {
        private readonly IContainer _container;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<PopulateCommandLineArguments, FetchCommandLineArguments, AggregateCommandLineArguments, StoresCommandLineArguments>((IEnumerable<string>)args)
                .WithParsed<PopulateCommandLineArguments>(commandLineArguments => new Program().Populate(commandLineArguments))
                .WithParsed<FetchCommandLineArguments>(commandLineArguments => new Program().Fetch(commandLineArguments))
                .WithParsed<AggregateCommandLineArguments>(commandLineArguments => new Program().Aggregate(commandLineArguments))
                .WithParsed<StoresCommandLineArguments>(commandLineArguments => new Program().ListStores(commandLineArguments))
                .WithNotParsed<object>(parserResult =>
                {
                    Console.WriteLine("The command line is incorrect:");
                    foreach (Error error in parserResult)
                    {
                        Console.WriteLine((object) error.Tag);
                    }
                });
        }

        public Program()
        {
            _container = IoC.InitializeIoC();
        }

        private void Populate(PopulateCommandLineArguments args)
        {
            EnableStores(args);
            SetConfigOverrides<PopulateActivitiesParameters>(p => p.NumberOfAccountsRequired = args.NumberOfAccounts, args.NumberOfAccounts > 0);
            SetConfigOverrides<PopulateActivitiesParameters>(p => p.NumberOfActivitiesPerAccount = args.NumberOfActivitiesPerAccount, args.NumberOfActivitiesPerAccount> 0);
            SetConfigOverrides<PopulateActivitiesParameters>(p => p.NumberOfActivitiesPerDay = args.NumberOfActivitiesPerDay, args.NumberOfActivitiesPerDay > 0);
            RunCommand<PopulateStores>();
        }

        private void Fetch(FetchCommandLineArguments args)
        {
            EnableStores(args);
            SetConfigOverrides<FetchActivitiesParameters>(p => p.AccountIds = args.AccountIds, !string.IsNullOrWhiteSpace(args.AccountIds));
            RunCommand<FetchActivities>();
        }

        private void Aggregate(AggregateCommandLineArguments args)
        {
            EnableStores(args);
            SetConfigOverrides<AggregateActivitiesParameters>(p => p.AccountIds = args.AccountIds, !string.IsNullOrWhiteSpace(args.AccountIds));
            RunCommand<AggregateActivities>();
        }

        private void ListStores(StoresCommandLineArguments args)
        {
            RunCommand<ListStores>();
        }

        private void SetConfigOverrides<TConfigType>(Action<TConfigType> setter, bool setCondition) where TConfigType : class, new()
        {
            if (setCondition)
            {
                var configProvider = _container.GetInstance<IConfigProvider>();
                var config = configProvider.Get<TConfigType>();
                setter(config);
            }
        }

        private void RunCommand<TCommand>() where TCommand : ICommand
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var task = StartCommand<TCommand>(cancellationTokenSource.Token);

            WaitForCommandToCompleteOrCancel(task, cancellationTokenSource);

            LogResults(task);
        }

        private Task<RunDetails> StartCommand<TCommand>(CancellationToken cancellationToken) where TCommand : ICommand
        {
            var command = _container.GetInstance<TCommand>();
            var task = command.DoAsync(cancellationToken);
            Console.WriteLine("Task executing - waiting for it to finish");
            return task;
        }

        private void WaitForCommandToCompleteOrCancel(Task commandTask, CancellationTokenSource cancellationTokenSource)
        {
            StartWaitingForManualCancelAsync(cancellationTokenSource);

            commandTask.Wait(cancellationTokenSource.Token);

            cancellationTokenSource.Cancel(false);
        }

        private void LogResults(Task<RunDetails> task)
        {
            LogSummary(task);
            LogDetail(task);
        }

        private void LogSummary(Task<RunDetails> task)
        {
            if (task.IsCompleted)
                Console.WriteLine("The task ran to completion");

            if (task.IsCanceled)
                Console.WriteLine("The task was cancelled");

            if (task.IsFaulted)
            {
                Console.WriteLine($"The task faulted - {task.Exception.GetType().Name} - {task.Exception.Message}");
            }
        }

        private void LogDetail(Task<RunDetails> task)
        {
            if (task.IsFaulted)
                return;

            var loggers = _container.GetInstance<IResultLogger[]>();
            var rd = new RunDetailsVisitor();
            rd.Visit(task.Result, loggers);
        }

        private Task StartWaitingForManualCancelAsync(CancellationTokenSource cancellationTokenSource)
        {
            return Task.Run((Action)(() =>
            {
                Console.WriteLine("press escape to cancel command");
                while (Console.ReadKey(true).Key != ConsoleKey.Escape && !cancellationTokenSource.IsCancellationRequested)
                    Console.WriteLine("Key ignored - press escape to quit");
                cancellationTokenSource.Cancel(false);
            }), cancellationTokenSource.Token);
        }

        private void EnableStores(StoreFilteringCommandLine commandLine)
        {
            IStoreRepository instance = _container.GetInstance<IStoreRepository>();
            if (commandLine.EnableStore.Any<string>())
            {
                foreach (string name in commandLine.EnableStore)
                    instance.EnableStore(name);
            }
            else
                instance.EnableAllStores();
        }
    }
}
