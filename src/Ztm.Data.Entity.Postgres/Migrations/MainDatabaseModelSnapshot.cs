﻿// <auto-generated />
using System;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NBitcoin;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ztm.Data.Entity.Postgres;

namespace Ztm.Data.Entity.Postgres.Migrations
{
    [DbContext(typeof(MainDatabase))]
    partial class MainDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.Block", b =>
                {
                    b.Property<int>("Height");

                    b.Property<long>("Bits");

                    b.Property<uint256>("Hash")
                        .IsRequired();

                    b.Property<uint256>("MerkleRoot")
                        .IsRequired();

                    b.Property<uint256>("MtpHashValue");

                    b.Property<int?>("MtpVersion");

                    b.Property<long>("Nonce");

                    b.Property<uint256>("Reserved1");

                    b.Property<uint256>("Reserved2");

                    b.Property<DateTime>("Time");

                    b.Property<int>("Version");

                    b.HasKey("Height");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.BlockTransaction", b =>
                {
                    b.Property<uint256>("BlockHash");

                    b.Property<uint256>("TransactionHash");

                    b.Property<int>("Index");

                    b.HasKey("BlockHash", "TransactionHash", "Index");

                    b.HasIndex("TransactionHash");

                    b.ToTable("BlockTransactions");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.Input", b =>
                {
                    b.Property<uint256>("TransactionHash");

                    b.Property<long>("Index");

                    b.Property<uint256>("OutputHash")
                        .IsRequired();

                    b.Property<long>("OutputIndex");

                    b.Property<byte[]>("Script")
                        .IsRequired();

                    b.Property<long>("Sequence");

                    b.HasKey("TransactionHash", "Index");

                    b.HasIndex("OutputHash", "OutputIndex");

                    b.ToTable("Inputs");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.Output", b =>
                {
                    b.Property<uint256>("TransactionHash");

                    b.Property<long>("Index");

                    b.Property<byte[]>("Script")
                        .IsRequired();

                    b.Property<long>("Value");

                    b.HasKey("TransactionHash", "Index");

                    b.ToTable("Outputs");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.ReceivingAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<bool>("IsLocked")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.ToTable("ReceivingAddresses");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.ReceivingAddressReservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<DateTime>("LockedAt");

                    b.Property<DateTime?>("ReleasedAt");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("ReceivingAddressReservations");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.Transaction", b =>
                {
                    b.Property<uint256>("Hash");

                    b.Property<long>("LockTime");

                    b.Property<long>("Version");

                    b.HasKey("Hash");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatcherRule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CallbackId");

                    b.Property<int>("Confirmation");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<Guid?>("CurrentWatchId");

                    b.Property<TimeSpan>("OriginalWaitingTime");

                    b.Property<TimeSpan>("RemainingWaitingTime");

                    b.Property<int>("Status");

                    b.Property<string>("SuccessData")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("TimeoutData")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<uint256>("TransactionHash")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CallbackId");

                    b.HasIndex("CurrentWatchId");

                    b.HasIndex("Status");

                    b.ToTable("TransactionConfirmationWatcherRules");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatcherWatch", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<Guid>("RuleId");

                    b.Property<uint256>("StartBlockHash")
                        .IsRequired();

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("Status");

                    b.Property<uint256>("TransactionHash")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RuleId");

                    b.HasIndex("StartBlockHash");

                    b.HasIndex("Status");

                    b.ToTable("TransactionConfirmationWatcherWatches");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.WebApiCallback", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<bool>("Completed");

                    b.Property<IPAddress>("RegisteredIp")
                        .IsRequired();

                    b.Property<DateTime>("RegisteredTime");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("WebApiCallbacks");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.WebApiCallbackHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CallbackId");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("InvokedTime");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CallbackId");

                    b.ToTable("WebApiCallbackHistories");
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.BlockTransaction", b =>
                {
                    b.HasOne("Ztm.Data.Entity.Contexts.Main.Block", "Block")
                        .WithMany("Transactions")
                        .HasForeignKey("BlockHash")
                        .HasPrincipalKey("Hash")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ztm.Data.Entity.Contexts.Main.Transaction", "Transaction")
                        .WithMany("Blocks")
                        .HasForeignKey("TransactionHash")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.Input", b =>
                {
                    b.HasOne("Ztm.Data.Entity.Contexts.Main.Transaction", "Transaction")
                        .WithMany("Inputs")
                        .HasForeignKey("TransactionHash")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.Output", b =>
                {
                    b.HasOne("Ztm.Data.Entity.Contexts.Main.Transaction", "Transaction")
                        .WithMany("Outputs")
                        .HasForeignKey("TransactionHash")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.ReceivingAddressReservation", b =>
                {
                    b.HasOne("Ztm.Data.Entity.Contexts.Main.ReceivingAddress", "Address")
                        .WithMany("Reservations")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatcherRule", b =>
                {
                    b.HasOne("Ztm.Data.Entity.Contexts.Main.WebApiCallback", "Callback")
                        .WithMany()
                        .HasForeignKey("CallbackId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatcherWatch", "CurrentWatch")
                        .WithMany()
                        .HasForeignKey("CurrentWatchId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatcherWatch", b =>
                {
                    b.HasOne("Ztm.Data.Entity.Contexts.Main.TransactionConfirmationWatcherRule", "Rule")
                        .WithMany("Watches")
                        .HasForeignKey("RuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ztm.Data.Entity.Contexts.Main.WebApiCallbackHistory", b =>
                {
                    b.HasOne("Ztm.Data.Entity.Contexts.Main.WebApiCallback", "Callback")
                        .WithMany("InvocationHistories")
                        .HasForeignKey("CallbackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
