using Desafio.Umbler.Features.DomainContext.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace Desafio.Umbler.Persistence.Models
{
    public class Domain
    {
        public Domain()
        {
            UpdatedAt = DateTime.Now;
        }

        public Domain(string domainName, WhoIsResult whoIsResult)
        {
            Name = domainName;
            Ip = whoIsResult.Ip;
            WhoIs = whoIsResult.WhoIs;
            Ttl = whoIsResult.Ttl;
            HostedAt = whoIsResult.HostedAt;
            UpdatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string WhoIs { get; set; }
        public int Ttl { get; set; }
        public string HostedAt { get; set; }

        public void Update(string domainName, WhoIsResult whoIsResult)
        {
            Name = domainName;
            Ip = whoIsResult.Ip;
            WhoIs = whoIsResult.WhoIs;
            Ttl = whoIsResult.Ttl;
            HostedAt = whoIsResult.HostedAt;
            UpdatedAt = DateTime.Now;
        }
    }  
}
