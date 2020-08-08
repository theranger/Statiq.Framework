﻿using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Cli;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.App
{
    [Description("Executes the specified pipelines.")]
    public class PipelinesCommand<TSettings> : EngineCommand<TSettings>
        where TSettings : BaseCommandSettings
    {
        public PipelinesCommand(
            IConfiguratorCollection configurators,
            Settings settings,
            IServiceCollection serviceCollection,
            Bootstrapper bootstrapper)
            : base(
                  configurators,
                  settings,
                  serviceCollection,
                  bootstrapper)
        {
        }

        protected override async Task<int> ExecuteEngineAsync(
            CommandContext commandContext,
            TSettings commandSettings,
            IEngineManager engineManager)
        {
            SetPipelines(commandContext, commandSettings, engineManager);
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                return await engineManager.ExecuteAsync(cancellationTokenSource)
                    ? (int)ExitCode.Normal
                    : (int)ExitCode.ExecutionError;
            }
        }

        protected virtual void SetPipelines(
            CommandContext commandContext,
            TSettings commandSettings,
            IEngineManager engineManager)
        {
            PipelinesCommandSettings buildSettings = commandSettings as PipelinesCommandSettings ?? commandContext.Data as PipelinesCommandSettings;
            if (buildSettings is object)
            {
                engineManager.Pipelines = buildSettings.Pipelines;
                engineManager.NormalPipelines = buildSettings.Pipelines is null || buildSettings.Pipelines.Length == 0 || buildSettings.NormalPipelines;
            }
        }
    }
}
