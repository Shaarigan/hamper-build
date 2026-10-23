// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using System.Text;
using Soe.Composable;
using Soe.Reactive;
using Soe.Runtime;

[assembly: RuntimeModule<Soe.Build.Main>("Build")]
namespace Soe.Build
{
    // ReSharper disable ClassCannotBeInstantiated
    
    /// <summary>
    /// Declares the main module for this tool
    /// </summary>
    #if EXPORT_HAMPER_CORE_SHARP
    public
    #else
    internal
    #endif
    sealed partial class Main : Source<CommandFragment, Main.DispatchingStrategy, Main.SubscriptionStrategy>, IRuntimeModule, IMainModule
    {
        private static readonly Main source;
        private static Shard? buildShard;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Main()
        {
            source = new Main();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Main()
        { }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Load()
        {
            buildShard = new Shard(new PageAllocator());
        }
        
        /// <inheritdoc/>
        public static int Run<ArgsSource>(ref ArgsSource args) 
            where ArgsSource : struct, IStreamSource
        {
            ProcessArgs(ref args);
            return Application.ExitCode;
        }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unload()
        {
            source.strategy.Dispose();
            buildShard?.Dispose();
            buildShard = null;
        }

        /// <summary>
        /// Initializes the configuration based on the argument source
        /// </summary>
        static void ProcessArgs<ArgsSource>(ref ArgsSource args) 
            where ArgsSource : struct, IStreamSource
        {
            try
            {
                using (CommandReader<ArgsSource> reader = new CommandReader<ArgsSource>(args, CommandReaderOptions.AllowVerbValues | CommandReaderOptions.IgnoreCase | CommandReaderOptions.IgnoreMatch))
                {
                    Span<Option> options = GetOptions(out int count).AsSpan().Slice(0, count);
                    while (reader.Read(options))
                    {
                        switch (reader.Fragment.Type)
                        {
                            #region Paths
                            case CommandToken.Verb:
                                {
                                    // TODO Collect paths and setup an entity for each of them
                                }
                                break;
                            #endregion
                            
                            #region Command
                            case CommandToken.Command:
                                if (ProcessFlags(options, reader.Fragment))
                                {
                                    return;
                                }
                                break;
                            #endregion
                        }
                    }
                }
            }
            finally
            {
                source.OnCompleted();
            }
        }
        
        /// <summary>
        /// Performs actions based on the options read
        /// </summary>
        static bool ProcessFlags(Span<Option> options, in CommandFragment fragment)
        {
            switch (fragment.Name)
            {
                #region --log
                case "log":
                    {
                        if (fragment.ToInteger(out int level))
                        {
                            Application.LogService.LogLevel = (SeverityFlags)level;
                        }
                        else if (Enum.TryParse(fragment.RawValue, true, out SeverityFlags severity))
                        {
                            Application.LogService.LogLevel = severity;
                        }
                    }
                    break;
                #endregion
                
                #region --help
                case "help":
                    {
                        ShowManPage(options);
                        source.OnNext(in fragment);
                    }
                    return true;
                #endregion

                #region Anything else
                default:
                    {
                        source.OnNext(in fragment);
                    }
                    break;
                #endregion
            }
            return false;
        }

        /// <summary>
        /// Displays the usage manual to the user
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ShowManPage(Span<Option> options)
        {
            if(Application.ExitCode == 0)
            {
                Console.Write("build [<targetPath...>] ");
            }
            Console.Write(InfoBuilder.Generate(options, header: (Application.ExitCode == 0), terminalWidth: Console.WindowWidth));
        }
    }
    
    // ReSharper restore ClassCannotBeInstantiated
}