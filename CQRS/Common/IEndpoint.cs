namespace CQRSDemo.Common;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
