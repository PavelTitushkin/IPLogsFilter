﻿// <auto-generated />
using System;
using System.Net;
using IPLogsFilter.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IPLogsFilter.Db.Migrations
{
    [DbContext(typeof(IPLogsFilterContext))]
    [Migration("20240611100417_EditPropertyExecutionTime")]
    partial class EditPropertyExecutionTime
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("IPLogsFilter.Abstractions.Entities.FiltredLogs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CountLogRecords")
                        .HasColumnType("integer")
                        .HasColumnName("count_log_records");

                    b.Property<string>("LogRecord")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("log_record");

                    b.HasKey("Id")
                        .HasName("pk_filtred_logs");

                    b.ToTable("filtred_logs", (string)null);
                });

            modelBuilder.Entity("IPLogsFilter.Abstractions.Entities.LogRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationName")
                        .HasColumnType("text")
                        .HasColumnName("application_name");

                    b.Property<IPAddress>("ClientIpAddress")
                        .IsRequired()
                        .HasColumnType("inet")
                        .HasColumnName("client_ip_address");

                    b.Property<string>("ClientName")
                        .HasColumnType("text")
                        .HasColumnName("client_name");

                    b.Property<string>("ClientVersion")
                        .HasColumnType("text")
                        .HasColumnName("client_version");

                    b.Property<string>("ContentLength")
                        .HasColumnType("text")
                        .HasColumnName("content_length");

                    b.Property<string>("ContentType")
                        .HasColumnType("text")
                        .HasColumnName("content_type");

                    b.Property<TimeSpan?>("ExecutionTime")
                        .HasColumnType("interval")
                        .HasColumnName("execution_time");

                    b.Property<long?>("ExecutionTimeTicks")
                        .HasColumnType("bigint")
                        .HasColumnName("ExecutionTime");

                    b.Property<int?>("MemroyUsage")
                        .HasColumnType("integer")
                        .HasColumnName("memroy_usage");

                    b.Property<string>("Method")
                        .HasColumnType("text")
                        .HasColumnName("method");

                    b.Property<string>("Path")
                        .HasColumnType("text")
                        .HasColumnName("path");

                    b.Property<DateTime>("RequestTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("request_time");

                    b.Property<string>("Stage")
                        .HasColumnType("text")
                        .HasColumnName("stage");

                    b.Property<string>("StatusCode")
                        .HasColumnType("text")
                        .HasColumnName("status_code");

                    b.Property<string>("StatusMesage")
                        .HasColumnType("text")
                        .HasColumnName("status_mesage");

                    b.HasKey("Id")
                        .HasName("pk_log_records");

                    b.ToTable("log_records", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}