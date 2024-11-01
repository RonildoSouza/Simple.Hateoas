namespace Simple.Hateoas.Sample.Core.Services
{
    public class PermissionServiceMock : IPermissionServiceMock
    {
        public bool HasPermission(int id) => id % 2 == 0;
    }
}
