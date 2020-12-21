using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlApp
{
    public class MySampler : Sampler
    {
        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            if(samplingParameters.Kind != System.Diagnostics.ActivityKind.Client)
            {
                return new SamplingResult(SamplingDecision.Drop);
            }

            return new SamplingResult(SamplingDecision.RecordAndSample);
        }
    }
}
