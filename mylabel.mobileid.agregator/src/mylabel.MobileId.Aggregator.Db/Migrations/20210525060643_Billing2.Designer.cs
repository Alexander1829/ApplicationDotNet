﻿// <auto-generated />
using System;
using mylabel.MobileId.Aggregator.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    [DbContext(typeof(AggregatorContext))]
    [Migration("20210525060643_Billing2")]
    partial class Billing2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.AdminUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AdminUsers");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.DIAuthorizationRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessTokenOnAggregatorHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AcrValues")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Dcid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Display")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("ErrorAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ErrorDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdgwClientId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdgwClientSecret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LoginHint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Msisdn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nonce")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RedirectUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResponseType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServingOperator")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateNew")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DIAuthorizationRequests");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.DiscoveryService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DiscoveryServices");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.PremiumInfoToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessTokenOnAggregatorHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccessTokenOnIdgw")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AfterSiOrDi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("PremiumInfoTokens");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.SIAuthorizationRequest", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessTokenOnAggregatorHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AcrValues")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("AggregatorNonce")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AggregatorNotificationToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("AuthorizationRequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ClientIdOnAggregator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("IdgwJwksUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Msisdn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PremiumInfoEndpoint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("RespondedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("SPNonce")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SPNotificationToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SPNotificationUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServingOperator")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorizationRequestId")
                        .IsUnique();

                    b.ToTable("SIAuthorizationRequests");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.SPNotificationUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ServiceProviderId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("SPNotificationUris");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.SPRedirectUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ServiceProviderId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("SPRedirectUris");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.SPToDiscoveryLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClientIdOnDiscovery")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientSecretOnDiscovery")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DiscoveryServiceId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("RedirectUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceProviderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DiscoveryServiceId");

                    b.HasIndex("ServiceProviderId", "DiscoveryServiceId")
                        .IsUnique();

                    b.ToTable("SPToDiscoveryLinks");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.ServiceProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AggregatorClientSecretHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BillingCtn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientIdOnAggregator")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsInactive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPremiumInfoSigned")
                        .HasColumnType("bit");

                    b.Property<int?>("JwksCachingInSeconds")
                        .HasColumnType("int");

                    b.Property<string>("JwksUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JwksValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("UseStoredJwksValue")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ClientIdOnAggregator")
                        .IsUnique();

                    b.ToTable("ServiceProviders");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.SPNotificationUri", b =>
                {
                    b.HasOne("mylabel.MobileId.Aggregator.Db.ServiceProvider", null)
                        .WithMany("AllowedNotificationUris")
                        .HasForeignKey("ServiceProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.SPRedirectUri", b =>
                {
                    b.HasOne("mylabel.MobileId.Aggregator.Db.ServiceProvider", null)
                        .WithMany("AllowedRedirectUris")
                        .HasForeignKey("ServiceProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.SPToDiscoveryLink", b =>
                {
                    b.HasOne("mylabel.MobileId.Aggregator.Db.DiscoveryService", "DiscoveryService")
                        .WithMany()
                        .HasForeignKey("DiscoveryServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mylabel.MobileId.Aggregator.Db.ServiceProvider", null)
                        .WithMany("Discoveries")
                        .HasForeignKey("ServiceProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiscoveryService");
                });

            modelBuilder.Entity("mylabel.MobileId.Aggregator.Db.ServiceProvider", b =>
                {
                    b.Navigation("AllowedNotificationUris");

                    b.Navigation("AllowedRedirectUris");

                    b.Navigation("Discoveries");
                });
#pragma warning restore 612, 618
        }
    }
}
