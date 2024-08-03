using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;

namespace ATMAPI.UnitTests.Mocks;
public class ApplicationDbContextText
{
    public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();
        var mockSet = queryable.BuildMockDbSet();
        return mockSet.Object;
    }

}