namespace Simple.Hateoas.Sample.Services
{
    public class PermissionServiceMock : IPermissionServiceMock
    {
        public bool HasPermission(int id) => id % 2 == 0;
    }
}
