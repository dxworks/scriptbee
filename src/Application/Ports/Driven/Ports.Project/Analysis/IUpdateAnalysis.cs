﻿using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Project.Analysis;

public interface IUpdateAnalysis
{
    Task<AnalysisInfo> Update(
        AnalysisInfo analysisInfo,
        CancellationToken cancellationToken = default
    );
}
