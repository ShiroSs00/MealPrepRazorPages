using System.Collections.Generic;

namespace WebAppRazor.BLL.DTOs
{
    public class ActivityDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    public class HabitHeatmapData
    {
        public string DateString { get; set; } = string.Empty; // YYYY-MM-DD
        public int Count { get; set; } // 0 = not completed, 1 = partially completed, 2 = fully completed
        public DateTime Date { get; set; }
    }

    public class DashboardDto
    {
        // Section 1: Health
        public double CurrentBMI { get; set; }
        public string BMIStatus { get; set; } = string.Empty;
        public double CurrentBMR { get; set; }
        public double CurrentTDEE { get; set; }
        public string WeightTrendText { get; set; } = string.Empty;
        public List<ActivityDataPoint> WeightHistory { get; set; } = new List<ActivityDataPoint>();

        // Section 2: Calories Today
        public double CaloriesConsumedToday { get; set; }
        public double CalorieTargetToday { get; set; }
        public List<ActivityDataPoint> CaloriesLast7Days { get; set; } = new List<ActivityDataPoint>();

        // Section 3: Nutrition Today
        public double ProteinToday { get; set; }
        public double CarbsToday { get; set; }
        public double FatToday { get; set; }

        // Section 4: Progress (Line chart Month/Quarter)
        // Re-using WeightHistory or a specific Calorie progress. We will use two separate lists.
        public List<ActivityDataPoint> ProgressWeight { get; set; } = new List<ActivityDataPoint>();

        // Section 5: Habit (Heatmap)
        public List<HabitHeatmapData> HabitHeatmap { get; set; } = new List<HabitHeatmapData>();
    }
}
