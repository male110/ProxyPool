namespace ProxyPool
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Proxy")]
    public partial class Proxy
    {
        public int Id { get; set; }

        [Required]
        [StringLength(36)]
        public string Adress { get; set; }

        public int Port { get; set; }

        [Required]
        [StringLength(50)]
        public string Source { get; set; }

        public int Speed { get; set; }
        
        public DateTime CreateDate { get; set; }
        
        public DateTime LastVerifyDate { get; set; }
    }
}
