namespace JobMarketplace.Api.Auth;


public sealed class CurrentUserServiceAccessor : ICurrentUserServiceAccessor
{
    private ICurrentUserService? _service;

    public ICurrentUserService Current =>
        _service ?? throw new InvalidOperationException("CurrentUserService has not been set for this request.");

    public void Set(ICurrentUserService service) => _service = service;
}