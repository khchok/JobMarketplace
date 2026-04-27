namespace JobMarketplace.Api.Auth;

public interface ICurrentUserServiceAccessor
{
    ICurrentUserService Current { get; }
    void Set(ICurrentUserService service);
}