using Common;

namespace Infrastructure
{
    public class MachineTime : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
