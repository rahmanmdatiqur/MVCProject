using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvMid_09.Models
{
    public class Client
    {
        public Client()
        {
            this.BookingEntries = new List<BookingEntry>();
        }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Phone { get; set; }
        public string Picture { get; set; }
        public bool MaritialStatus { get; set; }
        //Nev
        public virtual ICollection<BookingEntry> BookingEntries { get; set; }
    }
    public class Spot
    {
        public Spot()
        {
            this.BookingEntries = new List<BookingEntry>();
        }
        public int SpotId { get; set; }
        public string SpotName { get; set; }
        //Nev
        public virtual ICollection<BookingEntry> BookingEntries { get; set; }
    }
    public class BookingEntry
    {
        public int BookingEntryId { get; set; }
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        [ForeignKey("Spot")]
        public int SpotId { get; set; }

        //Nev
        public virtual Client Client { get; set; }
        public virtual Spot Spot { get; set; }
    }
    public class TourDbContext : DbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options)
        {

        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<BookingEntry> BookingEntries { get; set; }
    }
}
