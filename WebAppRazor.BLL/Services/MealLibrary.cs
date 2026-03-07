using WebAppRazor.BLL.DTOs;

namespace WebAppRazor.BLL.Services
{
    public static class MealLibrary
    {
        public static readonly (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions)[] BreakfastOptions = new[]
        {
            ("Phở bò", "Phở bò truyền thống với nước dùng đậm đà, thịt bò tái và rau thơm", 0.12, 0.55, 0.33, "Thịt bò, bánh phở, hành, rau thơm, nước dùng xương", "1. Luộc xương bò 2-3 giờ lấy nước dùng\n2. Trụng bánh phở nước sôi\n3. Xếp thịt bò tái lên phở\n4. Chan nước dùng nóng\n5. Thêm rau thơm, giá đỗ"),
            ("Bánh mì trứng ốp la", "Bánh mì giòn với trứng ốp la, dưa leo, đồ chua", 0.10, 0.50, 0.40, "Bánh mì, trứng gà, dưa leo, cà rốt, củ cải trắng", "1. Chiên trứng ốp la với ít dầu\n2. Nướng bánh mì giòn\n3. Xẻ bánh mì, cho trứng vào\n4. Thêm dưa leo, đồ chua\n5. Rưới nước tương"),
            ("Cháo gà", "Cháo gà nấu nhừ với gừng và hành phi", 0.15, 0.60, 0.25, "Gạo, thịt gà, gừng, hành lá, hành phi", "1. Nấu cháo gạo với nước dùng gà\n2. Xé nhỏ thịt gà\n3. Cho thịt gà vào cháo\n4. Thêm gừng thái sợi\n5. Rắc hành phi, hành lá"),
            ("Xôi xéo", "Xôi nếp vàng với đậu xanh và hành phi giòn", 0.08, 0.65, 0.27, "Nếp, đậu xanh, hành phi, nghệ", "1. Ngâm nếp 4 giờ, trộn nghệ\n2. Hấp đậu xanh, nghiền nhuyễn\n3. Đồ xôi chín\n4. Xới xôi ra đĩa, phủ đậu xanh\n5. Rắc hành phi"),
            ("Bún riêu cua", "Bún riêu với cua đồng, đậu phụ và rau sống", 0.14, 0.50, 0.36, "Bún, cua đồng, cà chua, đậu phụ, rau sống", "1. Giã cua, lọc lấy nước\n2. Phi cà chua, nấu nước dùng\n3. Đun nước cua sôi để gạch nổi\n4. Trụng bún, xếp ra tô\n5. Chan nước dùng, thêm rau sống")
        };

        public static readonly (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions)[] LunchOptions = new[]
        {
            ("Cơm tấm sườn bì", "Cơm tấm với sườn nướng, bì và trứng ốp la", 0.20, 0.45, 0.35, "Gạo tấm, sườn heo, bì heo, trứng, nước mắm", "1. Ướp sườn với sả, tỏi, mắm\n2. Nướng sườn trên than hoa\n3. Nấu cơm tấm\n4. Chiên trứng ốp la\n5. Xếp đĩa, chan nước mắm pha"),
            ("Bún chả Hà Nội", "Bún chả với thịt nướng thơm phức và nước chấm chua ngọt", 0.22, 0.45, 0.33, "Bún, thịt ba chỉ, thịt nạc vai, nước mắm, đường", "1. Ướp thịt với mắm, đường, tiêu\n2. Nướng thịt trên than\n3. Pha nước chấm chua ngọt\n4. Trụng bún, xếp ra đĩa\n5. Ăn kèm rau sống, bún"),
            ("Gà kho gừng + cơm", "Gà kho gừng đậm đà với cơm trắng và canh rau", 0.25, 0.40, 0.35, "Thịt gà, gừng, nước mắm, đường, cơm trắng", "1. Chặt gà miếng vừa ăn\n2. Phi gừng thơm\n3. Cho gà vào xào săn\n4. Thêm nước mắm, đường, nước\n5. Kho lửa nhỏ 20 phút"),
            ("Canh chua cá lóc + cơm", "Canh chua đậm đà với cá lóc tươi, ăn kèm cơm trắng", 0.22, 0.42, 0.36, "Cá lóc, thơm, cà chua, bạc hà, rau ngổ, me", "1. Nấu nước dùng me\n2. Cho cà chua, thơm vào\n3. Thả cá lóc đã sơ chế\n4. Thêm bạc hà, giá đỗ\n5. Nêm nếm vừa ăn"),
            ("Bò lúc lắc + cơm", "Bò lúc lắc áp chảo với rau xà lách và cơm trắng", 0.28, 0.38, 0.34, "Thịt bò thăn, tỏi, tiêu, xà lách, cà chua", "1. Cắt bò hạt lựu, ướp tiêu tỏi\n2. Áp chảo bò lửa lớn\n3. Trộn xà lách, cà chua\n4. Xếp bò lên đĩa rau\n5. Ăn kèm cơm trắng nóng")
        };

        public static readonly (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions)[] DinnerOptions = new[]
        {
            ("Cá hồi áp chảo + salad", "Cá hồi áp chảo giòn da với salad rau xanh", 0.30, 0.20, 0.50, "Cá hồi, xà lách, cà chua bi, dầu olive, chanh", "1. Ướp cá hồi với muối, tiêu\n2. Áp chảo cá hồi da giòn\n3. Trộn salad với dầu olive\n4. Xếp cá lên đĩa salad\n5. Vắt chanh tươi"),
            ("Canh bí đỏ thịt bằm + cơm", "Canh bí đỏ nấu thịt bằm bổ dưỡng với cơm trắng", 0.18, 0.48, 0.34, "Bí đỏ, thịt heo bằm, hành lá, cơm trắng", "1. Xào thịt bằm với hành\n2. Cho bí đỏ cắt khối vào\n3. Thêm nước, nấu bí mềm\n4. Nêm nếm vừa ăn\n5. Rắc hành lá"),
            ("Gỏi cuốn tôm thịt", "Gỏi cuốn tươi mát với tôm, thịt heo và rau sống", 0.22, 0.35, 0.43, "Tôm, thịt ba chỉ, bánh tráng, bún, rau sống, đậu phộng", "1. Luộc tôm, thịt ba chỉ\n2. Nhúng bánh tráng nước ấm\n3. Xếp rau, bún, thịt, tôm\n4. Cuốn chặt tay\n5. Pha nước chấm đậu phộng"),
            ("Mì xào hải sản", "Mì xào giòn với tôm, mực và rau cải", 0.20, 0.45, 0.35, "Mì trứng, tôm, mực, cải thìa, cà rốt", "1. Chiên mì giòn\n2. Xào tôm, mực với tỏi\n3. Thêm rau cải, cà rốt\n4. Nêm dầu hào, nước tương\n5. Đổ lên mì giòn"),
            ("Đậu hũ sốt cà + cơm", "Đậu hũ chiên giòn sốt cà chua thơm ngon, ăn kèm cơm", 0.15, 0.45, 0.40, "Đậu hũ, cà chua, hành, tỏi, cơm trắng", "1. Chiên đậu hũ vàng giòn\n2. Phi hành tỏi thơm\n3. Cho cà chua vào xào nhuyễn\n4. Thêm đậu hũ vào sốt\n5. Nêm nước tương, đường")
        };

        public static readonly (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions)[] SnackOptions = new[]
        {
            ("Sữa chua Hy Lạp + trái cây", "Sữa chua Hy Lạp ít đường với trái cây tươi mùa", 0.20, 0.50, 0.30, "Sữa chua Hy Lạp, chuối, dâu tây, mật ong", "1. Cho sữa chua ra bát\n2. Cắt trái cây nhỏ\n3. Xếp trái cây lên trên\n4. Rưới mật ong"),
            ("Sinh tố bơ", "Sinh tố bơ béo ngậy giàu chất béo tốt", 0.05, 0.35, 0.60, "Bơ, sữa tươi, đường, đá", "1. Gọt bơ, bỏ hạt\n2. Cho bơ, sữa, đường vào máy\n3. Xay nhuyễn\n4. Đổ ra ly, thêm đá"),
            ("Bánh chuối nướng", "Bánh chuối nướng thơm bùi kiểu truyền thống", 0.06, 0.60, 0.34, "Chuối, bột mì, đường, dừa nạo, sữa", "1. Nghiền chuối nhuyễn\n2. Trộn bột, đường, sữa\n3. Cho dừa nạo vào\n4. Đổ khuôn, nướng 180°C 30 phút"),
            ("Trứng luộc + trái cây", "Trứng luộc giàu protein kèm trái cây tươi", 0.35, 0.40, 0.25, "Trứng gà, cam, táo", "1. Luộc trứng 8 phút\n2. Bóc vỏ\n3. Cắt trái cây\n4. Dùng kèm"),
            ("Hạt mix dinh dưỡng", "Hỗn hợp các loại hạt giàu omega-3 và vitamin E", 0.12, 0.25, 0.63, "Hạt điều, hạt hạnh nhân, hạt óc chó, nho khô", "1. Trộn các loại hạt\n2. Rang nhẹ 5 phút\n3. Để nguội\n4. Cho thêm nho khô")
        };

        public static List<MealItemDto> GetAllAvailableMeals()
        {
            var all = new List<MealItemDto>();
            
            foreach (var m in BreakfastOptions) all.Add(MapToDto("Breakfast", m));
            foreach (var m in LunchOptions) all.Add(MapToDto("Lunch", m));
            foreach (var m in DinnerOptions) all.Add(MapToDto("Dinner", m));
            foreach (var m in SnackOptions) all.Add(MapToDto("Snack", m));
            
            return all;
        }

        private static MealItemDto MapToDto(string type, (string name, string desc, double proteinRatio, double carbRatio, double fatRatio, string ingredients, string instructions) data)
        {
            return new MealItemDto
            {
                MealType = type,
                Name = data.name,
                Description = data.desc,
                Ingredients = data.ingredients,
                CookingInstructions = data.instructions
                // Ratios and calories will be calculated based on target calories in the service
            };
        }
    }
}
