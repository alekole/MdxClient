using System.Configuration;
using MdxClientPooled;

namespace DynamicTyped.Data.Test
{
    public static class UnitTestHelpers
    {
        public static string GetCapellaDataTestConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["CapellaDataTest"].ConnectionString;
        }

        public static MdxConnection GetCapellaDataTestConnection()
        {
            return new MdxConnection(GetCapellaDataTestConnectionString());
        }
    }
}
