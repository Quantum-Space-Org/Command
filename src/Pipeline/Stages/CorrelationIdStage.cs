﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Quantum.CorrelationId;

namespace Quantum.Command.Pipeline.Stages
{
    public class CorrelationIdStage : IAmAPipelineStage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdStage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task Process<T>(T command, StageContext context)
        {
            var correlationId = _httpContextAccessor.GetCorrelationId();
            if (correlationId != null)
            {
                command.SetCorrelationId( correlationId);
            }
        }
    }
}