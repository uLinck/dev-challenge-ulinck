namespace Desafio.Umbler.Features.DomainContext.Dto
{
    public record WhoIsResult
    {
        public string WhoIs { get; init; }
        public string Ip { get; init; }
        public int Ttl { get; init; }
        public string HostedAt { get; init; }
    }
}
