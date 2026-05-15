namespace BetterCRM.Business.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string msg) : base(msg) { }

        public class UnauthorizedOperationException : DomainException
        {
            public UnauthorizedOperationException(string msg) : base(msg) { }
        }
        public class NotFoundException : DomainException
        {
            public NotFoundException(string msg) : base(msg) { }
        }
        public class ConflictException : DomainException
        {
            public ConflictException(string msg) : base(msg) { }
        }
        public class ForbiddenException : DomainException
        {
            public ForbiddenException(string msg) : base(msg) { }
        }
    }

}
