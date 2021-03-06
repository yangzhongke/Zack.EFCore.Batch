﻿// <auto-generated />
using System;
using System.Net;
using Demo.PostgreSQL.Npgsql.heggi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Demo.PostgreSQL.Npgsql.Migrations.AppDb
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20201216220745_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Demo.PostgreSQL.Npgsql.heggi.User", b =>
                {
                    b.Property<string>("Uid")
                        .HasColumnType("text")
                        .HasColumnName("uid");

                    b.Property<ValueTuple<IPAddress, int>?>("IPv4")
                        .HasColumnType("cidr")
                        .HasColumnName("ip");

                    b.HasKey("Uid");

                    b.ToTable("user");
                });
#pragma warning restore 612, 618
        }
    }
}
