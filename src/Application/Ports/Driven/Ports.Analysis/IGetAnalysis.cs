﻿using OneOf;
using ScriptBee.Domain.Model.Analysis;

namespace ScriptBee.Ports.Analysis;

public interface IGetAnalysis
{
    Task<OneOf<AnalysisInfo, AnalysisDoesNotExistsError>> GetById(
        AnalysisId analysisId,
        CancellationToken cancellationToken = default
    );
}
