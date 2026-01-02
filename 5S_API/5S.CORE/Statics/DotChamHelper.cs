using PLX5S.CORE.Entities.MD;

namespace PLX5S.CORE.Statics
{
    public enum DotCham
    {
        Dot0 = 0, // không cần chấm
        Dot1 = 1, // 1–7
        Dot2 = 2, // 8–15
        Dot3 = 3, // 16–23
        Dot4 = 4,  // >= 24
        Dot5 = 5  // Chấm tất cả
    }
    public static class RoleIds
    {
        public const string CHT = "CHT";
        public const string TK = "TK";
        public const string ATVSV = "ATVSV";
        public const string CQ = "717cae63-83e6-40af-a324-6fc41eb0f121";
    }

    public static class DotChamHelper
    {
        public static DotCham GetDot(DateTime date)
        {
            if (date.Day <= 7) return DotCham.Dot1;
            if (date.Day <= 15) return DotCham.Dot2;
            if (date.Day <= 23) return DotCham.Dot3;
            return DotCham.Dot4;
        }

        public static DotCham GetDot2(DateTime date, string? chucVuId = null)
        {
            if (chucVuId == RoleIds.CQ) return DotCham.Dot5;
            if (date.Day <= 7 && (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK)) return DotCham.Dot1;
            if (date.Day >= 8 && date.Day <= 15 && chucVuId == RoleIds.ATVSV) return DotCham.Dot2;
            if (date.Day >= 16 && date.Day <= 23 && (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK)) return DotCham.Dot3;
            if (date.Day >= 24 && chucVuId == RoleIds.ATVSV) return DotCham.Dot4;
            return 0;
        }

        public static DotCham GetDotDaQua(DateTime date, string? chucVuId = null)
        {
            if (chucVuId == RoleIds.CQ) return DotCham.Dot0;
            if (date.Day >= 8 && date.Day <= 15 && (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK)) return DotCham.Dot1;
            if (date.Day >= 16 && date.Day <= 23 && (chucVuId == RoleIds.CHT || chucVuId == RoleIds.TK)) return DotCham.Dot3;
            if (date.Day >= 24 && chucVuId == RoleIds.ATVSV) return DotCham.Dot4;
            return 0;
        }
        // Đợt bắt buộc theo chức vụ
        public static readonly Dictionary<string, DotCham[]> BatBuocTheoChucVu =
            new()
            {
                ["CHT"] = new[] { DotCham.Dot1, DotCham.Dot3 },
                ["TK"] = new[] { DotCham.Dot1, DotCham.Dot3 },
                ["ATVSV"] = new[] { DotCham.Dot2, DotCham.Dot4 }
            };
        public static readonly HashSet<string> ChiCanChamMotLan =
            new()
            {
                "QL",  // nếu có
                "CV",  // nếu có
                "717cae63-83e6-40af-a324-6fc41eb0f121" // Chuyên quản
            };
        public static int ToNumber(DotCham dot) => (int)dot;

        public static DotCham[] GetBatBuocThucTe(List<string> rolesInStore)
        {
            // Ưu tiên CHT
            if (rolesInStore.Contains("CHT"))
                return BatBuocTheoChucVu["CHT"];

            // Nếu không có CHT nhưng có TK → TK thay thế
            if (rolesInStore.Contains("TK"))
                return BatBuocTheoChucVu["TK"];

            // Nếu không có 2 role trên → lấy theo từng role khác nếu có
            foreach (var role in rolesInStore)
            {
                if (BatBuocTheoChucVu.TryGetValue(role, out var dots))
                    return dots;
            }

            // Không khớp role nào → không bắt buộc
            return Array.Empty<DotCham>();
        }

        public static string GetRoleName(string chucVuId)
        {
            return chucVuId switch
            {
                "CHT" => "Cửa hàng trưởng",
                "TK" => "Thủ kho",
                "ATVSV" => "An toàn vệ sinh viên",
                "QL" => "Quản lý",
                "CV" => "Chuyên viên",
                "717cae63-83e6-40af-a324-6fc41eb0f121" => "Chuyên quản",

                _ => chucVuId
            };
        }

    }
}
