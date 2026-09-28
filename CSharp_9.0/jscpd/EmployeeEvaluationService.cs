using System;
using System.Collections.Generic;

namespace NineBlock.Core.Services
{
    public class EmployeeEvaluationService
    {
        public string EvaluateEmployee(string employeeId, double performanceScore, double potentialScore)
        {
            // Boundary validation: Scores must be between 1.0 and 5.0
            if (performanceScore < 1.0 || performanceScore > 5.0)
            {
                throw new ArgumentOutOfRangeException(nameof(performanceScore), "Performance score must be between 1.0 and 5.0");
            }

            if (potentialScore < 1.0 || potentialScore > 5.0)
            {
                throw new ArgumentOutOfRangeException(nameof(potentialScore), "Potential score must be between 1.0 and 5.0");
            }

            int perfLevel = performanceScore switch
            {
                < 3.0 => 1, // Low
                < 4.0 => 2, // Medium
                _ => 3      // High
            };

            int potLevel = potentialScore switch
            {
                < 3.0 => 1, // Low
                < 4.0 => 2, // Medium
                _ => 3      // High
            };

            return (perfLevel, potLevel) switch
            {
                (3, 3) => "Star",
                (2, 3) => "Growth Potential",
                (1, 3) => "Enigma",
                (3, 2) => "High Performer",
                (2, 2) => "Core Player",
                (1, 2) => "Dilemma",
                (3, 1) => "Solid Professional",
                (2, 1) => "Effective",
                (1, 1) => "Risk",
                _ => "Unassigned"
            };
        }

        public string GetRecommendedAction(string quadrant)
        {
            return quadrant switch
            {
                "Star" => "Accelerate leadership development and award retention incentives.",
                "Growth Potential" => "Assign cross-functional projects and senior mentorship.",
                "Enigma" => "Identify root cause of performance blockers and re-evaluate role fit.",
                "High Performer" => "Reward execution; keep motivated in current specialized track.",
                "Core Player" => "Provide steady development and continuous recognition.",
                "Dilemma" => "Implement 60-day coaching plan; prepare for promotion or PIP.",
                "Solid Professional" => "Leverage subject matter expertise; do not force people management.",
                "Effective" => "Monitor performance stability; identify lateral growth opportunities.",
                "Risk" => "Initiate formal Performance Improvement Plan (PIP) or separation.",
                _ => "No action defined."
            };
        }
    }
}
