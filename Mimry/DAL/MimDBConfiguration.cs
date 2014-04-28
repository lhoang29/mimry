using System.Data.Entity;
using System.Data.Entity.SqlServer;
using Mimry.Models;

namespace Mimry.DAL
{
    public class MimDBConfiguration : DbConfiguration
    {
        public MimDBConfiguration()
        {
            SetDatabaseInitializer<MimDBContext>(new MimDBInitializer());
        }
    }
}