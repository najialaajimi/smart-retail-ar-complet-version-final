using System;
using UnityEngine;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Performance monitoring utility for tracking KPIs.
    /// Sprint 4: Performance testing and monitoring.
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        private static PerformanceMonitor _instance;
        public static PerformanceMonitor Instance => _instance;
        
        [Header("Monitoring Settings")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private float updateInterval = 1f;
        [SerializeField] private bool showOnScreenStats = false;
        
        [Header("Performance Thresholds")]
        [SerializeField] private float targetFPS = 30f;
        [SerializeField] private float maxMemoryMB = 500f;
        [SerializeField] private float maxLatencyMs = 1000f;
        
        // Performance metrics
        private float currentFPS;
        private float averageFPS;
        private float minFPS;
        private float maxFPS;
        private float memoryUsageMB;
        private int frameCount;
        private float fpsAccumulator;
        private float lastUpdateTime;
        
        // Latency tracking
        private float lastOperationLatency;
        private float averageLatency;
        private int latencyMeasurements;
        
        public float CurrentFPS => currentFPS;
        public float AverageFPS => averageFPS;
        public float MemoryUsageMB => memoryUsageMB;
        public float LastLatencyMs => lastOperationLatency;
        public float AverageLatencyMs => averageLatency;
        
        public event Action<PerformanceData> OnPerformanceUpdate;
        public event Action<string> OnPerformanceWarning;
        
        private void Awake()
        {
            _instance = this;
            ResetMetrics();
        }
        
        private void Update()
        {
            if (!enableMonitoring) return;
            
            // Calculate FPS
            frameCount++;
            fpsAccumulator += Time.unscaledDeltaTime;
            currentFPS = 1f / Time.unscaledDeltaTime;
            
            // Update min/max FPS
            if (currentFPS < minFPS) minFPS = currentFPS;
            if (currentFPS > maxFPS) maxFPS = currentFPS;
            
            // Periodic update
            if (Time.unscaledTime - lastUpdateTime >= updateInterval)
            {
                // Calculate average FPS
                averageFPS = frameCount / fpsAccumulator;
                
                // Get memory usage
                memoryUsageMB = GC.GetTotalMemory(false) / (1024f * 1024f);
                
                // Check thresholds and emit warnings
                CheckThresholds();
                
                // Emit update event
                OnPerformanceUpdate?.Invoke(GetPerformanceData());
                
                // Reset for next interval
                frameCount = 0;
                fpsAccumulator = 0;
                lastUpdateTime = Time.unscaledTime;
            }
        }
        
        /// <summary>
        /// Resets all performance metrics.
        /// </summary>
        public void ResetMetrics()
        {
            frameCount = 0;
            fpsAccumulator = 0;
            currentFPS = 0;
            averageFPS = 0;
            minFPS = float.MaxValue;
            maxFPS = 0;
            memoryUsageMB = 0;
            lastOperationLatency = 0;
            averageLatency = 0;
            latencyMeasurements = 0;
            lastUpdateTime = Time.unscaledTime;
        }
        
        /// <summary>
        /// Records latency for an operation.
        /// </summary>
        public void RecordLatency(float latencyMs)
        {
            lastOperationLatency = latencyMs;
            
            // Calculate running average
            latencyMeasurements++;
            averageLatency = ((averageLatency * (latencyMeasurements - 1)) + latencyMs) / latencyMeasurements;
            
            // Check threshold
            if (latencyMs > maxLatencyMs)
            {
                OnPerformanceWarning?.Invoke($"High latency detected: {latencyMs:F0}ms (threshold: {maxLatencyMs}ms)");
            }
        }
        
        /// <summary>
        /// Starts a latency measurement.
        /// Returns an ID to use when stopping the measurement.
        /// </summary>
        public float StartLatencyMeasurement()
        {
            return Time.realtimeSinceStartup;
        }
        
        /// <summary>
        /// Stops a latency measurement and records the result.
        /// </summary>
        public float StopLatencyMeasurement(float startTime)
        {
            float latencyMs = (Time.realtimeSinceStartup - startTime) * 1000f;
            RecordLatency(latencyMs);
            return latencyMs;
        }
        
        /// <summary>
        /// Checks performance against thresholds and emits warnings.
        /// </summary>
        private void CheckThresholds()
        {
            if (averageFPS < targetFPS)
            {
                OnPerformanceWarning?.Invoke($"Low FPS: {averageFPS:F1} (target: {targetFPS})");
            }
            
            if (memoryUsageMB > maxMemoryMB)
            {
                OnPerformanceWarning?.Invoke($"High memory usage: {memoryUsageMB:F1}MB (threshold: {maxMemoryMB}MB)");
            }
        }
        
        /// <summary>
        /// Gets current performance data.
        /// </summary>
        public PerformanceData GetPerformanceData()
        {
            return new PerformanceData
            {
                currentFPS = currentFPS,
                averageFPS = averageFPS,
                minFPS = minFPS,
                maxFPS = maxFPS,
                memoryUsageMB = memoryUsageMB,
                lastLatencyMs = lastOperationLatency,
                averageLatencyMs = averageLatency,
                timestamp = DateTime.Now
            };
        }
        
        /// <summary>
        /// Gets a formatted performance report.
        /// </summary>
        public string GetPerformanceReport()
        {
            var data = GetPerformanceData();
            
            return $@"=== Performance Report ===
Timestamp: {data.timestamp:yyyy-MM-dd HH:mm:ss}

Frame Rate:
  Current: {data.currentFPS:F1} FPS
  Average: {data.averageFPS:F1} FPS
  Min: {data.minFPS:F1} FPS
  Max: {data.maxFPS:F1} FPS
  Status: {(data.averageFPS >= targetFPS ? "✓ OK" : "⚠ Below Target")}

Memory:
  Usage: {data.memoryUsageMB:F1} MB
  Status: {(data.memoryUsageMB < maxMemoryMB ? "✓ OK" : "⚠ High")}

Latency:
  Last: {data.lastLatencyMs:F1} ms
  Average: {data.averageLatencyMs:F1} ms
  Status: {(data.averageLatencyMs <= maxLatencyMs ? "✓ OK" : "⚠ High")}
============================";
        }
        
        private void OnGUI()
        {
            if (!showOnScreenStats) return;
            
            int y = 10;
            int lineHeight = 20;
            
            GUI.Label(new Rect(10, y, 300, lineHeight), $"FPS: {currentFPS:F1} (Avg: {averageFPS:F1})");
            y += lineHeight;
            GUI.Label(new Rect(10, y, 300, lineHeight), $"Memory: {memoryUsageMB:F1} MB");
            y += lineHeight;
            GUI.Label(new Rect(10, y, 300, lineHeight), $"Latency: {lastOperationLatency:F1} ms");
        }
    }
    
    /// <summary>
    /// Performance data structure.
    /// </summary>
    [Serializable]
    public struct PerformanceData
    {
        public float currentFPS;
        public float averageFPS;
        public float minFPS;
        public float maxFPS;
        public float memoryUsageMB;
        public float lastLatencyMs;
        public float averageLatencyMs;
        public DateTime timestamp;
    }
}
