using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class PerformanceMetrics
{
    public ErrorCollection absoluteError { get; set; }
    public ErrorCollection relativeError { get; set; }
    private PerformanceMetricAbsError performanceMetricAbsError;
    private PerformanceMetricRelError performanceMetricRelError;
    public PerformanceMetrics()
    {
        absoluteError = new ErrorCollection();
        relativeError = new ErrorCollection();
        performanceMetricAbsError = new PerformanceMetricAbsError();
        performanceMetricRelError = new PerformanceMetricRelError();

    }
    public void UpdateError()
    {
        Tuple<float, float, float> errorStatistics;
        errorStatistics = performanceMetricAbsError.GetAbsoluteError();
        absoluteError.min = errorStatistics.Item1;
        absoluteError.max = errorStatistics.Item2;
        absoluteError.average = errorStatistics.Item3;
        errorStatistics = performanceMetricRelError.GetRelativeError();
        relativeError.min = errorStatistics.Item1;
        relativeError.max = errorStatistics.Item2;
        relativeError.average = errorStatistics.Item3;
    }
}
