using Microsoft.EntityFrameworkCore;
using Moq;

namespace TPVWPF.Tests
{
    public static class MockDbSet
    {
        public static Mock<DbSet<T>> Create<T>(IEnumerable<T> elements) where T : class
        {
            var queryable = elements.AsQueryable();
            var mock = new Mock<DbSet<T>>();

            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mock;
        }
    }
}
