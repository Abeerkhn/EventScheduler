using MediatR;

namespace EventScheduler.Data.Query
{
    public class GetAllUsersQuery:IRequest<List<User>>
    {
    }
}
