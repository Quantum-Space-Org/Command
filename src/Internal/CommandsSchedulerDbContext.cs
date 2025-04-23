using System;
using Microsoft.EntityFrameworkCore;
using Quantum.DataBase.EntityFramework;

namespace Quantum.Command.Internal;



public class CommandsSchedulerDbContext : QuantumDbContext
{
    public CommandsSchedulerDbContext(DbContextOptionsBuilder<QuantumDbContext> options) : base(
        new DbContextConfig(options))
    {
    }

    public DbSet<InternalCommand> InternalCommands { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InternalCommand>()
            .HasKey(a => a.Id);

        base.OnModelCreating(modelBuilder);
    }
}

public class InternalCommand
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Command { get; set; }
    public DateTime OccurredAt { get; }
    public bool Seen { get; set; }
    public DateTime? SeenAt { get; set; }

    private InternalCommand()
    {

    }
    public InternalCommand(string id, string type, string command)
    {
        Id = id;
        Type = type;
        Command = command;
        OccurredAt = DateTime.UtcNow;
    }

    internal void SetSeen() => Seen = true;

    internal void SetSeenAt() => SeenAt = DateTime.UtcNow;
}