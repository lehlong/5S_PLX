using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.CORE.Configuration.AD
{
    public class TblAdAccountRightConfig : IEntityTypeConfiguration<TblAdAccountRight>
    {
        public void Configure(EntityTypeBuilder<TblAdAccountRight> builder)
        {
            builder.HasKey(x => new { x.UserName, x.RightId });
            builder.HasOne(x=>x.Account).WithMany(x=>x.AccountRights).HasForeignKey(x=>x.UserName).IsRequired();
            builder.HasOne(x=>x.Right).WithMany(x=>x.AccountRights).HasForeignKey(x=>x.RightId).IsRequired();
        }
    }
}
