using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebAppRazor.BLL.DTOs;
using WebAppRazor.BLL.Interfaces;
using WebAppRazor.DAL.Data;

namespace WebAppRazor.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(int userId, string filterType)
        {
            var dto = new DashboardDto();
            
            DateTime today = DateTime.Today;
            DateTime startDate;
            DateTime endDate = DateTime.Now;
            bool isQuarter = filterType?.Equals("Quarter", StringComparison.OrdinalIgnoreCase) == true;

            int daysToSubtract = isQuarter ? 90 : 30;
            startDate = endDate.AddDays(-daysToSubtract);

            // Fetch Data
            var profile = await _context.HealthProfiles
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .FirstOrDefaultAsync();

            var progressEntries = await _context.ProgressEntries
                .Where(p => p.UserId == userId && p.RecordedAt >= startDate && p.RecordedAt <= endDate)
                .OrderBy(p => p.RecordedAt)
                .ToListAsync();

            // Fetch last 90 days for heatmap independent of filter to ensure heatmap has enough data
            var heatmapStartDate = today.AddDays(-90);
            var mealPlans = await _context.MealPlans
                .Include(m => m.MealItems)
                .Where(m => m.UserId == userId && m.PlanDate >= heatmapStartDate && m.PlanDate <= endDate)
                .OrderBy(m => m.PlanDate)
                .ToListAsync();

            // -----------------------------------------------------
            // Section 1: Health
            // -----------------------------------------------------
            if (profile != null)
            {
                dto.CurrentBMI = Math.Round(profile.BMI, 1);
                dto.CurrentBMR = Math.Round(profile.BMR, 1);
                dto.CurrentTDEE = Math.Round(profile.TDEE, 1);
                
                if (dto.CurrentBMI < 18.5) dto.BMIStatus = "Thiếu cân";
                else if (dto.CurrentBMI < 24.9) dto.BMIStatus = "Bình thường";
                else if (dto.CurrentBMI < 29.9) dto.BMIStatus = "Thừa cân";
                else dto.BMIStatus = "Béo phì";
            }

            foreach (var p in progressEntries)
            {
                dto.WeightHistory.Add(new ActivityDataPoint { Label = p.RecordedAt.ToString("MMM dd"), Value = p.Weight });
                dto.ProgressWeight.Add(new ActivityDataPoint { Label = p.RecordedAt.ToString("MMM dd"), Value = p.Weight });
            }

            // Calculate Weight Trend
            if (progressEntries.Count > 1)
            {
                double firstWeight = progressEntries.First().Weight;
                double lastWeight = progressEntries.Last().Weight;
                double diff = Math.Round(lastWeight - firstWeight, 1);
                
                string timeframe = isQuarter ? "90 ngày" : "30 ngày";
                if (diff < 0) dto.WeightTrendText = $"⬇ Tuyệt vời! Bạn đã giảm {Math.Abs(diff)} kg trong {timeframe} qua.";
                else if (diff > 0) dto.WeightTrendText = $"⬆ Bạn đã tăng {diff} kg trong {timeframe} qua.";
                else dto.WeightTrendText = $"⚖️ Trọng lượng của bạn duy trì ổn định trong {timeframe} qua.";
            }
            else
            {
                dto.WeightTrendText = "🌟 Hãy cập nhật cân nặng thường xuyên để AI theo dõi giúp bạn!";
            }

            // -----------------------------------------------------
            // Section 2: Calories & Section 3: Nutrition
            // -----------------------------------------------------
            var todaysMeals = mealPlans.Where(m => m.PlanDate.Date == today).ToList();
            dto.CalorieTargetToday = profile?.DailyCalorieTarget ?? 2000;

            foreach (var meal in todaysMeals)
            {
                dto.CaloriesConsumedToday += meal.MealItems.Sum(mi => mi.Calories);
                dto.ProteinToday += meal.MealItems.Sum(mi => mi.Protein);
                dto.CarbsToday += meal.MealItems.Sum(mi => mi.Carbs);
                dto.FatToday += meal.MealItems.Sum(mi => mi.Fat);
            }

            // Last 7 days calories
            for (int i = 6; i >= 0; i--)
            {
                var targetDate = today.AddDays(-i);
                var dailyMeals = mealPlans.Where(m => m.PlanDate.Date == targetDate).ToList();
                double dailyCals = dailyMeals.Sum(m => m.MealItems.Sum(mi => mi.Calories));
                
                dto.CaloriesLast7Days.Add(new ActivityDataPoint 
                { 
                    Label = targetDate.ToString("ddd"), 
                    Value = dailyCals 
                });
            }

            // -----------------------------------------------------
            // Section 4: Progress Monthly/Quarterly
            // -----------------------------------------------------
            // Handled mostly by WeightHistory mapping. Will just use ProgressWeight on frontend.

            // -----------------------------------------------------
            // Section 5: Habit Heatmap (last 90 days)
            // -----------------------------------------------------
            var mealsByDate = mealPlans.GroupBy(m => m.PlanDate.Date).ToDictionary(g => g.Key, g => g.ToList());

            for (int i = 0; i < 90; i++)
            {
                var hDate = heatmapStartDate.AddDays(i);
                int countStatus = 0;

                if (mealsByDate.TryGetValue(hDate, out var dayPlans))
                {
                    int totalItems = dayPlans.Sum(m => m.MealItems.Count);
                    int completedItems = dayPlans.Sum(m => m.MealItems.Count(mi => mi.IsCompleted));

                    if (totalItems > 0)
                    {
                        double completion = (double)completedItems / totalItems;
                        if (completion >= 0.8) countStatus = 2; // >80% is green (Good)
                        else if (completion > 0) countStatus = 1; // Light green (Partial)
                    }
                }

                dto.HabitHeatmap.Add(new HabitHeatmapData 
                { 
                    DateString = hDate.ToString("yyyy-MM-dd"), 
                    Count = countStatus,
                    Date = hDate
                });
            }

            return dto;
        }

        public async Task SeedMockDataAsync(int userId)
        {
            var profile = await _context.HealthProfiles
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .FirstOrDefaultAsync();
            if (profile == null) return;

            var rnd = new Random();
            double currentWeight = profile.Weight;
            double targetCalories = profile.DailyCalorieTarget;
            double heightM = profile.Height / 100.0;

            // Fix previously seeded records with 0 BMR
            var zeroBmrEntries = _context.ProgressEntries.Where(p => p.UserId == userId && p.BMR == 0).ToList();
            if (zeroBmrEntries.Any())
            {
                foreach(var entry in zeroBmrEntries)
                {
                    entry.BMR = Math.Round(profile.BMR + (entry.Weight - currentWeight) * 15, 0);
                    entry.TDEE = Math.Round(profile.TDEE + (entry.Weight - currentWeight) * 20, 0);
                    entry.Notes = "Dữ liệu mẫu";
                }
                await _context.SaveChangesAsync();
            }

            // Patch existing fake meals "Món ... mẫu" to realistic meals
            var fakeMeals = _context.MealItems.Where(m => m.Name.Contains("mẫu") && m.MealPlan.UserId == userId).ToList();
            if (fakeMeals.Any())
            {
                foreach (var item in fakeMeals)
                {
                    (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions) selectedOp = default;
                    double percentage = 0.25;

                    if (item.MealType == "Breakfast") { selectedOp = MealLibrary.BreakfastOptions[rnd.Next(MealLibrary.BreakfastOptions.Length)]; percentage = 0.25; }
                    else if (item.MealType == "Lunch") { selectedOp = MealLibrary.LunchOptions[rnd.Next(MealLibrary.LunchOptions.Length)]; percentage = 0.35; }
                    else if (item.MealType == "Dinner") { selectedOp = MealLibrary.DinnerOptions[rnd.Next(MealLibrary.DinnerOptions.Length)]; percentage = 0.30; }
                    else if (item.MealType == "Snack") { selectedOp = MealLibrary.SnackOptions[rnd.Next(MealLibrary.SnackOptions.Length)]; percentage = 0.10; }

                    if (selectedOp.name != null)
                    {
                        double itemCal = targetCalories * percentage;
                        item.Name = selectedOp.name;
                        item.Description = selectedOp.desc;
                        item.Calories = Math.Round(itemCal, 0);
                        item.Protein = Math.Round(itemCal * selectedOp.proteinRatio / 4, 1);
                        item.Carbs = Math.Round(itemCal * selectedOp.carbRatio / 4, 1);
                        item.Fat = Math.Round(itemCal * selectedOp.fatRatio / 9, 1);
                        item.Ingredients = selectedOp.ingredients;
                        item.CookingInstructions = selectedOp.instructions;
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Nếu đã gieo nhiều dữ liệu rồi thì bỏ qua phần tạo mới để tránh rác DB
            if (_context.ProgressEntries.Count(p => p.UserId == userId) > 50) 
            {
                return;
            }

            var today = DateTime.Today;

            for (int i = 30; i >= 1; i--)
            {
                var date = today.AddDays(-i);
                
                // 1. Tiến trình cân nặng (giảm dần từ +3kg xuống hiện tại)
                double mockWeight = currentWeight + (i * 0.1) + (rnd.NextDouble() * 0.5 - 0.25);
                
                _context.ProgressEntries.Add(new WebAppRazor.DAL.Models.ProgressEntry
                {
                    UserId = userId,
                    Weight = Math.Round(mockWeight, 1),
                    BMI = Math.Round(mockWeight / (heightM * heightM), 1),
                    BMR = Math.Round(profile.BMR + (mockWeight - currentWeight) * 15, 0),
                    TDEE = Math.Round(profile.TDEE + (mockWeight - currentWeight) * 20, 0),
                    Notes = "Dữ liệu mẫu",
                    RecordedAt = date
                });

                // 2. Lịch sử hoàn thành món ăn
                var plan = new WebAppRazor.DAL.Models.MealPlan
                {
                    UserId = userId,
                    Title = $"Thực đơn tự động {date:dd/MM}",
                    TargetCalories = targetCalories,
                    PlanDate = date,
                    CreatedAt = date
                };

                var seedMealTypes = new[] {
                    ("Breakfast", 0.25, MealLibrary.BreakfastOptions),
                    ("Lunch", 0.35, MealLibrary.LunchOptions),
                    ("Dinner", 0.30, MealLibrary.DinnerOptions),
                    ("Snack", 0.10, MealLibrary.SnackOptions)
                };

                foreach (var (type, percentage, options) in seedMealTypes)
                {
                    bool isCompleted = rnd.NextDouble() > 0.25;
                    var selectedOp = options[rnd.Next(options.Length)];
                    double itemCal = targetCalories * percentage;
                    
                    plan.MealItems.Add(new WebAppRazor.DAL.Models.MealItem
                    {
                        MealType = type,
                        Name = selectedOp.name,
                        Description = selectedOp.desc,
                        Calories = Math.Round(itemCal, 0),
                        Protein = Math.Round(itemCal * selectedOp.proteinRatio / 4, 1),
                        Carbs = Math.Round(itemCal * selectedOp.carbRatio / 4, 1),
                        Fat = Math.Round(itemCal * selectedOp.fatRatio / 9, 1),
                        Ingredients = selectedOp.ingredients,
                        CookingInstructions = selectedOp.instructions,
                        IsCompleted = isCompleted
                    });
                }
                _context.MealPlans.Add(plan);
            }

            await _context.SaveChangesAsync();
        }
    }
}
