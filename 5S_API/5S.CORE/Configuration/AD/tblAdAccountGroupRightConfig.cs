using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.CORE.Configuration.AD
{
    public class TblAdAccountGroupRightConfig : IEntityTypeConfiguration<TblAdAccountGroupRight>
    {
        public void Configure(EntityTypeBuilder<TblAdAccountGroupRight> builder)
        {
            builder.HasOne<TblAdRight>(x => x.Right).WithMany(y => y.AccountGroupRights).HasForeignKey(x => x.RightId);
            builder.HasOne<TblAdAccountGroup>(x => x.AccountGroup).WithMany(y => y.ListAccountGroupRight).HasForeignKey(x => x.GroupId);
        }
    }
}
