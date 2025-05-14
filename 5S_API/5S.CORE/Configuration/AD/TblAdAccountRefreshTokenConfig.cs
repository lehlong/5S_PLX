using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.CORE.Configuration.AD
{
    public class TblAdAccountRefreshTokenConfig : IEntityTypeConfiguration<TblAdAccountRefreshToken>
    {
        public void Configure(EntityTypeBuilder<TblAdAccountRefreshToken> builder)
        {
            builder.HasOne(x => x.Account).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserName).IsRequired();
        }
    }
}
