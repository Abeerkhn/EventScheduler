﻿// <auto-generated />
using System;
using EventScheduler.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventScheduler.Migrations.UserJoinEvent
{
    [DbContext(typeof(UserJoinEventContext))]
    partial class UserJoinEventContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EventScheduler.Models.UserJoinEvent", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UJEId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "EventId");

                    b.ToTable("UserEventJoins", "public");
                });
#pragma warning restore 612, 618
        }
    }
}
