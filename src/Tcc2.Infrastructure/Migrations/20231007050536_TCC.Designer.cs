﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tcc2.Infrastructure.Persistence.RelationalDatabase;

#nullable disable

namespace Tcc2.Infrastructure.Migrations
{
    [DbContext(typeof(TccContext))]
    [Migration("20231007050536_TCC")]
    partial class TCC
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Tcc2.Domain.Entities.Activity", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<TimeSpan>("End")
                        .HasColumnType("time(6)");

                    b.Property<long?>("PersonId")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<TimeSpan>("Start")
                        .HasColumnType("time(6)");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.ActivityDay", b =>
                {
                    b.Property<long?>("ActivityId")
                        .HasColumnType("bigint");

                    b.Property<short?>("DayId")
                        .HasColumnType("smallint");

                    b.HasKey("ActivityId", "DayId");

                    b.HasIndex("DayId");

                    b.ToTable("ActivityDay");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.Day", b =>
                {
                    b.Property<short>("Id")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Days");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.Person", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.ValueObjects.CompositeAddress", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Neighborhood")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<long?>("PersonId")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.Activity", b =>
                {
                    b.HasOne("Tcc2.Domain.Entities.Person", "Person")
                        .WithMany("Activities")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Tcc2.Domain.Entities.ValueObjects.Address", "Address", b1 =>
                        {
                            b1.Property<long>("ActivityId")
                                .HasColumnType("bigint");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<string>("Neighborhood")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<int>("Number")
                                .HasColumnType("int");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.HasKey("ActivityId");

                            b1.ToTable("Activities");

                            b1.WithOwner()
                                .HasForeignKey("ActivityId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.ActivityDay", b =>
                {
                    b.HasOne("Tcc2.Domain.Entities.Activity", "Activity")
                        .WithMany("ActivityDay")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tcc2.Domain.Entities.Day", "Day")
                        .WithMany("ActivityDay")
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("Day");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.ValueObjects.CompositeAddress", b =>
                {
                    b.HasOne("Tcc2.Domain.Entities.Person", "Person")
                        .WithOne("Address")
                        .HasForeignKey("Tcc2.Domain.Entities.ValueObjects.CompositeAddress", "PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Tcc2.Domain.Entities.ValueObjects.GeographicCoordinate", "GeographicCoordinate", b1 =>
                        {
                            b1.Property<long>("CompositeAddressId")
                                .HasColumnType("bigint");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double");

                            b1.Property<double>("Longitude")
                                .HasColumnType("double");

                            b1.HasKey("CompositeAddressId");

                            b1.ToTable("Addresses");

                            b1.WithOwner()
                                .HasForeignKey("CompositeAddressId");
                        });

                    b.Navigation("GeographicCoordinate");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.Activity", b =>
                {
                    b.Navigation("ActivityDay");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.Day", b =>
                {
                    b.Navigation("ActivityDay");
                });

            modelBuilder.Entity("Tcc2.Domain.Entities.Person", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("Address")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}