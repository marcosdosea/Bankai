using Core;
using Microsoft.EntityFrameworkCore;
using Service;

namespace ServiceTests
{
    [TestClass]
    public class AgendamentoServicesTests
    {
        private BankaiContext _context = null!;
        private Service.AgendamentoService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BankaiContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            _context = new BankaiContext(options);
            _service = new Service.AgendamentoService(_context);
        }

    }
}
