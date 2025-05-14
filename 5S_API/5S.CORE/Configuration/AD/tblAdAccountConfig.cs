using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PLX5S.CORE.Entities.AD;

namespace PLX5S.CORE.Configuration.AD
{
    public class TblAdAccountConfig : IEntityTypeConfiguration<TblAdAccount>
    {
        public void Configure(EntityTypeBuilder<TblAdAccount> builder)
        {
            builder.HasMany(x => x.Account_AccountGroups)
                .WithOne(g => g.Account)
                .HasForeignKey(x => x.UserName)
                .OnDelete(DeleteBehavior.Cascade);

           
        }
    }
}
