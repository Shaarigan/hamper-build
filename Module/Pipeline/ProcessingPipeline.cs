// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Collections.Embedded;
using Soe.Composable;

namespace Soe.Build
{
    /// <summary>
    /// 
    /// </summary>
    #if EXPORT_HAMPER_CORE_SHARP
    public
    #else
    internal
    #endif
    static partial class ProcessingPipeline
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ProcessingPipeline()
        {
            discoveryAdapter = new PipelineAdapter();
            parsingAdapter = new PipelineAdapter();
            lintingAdapter = new PipelineAdapter();
            analysisAdapter = new PipelineAdapter();
            classificationAdapter = new PipelineAdapter();
            buildAdapter = new PipelineAdapter();
            exportAdapter = new PipelineAdapter();
        }
        
        private static ProcessingPhase current;
        /// <summary>
        /// Gets the current pipeline phase
        /// </summary>
        public static ProcessingPhase Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return current; }
        }

        /// <summary>
        /// Gets the current version of the provided processing stream
        /// </summary>
        /// <param name="phase">The <see cref="ProcessingPhase"/> indicating the stream</param>
        /// <returns>The current version of the provided processing stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCurrentVersion(ProcessingPhase phase)
        {
            switch (phase)
            {
                #region Discovery
                case ProcessingPhase.Discovery: return discoveryAdapter.Version;
                #endregion

                #region Parsing
                case ProcessingPhase.Parsing: return parsingAdapter.Version;
                #endregion

                #region Linting
                case ProcessingPhase.Linting: return lintingAdapter.Version;
                #endregion

                #region Analysis
                case ProcessingPhase.Analysis: return analysisAdapter.Version;
                #endregion
                
                #region Classification
                case ProcessingPhase.Classification: return classificationAdapter.Version;
                #endregion

                #region Build
                case ProcessingPhase.BuildOrchestration: return buildAdapter.Version;
                #endregion

                #region Export
                case ProcessingPhase.Export: return exportAdapter.Version;
                #endregion
                
                #region Anything else
                default: return -1;
                #endregion
            }
        }

        /// <summary>
        /// Updates the processing pipeline until all objects have been finished
        /// </summary>
        internal static async Task Update(Component<ProcessingComponent> processingComponent)
        {
            do
            {
                // Process the most recent phase, then evaluate
                if (discoveryAdapter.Count > 0)
                {
                    current = ProcessingPhase.Discovery;
                    await discoveryAdapter.Flush();
                }
                else if (parsingAdapter.Count > 0)
                {
                    current = ProcessingPhase.Parsing;
                    await parsingAdapter.Flush();
                }
                else if (lintingAdapter.Count > 0)
                {
                    current = ProcessingPhase.Linting;
                    await lintingAdapter.Flush();
                }
                else if (analysisAdapter.Count > 0)
                {
                    current = ProcessingPhase.Analysis;
                    await analysisAdapter.Flush();
                }
                else if (classificationAdapter.Count > 0)
                {
                    current = ProcessingPhase.Classification;
                    await classificationAdapter.Flush();
                }
                else if (buildAdapter.Count > 0)
                {
                    current = ProcessingPhase.BuildOrchestration;
                    await buildAdapter.Flush();
                }
                else if (exportAdapter.Count > 0)
                {
                    current = ProcessingPhase.Export;
                    await exportAdapter.Flush();
                }
                SystemUpdate();
                void SystemUpdate()
                {
                    ReadOnlySpan<EntityId> entities = processingComponent.AsReadOnlySpan();
                    ReadOnlySpan<ProcessingComponent> components = processingComponent.AsSpan();
                    for (int i = 0; i < entities.Length; i++)
                    {
                        if (GetCurrentVersion(components[i].Phase) != components[i].Version)
                        {
                            processingComponent.Remove(entities[i--]);
                            entities = processingComponent.AsReadOnlySpan();
                            components = processingComponent.AsSpan();
                        }
                        else switch (components[i].Phase)
                        {
                            #region Discovery
                            case ProcessingPhase.Discovery:
                                {
                                    discoveryAdapter.Add(entities[i]);
                                }
                                break;
                            #endregion
                            
                            #region Parsing
                            case ProcessingPhase.Parsing:
                                {
                                    parsingAdapter.Add(entities[i]);
                                }
                                break;
                            #endregion
                            
                            #region Linting
                            case ProcessingPhase.Linting:
                                {
                                    lintingAdapter.Add(entities[i]);
                                }
                                break;
                            #endregion
                            
                            #region Analysis
                            case ProcessingPhase.Analysis:
                                {
                                    analysisAdapter.Add(entities[i]);
                                }
                                break;
                            #endregion
                            
                            #region Classification
                            case ProcessingPhase.Classification:
                                {
                                    classificationAdapter.Add(entities[i]);
                                }
                                break;
                            #endregion
                            
                            #region Build
                            case ProcessingPhase.BuildOrchestration:
                                {
                                    buildAdapter.Add(entities[i]);
                                }
                                break;
                            #endregion
                            
                            #region Export
                            case ProcessingPhase.Export:
                                {
                                    exportAdapter.Add(entities[i]);
                                }
                                break;
                            #endregion
                        }
                    }
                }
            }
            // Loop while entities need to be processed
            while (discoveryAdapter.Count > 0 ||
                   parsingAdapter.Count > 0 ||
                   lintingAdapter.Count > 0 ||
                   analysisAdapter.Count > 0 ||
                   classificationAdapter.Count > 0 ||
                   buildAdapter.Count > 0 ||
                   exportAdapter.Count > 0);
            
            discoveryAdapter.Dispose();
            parsingAdapter.Dispose();
            lintingAdapter.Dispose();
            analysisAdapter.Dispose();
            classificationAdapter.Dispose();
            buildAdapter.Dispose();
            exportAdapter.Dispose();
        }
    }
}