using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.CORE.Configuration.AD
{
    public class TblAdAccountGroupConfig : IEntityTypeConfiguration<TblAdAccountGroup>
    {
        public void Configure(EntityTypeBuilder<TblAdAccountGroup> builder)
        {
            builder.HasMany(x => x.Account_AccountGroups).WithOne(y => y.AccountGroup).HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
