using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLX5S.CORE.Entities.BU;

namespace PLX5S.CORE.Configuration.BU
{
    public class TblBuTinhDiemTieuChiConfig : IEntityTypeConfiguration<TblBuTinhDiemTieuChi>
    {
        public void Configure(EntityTypeBuilder<TblBuTinhDiemTieuChi> builder)
        {
            builder.HasOne(x => x.TieuChi)
                .WithMany(g => g.DiemTieuChi)
                .HasForeignKey(x => x.TieuChiId)
                .OnDelete(DeleteBehavior.Cascade);

           
        }
    }
}
