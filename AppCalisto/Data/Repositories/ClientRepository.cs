using AppCalisto.Data.Entities;

namespace AppCalisto.Data.Repositories
{
    public class ClientRepository : EntityRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;

        public ClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
